\ Information

These are the default tools loaded when DX-Forth is initially
built.

Users may change or add to the tools as needed.










\ Load block
forth definitions  decimal  system  warning off

marker -TOOLS

cr .( loading Tools )  2 #screens 1- thru

forth definitions  decimal  application  warning on








\ Subroutines
: l/s ( -- n )  b/buf c/l / ;  \ lines/screen

: esc? ( -- flag )  \ test key - space resumes
  key? dup if  key bl = if drop  key bl <> then  then ;

: .line ( line blk -- )
  block swap  c/l *  +  c/l -trailing  type ;








\ .S
\ Display stack
: .S ( ? -- ? )
  cr  depth ?dup if
    0 do  depth i - 1- pick .
  loop  then  ." <stack "
  [defined] fdrop [if]
  fdepth ?dup if
    0 do  fdepth i - 1- fpick fs.
    loop  ." <f-stack "
  then  [then] ;





\ LIST L N B
\ List screen n
: LIST ( n -- )
  cr  0  over  scr 2!  3 spaces  u.
  l/s 0 do
    cr  i 2 .r space  i  scr @ .line
  loop ;

: L ( -- )  scr @ list ;   \ list current screen
: N ( -- )  1 scr +! l ;   \ list 'next' screen
: B ( -- )  -1 scr +! l ;  \ list previous screen 'back'





\ WORDS-LIKE WORDS
\ List words in the context vocabulary
: .words ( a u -- )
  2>r  cr  0  context @  w>name
  begin  esc? not and  ?dup while
    dup (name) 2r@ caps search  -rot 2drop if
      dup c@ $40 and $1D - bl max emit limit over count
      $1F and + cell+ @ u< [char] | and bl max emit space
      dup .id  swap 1+ swap  out @ 64 < if  20 out @
      over mod - spaces  else  cr  50 ms  then
    then  n>name
  repeat  2r> 2drop  cr . ." words" ;

: WORDS-LIKE ( "pattern" -- )  token .words ;
: WORDS ( -- )  here 0 .words ;
behead .words .words
\ DUMP
: h.n ( u n -- )
  base @ >r  hex  <# 0 tuck do # loop #> type  r> base ! ;

\ Dump u bytes in hex and ascii
: DUMP ( offs u -- )
  cr  3 spaces  16 0 do  over i +  2 spaces  1 h.n  loop
  over + swap ?do
    cr  50 ms  i 4 h.n space
    16 0  do  i j + c@  2 h.n space  loop
    16 0  do  i j + c@  dup bl 127 within
      if  emit  else  drop  [char] . emit  then
    loop  esc? if leave then
  16 +loop ;


\ VOCS ORDER
\ List all vocabularies
: VOCS ( -- )
  voc-link begin  @ ?dup while  dup cell- .voc space  repeat ;

: ORDER ( -- )  context 2@
  cr ."  context: " .voc
  cr ."  current: " .voc ;

: .file ( -- )  file?
  if  scr ?  loadfile type  ."  ("  #screens 0 u.r  ." )"
  else  ." ---"  then  swap-file ;

: .stat ( u1 u2 -- )  swap 5 u.r ."  (" 5 u.r ."  free)" ;


\ FYI
\ Display statistics
: FYI ( -- )
  sys @  cr ." Dictionary"  dp 2@
  cr ."   applic: "  $100 -  application unused  .stat
  cr ."   system: "  limit -  system unused  .stat
  cr ." Wordlist  "  vocs  order
  cr ." Compile   "  dup if ." SYSTEM" else ." APPLICATION"
  then  sys !
  cr ." Path"  6 spaces  0 path if  2drop  else  type  then
  cr ." Scr file  " .file  cr  10 spaces .file ;

behead .file .stat



\ INDEX
\ Display index line of screens n1 thru n2
: INDEX ( n1 n2 -- )
  1+ swap do
    cr  50 ms
    i 3 .r space
    0 i .line
    esc? if leave then
  loop ;







\ QX
\ Display 'quick index' starting at screen n
: QX ( n -- )
  page  60 0 do
    i 20 /mod  26 *  swap at-xy
    dup #screens u< if
      dup 3 .r  space
      dup block 2+  21 type
    then  1+
  loop drop  cr ;






