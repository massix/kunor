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

namespace Kunor.Client {
	public class MainWindow : Gtk.Window {
		private Gtk.VBox main_container;
		private Gtk.MenuBar main_menu;
		private Gtk.HPaned middle_container;
		private Gtk.Statusbar status_bar;

		/* Builds up the main GUI */
		public MainWindow () : base ("Kunor") {
			BuildMenuBar ();

			main_container = new Gtk.VBox (false, 2);
			middle_container = new Gtk.HPaned ();
			status_bar = new Gtk.Statusbar ();

			main_container.PackStart (main_menu, false, false, 0);

			main_container.PackStart (middle_container);
			middle_container.Add1 (new GroupListBox (this));
			middle_container.Add2 (new Gtk.Label ("Child 2"));

			main_container.PackStart (status_bar, false, false, 0);
			status_bar.Push (0, "Statusbar");

			DestroyEvent += delegate (object o, DestroyEventArgs e) {
				Console.WriteLine ("Catch Destroy Event");
				Application.Quit ();
			};

			DeleteEvent += delegate (object o, DeleteEventArgs e) {
				Console.WriteLine ("Catch Delete Event");
				Application.Quit ();
			};

			Add (main_container);

			ShowAll ();
		}

		private void BuildMenuBar () {
			main_menu = new Gtk.MenuBar ();

			/* File menu */
			Gtk.Menu file = new Gtk.Menu ();
			Gtk.MenuItem file_item = new Gtk.MenuItem ("_File");
			Gtk.MenuItem exit_item = new Gtk.MenuItem ("_Exit");
			file.Append (exit_item);
			file_item.Submenu = file;

			exit_item.Activated += delegate (object o, EventArgs e) {
				Application.Quit ();
			};

			main_menu.Append (file_item);
		}
	}

	/* This widget holds the groups */
	public class GroupListBox : Gtk.ScrolledWindow {
		private NNTP.GroupList g_list;
		private Gtk.TreeView groups_view;
		private Gtk.ListStore groups_store;
		private Client.MainWindow partof;


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
			groups_view.AppendColumn ("Name", new Gtk.CellRendererText (), "text", 0);
			groups_view.AppendColumn ("Unread", new Gtk.CellRendererText (), "text", 1);

			/* Main event linked to the tree view */
			groups_view.RowActivated += delegate (object o, RowActivatedArgs args) {
				Gtk.TreeIter iter;
				Utils.PrintDebug (Utils.TAG_DEBUG, "Double clicked row: " + args.Path);

				if (groups_store.GetIter (out iter, args.Path)) {
					Utils.PrintDebug (Utils.TAG_DEBUG, "Selected NG: " +
									  groups_store.GetValue (iter, 0));
				}

				else
					Utils.PrintDebug (Utils.TAG_ERROR, "Couldn't retrieve NG name");
			};

			AddWithViewport (groups_view);
			ShowAll ();
		}
	}
}
