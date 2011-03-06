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
using Kunor.NNTP;
using Gtk;
using GLib;
using Pango;

namespace Kunor.Client {
	/* This widget holds the groups */
	public class GroupListBox : Gtk.ScrolledWindow {
		private NNTP.GroupList g_list;
		private Gtk.TreeView groups_view;
		private Gtk.ListStore groups_store;
		private Client.MainWindow partof;
		private enum Columns {
			COL_NAME, COL_UNREAD
		};

		public GroupListBox (MainWindow partof) : base () {
			/* Set some default values for properties */
			HscrollbarPolicy = Gtk.PolicyType.Automatic;
			VscrollbarPolicy = Gtk.PolicyType.Automatic;
			ResizeMode = Gtk.ResizeMode.Immediate;

			g_list = new NNTP.GroupList ();
			groups_store = new Gtk.ListStore (typeof (string), typeof (int));
			groups_view = new Gtk.TreeView ();

			/* Build up the TreeView, replacing the "unibo.cs" part with "u.c." */
			foreach (NNTP.Group s_group in g_list) {
			 	Gtk.TreeIter iter = groups_store.AppendValues (s_group.name.Replace ("unibo.cs", "u.c"),
															   (s_group.hi - s_group.low));
			}

			groups_view.Model = groups_store;
			groups_view.HeadersVisible = true;

			Gtk.CellRendererText cell_renderer = new Gtk.CellRendererText () {
				Editable = false,
				Ellipsize = Pango.EllipsizeMode.None,
				WrapWidth = -1
			};

			/* First column is resizable and gets extra space */
			Gtk.TreeViewColumn to_be_set;
			groups_view.AppendColumn ("Name", cell_renderer, "text", (int) Columns.COL_NAME);
			to_be_set = groups_view.GetColumn ((int) Columns.COL_NAME);
			to_be_set.Expand = true;

			/* Second column is not resizable and doesn't get extra space */
			groups_view.AppendColumn ("Unread", cell_renderer, "text", (int) Columns.COL_UNREAD);
			to_be_set = groups_view.GetColumn ((int) Columns.COL_UNREAD);
			to_be_set.Expand = false;

			/* Main event linked to the tree view */
			groups_view.RowActivated += delegate (object o, RowActivatedArgs args) {
				Gtk.TreeIter iter;
				Utils.PrintDebug (Utils.TAG_DEBUG, "Double clicked row: " + args.Path);

				if (groups_store.GetIter (out iter, args.Path)) {
					string groupname = (string) groups_store.GetValue (iter, (int) Columns.COL_NAME);
					Utils.PrintDebug (Utils.TAG_DEBUG, "Selected NG: " + groupname);

					NNTP.Group selected = g_list.FindByName (groupname.Replace ("u.c.", "unibo.cs."));
					Utils.PrintDebug (Utils.TAG_DEBUG, "Got: " + selected.name);

					NNTP.MessageList g_messagelist = selected.GetMessages ();
				}

				else
					Utils.PrintDebug (Utils.TAG_ERROR, "Couldn't retrieve NG name");
			};

			AddWithViewport (groups_view);
			ShowAll ();
		}
	}
}