\ SHOW LISTING
\ List screens n1 thru n2 in triads to printer
: SHOW ( n1 n2 -- )
  printer
  1+  swap 3 /  3 *  do
    cr ." Page "  i 3 /  1+ .
    11  out @ -  spaces  loadfile -path type
    i 3 +  i do
      cr  i dup #screens u< and  list
    loop  cr page  esc? if leave then  3
  +loop
  console ;

\ List all screens to printer
: LISTING ( -- )  0  #screens 1-  0 max  show ;

\ DIR
\ List disk directory
\ : DIR ( "path:filename" -- )
\   getfilename >fcb setusr
\   $80 setdma  ( fcb) 17 bdos dup 255 - if
\     cr begin
\       out @  c/l > if  cr  50 ms  then
\       32 * $80 + 1+ 8 2dup type ." ." + 3 type  3 spaces
\       0 18 bdos dup 255 =
\       esc? or
\     until
\   then drop rstusr ;




\ DELETE RENAME
\ Delete disk file
: DELETE ( "filename" -- )
  getfilename delete-file abort" can't delete file" ;

\ Rename disk file
: RENAME ( "oldfilename" "newfilename" -- )
  $80 getfilename >r  over r@ cmove  r> getfilename
  rename-file abort" can't rename file" ;







\\ INCLUDE
Load forth text source files

INCLUDE  ( "filename[.F]" -- )  \ load text file "filename"
INCLUDED ( c-addr u -- )        \ load named text file
ICLOSE   ( -- )                 \ close include file
LOADLINE ( -- u )               \ line number being loaded
\\       ( -- )                 \ skip remainder file/screen

Text files are automatically closed.  If an error occurs
only the current file is closed.  Use \\ to skip compilation
as QUIT will leave files open and cause loading problems.
Should the latter occur use ICLOSE or CLOSE-ALL.



\ INCLUDE
variable LOADLINE
fdb value tfdb

: fd    ( -- a )    tfdb @ ;
: tid   ( -- a )    fd cell+ ;
: tpos  ( -- a )    fd [ 2 cells ] literal + ;
: tfnb  ( -- a )    fd [ 4 cells ] literal + ;
: fname ( -- a u )  tfnb count ;

: ICLOSE ( -- )  fd @ if  tid @ close-file drop  fd off  then ;

: tread ( -- a u flag )  $80  dup #126 tid @ read-line throw
  >r  2dup over + swap ?do i c@  bl max  i c! loop  r> ;


\ INCLUDE
\ is source a text file
: tf? ( -- flag )  fd @  blk @ 0=  and ;

: tfill ( -- flag )
  tid @ file-position throw  tpos 2!
  tread >r  'source 2!  >in off  1 loadline +!  r> ;

: ?line ( -- )  \ reload current line
  fd @ if
    tpos 2@ tid @ reposition-file throw  tread drop 2drop
  then ;




\ INCLUDE
: tincl ( a u fid -- )
  tfdb loadline @ 2>r  source 2>r  >in @  blk @  2>r
  fdb to tfdb  fd [ 4 cells ] literal  erase  tid !
  16 min tfnb place  fname upper  fd on  loadline off
  begin  tfill  while  blk off  interpret  repeat  iclose
  2r> blk !  >in !  2r> 'source 2!  2r> loadline !  to tfdb
  ?line  ?block ;

: INCLUDED ( a u -- )
  2dup r/o open-file  abort" can't open file"
  ['] tincl catch ?dup if  fd @ if  cr fname type
  ."  Line "  loadline @ u.  iclose  then  throw  then ;

: INCLUDE ( "filename" )  getfilename s" fs" +ext included ;

\ INCLUDE
: ( ( "ccc<delim>" -- )
  tf? if
    begin  [char] ) parse + source + =
    while  tfill 0=  until then
  else  postpone (  then ;  immediate

: \\ ( -- )
  tf? if
    begin  tfill 0=  until
  else  postpone \\  then ;  immediate

: LOADFILE ( -- a u )
  tf? if  fname  else  loadfile  then ;


\ INCLUDE
\ new REFILL
:noname ( -- flag )
  tf? if
    tfill
  else  [ addr refill @ compile, ]  then ;  ( xt)

\ restore old REFILL if new forgotten
:noname [ addr refill @ ] literal is refill ; remember

( xt) is refill

behead tfdb fname
behead tread tincl


\ (*
\ Block comment  end with '*)'
: (* ( "ccc <delim>" )
  begin
    token dup if
      s" *)" compare
    else  2drop refill  then  0=
  until ; immediate








\ Delete headers
\ behead l/s .line
\ behead h.n h.n





























