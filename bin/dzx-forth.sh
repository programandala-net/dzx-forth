#!/bin/sh

# DZX-Forth loader for the Fuse emulator

# This file is part of DZX-Forth
# http://programandala.net/en.program.dzx-forth.html

# History
# 2015-02-14: +3
# 2015-02-22: +3e
# 2015-02-23: automatic graphics filter mode
# 2015-05-13: --debugger-command
# 2016-03-21: improved header

host=$(uname -n)
mode='3x'
if [ "$host" = "kinasus" ] ; then
  mode='2x'
fi
fuse-sdl \
	--graphics-filter $mode \
	--machine plus3e \
	--drive-plus3a-type 'Double-sided 80 track' \
	--drive-plus3b-type 'Double-sided 80 track' \
	--plus3disk dzx-forth.dsk \
	&
  
#--debugger-command 'br 25000' \
