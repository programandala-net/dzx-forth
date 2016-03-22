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

# Gforth (by Anton Ertl, Bernd Paysan et al.)
#   http://gnu.org/software/gforth

# Asciidoctor (by Dan Allen)
#   http//asciidoctor.org

################################################################
# Change history

# See at the end of the file.

################################################################
# TODO

# New: Versions with/out floating support, using Pasmo's command
# line to set the labels.
#
# Fix: Filenames can be 12 chars long in the disk
# ("filename.ext"), but they are limited to 10 chars because of
# the intermediate TAP file used to create de DSK with
# 'tap2dsk'.  'mkp3fs' doesn't have that limitation, because it
# creates the DSK directly from host system files, but how to
# create a host system file with the BASIC loader, in other
# format than TAP, and, the main problem, how to make 'mkp3fs'
# to save it as a BASIC file into the disk? It seems there's no
# way. The only solution I can think of is to automatically
# build a BASIC program, using Forth and BAS2TAP, that will
# rename the wrong filenames, and include it into the DSK.

################################################################
# Config

VPATH = src:doc:bin
MAKEFLAGS = --no-print-directory

.ONESHELL:

################################################################
# Main

.PHONY : all
all : dsk1 dsk2
#all : dsk1 dsk2 doc

# Disk 1
.PHONY : dsk1
dsk1:
	@make dzx-forth.dsk

# Disk 2 
.PHONY : dsk2
dsk2:
	@make dzx-forth_block_files.dsk

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
# Intermediate TAP files

# ----------------------------------------------
# Disk BASIC loader

# Note: <./tools/patch_the_loader.fs> is a Forth program that
# patches the DZX-Forth BASIC loader with the load and start
# addresses, taken from the kernel's symbols file.

# XXX FIXME -- <./tools/patch_the_loader.fs> depends on the
# Galope library
# (http://programamandala.net/en.program.galope.html), which is
# not published. A temporary solution is to remove the
# following recipe, as long as the address of the Forth system
# is not changed:

dzx-forth_loader.bas : dzx-forth_loader.bas.raw dzx-forth_kernel.tap
	@make dzx-forth_kernel.tap
	./tools/patch_the_loader.fs

dzx-forth_loader.tap : dzx-forth_loader.bas
	bas2tap -a10 -sDISK src/dzx-forth_loader.bas bin/dzx-forth_loader.tap

# ----------------------------------------------
# Kernel

dzx-forth_kernel.tap : dzx-forth.z80s
	pasmo -I src --name "DZXFORTH.B" --tap \
		src/dzx-forth.z80s bin/dzx-forth_kernel.tap \
		src/dzx-forth_symbols.z80s

# ----------------------------------------------
# Main TAP file

# The main TAP file is created only in order to create the DSK
# file from it. DZX-Forth can be loaded into a ZX Spectrum
# emulator using the TAP file, but the system itself has no tape
# support yet.

# XXX TODO convert fsb to fb
forth_fsb_files = $(wildcard src/*.fbs)

forth_block_files = $(wildcard src/*.fb)
forth_stream_files = $(wildcard src/*.fs)

dzx-forth_block_files.tap : $(forth_block_files)
	cd src ; \
	for file in $$(ls -1 *.fb); do \
		bin2code $$file $$file.tap; \
	done; \
	cat *.fb.tap > ../bin/dzx-forth_block_files.tap ; \
	rm -f *.fb.tap ; \
	cd -

dzx-forth_stream_files.tap : $(forth_stream_files)
	cd src ; \
	for file in $$(ls -1 *.fb); do \
		bin2code $$file $$file.tap; \
	done; \
	cat *.fb.tap > ../bin/dzx-forth_stream_files.tap ; \
	rm -f *.fb.tap ; \
	cd -

dzx-forth.tap : \
		dzx-forth_loader.tap \
		dzx-forth_kernel.tap \
		dzx-forth_block_files.tap
	cat \
		bin/dzx-forth_loader.tap \
		bin/dzx-forth_kernel.tap \
		bin/dzx-forth_block_files.tap \
		> bin/dzx-forth.tap ; \
	rm -f bin/dzx-forth_*.tap ; \

.PHONY : tap
tap:
	@make dzx-forth.tap

################################################################
# DSK disk images

# ----------------------------------------------
# Main disk, with the Forth system
# and (temporarily) the block files
# (copied from an intermediate TAP file,
# thus created with +3DOS headers):

# XXX TMP

dzx-forth.dsk : dzx-forth.tap
	tap2dsk -720 -cpmonly -label DZXForth bin/dzx-forth.tap bin/dzx-forth.dsk

# ----------------------------------------------
# Secondary disk, with the block files
# (copied directly from the host system,
# thus created without +3DOS headers):

# XXX TMP

dzx-forth_block_files.dsk : $(forth_block_files) $(forth_stream_files)
	mkp3fs -720 -cpmonly -label DZXForth \
		bin/dzx-forth_block_files.dsk \
		$(forth_block_files) \
		$(forth_stream_files)

################################################################
# Clean

.PHONY : clean
clean:
	rm -f bin/*.tap ; rm -f bin/*.dsk

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
#
# 2015-03-10: Changes.
#
# 2016-03-21: Updated comments.
