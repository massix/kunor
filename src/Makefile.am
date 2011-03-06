SUBDIRS = KunorSSH KunorNNTP KunorClient

PROGRAM = Kunor.exe

programdir = ${datadir}/${PACKAGE_NAME}
program_in_files = Utils.cs.in
program_DATA = ${PROGRAM}

DIR_SRCS = Kunor.cs $(program_in_files:.cs.in=.cs)

LIBS = 	-r:KunorNNTP/KunorNNTP.dll \
		-r:KunorClient/KunorClient.dll

CLEANFILES = ${PROGRAM}

OS_FLAGS = -d:DEBUG_ON

GMCS_FLAGS = -t:exe ${LIBS} ${OS_FLAGS}

all: ${SUBDIRS} ${PROGRAM}

${PROGRAM}: ${SUBDIRS} ${DIR_SRCS}
	${GMCS} ${GMCS_FLAGS} ${DIR_SRCS} ${GTK_SHARP_2_LIBS}


EXTRA_DIST = $(program_in_files:.cs.in=.cs)