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
	/* This widget shows the body of a specific message */
	public class MessageViewBox : Gtk.ScrolledWindow {
		private NNTP.Message shown_message;
		private Gtk.TextView main_text_view;

		/* By default, the Box is empty */
		public MessageViewBox () {
			HscrollbarPolicy = VscrollbarPolicy = Gtk.PolicyType.Automatic;
			main_text_view = new Gtk.TextView ();
			main_text_view.Editable = false;
			main_text_view.WrapMode = Gtk.WrapMode.Word;


			AddWithViewport (main_text_view);
			ShowAll ();
		}

		public void ShowMessage (Message msg) {
			TextBuffer buffer = main_text_view.Buffer;
			shown_message = msg;
			shown_message.RetrieveBody ();
			buffer.SetText (shown_message.body);
			main_text_view.Buffer = buffer;
		}

		public void Clear () {
			TextBuffer buffer = main_text_view.Buffer;
			buffer.SetText ("");
			main_text_view.Buffer = buffer;
		}
	}
}

