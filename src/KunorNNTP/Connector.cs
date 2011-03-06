/* Kunor - KUNOR's University NNTP Opensource Reader
 * Copyright (C) 2011 - Massimo Gengarelli <gengarel@cs.unibo.it>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */

using Kunor;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Kunor.NNTP {
	/* This Singleton class holds the connection to the underlying NNTP protocol */
	public class Connector : TcpClient {
		private static Connector instance;
		private string lastMessage;
		private NetworkStream stream;

		/* Connect the socket */
		private Connector () : base () {
			Connect (Constants.SERVER_ADDRESS, Constants.SERVER_PORT);
			stream = GetStream ();
			if (ReadResponseLine ().Contains ("200"))
				Utils.PrintDebug (Utils.TAG_DEBUG, "Connector is connected");
			else
				throw new NNTPConnectorException ("Failed while authenticating to the NNTP server");
		}

		public static Connector GetInstance () {
			Utils.PrintDebug (Utils.TAG_DEBUG, "Someone requested me!");
			try {
				if (instance == null)
					instance = new Connector ();
			}

			catch (NNTPConnectorException e) {
				Utils.PrintDebug (Utils.TAG_ERROR, "Could not connect to the NNTP server");
				return null;
			}

			return instance;
		}

		/* This is the public method that writes and reads */
		public string WriteAndRead (string message) {
			string response = "";
			string tmp = "";
			lastMessage = message + " \r\n";
			Write ();
			while (true) {
				tmp = ReadResponseLine ();
				Utils.PrintDebug (Utils.TAG_DEBUG, "Response line contains: " + tmp);
				/* Error occured while querying */
				if (tmp.StartsWith ("500 What") || tmp.StartsWith ("501 Syntax"))
					break;

				/* End of Line */
				if (tmp == ".\r\n" || tmp == ".\n" || tmp == "\r\n" || tmp == "\n")
					break;

				response += tmp;
			}

			return response.Trim ();
		}

		/* Function to switch working group, returns true on success, false otherwise */
		public bool SwitchGroup (string groupname) {
			lastMessage = "GROUP " + groupname + "\r\n";
			Write ();
			string result = ReadResponseLine ();
			if (result.StartsWith ("211"))
				return true;
			return false;
		}

		/* Write something to the server */
		private void Write () {
			UTF8Encoding enc = new UTF8Encoding ();
			byte[] writeBuffer = new byte[1024];
			writeBuffer = enc.GetBytes (lastMessage);
			stream.Write (writeBuffer, 0, writeBuffer.Length);
		}

		/* Reads the received response */
		private string ReadResponseLine () {
			UTF8Encoding enc = new UTF8Encoding ();
			byte[] response = new byte[2048];
			int count = 0;

			/* Cycle the response bytes until there's nothing left or a newline is encountered */
			while (true) {
				byte[] r_buff = new byte[2];
				int read = stream.Read (r_buff, 0, 1);
				if (read == 1) {
					response[count] = r_buff[0];
					count++;

					if (r_buff[0] == '\n')
						break;

					if (count > 2047)
						break;
				}

				else
					break;
			}

			return enc.GetString (response, 0, count);
		}
	}


	/* Class holding a single message */
	public class Message {
		/* used to retrieve the message from the listgroup NNTP command */
		private int int_msg_id;

		/* used when referencing a new post */
		private string str_msg_id;
		private string from;
		private string subject;
		private string date;
		private int lines;
		private string user_agent;

		/* Create a new message from the given headers */
		public Message (string header) {
			string[] headers_line = header.Split ('\n');

			/* Parsing of the headers */
			for (int i = 0; i < headers_line.Length; i++) {
				if (headers_line[i].StartsWith ("From: "))
					from = headers_line[i];

				else if (headers_line[i].StartsWith ("Date: "))
					date = headers_line[i];

				else if (headers_line[i].StartsWith ("Message-ID: "))
					str_msg_id = headers_line[i];

				else if (headers_line[i].StartsWith ("User-Agent: "))
					user_agent = headers_line[i];

				else if (headers_line[i].StartsWith ("Subject: "))
					subject = headers_line[i];

				else
					Utils.PrintDebug (Utils.TAG_DEBUG, "Header not known: " + headers_line[i]);
			}

			Utils.PrintDebug (Utils.TAG_DEBUG, "Got new message from: " + from);
		}
	}


	/* Class which creates and holds the references to the messages */
	public class MessageList : System.Collections.Generic.List<Message> {

		/* Build a new MessageList referred to the given newsgroup */
		public MessageList (string groupname) {
			Connector instance = Connector.GetInstance ();

			/* Switch to the given group or throw exception if it didn't exist */
			if (!instance.SwitchGroup (groupname))
				throw new NNTPConnectorException ("Group doesn't exist");

			/* Get the messages */
			string message_resp = instance.WriteAndRead ("LISTGROUP " + groupname);
			string[] messages = message_resp.Split ('\n');

			for (int i = 1; i < messages.Length; i++) {
				string headers = instance.WriteAndRead ("HEAD " + messages[i].Trim ());
				Add (new Message (headers));
			}
		}
	}


	/* Class holding a single group */
	public class Group {
		public string name { get; private set; }
		public int hi { get; private set; }
		public int low { get; private set; }
		public char status { get; private set; }
		private MessageList messages;

		/* Build a new group */
		public Group (string g_name, int g_hi, int g_low, char g_status) {
			name = g_name;
			hi = g_hi;
			low = g_low;
			status = g_status;
		}

		/* Get the MessageList of that group */
		public MessageList GetMessages () {
			if (messages == null)
				messages = new MessageList (name);

			return messages;
		}
	}

	/* Class which creates and holds the references to the groups */
	public class GroupList : System.Collections.Generic.List<Group> {

		public GroupList () : base () {
			Connector instance = Connector.GetInstance ();

			/* Get the groups from the connector */
			string a_groups = instance.WriteAndRead ("LIST ACTIVE unibo.cs.*");
			string[] s_groups = a_groups.Split ('\n');

			if (s_groups[0].StartsWith ("215")) {
				Utils.PrintDebug (Utils.TAG_DEBUG, "Beginning groups scan");

				/* Parse it, filtering the CS groups (redundant control) */
				for (int i = 1; i < s_groups.Length; i++) {
					string[] group = s_groups[i].Split (' ');
					Add (new Group (group[0], Int32.Parse (group[1]),
									Int32.Parse (group[2]), group[3][0]));
					Utils.PrintDebug (Utils.TAG_DEBUG, "Created new Group " + group[0]);
				}
			}

			else {
				Utils.PrintDebug (Utils.TAG_ERROR, "Error while retrieving groups list");
				throw new NNTPConnectorException ("Error while retrieving groups list");
			}
		}

		/* Find a group by its name */
		public Group FindByName (string name) {
			return Find (delegate (Group g) {
					return (g.name == name);
				});
		}
	}
}
