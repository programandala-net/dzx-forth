\ Information

NEWAPP is a skeletal program that allows users to quickly
develop an MS-DOS application. It provides often needed
tasks including error handling, command-line parsing, file
operations, buffered I/O, help screen, number and string
functions.

NEWAPP comprises two parts:

  NEWAPP.SCR   skeletal main program
  DOSLIB.SCR   function support library




\ Module loader

cr .( DOSLIB  2013-07-17 ) cr

base @  sys @  decimal  system

-? loadfile -path 2constant lib

-? : MODULE  2constant does> 2@ lib loaded ;

2 load  behead lib module

sys !  base !



\ Module directory - NEWAPP support
 5 6    module _Errors          \ error handler
 7 dup  module _Inout1          \ number output
 8 dup  module _Inout2          \ string & number input
 9 dup  module _Compare1        \ basic compare
 10 12  module _String1         \ basic strings
 13 dup module _String2         \ extra strings
 14 19  module _Parsing         \ command-line parsing
 20 22  module _Fileprims       \ file primitives
 23 31  module _Files           \ default files
 32 dup module _Bufinfile       \ buffered input file
 33 dup module _Bufoutfile      \ buffered output file
\ 34 dup module _Dos1            \ dosver dta
\ 35 dup module _Dos2            \ ctl-brk int
-->

\ Module directory - DOS & misc
 36 dup module _Disk            \ disk
\ 37 dup module _Memory          \ memory allocate
 38 39  module _Timedate1       \ time/date
 40 dup module _Timedate2       \ time/date
 41 dup module _Timepack        \ time/date packing
 42 dup module _Filematch       \ file find first/next
 43 dup module _Filestamp       \ file stamp/attribute
\ 44 dup module _Diskdir         \ directory
\ 45 dup module _Env             \ environment
\ 46 48  module _Exec            \ exec prog/command
\ 49 50  module _Video1          \ textcolor attrib cursor
\ 51 dup module _Video2          \ mode page
-->


\ Module directory - DOS & misc
\ 52 dup module _Timing1         \ timer
\ 53 dup module _Timing2         \ delay
\ 54 55  module _Device1         \ 8087 cpu keybd












\ Errors - +IS ?THROW ?CATCH
system
\ Add new behaviour to existing deferred word
: +IS ( xt <name> -- )  >r  :noname  r> compile,
  ' >body  dup >r  @ compile,  postpone ;  r> ! ;
application

\ THROW exception code n if flag is non-zero
: ?THROW ( flag n -- )  swap 0<> and throw ;

\ Perform CATCH intercepting exception code n only
: ?CATCH ( xt n -- n )  >r catch dup r> <> over and throw ;




\ Errors - ERROR1 ERROR2 ESC= ESCKEY? (?BREAK)
\ Quit to DOS with no msg and return code = 1
: ERROR1 ( -- )  abort ;

\ Quit with abort msg
: ERROR2 ( -- )  ."  ... aborting"  error1 ;

\ Test char for ESC or Ctrl-C
: ESC= ( char -- flag )  dup 27 =  swap 3 =  or ;

\ Check if ESC or Ctrl-C key was pressed
: ESCKEY? ( -- flag )  0  key? if  key esc=  or  then ;

\ Check user break
: (?BREAK) ( -- )  esckey? if  beep cr
  ." User break - exit program? " y/n if  error2  then  then ;
\ Inout1 - Number output
: (UD.) ( ud -- addr u )  <# #s #> ;
: (U.)  ( u  -- addr u )  0 (d.) ;
: (.)   ( n  -- addr u )  s>d (d.) ;
: UD.   ( ud -- )  (ud.) type space ;
: UD.R  ( ud n -- )  >r (ud.) r> over - spaces type ;

: (DH.) ( ud -- addr u )
  base @ >r hex <# 8 0 do # loop #> r> base ! ;
: (H.)  ( u -- addr u )  0 (dh.) 4 /string ;
: (HB.) ( u -- addr u )  (h.) 2 /string ;
: DH.   ( ud -- )  (dh.) type space ;
: H.    ( u -- )  (h.) type space ;
: HB.   ( u -- )  (hb.) type space ;


\ Inout2 - INPUT INPUT# BACKSPACES
\ Input string n chars max  false = empty or blanks
: INPUT ( n -- c-addr u true | false )
  here  dup rot accept  bl skip  -trailing
  dup if  -1  else  and  then ;

