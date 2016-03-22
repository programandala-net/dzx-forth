\ Assembler test suite

Z80 assembler test suite













\ Assembler test suite
forth definitions  decimal  application

marker ASMTEST

cr .( testing assembler: )  2 #screens 1- thru
cr .( passed)

/stack  forth  forget asmtest

decimal application





\ Assembler test suite
system

variable org

: /stack ( i*x -- )
  begin depth 0> while drop repeat ;

: init ( -- )
  /stack
  s" : RESET ; [ASM READY" evaluate
  here org ! ;




\ Assembler test suite
\ Test compiled code
: { ( -- )
  s" CHECK ASM]" evaluate
  [char] } parse evaluate
  here
  begin  dup org @ u>  while
    1-  dup c@  rot <> if
      org @ 10 dump  -1 abort" failed"
    then
  repeat drop
  s" FORGET RESET" evaluate  init ;

application


\ Assembler test suite
hex

\ : adr ( -- l h )  here 0 100 um/mod ;

3    constant b#
45   constant d#
67   constant n#
89AB constant nn

init





.( Z80 )
      M ADC                                             { 8E }
 d# (X) ADC                                      { 0DD 8E 45 }
 d# (Y) ADC                                      { 0FD 8E 45 }
     n# ACI                                         { 0CE 67 }
      E ADC                                             { 8B }

 B  DADC                                            { 0ED 4A }
 D  DADC                                            { 0ED 5A }
 H  DADC                                            { 0ED 6A }
 SP DADC                                            { 0ED 7A }





\ Z80
      M ADD                                             { 86 }
 d# (X) ADD                                      { 0DD 86 45 }
 d# (Y) ADD                                      { 0FD 86 45 }
     n# ADI                                          { C6 67 }
      E ADD                                             { 83 }

  B DAD                                                 { 09 }
  D DAD                                                 { 19 }
  H DAD                                                 { 29 }
 SP DAD                                                 { 39 }

  B DADX                                            { 0DD 09 }
  D DADX                                            { 0DD 19 }
  X DADX                                            { 0DD 29 }
 SP DADX                                            { 0DD 39 }
\ Z80
  B DADY                                            { 0FD 09 }
  D DADY                                            { 0FD 19 }
  Y DADY                                            { 0FD 29 }
 SP DADY                                            { 0FD 39 }

      M ANA                                            { 0A6 }
 d# (X) ANA                                     { 0DD 0A6 45 }
 d# (Y) ANA                                     { 0FD 0A6 45 }
     n# ANI                                         { 0E6 67 }
      E ANA                                            { 0A3 }

      M b# BIT                                      { 0CB 5E }
 d# (X) b# BIT                               { 0DD 0CB 45 5E }
 d# (Y) b# BIT                               { 0FD 0CB 45 5E }
      E b# BIT                                      { 0CB 5B }
\ Z80
 nn CC                                          { 0DC 0AB 89 }
 nn CM                                          { 0FC 0AB 89 }
 nn CNC                                         { 0D4 0AB 89 }
 nn CNZ                                         { 0C4 0AB 89 }

 nn CP                                          { 0F4 0AB 89 }
 nn CPE                                         { 0EC 0AB 89 }
 nn CPO                                         { 0E4 0AB 89 }
 nn CZ                                          { 0CC 0AB 89 }
 nn CALL                                        { 0CD 0AB 89 }

 CMC                                                    { 3F }



\ Z80
      M CMP                                            { 0BE }
 d# (X) CMP                                     { 0DD 0BE 45 }
 d# (Y) CMP                                     { 0FD 0BE 45 }
     n# CPI                                         { 0FE 67 }
      E CMP                                            { 0BB }

 CCD                                               { 0ED 0A9 }
 CCDR                                              { 0ED 0B9 }

 CCI                                               { 0ED 0A1 }
 CCIR                                              { 0ED 0B1 }

 CMA                                                    { 2F }
 DAA                                                    { 27 }

\ Z80
      M DCR                                             { 35 }
 d# (X) DCR                                      { 0DD 35 45 }
 d# (Y) DCR                                      { 0FD 35 45 }
      E DCR                                             { 1D }

  B DCX                                                 { 0B }
  D DCX                                                 { 1B }
  H DCX                                                 { 2B }
  X DCX                                             { 0DD 2B }
  Y DCX                                             { 0FD 2B }
 SP DCX                                                 { 3B }




\ Z80
 DI                                                    { 0F3 }
 EI                                                    { 0FB }

 here DJNZ                                          { 10 0FE }

 XCHG                                                  { 0EB }
 XTHL                                                  { 0E3 }
 XTIX                                              { 0DD 0E3 }
 XTIY                                              { 0FD 0E3 }
 EXAF                                                   { 08 }
 EXX                                                   { 0D9 }

 HLT                                                    { 76 }


