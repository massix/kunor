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
			/* Switch to the given group */
			Connector.GetInstance ().SwitchGroup (name);

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
