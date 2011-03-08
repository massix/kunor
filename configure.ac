AC_INIT([Kunor], [0.0.1], [gengarel@cs.unibo.it], [Kunor])
AC_CONFIG_SRCDIR([Makefile.am])
AM_MAINTAINER_MODE
AM_INIT_AUTOMAKE

dnl ---------------------
dnl check for basic stuff
dnl ---------------------

GMCS_REQUIRED=2.8.1.0

dnl AC_PROG_LIBTOOL
dnl AC_PROG_INTLTOOL

AC_PATH_PROG(GMCS, gmcs, no)
if test "x$GMCS" = "xno"; then
   AC_MSG_ERROR([Can't find mono g-compiler in your path!])
fi

AC_PATH_PROG(MONO, mono, no)
if test "x$MONO" = "xno"; then
   AC_MSG_ERROR([Can't find mono in your path!])
fi

AC_PATH_PROG(SSH, ssh, no)
if test "x$SSH" = "xno"; then
   AC_MSG_ERROR([Can't find ssh in your path!])
fi

AC_PATH_PROG(SSH_KEYGEN, ssh-keygen, no)
if test "x$SSH_KEYGEN" = "xno"; then
	AC_MSG_ERROR([Can't find ssh-keygen in your path!])
fi

AC_SUBST(MONO)
AC_SUBST(GMCS)
AC_SUBST(SSH)
AC_SUBST(SSH_KEYGEN)

dnl -------------------
dnl check for libraries
dnl -------------------

dnl GETTEXT_PACKAGE=amen
dnl AC_SUBST(GETTEXT_PACKAGE)
dnl AM_GLIB_GNU_GETTEXT

GTK_SHARP_REQUIRED=2.12.10
NOTIFY_SHARP_REQUIRED=0.4.0

PKG_CHECK_MODULES(GTK_SHARP_2, gtk-sharp-2.0 >= $GTK_SHARP_REQUIRED)
PKG_CHECK_MODULES(NOTIFY_SHARP, notify-sharp >= $NOTIFY_SHARP_REQUIRED)

AC_ARG_ENABLE(debug,
	AC_HELP_STRING([--enable-debug],
		[Use 'Debug' Configuration [default=NO]]),
		enable_debug=yes, enable_debug=no)
AM_CONDITIONAL(ENABLE_DEBUG, test x$enable_debug = xyes)
if test "x$enable_debug" = "xyes" ; then
   OS_CFLAGS=-d:DEBUG_ON
fi

AC_SUBST(GTK_SHARP_2_LIBS)
AC_SUBST(NOTIFY_SHARP_REQUIRED)
AC_SUBST(OS_CFLAGS)

AC_CONFIG_FILES([
	Makefile
	src/Makefile
	src/KunorNNTP/Makefile
	data/Makefile
	data/kunor
	data/kunor.desktop
	src/Utils.cs
	src/KunorSSH/Makefile
	src/KunorClient/Makefile])

AC_OUTPUT