\ Z80
 IM0                                                { 0ED 46 }
 IM1                                                { 0ED 56 }
 IM2                                                { 0ED 5E }

 n# IN                                              { 0DB 67 }

 E INP                                              { 0ED 58 }

      M INR                                             { 34 }
      E INR                                             { 1C }
 d# (X) INR                                      { 0DD 34 45 }
 d# (Y) INR                                      { 0FD 34 45 }



\ Z80
  B INX                                                 { 03 }
  D INX                                                 { 13 }
  H INX                                                 { 23 }
  X INX                                             { 0DD 23 }
  Y INX                                             { 0FD 23 }
 SP INX                                                 { 33 }

 IND                                               { 0ED 0AA }
 INDR                                              { 0ED 0BA }
 INI                                               { 0ED 0A2 }
 INIR                                              { 0ED 0B2 }

 PCHL                                                  { 0E9 }
 PCIX                                              { 0DD 0E9 }
 PCIY                                              { 0FD 0E9 }
\ Z80
 nn JC                                          { 0DA 0AB 89 }
 nn JM                                          { 0FA 0AB 89 }
 nn JNC                                         { 0D2 0AB 89 }
 nn JNZ                                         { 0C2 0AB 89 }
 nn JP                                          { 0F2 0AB 89 }
 nn JPE                                         { 0EA 0AB 89 }
 nn JPO                                         { 0E2 0AB 89 }
 nn JZ                                          { 0CA 0AB 89 }
 nn JMP                                         { 0C3 0AB 89 }

 here     JRC                                       { 38 0FE }
 here 2-  JRNC                                      { 30 0FC }
 here 4 - JRNZ                                      { 20 0FA }
 here 6 - JRZ                                       { 28 0F8 }
 here 8 - JMPR                                      { 18 0F6 }
\ Z80
 B STAX                                                 { 02 }
 D STAX                                                 { 12 }

      n# M MVI                                       { 36 67 }
       E M MOV                                          { 73 }
 n# d# (X) MVI                                { 0DD 36 45 67 }
 E  d# (X) MOV                                   { 0DD 73 45 }
 n# d# (Y) MVI                                { 0FD 36 45 67 }
 E  d# (Y) MOV                                   { 0FD 73 45 }

 nn STA                                          { 32 0AB 89 }




\ Z80
 nn SBCD                                     { 0ED 43 0AB 89 }
 nn SDED                                     { 0ED 53 0AB 89 }
 nn SHLD                                         { 22 0AB 89 }
 nn SIXD                                     { 0DD 22 0AB 89 }
 nn SIYD                                     { 0FD 22 0AB 89 }
 nn SSPD                                     { 0ED 73 0AB 89 }

 B LDAX                                                 { 0A }
 D LDAX                                                 { 1A }
 nn LDA                                          { 3A 0AB 89 }
 LDAI                                               { 0ED 57 }
 LDAR                                               { 0ED 5F }
 nn LBCD                                     { 0ED 4B 0AB 89 }


\ Z80
 nn  B LXI                                       { 01 0AB 89 }
 nn  D LXI                                       { 11 0AB 89 }
 nn  H LXI                                       { 21 0AB 89 }
 nn  X LXI                                   { 0DD 21 0AB 89 }
 nn  Y LXI                                   { 0FD 21 0AB 89 }
 nn SP LXI                                       { 31 0AB 89 }

 nn LDED                                     { 0ED 5B 0AB 89 }
 nn LHLD                                         { 2A 0AB 89 }

 STAI                                               { 0ED 47 }
 STAR                                               { 0ED 4F }



\ Z80
 nn LIXD                                     { 0DD 2A 0AB 89 }
 nn LIYD                                     { 0FD 2A 0AB 89 }
 nn LSPD                                     { 0ED 7B 0AB 89 }

 SPHL                                                  { 0F9 }
 SPIX                                              { 0DD 0F9 }
 SPIY                                              { 0FD 0F9 }

      M E MOV                                           { 5E }
 d# (X) E MOV                                    { 0DD 5E 45 }
 d# (Y) E MOV                                    { 0FD 5E 45 }
     n# E MVI                                        { 1E 67 }
      D E MOV                                           { 5A }


\ Z80
 LDD                                               { 0ED 0A8 }
 LDDR                                              { 0ED 0B8 }
 LDI                                               { 0ED 0A0 }
 LDIR                                              { 0ED 0B0 }

 NEG                                                { 0ED 44 }
 NOP                                                    { 00 }

      M ORA                                            { 0B6 }
 d# (X) ORA                                     { 0DD 0B6 45 }
 d# (Y) ORA                                     { 0FD 0B6 45 }
     n# ORI                                         { 0F6 67 }
      E ORA                                            { 0B3 }


