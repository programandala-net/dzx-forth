# Makefile for DZX-Forth

################################################################
# Requirements

# Pasmo (by Julián Albo)
# http://pasmo.speccy.org/

# tap2dsk from the Taptools (by John Elliott):
# http://www.seasip.info/ZX/unix.html

# bin2code (by MetalBrain):
# <http://metalbrain.speccy.org/link-eng.htm>.

################################################################
# Change log

# 2014-12-28: First draft.
#
# 2014-12-30: First working version: z80s>tap>dsk, adoc>html; 180 and 720 KB
# disks.
#
# 2015-01-05: Requirements.
#
# 2015-01-13: 720 KB disk commented out.
#
# 2015-01-14: Fixed the symbols filename. Added <bin> dir.
#
# 2015-01-20: <tools.fb> is included into the disk image, with <bin2code>.

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

VPATH = src:doc:bin
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
	pasmo -I src --name "DZXForth" --tapbas \
		src/dzx-forth.z80s bin/dzx-forth.tap \
		src/dzx-forth.symbols.z80s

.PHONY: tap
tap:
	@make dzx-forth.tap

#dzx-forth_720k.dsk: dzx-forth.tap
#	tap2dsk -720 -label DZXForth bin/dzx-forth.tap bin/dzx-forth_720k.dsk

dzx-forth_180k.dsk: dzx-forth.tap
	cd src/ ;\
	bin2code tools.fb tools.fb.tap ;\
	cd - ;\
	cat bin/dzx-forth.tap src/tools.fb.tap > bin/dzx-forth.package.tap ;\
	tap2dsk -180 -label DZXForth bin/dzx-forth.package.tap bin/dzx-forth_180k.dsk

.PHONY: dsk
dsk:
	@make dzx-forth_180k.dsk
#	@make dzx-forth_720k.dsk

