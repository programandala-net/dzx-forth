\ 8080 structured conditionals















.( 8080 structured conditionals )
assembler definitions hex system  warning off
0C2 constant 0=   0CA constant 0<>  0D2 constant U<
0DA constant U>=  0E2 constant PE   0EA constant PO
0F2 constant 0<   0FA constant 0>=  0C3 constant NEVER
aka U>= NC   aka U< CY   aka PE NO   aka PO OV

: THEN  here swap ! ;           aka HERE BEGIN
: UNTIL  c, , ;                 : IF  c, begin 0 , ;
: AHEAD  never if ;             : ELSE  ahead swap then ;
: WHILE  if swap ;              : AGAIN  never until ;
: REPEAT  again then ;

forth definitions decimal application  warning on