\ Input number n chars max  false = empty or blanks
: INPUT# ( n -- d true | false )
  input dup if  drop number?  then ;

\ Display n backspace characters
: BACKSPACES ( n -- )  0 max  0 ?do  8 emit  loop ;




\ Compare1 - DIGIT? ALPHA?
\ Return true if char is decimal digit
: DIGIT? ( char -- flag )  [char] 0 - 10 u< ;

\ Return true if char is alphabetical
: ALPHA? ( char -- flag )  upcase [char] A - 26 u< ;










\ String1 - SPLIT /SPLIT STRING/
\ Split string at character leaving first on top
: SPLIT ( a u c -- a2 u2 a3 u3 )
  >r 2dup r> scan 2swap 2 pick - ;

\ Split string at index n leaving first on top
: /SPLIT ( a u n -- a2 u2 )
  >r over r@ 2swap r> /string 2swap ;
\ code /SPLIT ( a u n -- a2 u2 )  ax pop  sp bx mov
\  2 [bx] push  ax 0 [bx] sub  ax 2 [bx] add  1push  end-code

\ Return u right-most characters of string
: STRING/ ( a1 u1 u -- a2 u2 )  >r + r@ - r> ;
\ code STRING/ ( a1 u1 u -- a2 u2 )
\  ax pop  bx pop  dx pop  bx dx add  ax dx sub  2push  end-code

\ String1 - C+STRING C/STRING STRING/C
\ Append character to end of string
: C+STRING ( c a u -- a2 u2 )  2dup 2>r + c! 2r> 1+ ;
\ code C+STRING ( c a u -- a2 u2 )  cx pop  bx pop  ax pop
\  bx push  cx bx add  cx inc  cx push  al 0 [bx] mov  next
\ end-code

\ Extract character from start of string
: C/STRING ( a u -- a2 u2 c )  1 /string over 1- c@ ;
\ code C/STRING ( a u -- a2 u2 char )  cx pop  ax pop  ax bx mov
\  cx dec  ax inc  ax push  cx push  ' c@ 1+ ) jmp  end-code

\ Extract character from end of string
: STRING/C ( a u -- a2 u2 c )  1- 2dup + c@ ;
\ code STRING/C ( a u -- a2 u2 c )  cx pop  bx pop  cx dec
\  bx push  cx push  cx bx add  ' c@ 1+ ) jmp  end-code
\ String1 - S=
\ Compare two strings for equality
: S= ( a1 u1 a2 u2 -- flag )  compare 0= ;













\ String2 - S+ STRING S!
255 ( buffer size )  -? create sb  dup , allot

\ Concatenate two strings placing result in temp buffer
: S+ ( a1 u1 a2 u2 -- a3 u3 )  2>r  sb @ umin  sb cell+ 0
  +string  sb @  over -  2r> rot min  2swap +string ;
behead sb sb
\ Define string variable with max length u
-? : STRING  create ( u -- )  255 min dup c, 0 c, allot
  does> ( -- sa su )  1+ count ;

\ Store string a u to string variable
: S! ( a u sa su -- )  drop  1- dup >r  1- c@ umin  r> place ;



\ Parsing - 'ARG ARG! ARGV
create 'ARG ( -- a )
  1 cells allot  \ current arg  0=none
  2 cells allot  \ string pointer

\ Assign string for argument parsing
: ARG! ( a u -- )  'arg cell+ 2!  'arg off ;  here 0 arg!

\ Parse next blank delimited argument. 'ARG OFF resets.
: ARGV ( -- a u -1 | 0 )
  1 'arg +!  'arg cell+ 2@  0 0
  'arg @  0 ?do
    2drop  bl skip  bl split
  loop  2swap 2drop
  dup if  -1  else  and  then ;

\ Parsing - BADOPTION /NUM /HEXNUM /NUMRANGE
: BADOPTION ( -- )  cr ." Invalid option"  error2 ;

\ Parse number n from string
: /NUM ( a u -- 0 0 n )  number?
  if  drop  else  badoption  then  0 0 ( stop parsing) rot ;

\ Parse hex number n from string
: /HEXNUM ( a u -- 0 0 n )  base @ >r  hex /num  r> base ! ;

\ As for /NUM but checks n3 is in the range n1 to n2
: /NUMRANGE ( a u n1 n2 -- 0 0 n3 )
  2>r  /num  dup 2r> between  0= if badoption then ;



