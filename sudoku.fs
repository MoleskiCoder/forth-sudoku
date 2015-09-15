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
  9 9 * cells allot


( bitset operations )

: mask@ ( n -- mask )
   1 swap lshift ;

: bit@ ( n bit# -- masked )
   mask@ and ;

: bit-set ( n bit# -- masked )
   mask@ or ;

: bit-clear ( n bit# -- masked )
   mask@ invert and ;


( Move translations )

: move-xy ( n -- x y )
   board-size /mod ;

: move-x ( n -- x )
   move-xy drop ;

: move-y ( n -- y )
   move-xy nip ;

: xy-move ( x y -- n )
   board-size * + ;

: move-box-xy ( n -- box-x box-y )
   move-xy box-size / swap box-size / swap ;


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

: .board-element ( n )
   board-element@ dup
   ."  "
   0= if drop ." - " else .  then
   ."  " ;

: .box-break-vertical ( -- )
   ." |" ;

: .box-break-horizontal ( -- )
   ." ------------+------------+-----------" ;

: .board
   board-size board-size * 0 do
     i move-x 0= if
       i move-y box-size mod 0= if
         cr cr
         .box-break-horizontal
       then
       cr cr
     else
       i move-x box-size mod 0= if
         .box-break-vertical
       then
     then
     i .board-element
   loop
   cr cr .box-break-horizontal cr ;


( Solver )

: initialise-possible
   board-size board-size * 0 do
     i board-element@ 0= if -1 else 0 then
     i possible!
   loop ;

: eliminate-row ( value y -- )
   9 0 do
     dup ( value y y )
     i ( value y y i )
     swap ( value y i y )
     xy-move ( value y n )
     rot ( y n value )
     dup ( y n value value )
     rot ( y value value n )
     eliminate ( y value )
     swap dup
   loop
   drop drop ;

: eliminate-row-possibilities ( y -- )
   9 0 do
     dup i swap ( y i y )
     xy-move ( y n )
     board-element@ ( y current )
     over ( y current y )
     ( y current y ) eliminate-row
   loop
   drop ;


( Go! )

.board

initialise-possible

\ bye
