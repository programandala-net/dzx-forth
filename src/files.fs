\ files.fs
\ 
\ This file is part of DZX-Forth
\
\ This file contains words extracted from <tools.fs> in order
\ to document them and implement them in the kernel of
\ DZX-Forth.
\
\ 2015-02-08: Code extracted from <tools.fs>.
\
\ 2015-02-13: A copy is converted to <files.fbs> in order to
\ load it into DZX-Forth and debug the code. This remains the
\ main file.


\ Unix-like commands

: RM ( "filename" -- )
  \ Delete a file.
  parse-filename delete-file
  ;
: MV ( "currentfilename" "newfilename" -- )
  \ Rename a file.
  parse-filename parse-filename 2swap rename-file
  ;






( description block )
\ Text files are automatically closed.  If an error occurs only
\ the current file is closed.  Use '\\' to skip compilation as
\ 'QUIT' will leave files open and cause loading problems.
\ Should the latter occur use 'ICLOSE' or 'CLOSE-ALL'.

variable LOADLINE  \ line number being loaded
fdb value tfdb

: fd           ( -- a )  tfdb @  ;
: fs-file-id   ( -- a )  fd cell+  ;
: fs-position  ( -- a )  fd [ 2 cells ] literal +  ;
: fs-filename  ( -- a )  fd [ 4 cells ] literal +  ;



( iclose tread tf? )
: ICLOSE ( -- )
  \ Close the included file.
  fd @ if  fs-file-id @ close-file throw  fd off  then
  ;

: tread ( -- ca len flag )
  $80  dup #126 fs-file-id @ read-line throw
  >r  2dup over + swap ?do i c@  bl max  i c! loop  r>  ;

: tf? ( -- flag )
  \ Is the source a text file?
  fd @  blk @ 0=  and  ;


( tfill ?line )
: tfill ( -- flag )
  fs-file-id @ file-position throw  fs-position 2!
  tread >r  'source 2!  >in off  1 loadline +!  r>
  ;

: ?line ( -- )
  \ Reload the current line
  fd @ if
    fs-position 2@ fs-file-id @ reposition-file throw
    tread drop 2drop
  then
  ;




( (included) )
: (included) ( ca len fid -- )
  \ ca len = filename
  tfdb loadline @ 2>r source 2>r  >in @  blk @  2>r
  fdb to tfdb
  fd [ 4 cells ] literal  erase  fs-file-id !
  16 min fs-filename place  fs-filename count upper
  fd on  loadline off
  begin   tfill
  while   blk off  interpret
  repeat  iclose
  2r> blk !  >in !  2r> 'source 2!  2r> loadline !  to tfdb
  ?line  ?block
  ;


( included include )
: INCLUDED ( ca len -- )
  \ ca len = filename
  2dup r/o open-file throw
  ['] (included) catch ?dup if
    fd @ if  cr fs-filename count type
    ."  Line "  loadline @ u.  iclose  then
  throw  then
  ;
: INCLUDE ( "filename" -- )
  parse-filename s" fs" +ext included  ;






( ( refill )
: ( ( "ccc<delim>" -- )
  \ Line comment -- end with ')'
  tf? if
    begin  [char] ) parse + source + =
    while  tfill 0=  until then
  else  postpone (  \ )
  then
  ;  immediate

: \\ ( -- )
  \ Skip the remainder of the file or screen.
  tf?   if  begin  tfill 0=  until
  else  postpone \\  then
  ;  immediate

: SOURCE-FILENAME ( -- ca len )  \ XXX NEW name
  tf? if  current-stream-file  else  current-block-file  then
  ;
:noname ( -- flag )
  \ New 'refill'
  tf? if  tfill
  else    [ addr refill @ compile, ]  \ XXX TODO use 'defers'
  then
  ;  is refill

: (* ( "ccc<delim>" )
  \ Block comment -- end with '*)'
  begin
    parse-name dup
    if    s" *)" compare
    else  2drop refill  then  0=
  until
  ;  immediate