\ Parsing - SETOPTION
\ Process each character in a switch option string
defer SETOPTION ( a u char -- a u )  ' drop is setoption

\\

\ Example of use
: (SETOPTION) ( a u char -- a u )
  upcase [char] A = if  A-variable on  else  badoption  then ;

' (setoption) is setoption





\ Parsing - PARSEOPTION
\ Parse multiple switch options from the command-line
: PARSEOPTION ( -- )
  begin
    argv
  while ( not end )
    c/string
    dup [char] / =  swap [char] - =  or if ( switch )
      begin  dup
      while  c/string setoption
      repeat  2drop
    else
      2drop  -1 'arg +! ( backup )  exit
    then
  repeat ;

\ Parsing - PARSEFILENAME
\ Parse one or more strings/filenames from the command-line
defer PARSEFILENAME ( -- )  ' noop is parsefilename

\\

\ Example of use
: (PARSEFILENAME) ( -- )
  argv 0= if ." no filename specified"  error1  then
  infile setname ;

' (parsefilename) is parsefilename




\ Parsing - PARSECMD
\ Parse command line setting options and filenames
: PARSECMD ( -- )
  cmdtail arg!  parseoption  parsefilename ;












\ Fileprims - ?FERROR
\ Display msg and abort on file error
: ?FERROR ( ior n -- )
  swap if
    cr ." File "  case
      3  of  ." read"  endof
      4  of  ." write"  endof
      5  of  ." position"  endof
      6  of  ." size"  endof
    endcase
    ."  error"  error2
  else  drop  then ;




\ Fileprims - FREAD FWRITE FREADLN FWRITELN
\ Read binary
: FREAD ( a u fid -- a u2 )
  ?break  >r over swap r>  read-file  3 ?ferror ;

\ Write binary
: FWRITE ( a u fid -- )  ?break  write-file  4 ?ferror ;

\ Read text  flag=0 if end-of-file
: FREADLN ( a u fid -- a u2 flag )
  ?break  >r over swap r>  read-line  3 ?ferror ;

\ Write text
: FWRITELN ( a u fid -- )  ?break  write-line  4 ?ferror ;


\ Fileprims - FPOS FREPOS FSIZE FRESIZE
\ File position
: FPOS ( fid  -- ud )  file-position  5 ?ferror ;

\ Reposition file
: FREPOS ( ud fid  -- )  reposition-file  5 ?ferror ;

\ File size
: FSIZE ( fid  -- ud )  file-size  6 ?ferror ;

\ Resize file
\ : FRESIZE ( ud fid  -- )  resize-file  6 ?ferror ;




\ Files - HANDLE SETNAME FILENAME .FILE
\ Create file handle
-? : HANDLE ( "name" -- ; -- handle )  create
  0 ,                      \ FID  0=closed
  0 c, 79 chars  allot ;   \ filename

\ Assign filename to a handle
: SETNAME ( a u handle -- )
  dup off  cell+ >r  79 min  r@ place  r> count upper ;

\ Get filename
: FILENAME ( handle -- a u )  cell+ count ;

\ Display filename
: .FILE ( handle -- )  filename type ;

\ Files - FOPEN (FOPEN)
: FOPEN ( fam handle -- ior )
  dup >r  filename rot open-file  tuck  0= and  r> ! ;

: (FOPEN) ( fam handle -- )
  tuck  fopen  ?dup if
    cr  over .file
    $FF and  case
      2  of  ."  file not found"  endof
      3  of  ."  path not found"  endof
      4  of  ."  too many open files"  endof
      5  of  ."  access denied"  endof
      ."  open error"
    endcase  error2
  then  drop ;

\ Files - FMAKE (FMAKE)
variable WRTCHK  wrtchk on          \ overwrite check

: FMAKE ( fam handle -- ior )
  dup >r  filename rot create-file  tuck  0= and  r> ! ;

: (FMAKE) ( fam handle -- )
  tuck  wrtchk @ if ( overwrite check )
    dup filename  r/o open-file  0= if
      close-file
      beep  cr  over .file
      ."  exists - overwrite? "  y/n 0= if error2 then
    then drop
  then  fmake if
    cr  .file  ."  make error"  error2
  then  drop ;
\ Files - FCLOSE
: FCLOSE ( handle -- ior )
  dup  @ dup if  close-file  then  swap off ;













\ Files - default handles
handle INFILE       \ input file handle
handle OUTFILE      \ output file handle

