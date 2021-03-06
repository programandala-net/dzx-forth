\ Miser's CASE

Extend CASE with Pascal and C style features

COND
THENS
EQUAL
RANGE
WHEN
CONTINUE
END-CASE (synonym for THENS)

See demo screen for usage



\ Miser's CASE
forth definitions decimal

cr .( loading Miser's CASE )   2 5 thru
\   .( loading Case demo )      6   load











\ Miser's CASE
application

label bran
  b h mov  c l mov  m c mov  h inx  m b mov  next  end-code

code (equal)
  h pop  d pop  cmpu call  bran jz  d push  b inx  b inx
  next  end-code

variable 'h

code (range)
  h pop  d pop  ssub call  xthl  'h shld  ssub call
  d pop  cmpu call  bran jz  bran jc  'h lhld  h push
  b inx  b inx  next  end-code
\ Miser's CASE
system
\ Wil Baden's COND THENS  (now in DX-FORTH kernel)
\ : COND
\  cs-mark ; immediate

\ : THENS
\  begin  cs-test while  postpone then  repeat cs-drop ;
\  immediate

\ Add Pascal-like features
: EQUAL
  postpone (equal) >mark ; immediate

: RANGE
  postpone (range) >mark ; immediate
\ Miser's CASE
: WHEN
  postpone else  cs-push  postpone thens  cs-pop ; immediate

\ Add C Switch flow-through
: CONTINUE
  cs-push  postpone thens  cs-mark  cs-pop ; immediate

\ Like ENDCASE but does not DROP
aka THENS END-CASE

application




\ Miser's CASE
behead bran (range)














\ Case demo
: test ( n )  space
  case [ hex ]
    cond  0  20 range
          7F    equal  when  ." Control char "       else
    cond  20 2F range
          3A 40 range
          5B 60 range
          7B 7E range  when  ." Punctuation "        else
    cond  30 39 range  when  ." Digit "              else
    cond  41 5A range  when  ." Upper case letter "  else
    cond  61 7A range  when  ." Lower case letter "  else
      ." Not a character " [ decimal ]
  endcase ;
-->

\ Case demo
cr cr .( [press any key] ) key drop
cr cr .( Miser's CASE demo ...) cr

cr  char a  .(   ) dup emit  test
cr  char ,  .(   ) dup emit  test
cr  char 8  .(   ) dup emit  test
cr  char ?  .(   ) dup emit  test
cr  char K  .(   ) dup emit  test
cr  0              dup 3 .r  test
cr  127            dup 3 .r  test
cr  128            dup 3 .r  test




