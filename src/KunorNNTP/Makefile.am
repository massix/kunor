NNTP_SRCS =		Connector.cs \
				Constants.cs \
				Exceptions.cs \
				MessagesConnector.cs \
				GroupsConnector.cs

LIBRARY = KunorNNTP.dll
librarydir = ${datadir}/${PACKAGE_NAME}/
library_DATA = ${LIBRARY}
CLEANFILES = ${LIBRARY}

GMCS_FLAGS = -t:library -out:${LIBRARY}
UTILS_FILE = ../Utils.cs

all: ${LIBRARY}

${LIBRARY}: ${NNTP_SRCS}
	${GMCS} ${GMCS_FLAGS} ${GTK_SHARP_2_LIBS} ${OS_CFLAGS} ${NNTP_SRCS} ${UTILS_FILE}


EXTRA_DIST = ${NNTP_SRCS}