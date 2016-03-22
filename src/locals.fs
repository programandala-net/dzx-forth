\ locals

based on locals code by B. Muench

( ANS )
(LOCAL)  LOCALS|  TO

( optional )
LOCAL  ADDR







\ locals
forth definitions  decimal
application

2  #screens 1-  thru











\ locals
#26 user LP  \ locals pointer (don't change)

\ add locals to CATCH
-? : CATCH  ( xt -- except# | 0 )
  lp @ >r catch r> over if lp ! else drop then ;










\ locals
label lods  \ HL <- (IP)+
  b ldax  a l mov  b inx  b ldax  a h mov  b inx  ret  end-code

label ladr  \ HL <- address of local
  LP [up]  m e mov  h inx  m d mov  lods call  xchg  l a mov
  e sub  a l mov  h a mov  d sbb  a h mov  ret  end-code

code L@ ( -- x )  \ fetch local
  ladr call  m a mov  h inx  m h mov  a l mov  1push  end-code

code L! ( x -- )  \ store local
  ladr call  d pop  e m mov  h inx  d m mov  next  end-code

code L& ( -- addr )  \ address of local
  ladr call  1push  end-code
\ locals
\ build locals frame & init locals
code L{  ( i*x -- ) ( R: -- lp i*x )
  LP [up]  h push  m e mov  h inx  m d mov
  RPP lhld  h dcx  d m mov  h dcx  e m mov
  d pop  l a mov  d stax  d inx  h a mov  d stax
  xchg  lods call  l a mov  xchg  1 $:  d pop
  h dcx  d m mov  h dcx  e m mov  a dcr  1 $ jnz
  RPP shld  next  end-code

\ remove locals frame
code }L  ( -- ) ( R: lp i*x -- )
  LP [up]  h push  m e mov  h inx  m d mov
  xchg  m e mov  h inx  m d mov  h inx  RPP shld
  h pop  e m mov  h inx  d m mov  next  end-code

\ locals
system

: ERR? ( x -- )  abort" locals error" ;
: TOKEN ( "name" -- c-addr u )  bl word count dup 0= err? ;

#128 constant #NB     \ name buffer size
create NB  #nb allot  \ name buffer
variable LC           \ locals count
variable NP           \ name pointer
variable PA           \ patch address

: L[ ( -- )  lc off  nb #nb erase  nb np ! ;
: ]L ( -- )  lc @ if  postpone }l  then ;


\ locals
\ search local names
: L= ( c-addr -- index | 0 )
  0  state @ if  \ compiling only
    nb >r
    begin  1+  r@ c@ 0<> and  dup
    while  over count  r> count  2dup + >r
           caps compare 0=
    until  then  r> drop
  then  nip ;

\ local offset
: LOS ( ? index -- u )  nip cells ;



\ locals
\ new FIND
: LFIND ( c-addr -- c-addr 0 | xt flag )
  dup l= ?dup if  postpone l@  los -1  exit  then
  [ addr find @ compile, ] ;

\ add local
: +LOC ( c-addr u -- )
  np @  2dup +  nb #nb 2-  + u> err?
  over 1+ np +!  place
  lc @ 0= if  postpone l{  here pa !  0 , then
  1 lc +!  1 pa @ +! ;




\ (LOCAL) LOCAL LOCALS|
: (LOCAL) ( c-addr u -- )  \ ANS
  dup if  +loc  else  2drop  then ;

\ assign locals

: LOCAL ( "name" )  token (local) ;  immediate

: LOCALS| ( "name1...namen |" )  \ ANS
  begin  token  2dup s" |" compare
  while  +loc  repeat  2drop ;  immediate





\ TO ADDR
-? : TO ( x "name" -- )  \ ANS
  >in @  bl word l= ?dup
  if  postpone l!  los ,  exit  then
  >in !  postpone to ;  immediate

\ Address of a local
-? : ADDR ( "name" -- addr )
  >in @  bl word l= ?dup
  if  postpone l&  los ,  exit  then
  >in !  postpone addr ;  immediate





\ locals
-? : EXIT   ]l  postpone exit ;       immediate
-? : ;      ]l  postpone ; ;          immediate
-? : DOES>  ]l  postpone does>  l[ ;  immediate
-? : ;CODE  ]l  postpone ;code ;      immediate
-? : :  :  l[ ;
-? : :NONAME  :noname  l[ ;

\ add to remember chain
:noname  [ addr find @ ] literal is find ;  remember
' lfind is find

application



\ locals
behead lods +loc














