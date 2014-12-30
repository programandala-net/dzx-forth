#!/bin/sh

# 2014-12-16: First version.
# 2014-12-28: Much improved.
# 2014-12-29: 'tap2dsk' added (from Taptools, by Jhon Elliot).
# 2014-12-30: Abandoned in favor of Makefile.

# XXX TODO versions with/out floating support, using Pasmo's command line to
# set the labels.

all=0

if [ "$#" -gt 1 ]
then
  echo "Usage:"
  echo "$0 [all | tap | doc ]"
  exit 1
elif [ "$#" -eq 0 -o "$1" = "all" ]
then
  all=1
fi

if [ "$1" =  "tap" -o "$all" -eq 1 ]
then
  echo "Assembling to TAP file..."
  cd src
  pasmo --name DZXForth --tapbas dzx-forth.z80s ../dzx-forth.tap && \
  echo "Creating the DSK file..." && \
  tap2dsk -720 -label DZXForth ../dzx-forth.tap ../dzx-forth.dsk
  cd - > /dev/null
fi

if [ "$1" =  "doc" -o "$all" -eq 1 ]
then
  echo "Making HTML files..."
  cd doc
#  asciidoctor dzx-forth.adoc -o dzx-forth.html
  asciidoctor dzx-forth_glossary.adoc -o dzx-forth_glossary.html
  cd - > /dev/null
fi
