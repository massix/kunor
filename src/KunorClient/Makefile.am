CLIENT_SRCS = MainWindow.cs

DEPS = ../KunorNNTP/KunorNNTP.dll

LIBRARY = KunorClient.dll
librarydir = ${datadir}/${PACKAGE_NAME}/
library_DATA = ${LIBRARY}
CLEANFILES = ${LIBRARY}

all: ${LIBRARY}

${LIBRARY}: ${NNTP_SRCS}
	${GMCS} -t:library -r:${DEPS} ${GTK_SHARP_2_LIBS} -out:KunorClient.dll ${CLIENT_SRCS}

EXTRA_DIST = ${CLIENT_SRCS}
