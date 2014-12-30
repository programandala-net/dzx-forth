# Makefile for DZX-Forth

################################################################
# Change log

# 2014-12-28: First draft.
#
# 2014-12-30: First working version: z80s>tap>dsk, adoc>html.

################################################################
# TODO

# - Versions with/out floating support, using Pasmo's command line to set the
# labels.

################################################################
# Notes

# $? list of dependencies changed more recently than current target
# $@ name of current target
# $> name of current dependency
# $* name of current dependency without extension

################################################################
# Config

VPATH = src:doc
MAKEFLAGS = --no-print-directory

.PHONY: all
all: tap dsk doc

################################################################
# Documentation

dzx-forth.html: dzx-forth.adoc
	asciidoctor doc/dzx-forth.adoc -o doc/dzx-forth.html

dzx-forth_glossary.html: dzx-forth_glossary.adoc
	asciidoctor doc/dzx-forth_glossary.adoc -o doc/dzx-forth_glossary.html

.PHONY: doc
doc:
	@make dzx-forth.html dzx-forth_glossary.html

################################################################
# Program

dzx-forth.tap: dzx-forth.z80s
	pasmo -I src --name "DZXForth.bin" --tapbas src/dzx-forth.z80s dzx-forth.tap

.PHONY: tap
tap:
	@make dzx-forth.tap

dzx-forth.dsk: dzx-forth.tap
	tap2dsk -720 -label DZXForth dzx-forth.tap dzx-forth.dsk

.PHONY: dsk
dsk:
	@make dzx-forth.dsk

################################################################
# Examples to learn from

# .PHONY: mrproper
# PLATFORMS := os4 morphos win32 win32-gcc win64-gcc beos haiku osx linux linux-nogl gphwiz aitouchbook aros
# mrproper:
#	@for plat in $(PLATFORMS); do $(MAKE) -f $(VPATH)/Makefile clean PLATFORM=$${plat} ; done
#
#
#
#DOCINC =			\
#	docsrc/_cassette.1	\
#	docsrc/_vars.1		\
#	docsrc/_video.1
#
#ROM =	atmos.rom		\
#	hyperbasic.rom		\
#	telemon-2.4.rom
#
#NAMEVER = xeuphoric-$(VERSION)
#DISTTMP = ~/tmp/$(NAMEVER)
#dist:
#	./configure --prefix /usr/local
#	rm -rf $(DISTTMP)
#	mkdir -p $(DISTTMP)
#	tar cf -			\
#	  .xeuphoricrc			\
#	  CHANGES			\
#	  COPYING			\
#	  Makefile			\
#	  README			\
#	  TODO				\
#	  VERSION			\
#	  configure			\
#	  doc/euphoric-0.99b/HISTORIC	\
#	  doc/euphoric-0.99b/README	\
#	  doc/xeuphoric.1		\
#	  docsrc/xeuphoric.1		\
#	  $(DOCINC)			\
#	  scripts/process		\
#	  scripts/txt2c			\
#	  scripts/warn-perl		\
#	  test/findlastcomp.test	\
#	  $(ROM)			\
#	  | (cd $(DISTTMP) && tar xf -)
#	n=$(NAMEVER); cd $(DISTTMP)/.. && tar -cf $$n.tar $$n
#	gzip -f $(DISTTMP)/../$(NAMEVER).tar
#	#rm -rf $(DISTTMP)
