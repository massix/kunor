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
using Gtk;
using Kunor;

namespace Kunor {
	public class Kunor {
		public static void Main () {
			Console.WriteLine ("Welcome to {0} version {1}",
							   Utils.PACKAGE, Utils.VERSION);

			Utils.PrintDebug (Utils.TAG_DEBUG, "DEBUG IS ON");
			string[] result = new string[2];
			Application.Init ();
			try {
				NNTP.Connector mainConnector = NNTP.Connector.GetInstance ();
				Client.MainWindow mw = new Client.MainWindow ();

				Application.Run ();
				mainConnector.Close ();
			}

			catch (NNTP.NNTPConnectorException e) {
				/* TODO: Try to reconnect using different parameters */
				Console.WriteLine ("NNTPConnectorException: " + e.Message);
			}

			catch (Exception e) {
				Console.WriteLine ("Exception: " + e.Message);
			}
		}
	}
}
