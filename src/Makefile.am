SUBDIRS = KunorSSH KunorNNTP KunorClient

PROGRAM = Kunor.exe

programdir = ${datadir}/${PACKAGE_NAME}
program_in_files = Utils.cs.in
program_DATA = ${PROGRAM}

DIR_SRCS = Kunor.cs $(program_in_files:.cs.in=.cs)
LIBS = KunorNNTP/KunorNNTP.dll,KunorClient/KunorClient.dll
CLEANFILES = ${PROGRAM}

all: ${SUBDIRS} ${PROGRAM}

${PROGRAM}: ${SUBDIRS} ${DIR_SRCS}
	${GMCS} -t:exe ${DIR_SRCS} ${GTK_SHARP_2_LIBS} -r:${LIBS}
