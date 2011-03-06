NNTP_SRCS = Connector.cs Constants.cs Exceptions.cs

LIBRARY = KunorNNTP.dll
librarydir = ${datadir}/${PACKAGE_NAME}/
library_DATA = ${LIBRARY}
CLEANFILES = ${LIBRARY}

all: ${LIBRARY}

${LIBRARY}: ${NNTP_SRCS}
	${GMCS} -t:library -out:KunorNNTP.dll ${NNTP_SRCS}


EXTRA_DIST = ${NNTP_SRCS}