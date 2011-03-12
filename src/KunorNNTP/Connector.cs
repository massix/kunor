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
using System.Text.RegularExpressions;
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


	/* Class holding a single message and its children */
	public class Message {
		/* used to retrieve the message from the listgroup NNTP command */
		private int int_msg_id;

		/* used when referencing a new post */
		public string str_msg_id { get; private set; }
		public string s_from { get; private set; }
		public string subject { get; private set; }
		public string date { get; private set; }
		public DateTime date_time { get; private set; }
		public int lines { get; private set; }
		public string user_agent { get; private set; }
		public string[] refers { get; private set; }

		/* Holds the messages that refer to this one */
		public List<Message> children { get; private set; }

		/* Some read-only properties */
		public bool has_children {
			get {
				return (children != null);
			}
			private set {
				/* Do nothing */
			}
		}

		public bool has_refers {
			get {
				return (refers != null);
			}
			private set {
				/* Do nothing */
			}
		}

		/* Create a new message from the given headers */
		public Message (int id, string header) {
			string[] headers_line = header.Split ('\n');
			int_msg_id = id;

			/* Parsing of the headers */
			for (int i = 0; i < headers_line.Length; i++) {
				if (headers_line[i].StartsWith ("From: ")) {
					Regex classic_email_field = new Regex ("<[A-z0-9]*@.*>");
					Regex cs_email_field = new Regex ("[A-z0-9]*@cs.unibo.it");
					s_from = headers_line[i].Replace ("From: ", "");
					s_from = classic_email_field.Replace (s_from, "");
					s_from = cs_email_field.Replace (s_from, "");
					s_from = s_from.Replace ("(", "");
					s_from = s_from.Replace (")", "");
					s_from = s_from.Trim ();
				}

				else if (headers_line[i].StartsWith ("Date: ")) {
					date = headers_line[i].Replace ("Date: ", "").Replace ("(UTC)", "");
					try {
						date_time = DateTime.Parse (date.Trim ());
						Utils.PrintDebug (Utils.TAG_DEBUG, "Able to get date from: " + date);
						date = date_time.ToString ("dd.MM.yy");
					}
					catch (ArgumentNullException e) {
						Utils.PrintDebug (Utils.TAG_ERROR, "Date is null");
						date = "Null";
					}
					catch (FormatException e) {
						Utils.PrintDebug (Utils.TAG_ERROR, "Not a valid format for Date: " + date);
						date = "N/A";
					}
				}

				else if (headers_line[i].StartsWith ("Message-ID: "))
					str_msg_id = headers_line[i].Replace ("Message-ID: ", "");

				else if (headers_line[i].StartsWith ("User-Agent: "))
					user_agent = headers_line[i].Replace ("User-Agent: ", "");

				else if (headers_line[i].StartsWith ("Subject: "))
					subject = headers_line[i].Replace ("Subject: ", "");

				else if (headers_line[i].StartsWith ("References: "))
					refers = headers_line[i].Replace ("References: ", "").Split (' ');
			}

			Utils.PrintDebug (Utils.TAG_DEBUG, "Got new message from: " + s_from);
			Utils.PrintDebug (Utils.TAG_DEBUG, "  ID: " + str_msg_id);
			if (has_refers) {
				Utils.PrintDebug (Utils.TAG_DEBUG, "  REFERS ");
				for (int i = 0; i < refers.Length; i++)
					Utils.PrintDebug (Utils.TAG_DEBUG, "    [" + i + "] " + refers[i]);
			}
		}

		public void AppendChild (Message m) {
			if (children == null)
				children = new List<Message> ();

			children.Add (m);
		}
	}


	/* Class which creates and holds the references to the messages */
	public class MessageList : System.Collections.Generic.List<Message> {

		/* Build a new MessageList referred to the given newsgroup */
		public MessageList (string groupname, int low, int hi) {
			Connector instance = Connector.GetInstance ();

			/* Switch to the given group or throw exception if it didn't exist */
			if (!instance.SwitchGroup (groupname))
				throw new NNTPConnectorException ("Group doesn't exist");

			/* Get the messages */
			string range = low + "-" + hi;
			string message_resp = instance.WriteAndRead ("LISTGROUP " + groupname + " " + range);
			string[] messages = message_resp.Split ('\n');

			for (int i = 1; i < messages.Length; i++) {
				string headers = instance.WriteAndRead ("HEAD " + messages[i].Trim ());
				Message to_be_added = new Message (i, headers);

				/* It is not a root message, we should find its father */
				if (to_be_added.has_refers) {
					Utils.PrintDebug (Utils.TAG_DEBUG, "  Looking for father");
					Message father = Find ((Message msg) => {
							if (msg.str_msg_id != null)
								return (msg.str_msg_id.Trim () == to_be_added.refers[0].Trim ());
							return false;
						});

					if (father != null) {
						Utils.PrintDebug (Utils.TAG_DEBUG, "  Found father");
						father.AppendChild (to_be_added);
						continue;
					}
				}

				/* Add it as a root message if it is a valid message */
				if (to_be_added.str_msg_id != null)
					Add (to_be_added);
			}
		}
	}

	/* Class holding a single group */
	public class Group {
		public string name { get; private set; }
		public int hi { get; private set; }
		public int low { get; private set; }
		public char status { get; private set; }
		public int lastRetrieved { get; private set; }
		private readonly int BULK = 50;
		private int count;
		private MessageList messages;
		private MessageList[] older_messages;

		/* Build a new group */
		public Group (string g_name, int g_hi, int g_low, char g_status) {
			name = g_name;
			hi = g_hi;
			low = g_low;
			status = g_status;
			lastRetrieved = -1;
			count = 0;
		}

		/* Get the MessageList of that group */
		public MessageList GetMessages (bool older) {
			/* First retrieve */
			if (messages == null) {
				if ((hi - low) < BULK) {
					lastRetrieved = low;
					messages = new MessageList (name, low, hi);
				}
				else  {
					lastRetrieved = hi - BULK;
					messages = new MessageList (name, lastRetrieved, hi);
				}
			}

			/* We can get older messages and the user wants to. */
			else if (lastRetrieved != low && older) {
				if ((lastRetrieved - low) > BULK) {
					older_messages[count++] = new MessageList (name, lastRetrieved - BULK, lastRetrieved);
					lastRetrieved -= BULK;
				}
				else {
					older_messages[count++] = new MessageList (name, low, lastRetrieved);
					lastRetrieved = low;
				}
			}

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
