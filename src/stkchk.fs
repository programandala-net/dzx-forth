\ Stack Balance Checking Utility

\ Helps locate words which should be stack-neutral but aren't!

\ Use: place [S S] around the words you wish to test for balance e.g.

\     BEGIN [S  ...  S] AGAIN

\ Use as many [S S] pairs as needed.  Should the stack level
\ change at run-time, the application will stop and display
\ the screen number where the offending definition was compiled.

\ Nesting to 16 levels is allowed.


CREATE csd  0 C, 16 CELLS ALLOT  \ stack depth array

: [S  ( -- )
  DEPTH  csd COUNT 1+ 15 AND  DUP csd C!  CELLS + ! ;

: [s]  ( blk -- )
  DUP SCR !  >R
  DEPTH  csd COUNT  DUP 1- csd C!  CELLS + @ - IF
    PAGE ." Stack changed: Scr# = "  R@ . CR QUIT
  THEN  R> DROP ;

: S]  ( -- )
  BLK @  POSTPONE LITERAL  POSTPONE [s] ;  IMMEDIATE