\ Z80
 OUTDR                                             { 0ED 0BB }
 OUTIR                                             { 0ED 0B3 }
  E OUTP                                            { 0ED 59 }
 n# OUT                                             { 0D3 67 }
 OUTD                                              { 0ED 0AB }
 OUTI                                              { 0ED 0A3 }

 PSW POP                                               { 0F1 }
   B POP                                               { 0C1 }
   D POP                                               { 0D1 }
   H POP                                               { 0E1 }
   X POP                                           { 0DD 0E1 }
   Y POP                                           { 0FD 0E1 }


\ Z80
 PSW PUSH                                              { 0F5 }
   B PUSH                                              { 0C5 }
   D PUSH                                              { 0D5 }
   H PUSH                                              { 0E5 }
   X PUSH                                          { 0DD 0E5 }
   Y PUSH                                          { 0FD 0E5 }

      M b# RES                                      { 0CB 9E }
 d# (X) b# RES                               { 0DD 0CB 45 9E }
 d# (Y) b# RES                               { 0FD 0CB 45 9E }
      E b# RES                                      { 0CB 9B }




\ Z80
 RET                                                   { 0C9 }
 RC                                                    { 0D8 }
 RM                                                    { 0F8 }
 RNC                                                   { 0D0 }
 RNZ                                                   { 0C0 }
 RP                                                    { 0F0 }
 RPE                                                   { 0E8 }
 RPO                                                   { 0E0 }
 RZ                                                    { 0C8 }

 RETI                                               { 0ED 4D }
 RETN                                               { 0ED 45 }



\ Z80
      M RALR                                        { 0CB 16 }
 d# (X) RALR                                 { 0DD 0CB 45 16 }
 d# (Y) RALR                                 { 0FD 0CB 45 16 }
      E RALR                                        { 0CB 13 }
        RAL                                             { 17 }

      M RLCR                                        { 0CB 06 }
 d# (X) RLCR                                 { 0DD 0CB 45 06 }
 d# (Y) RLCR                                 { 0FD 0CB 45 06 }
      E RLCR                                        { 0CB 03 }
        RLC                                             { 07 }
        RLD                                         { 0ED 6F }



\ Z80
      M RARR                                        { 0CB 1E }
 d# (X) RARR                                 { 0DD 0CB 45 1E }
 d# (Y) RARR                                 { 0FD 0CB 45 1E }
      E RARR                                        { 0CB 1B }
        RAR                                             { 1F }

      M RRCR                                        { 0CB 0E }
 d# (X) RRCR                                 { 0DD 0CB 45 0E }
 d# (Y) RRCR                                 { 0FD 0CB 45 0E }
      E RRCR                                        { 0CB 0B }
        RRC                                             { 0F }
        RRD                                         { 0ED 67 }



\ Z80
 0 RST                                                 { 0C7 }
 1 RST                                                 { 0CF }
 2 RST                                                 { 0D7 }
 3 RST                                                 { 0DF }
 4 RST                                                 { 0E7 }
 5 RST                                                 { 0EF }
 6 RST                                                 { 0F7 }
 7 RST                                                 { 0FF }

      M SBB                                             { 9E }
 d# (X) SBB                                      { 0DD 9E 45 }
 d# (Y) SBB                                      { 0FD 9E 45 }
     n# SBI                                         { 0DE 67 }
      E SBB                                             { 9B }

\ Z80
  B DSBC                                            { 0ED 42 }
  D DSBC                                            { 0ED 52 }
  H DSBC                                            { 0ED 62 }
 SP DSBC                                            { 0ED 72 }

      M b# SET                                     { 0CB 0DE }
 d# (X) b# SET                              { 0DD 0CB 45 0DE }
 d# (Y) b# SET                              { 0FD 0CB 45 0DE }
      E b# SET                                     { 0CB 0DB }

      M SLAR                                        { 0CB 26 }
 d# (X) SLAR                                 { 0DD 0CB 45 26 }
 d# (Y) SLAR                                 { 0FD 0CB 45 26 }
      E SLAR                                        { 0CB 23 }

\ Z80
      M SRAR                                        { 0CB 2E }
 d# (X) SRAR                                 { 0DD 0CB 45 2E }
 d# (Y) SRAR                                 { 0FD 0CB 45 2E }
      E SRAR                                        { 0CB 2B }

      M SRLR                                        { 0CB 3E }
 d# (X) SRLR                                 { 0DD 0CB 45 3E }
 d# (Y) SRLR                                 { 0FD 0CB 45 3E }
      E SRLR                                        { 0CB 3B }

 STC                                                    { 37 }




\ Z80
      M SUB                                             { 96 }
 d# (X) SUB                                      { 0DD 96 45 }
 d# (Y) SUB                                      { 0FD 96 45 }
     n# SUI                                         { 0D6 67 }
      E SUB                                             { 93 }

      M XRA                                            { 0AE }
 d# (X) XRA                                     { 0DD 0AE 45 }
 d# (Y) XRA                                     { 0FD 0AE 45 }
     n# XRI                                         { 0EE 67 }
      E XRA                                            { 0AB }




