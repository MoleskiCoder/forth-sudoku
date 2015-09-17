( SUDOKU SOLVER )

( 
An aide memoire to the layout of the puzzle

   0          1          2

0  1  2  - 3  4  5  - 6  7  8
-----------------------------
00 01 02   03 04 05   06 07 08 - 0

09 10 11   12 13 14   15 16 17 - 1  0

18 19 20   21 22 23   24 25 26 - 2

                               -

27 28 29   30 31 32   33 34 35 - 3

36 37 38   39 40 41   42 43 44 - 4  1

45 46 47   48 49 50   51 52 53 - 5

                               -

54 55 56   57 58 59   60 61 62 - 6

63 64 65   66 67 68   69 70 71 - 7  2

72 73 74   75 76 77   78 79 80 - 8
)

3 constant box-size
9 constant board-size


create board

8 , 7 , 0 , 0 , 0 , 0 , 0 , 4 , 0 ,

0 , 0 , 0 , 0 , 0 , 1 , 0 , 0 , 0 ,

0 , 0 , 1 , 3 , 0 , 5 , 9 , 0 , 6 ,

9 , 0 , 4 , 0 , 1 , 0 , 0 , 6 , 8 ,

3 , 0 , 6 , 9 , 0 , 8 , 0 , 0 , 0 ,

0 , 0 , 0 , 0 , 0 , 0 , 4 , 0 , 1 ,

0 , 0 , 5 , 0 , 3 , 7 , 0 , 0 , 0 ,

0 , 4 , 8 , 6 , 0 , 9 , 2 , 3 , 0 ,

0 , 0 , 0 , 0 , 0 , 0 , 0 , 5 , 0 ,


create possible-moves
  board-size board-size * cells allot


( bitset operations )

: mask@ ( n -- mask )
   1 swap lshift ;

: bit@ ( n bit# -- masked )
   mask@ and ;

: bit-set? ( n bit# -- f )
   bit@ 0<> ;

: bit-set ( n bit# -- n )
   mask@ or ;

: bit-clear ( n bit# -- n )
   mask@ invert and ;

: bit-count ( n -- count )
   0 swap
   board-size 0 do
     dup i bit-set? if
       swap 1+ swap
     then
   loop
   drop ;

: bit-singular ( n - f )
   bit-count 1 = ;

: first-set-bit ( n -- bit#/n )
   board-size 0 do
     dup i bit-set? if
       drop i leave
     then
   loop ;


( Move translations )

( xy <-> move )

: move>xy ( n -- x y )
   board-size /mod ;

: move>x ( n -- x )
   move>xy drop ;

: move>y ( n -- y )
   move>xy nip ;

: xy>move ( x y -- n )
   board-size * + ;

( box translations )

: move>box-xy ( n -- box-x box-y )
   move>xy box-size / swap box-size / swap ;

: box>box-xy ( box-n -- box-x box-y )
   box-size /mod ;

: box>first-move ( box -- n )
   box>box-xy
   board-size box-size * * 
   swap box-size * + ;

: box-offset>move-offset ( n -- offset )
   box>box-xy board-size * + ;

: box-offset>move ( box offset -- n )
   box-offset>move-offset swap box>first-move + ;


( Possible handlers.  Possible set is an array of bitsets )

: possible ( n -- addr )
   cells possible-moves + ;

: possible@ ( n -- value )
   possible @ ;

: possible! ( value n -- )
   possible ! ;

: eliminate ( bit# n -- )
   dup possible@
   rot bit-clear
   swap possible! ;


( Current boards, start -> solved. 1 - 9, zero is an entry in the possible set )

: board-element ( n -- addr )
   cells board + ;

: board-element@ ( n -- value )
   board-element @ ;

: board-element! ( value n -- )
   board-element ! ;


( Board display )

: .board-element ( n -- )
   ."  "
   board-element@ dup 0= if drop ." - " else .  then
   ."  " ;

: .box-break-vertical ( -- )
   ." |" ;

: .box-break-horizontal ( -- )
   ." ------------+------------+-----------" ;

: .board ( -- )
   board-size board-size * 0 do
     i move>x 0= if
       i move>y box-size mod 0= if
         cr cr
         .box-break-horizontal
       then
       cr cr
     else
       i move>x box-size mod 0= if
         .box-break-vertical
       then
     then
     i .board-element
   loop
   cr cr .box-break-horizontal cr ;


( Possibles display )

: .possible ( n -- )
   ."  "
   possible@ dup 0= if
     drop ." - "
   else
     board-size 0 do
       dup i bit-set? if
         i 1+ .
       then
     loop
     drop
   then
   ."  " ;

: .possibles ( -- )
   board-size 0 do
     board-size 0 do
       i j xy>move .possible
     loop
     cr
   loop ;

( Solver )

: initialise-possible ( -- )
   board-size board-size * 0 do
     i board-element@ 0= if -1 else 0 then
     i possible!
   loop ;

( Column eliminations )

: eliminate-column ( value x -- )
   board-size 0 do
     over over
     i xy>move
     eliminate
   loop
   2drop ;

: eliminate-column-possibilities ( x -- )
   board-size 0 do
     dup i xy>move board-element@
     dup 0= if
       drop
     else
       1- over eliminate-column
     then
   loop
   drop ;

: eliminate-all-column-possibilities ( -- )
   board-size 0 do
     i eliminate-column-possibilities
   loop ;

( Row eliminations )

: eliminate-row ( value y -- )
   board-size 0 do
     i over xy>move
     rot dup rot
     eliminate
     swap
   loop
   2drop ;

: eliminate-row-possibilities ( y -- )
   board-size 0 do
     i over xy>move board-element@
     dup 0= if
       drop
     else
       1- over eliminate-row
     then
   loop
   drop ;

: eliminate-all-row-possibilities ( -- )
   board-size 0 do
     i eliminate-row-possibilities
   loop ;

( Box eliminations )

: eliminate-box ( value box -- )
   board-size 0 do
     over over ( value box value box )
     i box-offset>move ( value box value n )
     eliminate ( value box )
   loop
   2drop ;

: eliminate-box-possibilities ( box -- )
   board-size 0 do
     dup i box-offset>move board-element@
     dup 0= if
       drop
     else
       1- over eliminate-box
     then
   loop
   drop ;

: eliminate-all-box-possibilities ( -- )
   board-size 0 do
     i eliminate-box-possibilities
   loop ;


( Global eliminator )

: transfer-singular-possibilities ( -- )
   0
   board-size board-size * 0 do
     i possible@ dup bit-singular if
       first-set-bit 1+ i board-element!
       0 i possible!
       1+
     else
       drop
     then
   loop ;

: eliminate-all-possibilities ( -- )
   eliminate-all-row-possibilities
   eliminate-all-column-possibilities
   eliminate-all-box-possibilities ;

: solve ( -- )
   begin
     eliminate-all-possibilities
     transfer-singular-possibilities
     dup cr ." Eliminated:" .
   0= until cr ;


( Go! )

.board

initialise-possible

." ----------------------------------"
solve
." ----------------------------------"

.board
.possibles


\ bye
