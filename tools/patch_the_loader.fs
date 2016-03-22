#! /usr/bin/env gforth

\ This file is part of DZX-Forth
\ http://programandala.net/en.program.dzx-forth.html

\ This program patches the source of the DZX-Forth BASIC loader
\ with the proper code address, extracted from the Z80 symbols
\ file created during the compilation.

\ This program is written in Forth with Gforth
\ (http://gnu.org/software/gforth/).

\ ==============================================================
\ History

\ 2015-02-13

\ ==============================================================
\ Requirements

\ Gforth path to user local sources
\ required until Gforth supports <.gforthrc> (planned for v0.8);
\ change to suit your own directory:
fpath path+ ~/forth

\ Galope library
\ (http://programandala.net/en.program.galope.html)
require galope/replaced.fs
require galope/unslurp-file.fs

\ ==============================================================
\ Constants

\ XXX FIXME -- make the path relative:
s" ~/zx_spectrum/dzx-forth/src/"      2constant dir

dir s" dzx-forth_symbols.z80s" s+     2constant symbols-file
dir s" dzx-forth_loader.bas.raw" s+   2constant raw-loader-file
dir s" dzx-forth_loader.bas" s+       2constant loader-file

\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ Main

: haystack  ( -- ca len )
  \ Return the contents of the symbols file.
  symbols-file slurp-file
  ;
: needle  ( ca len -- ca' len' )
  \ Convert the symbol to the string actually searched for,
  \ surrounded by new line and horizontal tab characters.
  s\" \n" 2swap s+ s\" \t" s+
  ;
: symbol$@ ( ca1 len1 -- ca2 len2 )
  \ Feth the string value of a symbol from the symbols file.
  \ ca1 len1 = symbol defined in the Z80 symbols file
  \ ca2 len2 = its hex value as a text string
  needle haystack 2swap search 0= abort" Z80 symbol not found"
  s" EQU " search 2drop 4 + 5 \ get the string of the symbol value
  ;
: hex>decimal$  ( ca1 len1 -- ca2 len2 )
  \ Convert a hex number to decimal, both in strings.
  hex evaluate decimal s>d <# #s #>
  ;
: patch  ( -- )
  raw-loader-file slurp-file
  s" start" symbol$@ hex>decimal$
  s" XXXXX" replaced
  loader-file unslurp-file
  ;

patch bye

\ vim: ts=2:sts=2:et:tw=64
