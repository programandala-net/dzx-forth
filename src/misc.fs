\ Miscellaneous functions















\ Common forth words
: 0<= ( n -- flag )  0> 0= ;
: 0>= ( n -- flag )  0< 0= ;
: <=  ( n1 n2 -- flag )  > 0= ;
: >=  ( n1 n2 -- flag )  < 0= ;
: U<= ( u1 u2 -- flag )  u> 0= ;
: U>= ( u1 u2 -- flag )  u< 0= ;









\ Common forth words
\ Duplicate a triple number
: 3DUP ( x1 x2 x3 -- x1 x2 x3 x1 x2 x3 )  dup 2over rot ;

\ Negate n1/d1 if n is negative  (FIG: +- D+-)
: ?NEGATE ( n1 n -- n2 )  0< if negate then ;
: ?DNEGATE ( d1 n -- d2 )  0< if dnegate then ;









\ +UNDER

: +UNDER ( x b n .. x+n b )  rot + swap ;
\\
code +UNDER ( x b n .. x+n b )
  ax pop  sp bx mov  ax 2 [bx] add  next  end-code










\ Number utilities
: (UD.) ( ud -- a u )  <# #s #> ;
: (U.)  ( u  -- a u )  0 (d.) ;
: (.)   ( n  -- a u )  s>d (d.) ;
: UD.   ( ud -- )  (ud.) type space ;
: UD.R  ( ud n -- )  >r (ud.) r> over - spaces type ;

: (DH.) ( ud -- a u )
  base @ >r hex <# 8 0 do # loop #> r> base ! ;
: (H.)  ( u -- a u )  0 (dh.) 4 /string ;
: (HB.) ( u -- a u )  (h.) 2 /string ;
: H.    ( u -- )  (h.) type space ;
: HB.   ( u -- )  (hb.) type space ;



\ NUMBER? (old version)
: NUMBER? ( c-addr u -- d true | 0 )
  over c@ [char] - = >r  0. 2swap  dup if
    r@ 1 and /string  >number  dpl on
    begin
      dup if  over c@ bl <> and  then
    dup while
      over c@  dup [char] : =
      swap [char] +  [char] 0  within  or
    0= if  r> drop 2drop 2drop 0 exit  then
      1 /string  dpl off  >number
    repeat
  then  2drop  r> if dnegate then  true ;

\ Patch into forth interpreter
\  ' NUMBER?  SYS-VEC #18 + @  !  FREEZE
\ Comma formatted numeric output
\ Adapted from code by C.Curley FD15/5

\ As for #S but places a comma after each 3 digits
: #,S ( +d -- 0. )  0  begin  >r  #  2dup or
  while  r> 1+  dup 3 = if  [char] , hold  drop 0  then
  repeat  r> drop ;

: (D,.) ( d -- a u )  tuck dabs <# #,s rot sign #> ;
: D,.   ( d -- )      (d,.) type space ;
: D,.R  ( d n -- )    >r (d,.) r> over - spaces type ;

: (UD,.) ( ud -- a u )  <# #,s #> ;
: UD,.   ( ud -- )      (ud,.) type space ;
: UD,.R  ( ud n -- )    >r (ud,.) r> over - spaces type ;

\ FOR NEXT
\ Count is placed on return stack and decremented at NEXT,
\ terminates when zero.
sys @  application
-? code nxt ( run-time for NEXT )
   rpp lhld  m e mov  h inx  m d mov  d dcx
   e a mov  d ora  1 $ jz  d m mov  h dcx  e m mov
   c l mov  b h mov  m c mov  h inx  m b mov  next
1 $:  h inx  rpp shld  b inx  b inx  next  end-code

system
: FOR ( u -- )  postpone >r  postpone begin ;  immediate
: NEXT ( -- )  postpone nxt  <resolve ;  immediate

sys !  behead nxt nxt

\ Misc functions
\ Input string of n chars max, return false if empty or blanks
: INPUT ( n -- c-addr u true | false )
  here  dup rot accept  bl skip  dup if  -1  else  and  then ;

\ Convert hex string to a double number
: HEXNUMBER? ( c-addr u -- d -1 | 0 )
  base @ >r  hex number?  r> base ! ;

\ Display n backspace characters
: BACKSPACES ( n -- )  0 max  0 ?do  8 emit  loop ;





\ Exception tools
\ THROW exception code n if flag is non-zero
: ?THROW ( flag n -- )  swap 0<> and throw ;

