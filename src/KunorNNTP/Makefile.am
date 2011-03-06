NNTP_SRCS = Connector.cs Constants.cs Exceptions.cs

LIBRARY = KunorNNTP.dll
librarydir = ${datadir}/${PACKAGE_NAME}/
library_DATA = ${LIBRARY}
CLEANFILES = ${LIBRARY}

GMCS_FLAGS = -t:library -out:${LIBRARY}
OS_FLAGS = -d:DEBUG_ON
UTILS_FILE = ../Utils.cs

all: ${LIBRARY}

${LIBRARY}: ${NNTP_SRCS}
	${GMCS} ${GMCS_FLAGS} ${OS_FLAGS} ${NNTP_SRCS} ${UTILS_FILE}


EXTRA_DIST = ${NNTP_SRCS}