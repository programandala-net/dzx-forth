# Makefile for DZX-Forth

################################################################
# Change log

# 2014-12-28: Start.

################################################################
# Notes

# $? list of dependencies changed more recently than current target
# $@ name of current target
# $> name of current dependency
# $* name of current dependency without extension

################################################################

src_dir = src
doc_dir = doc
# usage example: $(src_dir)

#all: doc/dzx-forth.html dzx-forth.tap
all: tap doc

################################################################
# Documentation

doc/dzx-forth.html: doc/dzx-forth.adoc
	asciidoctor doc/dzx-forth.adoc -o doc/dzx-forth.html

doc/dzx-forth_glossary.html: doc/dzx-forth_glossary.adoc
	asciidoctor doc/dzx-forth_glossary.adoc -o doc/dzx-forth_glossary.html

.SUFFIXES: .adoc .html
.adoc.html:
	@echo "Making the " $* " .html"
#	asciidoctor doc/$< -o doc/$*.html
	asciidoctor doc/$< -o doc/$@

ex_doc:
	@make doc/dzx-forth.html

.PHONY doc:
	asciidoctor doc/dzx-forth.adoc -o doc/dzx-forth.html
	asciidoctor doc/dzx-forth_glossary.adoc -o doc/dzx-forth_glossary.html

################################################################
# Program

dzx-forth.tap: src/dzx-forth.z80s
	cd src && \
	pasmo --name DZX-Forth --tapbas dzx-forth.z80s ../dzx-forth.tap && \
	cd -

tap:
	@make dzx-forth.tap

################################################################
# Examples to learn from

# .PHONY: mrproper
# PLATFORMS := os4 morphos win32 win32-gcc win64-gcc beos haiku osx linux linux-nogl gphwiz aitouchbook aros
# mrproper:
#	@for plat in $(PLATFORMS); do $(MAKE) -f $(VPATH)/Makefile clean PLATFORM=$${plat} ; done
#
#

DOCINC =			\
	docsrc/_cassette.1	\
	docsrc/_fsel.1		\
	docsrc/_glossary.1	\
	docsrc/_keyboard.1	\
	docsrc/_keys.1		\
	docsrc/_setup.1		\
	docsrc/_sound.1		\
	docsrc/_vars.1		\
	docsrc/_video.1

ROM =	atmos.rom		\
	hyperbasic.rom		\
	jasmin.rom		\
	microdisc.rom		\
	oric1.rom		\
	teleass.rom		\
	telematic.rom		\
	telemon-2.4.rom

NAMEVER = xeuphoric-$(VERSION)
DISTTMP = ~/tmp/$(NAMEVER)
dist:
	./configure --prefix /usr/local
	rm -rf $(DISTTMP)
	mkdir -p $(DISTTMP)
	tar cf -			\
	  .xeuphoricrc			\
	  CHANGES			\
	  COPYING			\
	  Makefile			\
	  README			\
	  TODO				\
	  VERSION			\
	  configure			\
	  doc/euphoric-0.99b/HISTORIC	\
	  doc/euphoric-0.99b/HISTORIQ	\
	  doc/euphoric-0.99b/LISEZMOI	\
	  doc/euphoric-0.99b/MANUAL	\
	  doc/euphoric-0.99b/MANUEL	\
	  doc/euphoric-0.99b/README	\
	  doc/xeuphoric.1		\
	  docsrc/xeuphoric.1		\
	  $(DOCINC)			\
	  scripts/process		\
	  scripts/txt2c			\
	  scripts/warn-perl		\
	  src/1793.S			\
	  src/6502.S			\
	  src/6502.h			\
	  src/6502lite.h		\
	  src/6551.S			\
	  src/8912.S			\
	  src/banks.S			\
	  src/charset.S			\
	  src/clock.S			\
	  src/datapath.c		\
	  src/debug.S			\
	  src/dirct.c			\
	  src/dirct.h			\
	  src/dsp.c			\
	  src/dsp.h			\
	  src/fsel.c			\
	  src/fsel.h			\
	  src/hardware.S		\
	  src/hardware.h		\
	  src/host.c			\
	  src/hosttraps.c		\
	  src/hosttraps.h		\
	  src/interrup.c		\
	  src/keyb_us.c			\
	  src/keyb_us.h			\
	  src/locate.c			\
	  src/misc.c			\
	  src/oric.c			\
	  src/rcfiles.c			\
	  src/screenshot.c		\
	  src/screenshot.h		\
	  src/setup.c			\
	  src/setup.h			\
	  src/stats.c			\
	  src/stats.h			\
	  src/tape.S			\
	  src/tape2.c			\
	  src/time.S			\
	  src/time.h			\
	  src/traps.S			\
	  src/traps.h			\
	  src/ula.c			\
	  src/version.c			\
	  src/via1.S			\
	  src/via2.S			\
	  src/x11.c			\
	  src/x11.h			\
	  src/x11kb.c			\
	  src/x11kb.h			\
	  src/x11render.c		\
	  src/x11render1bpp.c		\
	  src/xeuphoric.h		\
	  src-ng/6551.h			\
	  src-ng/Makefile		\
	  src-ng/README.hack		\
	  src-ng/README2		\
	  src-ng/TODO			\
	  src-ng/configure.h		\
	  src-ng/cpu6502-impl.h		\
	  src-ng/cpu6502-other.cc	\
	  src-ng/cpu6502.cc		\
	  src-ng/cpu6502.h		\
	  src-ng/fdc.h			\
	  src-ng/font.cc		\
	  src-ng/font.h			\
	  src-ng/hud.cc			\
	  src-ng/hud.h			\
	  src-ng/main.cc		\
	  src-ng/oric-impl.h		\
	  src-ng/oric.h			\
	  src-ng/psg8912.h		\
	  src-ng/test.cc		\
	  src-ng/ula-x11.cc		\
	  src-ng/ula-x11.h		\
	  src-ng/ula.h			\
	  src-ng/uladefs.h		\
	  src-ng/via6522.h		\
	  src-ng/x11-drawchar1.cc	\
	  src-ng/x11.cc			\
	  src-ng/x11.h			\
	  src-ng/x11drawchar.cc		\
	  src-ng/x11priv.cc		\
	  src-ng/x11priv.h		\
	  src-ng/x11render.cc		\
	  src-ng/x11render1bpp.cc	\
	  src-ng/xeuphoric-ng.h		\
	  test/findlastcomp.test	\
	  $(ROM)			\
	  | (cd $(DISTTMP) && tar xf -)
	n=$(NAMEVER); cd $(DISTTMP)/.. && tar -cf $$n.tar $$n
	gzip -f $(DISTTMP)/../$(NAMEVER).tar
	#rm -rf $(DISTTMP)

