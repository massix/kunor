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
using System.Threading;
using Kunor.NNTP;
using Gtk;
using GLib;
using Pango;

namespace Kunor.Client {
	/* This widget holds the messages for a specific group */
	public class MessageListBox : Gtk.ScrolledWindow {
		private NNTP.MessageList msg_list;
		private Gtk.TreeView message_view;
		private Gtk.TreeStore message_store;
		private enum Columns {
			COL_FROM, COL_SUBJECT, COL_DATE
		};

		/* By default, the MessageListBox doesn't hold any messages */
		public MessageListBox () {
			HscrollbarPolicy = Gtk.PolicyType.Automatic;
			VscrollbarPolicy = Gtk.PolicyType.Automatic;
			ResizeMode = Gtk.ResizeMode.Immediate;

			message_store = new Gtk.TreeStore (typeof (string), typeof (string), typeof (string));
			message_view = new Gtk.TreeView ();

			message_view.Model = message_store;
			message_view.HeadersVisible = true;

			Gtk.CellRendererText cell_renderer = new Gtk.CellRendererText () {
				Editable = false,
				Ellipsize = Pango.EllipsizeMode.None,
				WrapWidth = -1
			};

			/* Set the properties for the columns */
			Gtk.TreeViewColumn to_be_set;
			message_view.AppendColumn ("From", cell_renderer, "text", (int) Columns.COL_FROM);
			to_be_set = message_view.GetColumn ((int) Columns.COL_FROM);
			to_be_set.Expand = false;

			message_view.AppendColumn ("Subject", cell_renderer, "text", (int) Columns.COL_SUBJECT);
			to_be_set = message_view.GetColumn ((int) Columns.COL_SUBJECT);
			to_be_set.Expand = true;

			message_view.AppendColumn ("Date", cell_renderer, "text", (int) Columns.COL_DATE);
			to_be_set = message_view.GetColumn ((int) Columns.COL_DATE);
			to_be_set.Expand = false;

			AddWithViewport (message_view);
			ShowAll ();
		}

		public void SwitchMessageList (NNTP.MessageList new_message_list) {
			msg_list = new_message_list;
			Utils.PrintDebug (Utils.TAG_DEBUG, "Fuck the what?");
		}
	}
}