0 value  INBUF      \ input file buffer
variable INSIZ
variable INPTR

0 value  OUTBUF     \ output file buffer
variable OUTSIZ
variable OUTPTR

: RESETINBUF  ( -- )  inbuf inptr !  insiz off ;
: RESETOUTBUF ( -- )  outbuf outptr !  outsiz off ;


\ Files - OPENINFILE MAKEOUTFILE OPENOUTFILE
\ Open file for input using file access mode
: OPENINFILE ( fam -- )  infile (fopen)  resetinbuf ;

\ Create file for output using file access mode
: MAKEOUTFILE ( fam -- )  outfile (fmake)  resetoutbuf ;

\ Open existing file for output using file access mode
: OPENOUTFILE ( fam -- )  outfile (fopen)  resetoutbuf ;







\ Files - CLOSEINFILE CLOSEOUTFILE CLOSEFILES
defer (FLUSHWRITE)  ' false is (flushwrite)

\ Close input file - errors not reported
: CLOSEINFILE ( -- )  infile fclose drop ;

\ Close output file - errors not reported
: CLOSEOUTFILE ( -- )
  outfile  dup @ if  (flushwrite) drop  then  fclose drop ;

\ Close all files - errors not reported
defer CLOSEFILES ( -- )

:noname ( -- )  closeinfile  closeoutfile ;  is closefiles

' closefiles +is errfix  \ close files on error
\ Files - DELOUTFILE REPOSIN/OUTFILE IN/OUTFILEPOS
\ Close and delete output file - errors not reported
: DELOUTFILE ( -- )  outfile @  closeoutfile
  if  outfile filename  delete-file drop  then ;

\ Reposition input file to position d
: REPOSINFILE ( ud -- )  infile @ frepos resetinbuf ;

\ Reposition output file to position d
: REPOSOUTFILE ( ud -- )  (flushwrite) drop  outfile @ frepos ;

\ Input file position
: INFILEPOS ( -- ud )  infile @ fpos  insiz @  0 d- ;

\ Output file position
: OUTFILEPOS ( -- ud )  outfile @ fpos  outsiz @  0 d+ ;
\ Files - READDATA WRITEDATA READTEXT WRITETEXT
\ Read binary from input file
: READDATA ( a u1 -- a u2 )  infile @ fread ;

\ Write binary to output file
: WRITEDATA ( a u -- )  outfile @ fwrite ;

\ Read text from input file  flag=0 if end-of-file
: READTEXT ( a u1 -- a u2 flag )  infile @ freadln ;

\ Write text to output file
: WRITETEXT ( a u -- )  outfile @ fwriteln ;




\ Bufinfile - REFILLREAD READCHAR
here to INBUF  1024 allot  resetinbuf

\ Refill read buffer
: REFILLREAD ( -- )
  resetinbuf  inbuf 1024 readdata  insiz !  drop ;

\ Read char from buffered input file
\ : READCHAR ( -- char -1 | 0 )  insiz @ 0= if refillread then
\  insiz @ if inptr @ c@ 1 inptr +! -1 insiz +! -1 else 0 then ;

code READCHAR ( -- char -1 | 0 )  insiz lhld  l a mov  h ora
  1 $ jnz  c: refillread ;c  1 $:  insiz lhld  l a mov  h ora
  2 $ jz  inptr lhld  m e mov  0 d mvi  d push
  h inx  inptr shld  insiz lhld  h dcx  insiz shld  -1 h lxi
  2 $: 1push  end-code
\ Bufoutfile - FLUSHWRITE WRITECHAR
here to OUTBUF  1024 allot  resetoutbuf

\ Flush write buffer
:noname ( -- ior )  outbuf  outsiz @  outfile @  write-file
  resetoutbuf  ?break ;  is (flushwrite)

: FLUSHWRITE ( -- )  (flushwrite)  4 ?ferror ;

\ Write char to buffered output file
\ : WRITECHAR ( char -- )  outsiz @ 1024 = if flushwrite then
\  outptr @ c! 1 outptr +! 1 outsiz +! ;
code WRITECHAR ( char -- )  1024 d lxi  outsiz lhld  l a mov
  e cmp  1 $ jnz  h a mov  d cmp  1 $ jnz  c: flushwrite ;c
  1 $:  d pop  outptr lhld  e m mov  h inx  outptr shld
  outsiz lhld  h inx  outsiz shld  next  end-code
