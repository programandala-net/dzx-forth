\ BREAK GO
Breakpoint tool (adapted from Forth Dimensions 5/1)

BREAK is inserted into the application source code at the point
to be debugged.  When BREAK is subsequently executed, the
application is temporarily halted and the current stack contents
displayed.  The user will then be in a special interpret loop
(indicated by the '<ok>' prompt) during which time the system
may be examined.  The application can be resumed at any time
using GO.

Executing QUIT or ABORT while halted (e.g. as a result of
mistyping a command) will result in the user dropping back to
forth.


\ BREAK GO
forth definitions system
variable bsd   create buf 80 allot

: bip ( -- )
  begin cr ." <ok> " buf dup 80 accept space evaluate again ;

\ Halt application
: BREAK ( i*x -- i*x )  cr ." BREAK  stack = " .s  depth bsd !
  ['] bip catch dup -256 - if throw else drop then ;

\ Resume application
: GO ( i*x -- i*y )
  depth bsd @ - bsd off abort" stack changed" -256 throw ;
application
behead bsd bip
















