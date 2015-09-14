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

3 constant box-size
9 constant board-size

create sudoku-data

8 , 7 , 0 , 0 , 0 , 0 , 0 , 4 , 0 ,

0 , 0 , 0 , 0 , 0 , 1 , 0 , 0 , 0 ,

0 , 0 , 1 , 3 , 0 , 5 , 9 , 0 , 6 ,

9 , 0 , 4 , 0 , 1 , 0 , 0 , 6 , 8 ,

3 , 0 , 6 , 9 , 0 , 8 , 0 , 0 , 0 ,

0 , 0 , 0 , 0 , 0 , 0 , 4 , 0 , 1 ,

0 , 0 , 5 , 0 , 3 , 7 , 0 , 0 , 0 ,

0 , 4 , 8 , 6 , 0 , 9 , 2 , 3 , 0 ,

0 , 0 , 0 , 0 , 0 , 0 , 0 , 5 , 0 ,

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

: sudoku-element ( n -- addr )
   cells sudoku-data + ;

: sudoku-element@ ( n -- value )
   sudoku-element @ ;

: sudoku-data! ( value n -- )
   sudoku-element @ ;

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
     i .sudoku-element
   loop
   cr cr .box-break-horizontal cr ;

.sudoku-board

bye