\ Dos1 - DOSVER GETDTA SETDTA
\ DOS version
\ : DOSVER ( -- minor major )  $30 doscall  'AH c@  'AX c@ ;

\ Get/set DTA address
: GETDTA ( -- seg offs )  $2F doscall  'ES @  'BX @ ;

: SETDTA ( seg offs -- )
  'DX !  'DS !  $1A 'AH c!  $21 intcall ;







\ Dos2 - GETCBRK SETCBRK GETINT SETINT
\ Get/set Ctrl-Brk   0=off 1=on
: GETCBRK ( -- n )  0 'AX c!  $33 doscall  'DX c@ ;
: SETCBRK ( n -- )  'DX c!  1 'AX c!  $33 doscall ;

\ Get/set interrupt
: GETINT ( n -- seg offs )
  'AX c!  $35 doscall  'ES @  'BX @ ;

: SETINT ( seg offs n -- )
  $2500 or 'AX !  'DX !  'DS !  $21 intcall ;





\ Disk - DISKFREE DISKSIZE GETDISK SELDISK RESETDISK
-? : dsk ( n reg -- d )
  swap 'DX c!  $36 doscall  @  'AX @  um* 'CX @  1 m*/ ;

\ Get freespace/size on drive n  0=default 1=A 2=B etc
: DISKFREE ( n -- d )  'BX  dsk ;
: DISKSIZE ( n -- d )  'DX  dsk ;  behead dsk dsk

\ Get/select current drive  0=A 1=B etc
: GETDISK ( -- dsk )  $19 doscall 'AX c@ ;
: SELDISK ( dsk -- )  'DX c!  $0E doscall ;

\ Reset drives - use before disk change, resets DTA
: RESETDISK ( -- )  $0D doscall ;


\ Memory - GETMEM RELMEM SETMEM
\ Allocate u paragraphs of memory
: GETMEM ( par -- seg|maxpar ior )
  'BX !  $48 doscall  doserr?
  dup if  'BX  else  'AX  then  @  swap ;

\ Free previously allocated memory
: RELMEM ( seg -- ior )
  'ES !  $49 doscall  doserr? ;

\ Resize previously allocated memory
: SETMEM ( seg par -- maxpar ior )
  'BX !  'ES !  $4A doscall  'BX @  doserr? ;



\ Timedate1 - TIME DATE !TIME !DATE
\ Get current time/date
: TIME ( -- sec min hour )
  $2C doscall  'DH c@  'CX c@  'CH c@ ;

: DATE ( -- day mon year )
  $2A doscall  'DX c@  'DH c@  'CX @ ;

\ Set current time/date
: !TIME ( sec min hour -- error )
  'CH c!  'CX c!  'DH c!  0 'DX c!  $2D doscall  'AX c@ ;

: !DATE ( day mon year -- error )
  'CX !  'DH c!  'DX c!  $2B doscall  'AX c@ ;


\ Timedate1 - H:M:S D-M-Y M-D-Y Y-M-D
\ Convert time to string
: H:M:S ( sec min hour -- addr u )  swap rot  <#
  2 0 do  0 # # 2drop  [char] : hold  loop  0 # #  #> ;

\ Convert date to string
: D-M-Y ( day mon year -- addr u )  <#  0 # # # #
  2 0 do  2drop  [char] - hold  0 # #  loop  #> ;

\ Convert date to string
: M-D-Y ( day mon year -- addr u )  rot swap d-m-y ;

\ Convert date to string
: Y-M-D ( day mon year -- addr u )  swap rot <#
  2 0 do  0 # # 2drop  [char] - hold  loop  0 # # # # #> ;

\ Timedate2 - $MONTH D-MMM-Y
\ Convert month to string
: $MONTH ( n -- a u )
  1- 3 *  s" JanFebMarAprMayJunJulAugSepOctNovDec" drop + 3 ;

\ Convert date to string
: D-MMM-Y ( day mon year -- a u )
  <#  0 # # # # 2drop  [char] - hold  $month  over + 1-
  do i c@ hold -1 +loop  [char] - hold  0 # #  #> ;







\ Timepack - PACKDATE PACKTIME UNPACKDATE UNPACKTIME
\ Pack date in MSDOS format
: PACKDATE ( day mon year -- date )
  1980 - 9 lshift  swap 15 and  5 lshift  or  swap 31 and  or ;

\ Pack time in MSDOS format
: PACKTIME ( sec min hour -- time )
  11 lshift  swap 63 and  5 lshift  or  swap 2/ 31 and  or ;

