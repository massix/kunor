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

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Kunor.NNTP {
	/* This class creates the first connection to the underlying NNTP protocol */
	public class Connector : TcpClient {
		private string lastMessage;
		private NetworkStream stream;

		/* Connect the socket */
		public Connector () : base () {
			Connect (Constants.SERVER_ADDRESS, Constants.SERVER_PORT);
			stream = GetStream ();
			if (ReadResponseLine ().Contains ("200"))
				Console.WriteLine ("Connected");
			else
				throw new NNTPConnectorException ("Failed while authenticating to the NNTP server");
		}

		/* This is the public method that writes and reads */
		public string WriteAndRead (string message) {
			string response = "";
			string tmp = "";
			lastMessage = message + "\r\n";
			Write ();
			while (true) {
				tmp = ReadResponseLine ();
				/* Error occured while querying */
				if (tmp.Contains ("500 What"))
					break;

				/* End of Line */
				if (tmp == ".\r\n" || tmp == ".\n" || tmp == "\r\n" || tmp == "\n")
					break;

				response += tmp;
			}

			return response.Trim ();
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
			byte[] response = new byte[1024];
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
				}
				else
					break;
			}

			return enc.GetString (response, 0, count);
		}
	}
}
