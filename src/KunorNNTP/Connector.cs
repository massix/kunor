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
		public string WriteAndRead (string message, string encoding = "UTF-8") {
			string response = "";
			string tmp = "";
			lastMessage = message + " \r\n";
			Write ();
			while (true) {
				tmp = ReadResponseLine (encoding);
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

		/* This is similar to the WriteAndRead but it's specific for getting bodies of articles */
		public string GetArticleBody (int message_id, string encoding = "ISO-8859-15") {
			if (encoding == "")
				encoding = "ISO-8859-15";
			string response = "";
			lastMessage = "BODY " + message_id.ToString () + " \r\n";
			Write ();
			if (!ReadResponseLine (encoding).StartsWith ("222"))
				return ("Could not retrieve BODY of article " + message_id.ToString ()).Trim ();
			while (true) {
				string tmp = ReadResponseLine (encoding);
				if (tmp == ".\r\n" || tmp == ".\n")
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
		private string ReadResponseLine (string encoding = "UTF-8") {
			Encoding enc = Encoding.GetEncoding (encoding);
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
}