\ Unpack MSDOS format date
: UNPACKDATE ( date -- day mon year )
  dup 31 and  swap 5 rshift  dup 15 and  swap 4 rshift 1980 + ;

\ Unpack MSDOS format time
: UNPACKTIME ( time -- sec min hour )
  dup 31 and 2*  swap 5 rshift  dup 63 and  swap 6 rshift ;
\ Filematch - FINDFIRST FINDNEXT DTA.ATTR DTA.TIME DTA.DATE ...
\ Find first matching file. Assume default DTA
: FINDFIRST ( a u attrib -- ior )
  'CX !  >fname 1+ 'DX !  $4E doscall  doserr? ;

\ Find next matching file
: FINDNEXT ( -- ior )  $4F doscall  doserr? ;

\ Matched file data. Assume default DTA.
: DTA.ATTR ( -- attrib )  [ $80 $15 + ] literal c@ ;
: DTA.TIME ( -- time )    [ $80 $16 + ] literal @ ;
: DTA.DATE ( -- date )    [ $80 $18 + ] literal @ ;
: DTA.SIZE ( -- ud )      [ $80 $1A + ] literal 2@ swap ;
: DTA.NAME ( -- addr u )  [ $80 $1E + ] literal zcount ;


\ Filestamp - FILESTAMP !FILESTAMP FILEATTR !FILEATTR
\ Get disk file packed timestamp
: FILESTAMP ( fid -- date time ior )
  'BX !  0 'AX c!  $57 doscall  'DX @  'CX @  doserr? ;

\ Set disk file packed timestamp
: !FILESTAMP ( date time fid -- ior )
  'BX !  'CX !  'DX !  1 'AX c!  $57 doscall  doserr? ;

\ Get disk file attributes
aka file-status FILEATTR ( a u -- attrib ior )

\ Set disk file attributes
: !FILEATTR ( a u attrib -- ior )
  'CX !  >fname 1+ 'DX !  1 'AX c!  $43 doscall  doserr? ;

\ Diskdir - CHDIR MKDIR RMDIR
-? : dir ( a u fn -- ior )
  -rot  >fname 1+ 'DX !  doscall  doserr? ;

\ Directory change/make/remove
: CHDIR ( a u -- ior )  $3B dir ;
: MKDIR ( a u -- ior )  $39 dir ;
: RMDIR ( a u -- ior )  $3A dir ;  behead dir dir








\ Env - ENVSEG GETENV
\ Return DOS environment segment
: ENVSEG ( -- seg )  $2C @ ;

\ Search DOS environment for string a u.  Return null
\ terminated remainder.  Null not in count.
: GETENV ( a u -- seg zadr u2 true | false )
  2>r  envseg  dup sseg !
  0  begin  2dup @l  while  1+  repeat  2+
  r@  0  rot  2r>  caps search
  if  rot /string  2dup 0 scan  nip -  true
  else  2drop 2drop  0  then  cseg sseg ! ;




\ Exec - pb .. !fcb
[undefined] GETDTA [if] _Dos1 [then]
[undefined] GETENV [if] _Env [then]
warning @  warning off
create pb  14  allot  \ parameter block
create ct  128 allot  \ command tail
create f1  37  allot  \ fcb1
create f2  37  allot  \ fcb2

