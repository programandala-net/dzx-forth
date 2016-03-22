\ Co-operative Forth multitasker
/TASKER ( -- )              initialize multitasker
TASK ( u s r "ccc" ; -- tcb )  create task control block
ACTIVATE ( tcb -- )         initialize and run task
AWAKE ( tcb -- )            resume task
SLEEP ( tcb -- )            suspend task
STOP ( -- )                 stop current task, switch to next
MULTI ( -- )                start/resume multitasker
SINGLE ( -- )               suspend multitasker
PAUSE ( -- )                switch to next active task
GET ( sem -- )              get resource
GRAB ( sem -- )             grab resource without PAUSE
RELEASE ( sem -- )          release resource
#FLOAT ( u -- )             per-task f/p stack bytes


\ Load screen
forth definitions decimal
sys @  application

cr .( loading Multitasker )  2 #screens 1- thru

sys !









\ #FLOAT STATUS LINK TOS /TASKER HIS
fs0 @ s0 @ - value #FLOAT       \ default f/p stack base

\ Define reserved user variables
0 user TOS                      \ save top of stack
2 user STATUS  status on        \ task active flag
4 user LINK                     \ link to next task's user

link value tlink                \ topmost LINK

\ Initialize multitasker
: /TASKER ( -- )  status tlink ! ;  /tasker

\ Calculate task local user address
: HIS ( tcb user -- user' )  tos - + ;

\ (pause)
\ Pause current task & switch to next active
code (pause) ( -- )
  -1 a mvi                                  \ wake
  b push  rpp lhld  h push                  \ push IP RP
  0 h lxi  sp dad                           \ save SP to TOS
  xchg  up lhld  e m mov  h inx  d m mov
  h inx  a m mov                            \ wake or sleep
1 $:  h inx  h inx  m e mov  h inx  m d mov  \ find active task
  xchg  m a mov  a ora  1 $ jz
  0 m mvi                                   \ sleep
  h dcx  h dcx  up shld                     \ load UP
  m e mov  h inx  m d mov  xchg  sphl       \ restore SP
  h pop  rpp shld  b pop                    \ pop RP IP
  next  end-code

\ STOP AWAKE SLEEP SINGLE MULTI
\ Stop current task & switch to next active
code STOP ( -- )  a xra  ' (pause) 2+ jmp  end-code

\ Resume a task
: AWAKE ( tcb -- )  2+ on ;

\ Suspend a task
: SLEEP ( tcb -- )  2+ off ;

\ Suspend multitasker
: SINGLE ( -- )  ['] noop is pause ;

\ Start/resume multitasker
: MULTI ( -- )  ['] (pause) is pause ;

\ ACTIVATE
\ Initialize task.  Execution begins with word following
\ ACTIVATE.  Must be used within a definition.
: ACTIVATE ( tcb -- )
  dup s0 his @                   \ get task stack
\  2-  over fs0 his @  over !    \ push FS0
  2-  r>  over !                 \ push start IP
  2-  over r0 his @  over !      \ push R0
  over tos his !                 \ set TOS
  dup catcher his off            \ set CATCHER
  awake ;





\ TASK
system
\ create task control block
: TASK ( u s r "ccc" ; -- tcb )
  create  here >r
  rot dup allot  up @ r@ rot cmove      \ copy USER vars
  here dup r@ dp his 2!                 \ DP DPS
  swap allot  here r@ s0 his !          \ S0
\  #float allot  here r@ fs0 his !       \ FS0
  allot  here r@ r0 his !               \ R0
  r@ sleep  r@ status his tlink !       \ sleep & add task
  r> link his to tlink  /tasker ;       \
application



\\ Multitasker - GET RELEASE
\ Semaphore support

\ Get resource
code GET ( sem -- )
1 $:  c: pause ;c  bx pop  up ) ax mov
  0 [bx] cx mov  2 $ jcxz  cx ax cmp  3 $ jz  bx push  1 $ ju
2 $:  ax 0 [bx] mov  3 $:  next  end-code

\ Release resource
code RELEASE ( sem -- )
  bx pop  up ) ax mov  0 [bx] ax sub
  1 $ jnz  ax 0 [bx] mov  1 $:  next  end-code



\\ Multitasker - GRAB GET RELEASE
\ Grab resource without PAUSE
code GRAB ( sem -- )
1 $:  bx pop  up ) ax mov
  0 [bx] cx mov  2 $ jcxz  cx ax cmp  3 $ jz
  bx push  si dec  si dec  ' pause ) jmp
2 $:  ax 0 [bx] mov  3 $:  next  end-code

\ Get resource
: GET ( sem -- )  pause grab ;

\ Release resource
code RELEASE ( sem -- )
  bx pop  up ) ax mov  0 [bx] ax sub  1 $ jnz
  ax 0 [bx] mov  1 $:  next  end-code

\ GRAB GET
\ Grab resource without PAUSE
code GRAB ( sem -- )
  d pop  up lhld  xchg  b push  \ hl=sem  de=up
  m c mov  h inx  m b mov  h dcx  c a mov  b ora  2 $ jz
  e a mov  c cmp  1 $ jnz  d a mov  b cmp  3 $ jz
1 $:  b pop  b dcx  b dcx  h push  ' pause jmp
2 $:  e m mov  h inx  d m mov
3 $:  b pop  next  end-code

\ Get resource
: GET ( sem -- )  pause grab ;




\ RELEASE
\ Release resource
code RELEASE ( sem -- )
  up lhld  d pop  xchg  e a mov  m sub  1 $ jnz
  d a mov  h inx  m sbb  1 $ jnz
  a m mov  h dcx  a m mov
1 $:  next  end-code









\ Multitasker - discard heads
behead tlink tlink
behead (pause) (pause)













\\ Multitasker - Demo 1
128 64 64 task DCOUNTING    \ task1 control block
128 64 64 task HCOUNTING    \ task2 control block
44 user CNT   variable SCREEN  screen off

: DCOUNTER ( -- )  dcounting activate  decimal  0 cnt !
  begin  screen get ( get-xy)  0 2 at-xy  cnt @  dup 0 10 d.r
  1+ cnt ! ( at-xy)  screen release  pause  again ;

: HCOUNTER ( -- )  hcounting activate  hex  0 cnt !
  begin  screen get ( get-xy)  15 2 at-xy  cnt @  dup 0 10 d.r
  1- cnt ! ( at-xy)  screen release  pause  again ;

: RUN ( -- )  /tasker  status on  page ." 2 tasks counting:"
  dcounter hcounter multi  begin key? until key  drop single ;
cr .( Save demo1? ) y/n [if]  turnkey run DEMO1  bye  [then]
\\ Multitasker - Demo 2
128 200 64 task UCOUNTING   \ task1 control block
128 200 64 task DCOUNTING   \ task2 control block
44 user CNT   variable SCREEN  screen off

: UPCOUNT ( -- )  ucounting activate  0e cnt f!
  begin  screen get ( get-xy)  0 2 at-xy  cnt f@  fdup 0 10 f.r
  1e f+  cnt f! ( at-xy)  screen release  pause  again ;

: DOWNCOUNT ( -- )  dcounting activate  0e cnt f!
  begin  screen get ( get-xy) 15 2 at-xy  cnt f@  fdup 0 10 f.r
  1e f-  cnt f! ( at-xy)  screen release  pause  again ;

: RUN ( -- )  /tasker  status on  page ." 2 f/p tasks counting:"
  upcount downcount multi  begin key? until key  drop single ;
cr .( Save demo2? ) y/n [if]  turnkey run DEMO2  bye  [then]
