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

AC_SUBST(MONO)
AC_SUBST(GMCS)

dnl -------------------
dnl check for libraries
dnl -------------------

dnl GETTEXT_PACKAGE=amen
dnl AC_SUBST(GETTEXT_PACKAGE)
dnl AM_GLIB_GNU_GETTEXT

GTK_SHARP_REQUIRED=2.12.10

PKG_CHECK_MODULES(GTK_SHARP_2, gtk-sharp-2.0 >= $GTK_SHARP_REQUIRED)

AC_SUBST(GTK_SHARP_2_LIBS)

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
