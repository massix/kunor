CLIENT_SRCS =	MainWindow.cs \
				GroupListBox.cs \
				MessageListBox.cs \
				MessageViewBox.cs

DEPS = -r:../KunorNNTP/KunorNNTP.dll

LIBRARY = KunorClient.dll
librarydir = ${datadir}/${PACKAGE_NAME}/
library_DATA = ${LIBRARY}
CLEANFILES = ${LIBRARY}

GMCS_FLAGS = -t:library ${DEPS} ${OS_CFLAGS} -out:${LIBRARY}

all: ${LIBRARY}

${LIBRARY}: ${NNTP_SRCS}
	${GMCS} ${GMCS_FLAGS} ${GTK_SHARP_2_LIBS} ${CLIENT_SRCS}

EXTRA_DIST = ${CLIENT_SRCS}