\ Perform CATCH intercepting exception code n only
: ?CATCH ( xt n -- n )  >r catch dup r> <> over and throw ;

\ Intercepting a range of exception codes
\  ['] do-it CATCH  case
\    x1  of  ...  endof  \ catch error x1
\    x2  of  ...  endof  \ catch error x2
\    dup throw           \ throw all others
\  endcase



\ DEFER tools
sys @  system
\ Add new behaviour to existing deferred word
: +IS ( xt <name> -- )  >r  :noname  r> compile,
  ' >body  dup >r  @ compile,  postpone ;  r> ! ;

\ Compile current behaviour of a deferred word
: DEFERS ( <name> -- )  ' >body @ compile, ;  immediate

sys !






\ Stack tools
\ A version of ?STACK for use within turnkey applications.
\ Checks data, return and fp stacks for under/overflow.

: ?STACK ( ? -- ? )
  sp@  s0 @  over u<  swap  pad u<  or abort" stack?"
  r0 @  rp@  u<  rp@  fs0 @  u<  or abort" r-stack?"
\  fs0 @  fsp @  u<  fsp @  fs0 @  [ sys-vec #20 + ] literal
\  @  -  u<  or abort" f-stack?"
;






\\ Quad - DUM*
\ Multiply doubles leaving quad. Unsigned.
code DUM* ( ud1 ud2 -- uq )
  di pop  bx pop  cx pop  dx pop
  2 # sp sub  bp push  si push
  dx si mov  bx ax mov  dx mul
  sp bp mov  ax 4 [bp] mov
  dx si xchg  di ax mov  dx mul
  ax si add  0 # dx adc
  bx ax mov  dx bx mov  cx mul
  bp bp xor  ax si add  dx bx adc
  0 # bp adc  cx ax mov  di mul
  bx ax add  bp dx adc  si bx mov  si pop  bp pop
  bx push  ax push  dx push  next  end-code


\\ Quad - DUM/MOD
\ Divide quad by double. Unsigned.
code DUM/MOD ( uq ud -- udrem udquot )
  di pop  es pop  ax pop  bx pop  cx pop  dx pop
  si push  bp push  es si mov  #32 # bp mov
1 $:  dx shl  cx rcl  bx rcl  ax rcl  2 $ jnc
  si bx sub  di ax sbb  3 $ ju
2 $:  si bx sub  di ax sbb  3 $ jnc
  si bx add  di ax adc  1 # dx sub  0 # cx sbb
3 $:  dx inc  4 $ jnz  cx inc
4 $:  bp dec  1 $ jnz  bp pop  si pop
  bx push  ax push  dx push  cx push  next  end-code




\\ Double - D* DU/MOD
\ Multiply doubles. Signed or unsigned
code D* ( d|ud1 d|ud2 -- d|ud3 )
  cx pop  bx pop  ax pop  di pop  bx mul
  cx ax xchg  di mul  ax cx add  di ax xchg  bx mul
  cx dx add  ax push  dx push  next  end-code

\ Divide doubles. Unsigned.
code DU/MOD ( ud1 ud2 -- udrem udquot )
  di pop  bx pop  dx pop  ax pop  si push  bp push
  bx si mov  cx cx sub  bx bx sub  #33 # bp mov
1 $:  bx rcl  cx rcl  si bx sub  di cx sbb  2 $ jnc
  si bx add  di cx adc
2 $:  cmc  ax rcl  dx rcl  bp dec  1 $ jnz  bp pop  si pop
  bx push  cx push  ax push  dx push  next  end-code

\\ Double - FD/MOD
\ Divide doubles signed by unsigned. Floored.
code FD/MOD ( d ud -- drem dquot )
  di pop  ax pop  cx pop  dx pop  si push  bp push
  ax si mov  bx bx sub  ax ax sub  cx cx or  1 $ jns
  bx dec  ax dec  si bx add  di ax adc
1 $:  #32 # bp mov
2 $:  dx shl  cx rcl  bx rcl  ax rcl  3 $ jnc
  si bx sub  di ax sbb  4 $ ju
3 $:  si bx sub  di ax sbb  4 $ jnc
  si bx add  di ax adc  1 # dx sub  0 # cx sbb
4 $:  dx inc  5 $ jnz  cx inc
5 $:  bp dec  2 $ jnz  bp pop  si pop
  bx push  ax push  dx push  cx push  next
end-code

\ Double - high level
\ Multiply doubles. Signed or unsigned.
: D* ( d|ud1 d|ud2 -- d|ud3 )
  >r swap >r 2dup um* rot r> * + rot r> * + ;

\ Divide doubles. Unsigned.
: DU/MOD ( ud1 ud2 -- udrem udquot )
  0. 2rot  #32 0 do  2 pick over 2>r  d2*  2swap d2*  r> 0<
  1 and m+  2dup  7 pick 7 pick  du< 0=  r> 0<  or  if  5 pick
  5 pick  d-  2swap 1 m+  else  2swap  then  loop  2rot 2drop ;

\ Divide doubles. Signed. Symmetric.
: D/MOD ( d1 d2 -- drem dquot )
  2 pick  2dup xor 2>r  dabs  2swap dabs  2swap du/mod  r>
  0< if dnegate then  r> 0< if 2swap dnegate 2swap then ;

\ Double operations
\ Memory
: M+! ( n a -- )  dup >r 2@ rot m+ r> 2! ;
: D+! ( d a -- )  dup >r 2@ d+ r> 2! ;

\ Logical
: DAND ( xd1 xd2 -- xd3 )  rot and >r and r> ;
: DOR  ( xd1 xd2 -- xd3 )  rot or >r or r> ;
: DXOR ( xd1 xd2 -- xd3 )  rot xor >r xor r> ;
: DINVERT ( xd1 -- xd2 )  invert swap invert swap ;

\ Shift
: DLSHIFT ( xd1 n -- xd2 )  0 ?do d2* loop ;
: DRSHIFT ( xd1 n -- xd2 )  0 ?do d2/ $7FFF and loop ;


\ Mixed - MU/MOD UT* UT/ UM*/
\ Divide double by single. Unsigned.
: MU/MOD ( ud u -- urem udquot )
  >r 0 r@ um/mod r> swap >r um/mod r> ;

\ Multiply double by single leaving triple. Unsigned.
: UT* ( ud u -- ut )  dup rot um* 2>r um* 0 2r> d+ ;

\ Divide triple by single leaving double. Unsigned.
: UT/ ( ut u -- ud )
  dup >r um/mod swap rot 0 r@ um/mod
  swap rot r> um/mod nip 0 2swap swap d+ ;

\ Unsigned M/*
: UM*/ ( ud1 u1 u2 -- ud2 )  >r ut* r> ut/ ;

\ Mixed - FDM/MOD
\ Divide signed double by positive single. Floored.
: FDM/MOD ( d +n -- drem dquot )
  tuck >r s>d r> fm/mod >r swap um/mod 0 swap r> ;












\ 16-bit fast integer square root
\ Returns root and remainder, or 0 -1 if n is negative  FD14/5
: SQRT ( +n -- root rem )
  dup 0<  if  drop 0 -1  else
    0 swap 16384 ( 2^14 )
    begin
      >r  dup  2 pick  -  r@ -  dup 0<
      if    drop swap 2/
      else  nip  swap 2/  r@ +  then
      swap  r> 2/
      2/  dup 0=
    until  drop
  then ;



\ 32-bit fast integer square root
\ Returns root and remainder, or 0 -1 if d is negative  FD14/5
: DSQRT ( +d -- droot drem )
  dup 0<  if  2drop 0. -1.  else
    0. 2swap 1073741824. ( 2^30 )
    begin
      2>r  2dup  5 pick 5 pick  d-  2r@ d-  dup 0<
      if    2drop  2swap d2/
      else  2swap 2drop  2swap d2/  2r@ d+  then
      2swap  2r> d2/
      d2/  2dup d0=
    until  2drop
  then ;



\ 32-bit integer square root
\ Returns root or -1 if d is negative  M.Barr
code DSQRT ( +d -- u )
  cx pop  bx pop  3 $ jcxz  cx dx mov  -1 # di mov
1 $: dx shl  2 $ jc  dx shl  2 $ jc  di shr  1 $ ju
2 $: cx dx mov  bx ax mov  di dx cmp  4 $ jnc  di div
  di ax cmp  4 $ jnc  ax di add  di rcr  2 $ ju
3 $: bx dx mov  $FF # di mov  bx bx or  1 $ jnz
  bx di mov  4 $: di push  next  end-code







\ 31-bit integer square root
code DSQRT ( +d -- +n )
  bx pop  dx pop  ax ax sub  di di sub  16 # cx mov
1 $:  dx shl  bx rcl  di rcl  dx shl  bx rcl  di rcl
  ax shl  ax shl  ax inc  ax di cmp  2 $ jc  ax di sub
  ax inc  2 $:  ax shr  1 $ loop  ax push  next  end-code










\ Simple random number generator
\ LCS generator from 'Starting Forth'

variable RND  1 rnd !

\ Get random number
: RAND ( -- u )  rnd @  31421 *  6727 +  dup rnd ! ;

\ Get random number between 0 and u-1
: RANDOM ( u -- 0..u-1 )  rand um* nip ;






\ Minimum standard random number generator
\ Multiply doubles. Signed or unsigned.
: D* ( d|ud1 d|ud2 -- d|ud3 )
  >r swap >r 2dup um* rot r> * + rot r> * + ;

\ LCS generator using Turbo-C algorithm
2variable RND  1. rnd 2!

\ Get random number
: RAND ( -- u )
  rnd 2@ $015A4E35. d* 1. d+ tuck rnd 2! ;

\ Get random number between 0 and u-1
: RANDOM ( u -- 0..u-1 )  rand um* nip ;


\ CRC-16
\ User must set initial CRC to 0

: CRC-16 ( crc byt -- crc' )
  swap  8 0 do  2dup xor  1 and  if  1 rshift  $A001 xor
  else  1 rshift  then  swap  1 rshift  swap  loop nip ;
\\
code CRC-16 ( crc byt -- crc' )
  dx pop  ax pop  dx ax xor  8 # cx mov  1 $: ax 1 shr
  2 $ jnc  $A001 # ax xor  2 $: 1 $ loop  1push  end-code






\ CRC-16 using lookup table
\ User must set initial CRC to 0
-? create tb  256 cells allot  \ crctable

marker dispose
:noname ( -- )  256 0 do  i  8 0 do  dup 1 and  >r
  1 rshift  r> if  $A001 xor  then  loop  i cells  tb +  !
  loop ;  execute dispose

: CRC-16 ( crc byt -- crc' )
  over  xor  255 and  cells  tb +  @  swap  8 rshift  xor ;

\ code CRC-16  ( crc byt -- crc' )
\  bx pop  dx pop  dl bl xor  bx bx add  tb # bx add  0 [bx]
\  ax mov  dl dh xchg  dh dh sub  dx ax xor  1push  end-code
behead tb tb
\ CRC-CCITT
\ User must set initial CRC to 0

: CRC-CCITT ( crc byt -- crc' )
  swap  8 0 do  2dup xor  1 and  if  1 rshift  $8408 xor
  else  1 rshift  then  swap  1 rshift  swap  loop nip ;
\\
code CRC-CCITT ( crc byt -- crc' )
  dx pop  ax pop  dx ax xor  8 # cx mov  1 $: ax 1 shr
  2 $ jnc  $8408 # ax xor  2 $: 1 $ loop  1push  end-code






\ CRC-CCITT using lookup table
\ User must set initial CRC to 0
-? create tb  256 cells allot  \ crctable

create dispose
:noname ( -- )  256 0 do  i  8 0 do  dup 1 and  >r
  1 rshift  r> if  $8408 xor  then  loop  i cells  tb +  !
  loop ;  execute  forget dispose

: CRC-CCITT ( crc byt -- crc' )
  over  xor  255 and  cells tb + @  swap  8 rshift  xor ;

\ code CRC-CCITT ( crc byt -- crc' )
\  bx pop  dx pop  dl bl xor  bx bx add  tb # bx add  0 [bx]
\  ax mov  dl dh xchg  dh dh sub  dx ax xor  1push  end-code
behead tb tb
\ CRC-XMODEM
\ User must set initial CRC to 0

: CRC-XMODEM ( crc byt -- crc' )
  8 lshift  swap  8 0 do  2dup xor  $8000 and  if  2*
  $1021 xor  else  2*  then  swap  2*  swap  loop nip ;
\\
code CRC-XMODEM  ( crc byt -- crc' )
  dx pop  ax pop  dl dh xchg  dx ax xor  8 # cx mov
  1 $: ax 1 shl  2 $ jnc  $1021 # ax xor  2 $: 1 $ loop
  1push  end-code





\ CRC-XMODEM using lookup table
\ User must set initial CRC to 0
-? create tb  256 cells allot  \ crctable

create dispose
:noname ( -- )  256 0 do  0  i  8 lshift  8 0 do  dup
  $8000 and >r  2*  r> if  $1021 xor  then  swap  2*  swap
  loop nip  i cells  tb +  !  loop ;  execute  forget dispose

: CRC-XMODEM ( crc byt -- crc' )
  over  8 rshift  xor  cells  tb +  @  swap  8 lshift  xor ;

\ code CRC-XMODEM ( crc byt -- crc' )
\  bx pop  dx pop  dh bl xor  bx bx add  tb # bx add
\  0 [bx] ax mov  dl ah xor  1push  end-code
behead tb tb
\ CRC32-CCITT
\ User must set initial CRC to -1 and apply DINVERT to final CRC
: DINVERT ( d1 -- d2 )  invert swap invert swap ;

: CRC32-CCITT ( dcrc byt -- dcrc' )
  8 0 do  rot rot  over  3 pick  xor  1 and >r  d2/ 32767 and
  r> if  $EDB8 xor swap  $8320 xor swap  then  rot  1 rshift
  loop drop ;
\\
code CRC32-CCITT ( dcrc byt -- dcrc' )
  bx pop  ax pop  dx pop  8 # cx mov  1 $: bl bh mov  dl bh xor
  ax 1 shr  dx 1 rcr  bh 1 shr  2 $ jnc  $EDB8 # ax xor
  $8320 # dx xor  2 $: bl 1 shr  1 $ loop  2push  end-code



\ CRC32-CCITT using lookup table
\ User must set initial CRC to -1 and apply DINVERT to final CRC
: DINVERT ( d1 -- d2 )  invert swap invert swap ;
-? create tb  256 2* cells allot  \ crctable
create dispose
:noname ( -- )  256 0 do  i 0  8 0 do  over 1 and >r
  d2/ 32767 and  r> if  $EDB8 xor swap  $8320 xor swap  then
  loop  i cells  2*  tb +  2!  loop ;  execute  forget dispose
: CRC32-CCITT ( dcrc byt -- dcrc' )
  2 pick xor 255 and  2* cells tb + 2@  2>r  8 0 do
  d2/ loop  255 and  r> xor swap  r> xor swap ;
\ code CRC32-CCITT  ( dcrc byt -- dcrc' )
\  bx pop  ax pop  dx pop  dl bl xor  bx bx add  bx bx add
\  tb # bx add  dh dl mov  al dh mov  ah al mov  ah ah sub
\  0 [bx] ax xor  2 [bx] dx xor  2push  end-code
behead tb tb
\ BYTE benchmark - Sieve of Eratosthenes
8190 constant SIZE  create FLAGS  size allot

: PRIME ( -- )
  flags size 1 fill
  0 size 0 do
    flags i + c@ if
      i dup + 3 + dup i +
      begin dup size <
      while 0 over flags + c! over +
      repeat drop drop 1+
    then
  loop cr . ." Primes " ;

: SIEVE ( -- )  cr 10 0 do prime loop ;

\ Interface Age benchmark
: BENCH ( 1000 -- )
  dup 2 / 1+ swap cr ." Starting " CR
  1 do dup i 1 rot
    2 do drop dup i /mod
      dup 0= if drop drop 1 leave
        else 1 = if drop 1
          else dup 0 > if drop 1
            else 0= if 0 leave then
            then
          then
        then
      loop
    if 4 .r else drop then
    loop
  drop cr ." Finished. " ;
\ Simple locals - based on 'Anonymous Things' FD12/1
\  LOCAL <local>             headerless CREATE
\ :NONAME ... ;  IS %1       create a nameless function
\ : FUNCTION  ... %1 ... ;   use it in another function
system
: LOCAL ( <local> )  \ assign HERE to a local
  here  ' >body !  $CD c,  'next , ;

: LOCAL| ( "name" )  \ define a local
  sys @  system  ['] noop value  immediate  sys !
  does>  @  state @ if  compile,  else  execute  then ;
application
\ predefined locals
local| %1  local| %2  local| %3  local| %4
local| %5  local| %6  local| %7  local| %8

\ PARSE-WORD
sys @  system

: PARSE-WORD ( char -- c-addr u )
  >r source >in @ /string over swap r@ skip drop
  swap - >in +! r> parse ;

\ : PARSE-NAME ( -- c-addr u )  bl parse-word ;

sys !






\\ MOVEL
\ smart intersegment block move
code MOVEL ( seg1 offs1 seg2 offs2 u -- )
  sp bx mov  3 $ ) call  3 $ ) call
  cx pop  di pop  bx pop  dx pop  ax pop  si push  ds push
  dx si mov  bx ax cmp  1 $ jb  2 $ ja  di si cmp  2 $ ja
1 $:  cx si add  cx di add  si dec  di dec  std
2 $:  bx es mov  ax ds mov  rep byte movs
  cld  ds pop  si pop  next
3 $:  bx inc  bx inc  0 [bx] ax mov
  4 # cl mov  ax cl shr  $0F # 0 [bx] and
  bx inc  bx inc  ax 0 [bx] add  ret
end-code



\ Multi-dimension array
-? : ARRAY ( dimn..dim1 n itemsize "name" )
  create
    >r dup c, 1 swap 0 do over , * loop r> dup , *
    dup 0< abort" array size" allot
  does> ( idxn..idx1 -- addr )
    count 0 tuck do >r cell+ dup @ rot r> + * loop + cell+ ;









\\ Multi-dimension array
sys @  application
-? code doa ( idxn..idx1 -- addr )  bx pop ( pfa)
  0 [bx] cl mov  ch ch sub  bx inc  ax ax sub
1 $:  dx pop  dx ax add  2 # bx add  0 [bx] mul  1 $ loop
  bx inc  bx inc  ax bx add  bx push  next  end-code

system
-? : ARRAY
  ['] doa build ( dimn..dim1 n itemsize "name" )
  >r dup c, 1 swap 0 do over , * loop r> dup , *
  dup 0< abort" array size" allot ;

sys !  behead doa doa


\ FATAN2
: FATAN2 ( y x -- r )
  fdup f0< >r
  fdup f0= if
    fswap f< if
      [ pi  0.5e f* ] fliteral
    else
      [ pi -0.5e f* ] fliteral
    then
  else  f/ fatan  then
  r> if
    pi fover f0> if  f-  else  f+  then
  then ;



--> FIELD
\ Define a field within a data structure

\ -? : FIELD
\  create ( offs1 size "name" -- offs2 )  over , +
\  does> ( a1 -- a2 )  @ + ;

-? : FIELD
  create ( offs1 size "name" -- offs2 )  over , +
  ;code ( a1 -- a2 )
    h pop  d pop  m a mov  e add  a e mov  h inx
    m a mov  d adc  a d mov  d push  next  end-code

aka field +FIELD  \ Forth200x name


\ FIELD
\ Define a field within a data structure
sys @  application
-? code dof ( a1 -- a2 )  h pop ( pfa)
  d pop  m a mov  e add  a e mov  h inx
  m a mov  d adc  a d mov  d push  next  end-code

system
: FIELD ( offs1 size "name" -- offs2 )
  ['] dof build over , + ;

sys !  behead dof dof

aka field +FIELD  \ Forth200x name


\ FVALUE FTO
sys @  system

: FVALUE  ['] f@ build f, ;

: FTO ( r "name" -- )  postpone addr
  state @ if  postpone f!  else  f!  then ; immediate

sys !







\ @+ !+
: @+ ( a1 -- a2 n )  dup 2+ swap @ ;
: !+ ( n a1 -- a2 )  tuck ! 2+ ;
\\
code @+ ( a1 -- a2 n )  sp di mov
  0 [di] bx mov  2 # 0 [di] add  0 [bx] push  next
end-code

code !+ ( n a1 -- a2 )
  bx pop  0 [bx] pop  2 # bx add  bx push  next
end-code





\ F@+
: F@+ ( a1 -- a2 r )
  dup [ 1 floats ] literal + swap f@ ;
\\
code F@+ ( a1 -- a2 r )
  sp di mov  0 [di] push  1 floats # 0 [di] add  ' f@ ) jmp
end-code









\\ R-BUF
\ Allocate u bytes on return stack. R@ gives buffer address.
\ Discard return stack item before exiting definition.

-? label rb  \ discards buffer on exit
  here cell+ ,  0 [bp] bp mov  ' (exit) ) jmp  end-code

code R-BUF ( u -- )
  ax pop  ax inc  $FFFE # ax and  ( make even )
  bp bx mov  ax bp sub  bp push  6 # bp sub
  bx 4 [bp] mov  rb # 2 [bp] mov  0 [bp] pop  next  end-code

behead rb rb



\ LOHI CPACK
\ split into low and high bytes
code LOHI ( x -- lo hi )
  0 h lxi  d pop  l a mov  d l mov  a d mov  2push  end-code

\ pack low and high bytes
code CPACK ( lo hi -- x )
  d pop  h pop  e h mov  1push  end-code