: fcb! ( zadr fcb  -- zadr' )
  'DI !  'SI !  cseg 'ES !  1 'AX c!  $29 doscall  'SI @ ;

: !fcb ( -- )
  ct 1+  f1 fcb!  f2 fcb! drop
  cseg f1 [ pb 6 + ]  literal 2!
  cseg f2 [ pb 10 + ] literal 2! ;
\ Exec - (exec)
: (exec) ( a u seg zadr flag -- ior )
  getdta 2>r
  >r  2swap
  pb 14 erase  cseg ct [ pb 2+ ] literal 2!
  ct 1+ 0  2 pick  r@ and if  s" /C "  2swap +string  then
  +string  dup ct c!  + $0D swap c!
  r> 0= if !fcb then
  'DX !  'DS !  pb 'BX !  cseg 'ES !  $4B00 'AX !
  $21 intcall  doserr?
  2r> setdta ;

warning !



\ Exec - EXEC SHELL RETCODE
\ Execute program
: EXEC ( param u prog u -- ior )
  >fname 1+  cseg swap  false (exec) ;

\ Shell to DOS with optional command
: SHELL ( a u -- ior )
  s" COMSPEC=" getenv and
  if  true (exec)  else  drop  $FEFF  then ;

\ Get subprocess return code
: RETCODE ( -- type code )
  $4D doscall  'AH c@  'AX c@ ;

behead pb (exec)

\ Video1 - text colors
0  constant BLACK       1  constant BLUE
2  constant GREEN       3  constant CYAN
4  constant RED         5  constant MAGENTA
6  constant BROWN       7  constant LTGRAY
8  constant GRAY        9  constant LTBLUE
10 constant LTGREEN     11 constant LTCYAN
12 constant LTRED       13 constant LTMAGENTA
14 constant YELLOW      15 constant WHITE







\ Video1 - BORDER HI -HI BLINK -BLINK SETCUR CURSOR -CURSOR
\ Set text border
: BORDER ( u -- )  'BX !  $B00 'AX !  $10 intcall ;

-? : attr  ( and or -- )  attrib c@  or and  attrib c! ;

\ Set video attribute
: HI     ( -- )  $FF $08 attr ;
: -HI    ( -- )  $F7 0   attr ;
: BLINK  ( -- )  $FF $80 attr ;
: -BLINK ( -- )  $7F $00 attr ;  behead attr attr

\ Cursor set/normal/off
: SETCUR  ( x -- )  'CX !  $100 'AX !  $10 intcall ;
: CURSOR  ( -- )  $0607 setcur ;
: -CURSOR ( -- )  $2000 setcur ;
\ Video2 - VMODE VMODE! VPAGE VPAGE!
\ Get/set video mode
: VMODE  ( -- n )  $F00 'AX !  $10 intcall  'AX c@ ;
: VMODE! ( n -- )  $FF and 'AX !  $10 intcall ;

\ Get/set active video page
: VPAGE  ( -- n )  $F00 'AX !  $10 intcall  'BX 1+ c@ ;
: VPAGE! ( n -- )  dup $106 c!  $500 + 'AX !  $10 intcall ;








\ Timing1 - /TIMER TIMER TICKS>MS .TIMER
\ Get BIOS ticks  1 tick = 54.9254 mS
aka ticks /TIMER ( -- d )  \ Reset timer

\ Get elapsed time in ticks (24 hours max)
: TIMER ( d1 -- d2 )  ticks  2swap d-  dup 0< if
  ( cross midnight)  #1573040. d+  then ;

\ Convert ticks to milliseconds
: TICKS>MS ( d1 -- d2 )  #14006 #255 m*/ ;

\ Display elapsed time in milliseconds
: .TIMER ( d -- )  timer ticks>ms <# #s #> type ."  mS " ;



\ Timing2 - (uSEC) uSEC
\ Wait AL * 0.8381uS  Uses Timer 2
label (uSEC)  \ AL = 127 max
  al ah mov  $61 # al in  $FC # al and
  1 # al or  $EB ,  $61 # al out
  pushf  cli  $90 # al mov  $43 # al out
  $61E4 ,  ah al mov  $42 # al out
1 $: $61E4 ,  $80 # al mov  $43 # al out
  $61E4 ,  $42 # al in  al shl  1 $ jnc
  popf  ret  end-code

\ Wait u * 0.8381uS  Uses Timer 2
code uSEC ( u -- )  \ u = 127 max
  ax pop  (uSEC) ) call  next  end-code


\ Device1 - 8087? CPU?
\ Test/init 80x87
code 8087? ( -- flag )
  ax ax sub  ax push  sp bp xchg  $E3DB , ( FINIT )
  #100 # cx mov  1 $: 1 $ loop  $7ED9 , 0 c, ( FSTCW [BP] )
  sp bp xchg  bx pop  bx bx or  2 $ jz  ax dec  2 $:  1push
end-code

\ Get CPU type
code CPU? ( -- n )  \ n= $86, $286, $386
  pushf  $86 # ax mov  sp push  bx pop  bx sp cmp  1 $ jnz
  2 # ah mov  pushf  bx pop  $F0 # bh or  bx push  popf  pushf
  bx pop  $F0 # bh and  1 $ jz  ah inc  1 $:  popf  1push
end-code


\ Device1 - EH-KEYBOARD?
\ Enhanced keyboard hardware test
: EH-KEYBOARD? ( -- flag )
  $40 $96 c@l  $10 and  0<> ;




























