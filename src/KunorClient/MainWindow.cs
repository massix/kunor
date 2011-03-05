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
		private Gtk.VBox mainContainer;
		private Gtk.HBox middleContainer;


		public MainWindow () : base ("Kunor") {
			Resize (200, 200);

			mainContainer = new Gtk.VBox (true, 2);
			mainContainer.PackStart (new Label ("Label one"));
			mainContainer.PackStart (new Label ("Label two"));
			mainContainer.ShowAll ();

			DestroyEvent += delegate (object o, DestroyEventArgs e) {
				Console.WriteLine ("Catch Destroy Event");
				Application.Quit ();
			};

			DeleteEvent += delegate (object o, DeleteEventArgs e) {
				Console.WriteLine ("Catch Delete Event");
				Application.Quit ();
			};

			Add (mainContainer);

			ShowAll ();
		}
	}
}
