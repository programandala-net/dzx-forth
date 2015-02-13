# Makefile for DZX-Forth

################################################################
# Requirements

# Pasmo (by Julián Albo)
# http://pasmo.speccy.org/

# Taptools (by John Elliott):
# http://www.seasip.info/ZX/unix.html

# bin2code and bin2tap (by MetalBrain):
# <http://metalbrain.speccy.org/link-eng.htm>.

################################################################
# Change log

# 2014-12-28: First draft.
#
# 2014-12-30: First working version: z80s>tap>dsk, adoc>html;
# 180 and 720 KB disks.
#
# 2015-01-05: Requirements.
#
# 2015-01-13: 720 KB disk commented out.
#
# 2015-01-14: Fixed the symbols filename. Added <bin> dir.
#
# 2015-01-20: <tools.fb> is included into the disk image, with
# <bin2code>.
#
# 2015-01-24: 720 KB disk used, to hold all block files.
# <mkp3fs> is used in order to copy the block files without a
# +3DOS header.

################################################################
# TODO

# - Versions with/out floating support, using Pasmo's command
#   line to set the labels.
# - Add the program itself to the disk image. Steps needed:
#   z80s>tap, symbols>disk.bas, disk.bas->disk.tap, src/>taps,
#   tap2dsk

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

.PHONY : all
all : tap dsk doc

################################################################
# Documentation

dzx-forth.html : dzx-forth.adoc
	asciidoctor doc/dzx-forth.adoc -o doc/dzx-forth.html

dzx-forth_glossary.html : dzx-forth_glossary.adoc
	asciidoctor doc/dzx-forth_glossary.adoc -o doc/dzx-forth_glossary.html

.PHONY : doc
doc:
	@make dzx-forth.html dzx-forth_glossary.html

################################################################
# Program

# ----------------------------------------------
# Disk BASIC loader

disk.bas : disk.bas.raw dzxforth.bin.tap
	@make dzxforth.bin.tap
	./_tools/patch_the_loader.fs

disk.tap : disk.bas
	bas2tap -a10 -sDISK src/disk.bas bin/disk.tap

# ----------------------------------------------
# Kernel

dzxforth.bin.tap : dzx-forth.z80s
	pasmo -I src --name "DZXFORTH.B" --tap \
		src/dzx-forth.z80s bin/dzxforth.bin.tap \
		src/dzx-forth.symbols.z80s

# XXX OLD
# TAP with kernel and tape BASIC loader
# dzx-forth.tap : dzx-forth.z80s
# 	pasmo -I src --name "DZXForth" --tapbas \
# 		src/dzx-forth.z80s bin/dzx-forth.tap \
# 		src/dzx-forth.symbols.z80s

# XXX OLD
# dzx-forth.bin : dzx-forth.z80s
# 	pasmo -I src \
# 		src/dzx-forth.z80s bin/dzx-forth.bin \
# 		src/dzx-forth.symbols.z80s
# dzx-forth.tap : dzx-forth.bin

# ----------------------------------------------
# TAP file

forth_block_files = $(wildcard src/*.fb)

block_files.tap : $(forth_block_files)
	for file in $(forth_block_files); do \
		bin2tap $$file $$file.tap; \
	done; \
	cat src/*.fb.tap > bin/forth_block_files.tap

dzx-forth.tap : disk.tap dzxforth.bin.tap forth_block_files.tap
	cat bin/disk.tap bin/dzxforth.bin.tap bin/forth_block_files.tap > bin/dzx-forth.tap

.PHONY : tap
tap:
	@make dzx-forth.tap

# ----------------------------------------------
# DSK disk image

dzx-forth_720k.dsk : dzx-forth.tap
	tap2dsk -720 -label DZXForth bin/dzx-forth.tap bin/dzx-forth_720k.dsk

#dzx-forth_720k.dsk : \
#		dzx-forth.tap \
#		src/*.fs \
#		src/*fb
#	mkp3fs -720 -label DZXForth bin/dzx-forth_720k.dsk src/*.fb src/*.fbs src/*.fs

.PHONY : dsk
dsk:
	@make dzx-forth_720k.dsk

