( SUDOKU SOLVER )

(
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

create sudoku-data

8 c, 7 c, 0 c, 0 c, 0 c, 0 c, 0 c, 4 c, 0 c,

0 c, 0 c, 0 c, 0 c, 0 c, 1 c, 0 c, 0 c, 0 c,

0 c, 0 c, 1 c, 3 c, 0 c, 5 c, 9 c, 0 c, 6 c,

9 c, 0 c, 4 c, 0 c, 1 c, 0 c, 0 c, 6 c, 8 c,

3 c, 0 c, 6 c, 9 c, 0 c, 8 c, 0 c, 0 c, 0 c,

0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 4 c, 0 c, 1 c,

0 c, 0 c, 5 c, 0 c, 3 c, 7 c, 0 c, 0 c, 0 c,

0 c, 4 c, 8 c, 6 c, 0 c, 9 c, 2 c, 3 c, 0 c,

0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 5 c, 0 c,

: move-xy ( n -- x y )
   9 /mod ;

: move-x ( n -- x )
   move-xy drop ;

: move-y ( n -- y )
   move-xy nip ;

: xy-move ( x y -- n )
   9 * + ;

: move-box-xy ( n -- box-x box-y )
   move-xy 3 / swap 3 / swap ;

: sudoku-element ( n -- addr )
   sudoku-data + ;

: sudoku-element@ ( n -- value )
   sudoku-element c@ ;

: sudoku-data! ( value n -- )
   sudoku-element c@ ;

: .sudoku-element ( n )
   sudoku-element@ dup
   ."  "
   0= if drop ." - " else .  then
   ."  " ;

: .box-break-vertical ( -- )
   ." |" ;

: .box-break-horizontal ( -- )
   ." ------------+------------+-----------" ;

: .sudoku-board
   9 9 * 0 do
     i move-x 0= if
       i move-y 3 mod 0= if
         cr cr
         .box-break-horizontal
       then
       cr cr
     else
       i move-x 3 mod 0= if
         .box-break-vertical
       then
     then
     i .sudoku-element
   loop
   cr cr .box-break-horizontal cr ;

.sudoku-board

bye
