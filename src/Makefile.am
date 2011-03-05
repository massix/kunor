SUBDIRS = KunorClient KunorNNTP KunorSSH

PROGRAM = Kunor.exe

programdir = ${datadir}/${PACKAGE_NAME}
program_DATA = ${PROGRAM}

DIR_SRCS = Kunor.cs
LIBS = KunorNNTP/KunorNNTP.dll,KunorClient/KunorClient.dll
CLEANFILES = ${PROGRAM}

all: ${SUBDIRS} ${PROGRAM}

${PROGRAM}: ${SUBDIRS} ${DIR_SRCS}
	${GMCS} -t:exe ${DIR_SRCS} ${GTK_SHARP_2_LIBS} -r:${LIBS}
