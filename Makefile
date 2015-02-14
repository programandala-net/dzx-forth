# DZX-Forth Makefile

# By Marcos Cruz (programandala.net)
# http://programandala.net/en.program.dzx-forth.html

################################################################
# Requirements

# Pasmo (by Julián Albo)
#   http://pasmo.speccy.org/

# tap2dsk from Taptools (by John Elliott)
#   http://www.seasip.info/ZX/unix.html

# bin2code (by MetalBrain)
#   http://metalbrain.speccy.org/link-eng.htm

# bas2tap (by Martijn van der Heide)
#   Utilities section of
#   http://worldofspectrum.org

################################################################
# Change history

# See at the end of the file.

################################################################
# TODO

# - Versions with/out floating support, using Pasmo's command
#   line to set the labels.

################################################################
# Config

VPATH = src:doc:bin
MAKEFLAGS = --no-print-directory

.PHONY : all
all : dsk doc

.ONESHELL:

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


dzx-forth_loader.bas : dzx-forth_loader.bas.raw dzx-forth_kernel.tap
	@make dzx-forth_kernel.tap
	./_tools/patch_the_loader.fs

# Note: <./_tools_patch_the_loader.fs> called above is a Forth
# program that patches the DZX-Forth BASIC loader with the load
# and start address of the kernel.

dzx-forth_loader.tap : dzx-forth_loader.bas
	bas2tap -a10 -sDISK src/dzx-forth_loader.bas bin/dzx-forth_loader.tap

# ----------------------------------------------
# Kernel

dzx-forth_kernel.tap : dzx-forth.z80s
	pasmo -I src --name "DZXFORTH.B" --tap \
		src/dzx-forth.z80s bin/dzx-forth_kernel.tap \
		src/dzx-forth_symbols.z80s

# ----------------------------------------------
# TAP file

# Note: The TAP file is created only in order to create the DSK
# file from it. DZX-Forth can be loaded into a ZX Spectrum
# emulator using the TAP file, but the system itself has no tape
# support.

forth_block_files = $(wildcard src/*.fb)

dzx-forth_block_files.tap : $(forth_block_files)
	cd src ; \
	for file in $$(ls -1 *.fb); do \
		bin2code $$file $$file.tap; \
	done; \
	cat *.fb.tap > ../bin/dzx-forth_block_files.tap ; \
	rm -f *.fb.tap ; \
	cd - > /dev/null

dzx-forth.tap : dzx-forth_loader.tap dzx-forth_kernel.tap dzx-forth_block_files.tap
	cat \
		bin/dzx-forth_loader.tap \
		bin/dzx-forth_kernel.tap \
		bin/dzx-forth_block_files.tap \
		> bin/dzx-forth.tap

.PHONY : tap
tap:
	@make dzx-forth.tap

# ----------------------------------------------
# DSK disk image

dzx-forth.dsk : dzx-forth.tap
	tap2dsk -720 -label DZXForth bin/dzx-forth.tap bin/dzx-forth.dsk

.PHONY : dsk
dsk:
	@make dzx-forth.dsk

################################################################
# Notes about GNU make

# $? list of dependencies changed more recently than current target
# $@ name of current target
# $> name of current dependency
# $* name of current dependency without extension

################################################################
# Change history

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
#
# 2015-02-14: Rewritten: the DSK includes not only the Forth
# block files but also the loader and the kernel; no TAP needed
# anymore, though a whole TAP is created as part of the process.
# <tap2dsk> has to be used instead of <mkp3fs>.
