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

( 
 hard board
 http://www.websudoku.com/?level=3&set_id=9869650069
)

  2 , 0 , 8  ,  0 , 6 , 0  ,  0 , 9 , 0 ,

  0 , 0 , 0  ,  0 , 0 , 0  ,  0 , 3 , 0 ,

  6 , 0 , 0  ,  0 , 0 , 0  ,  4 , 0 , 7 ,

( --------------------------------------- )

  1 , 5 , 0  ,  2 , 0 , 0  ,  0 , 0 , 0 ,

  0 , 6 , 0  ,  4 , 0 , 1  ,  0 , 8 , 0 ,

  0 , 0 , 0  ,  0 , 0 , 9  ,  0 , 7 , 6 ,

( --------------------------------------- )

  3 , 0 , 6  ,  0 , 0 , 0  ,  0 , 0 , 0 ,

  0 , 9 , 0  ,  0 , 0 , 0  ,  0 , 0 , 0 ,

  0 , 7 , 0  ,  0 , 1 , 0  ,  3 , 0 , 4 ,


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

: box-xy>offset ( box-x box-y -- offset )
   box-size * + ;

: box>first-move ( box -- n )
   box>box-xy
   board-size box-size * * 
   swap box-xy>offset ;

: box-offset>move-offset ( n -- offset )
   box>box-xy xy>move ;

: box-offset>move ( box offset -- n )
   box-offset>move-offset swap box>first-move + ;

: box-box-xy>move ( box box-x box-y -- n )
   box-xy>offset box-offset>move ;


( Possible handlers.  Possible set is an array of bitsets )

: possible ( n -- addr )
   cells possible-moves + ;

: possible@ ( n -- value )
   possible @ ;

: possible! ( value n -- )
   possible ! ;

: possible? ( number n -- f )
   possible@ swap bit-set? ;

: xy-possible? ( number x y -- f )
   xy>move possible? ;

: box-offset-possible? ( box offset -- f )
   box-offset>move possible? ;

: eliminate ( bit# n -- )
   dup possible@
   rot bit-clear
   swap possible! ;

: single-possibility! ( value n -- )
   swap 0 swap bit-set
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


( Dangling possibilities
  Look for where a possibility is only
  mentioned once in a row, column or box )

variable dangled

variable dangling

: dangling++ ( -- )
   1 dangling +! ;

: dangling? ( -- f )
   dangling @ 1 = ;

: dangle ( n -- )
   dangled ! dangling++ ;

( Row dangling ... )

: dangling-in-row? ( number y -- f )
   0 dangling !
   board-size 0 do
     over over
     i swap xy-possible? if
       i dangle
     then
   loop
   2drop
   dangling? ;

: eliminate-dangling-in-row ( value row -- )
   over over
   dangling-in-row? if
     dangled @
     swap xy>move
     single-possibility!
   else
     2drop
   then ;

: eliminate-all-row-dangling ( value -- )
   board-size 0 do
     dup i eliminate-dangling-in-row
   loop drop ;

( Column dangling ... )

: dangling-in-column? ( number x -- f )
   0 dangling !
   board-size 0 do
     over over
     i xy-possible? if
       i dangle
     then
   loop
   2drop
   dangling? ;

: eliminate-dangling-in-column ( value column -- )
   over over
   dangling-in-column? if
     dangled @
     xy>move
     single-possibility!
   else
     2drop
   then ;

: eliminate-all-column-dangling ( value -- )
   board-size 0 do
     dup i eliminate-dangling-in-column
   loop drop ;

( Box dangling ... )

: dangling-in-box? ( number box -- f )
   0 dangling !
   board-size 0 do
     over over
     i box-offset-possible? if
       i dangle
     then
   loop
   2drop
   dangling? ;

: eliminate-dangling-in-box ( value box -- )
   over over
   dangling-in-box? if
     dangled @
     box-offset>move
     single-possibility!
   else
     2drop
   then ;

: eliminate-all-box-dangling ( value -- )
   board-size 0 do
     dup i eliminate-dangling-in-box
   loop drop ;


( The coup de dangling grace! )

: eliminate-all-dangling ( -- )
   board-size 0 do
     i eliminate-all-row-dangling
     i eliminate-all-column-dangling
     i eliminate-all-box-dangling
   loop ;

( "Candidate Lines" )

create eliminator-data 3 cells allot

: erase-eliminator-data
   eliminator-data 3 cells erase ;

: eliminate-box-candidate-row-value ( box row value -- )
   erase-eliminator-data
   drop drop drop ;

: eliminate-box-candidate-column-value ( box column value -- )
   erase-eliminator-data
   drop drop drop ;

: eliminate-box-candidate-row ( box row -- )
   board-size 0 do
     over over
     i eliminate-box-candidate-row-value
   loop 2drop ;

: eliminate-box-candidate-column ( box column -- )
   board-size 0 do
     over over
     i eliminate-box-candidate-column-value
   loop 2drop ;

: eliminate-box-candidate-rows ( box -- )
   box-size 0 do
     dup i eliminate-box-candidate-row
   loop drop ;

: eliminate-box-candidate-columns ( box -- )
   box-size 0 do
     dup i eliminate-box-candidate-column
   loop drop ;

: eliminate-box-candidate-lines ( box -- )
   dup
   eliminate-box-candidate-rows
   eliminate-box-candidate-columns ;

: eliminate-all-candidate-lines ( -- )
   board-size 0 do
     i eliminate-box-candidate-lines
   loop ;


( -------------- Easy ------------------ )

( "Single Position" *Done*
  This is the easiest technique to apply by eye - and the one that
  most people use first when completing paper Sudoku puzzles.

  Choose a row, column or box, and then go through each of the
  numbers that hasn't already been placed. Because of other placements,
  the positions where you could place that number will be limited. Often
  there will be two or three places that are valid, but if you're lucky,
  there'll only be one. If you've narrowed it down to only one valid
  place where you can put the number- you can fill that number
  straight in, since it can't go anywhere else!

  https://www.sudokuoftheday.com/techniques/single-position/ )

( "Single Candidate" *Done*
  This technique is very easy - especially if you're using pencilmarks
  to store what candidates are still possible within each cell.

  If you've managed to rule out all other possibilities for a
  particular cell <by examining the surrounding column, row and box>,
  so there's only one number left that could possibly fit there - you
  can fill in that number.

  https://www.sudokuoftheday.com/techniques/single-candidate/ )

( -------------- Medium ------------------ )

( "Candidate line"
  Find where a candidate within a box is
  in a single row or column, and eliminate
  other boxes in the row or column line.

  https://www.sudokuoftheday.com/techniques/candidate-lines/ )

( "Double Pairs"
  This technique relies on spotting two pairs of
  candidates for a value, and using these to rule
  out candidates from other boxes.

  https://www.sudokuoftheday.com/techniques/double-pairs/ )

( "Multiple Lines"
  This is very similar to the Double Pairs Test, but
  is a little harder to spot. It works in the same way,
  but the candidates that occupy the lines could be spread
  across two of the blocks, and there could be several
  candidates in each line.

  https://www.sudokuoftheday.com/techniques/multiple-lines/ )

( -------------- Advanced ------------------ )

( "Naked Pairs/Triples"
  This technique is known as "Naked Subset" or "Disjoint Subset"
  in general, and works by looking for candidates that can be removed
  from other cells. Naked Pairs are when there are just two candidates
  being looked for, Naked Triple when there are three, and Naked Quads
  when there are four.

  https://www.sudokuoftheday.com/techniques/naked-pairs-triples/ )

( "Hidden Pairs/Triples"
  Hopefully you've got the hang of finding Naked Pairs and Triples - if
  not, practise looking for those before trying to understand the
  hidden equivalent!

  Hidden pairs and triples are quite a bit trickier to spot - they're
  hiding after all!

  This technique is also known as Hidden Subset or Unique Subset,
  in general.

  https://www.sudokuoftheday.com/techniques/hidden-pairs-triples/ )

( -------------- Master ------------------ )

( "X-Wings"
  X-Wings are fairly easy to spot, but a little harder
  to understand than some other techniques. Like others
  it relies on using positions of pencilmarks to infer
  enough to allow you to eliminate some other candidates.

  X-Wings are when there are two lines, each having the
  same two positions for a number.

  https://www.sudokuoftheday.com/techniques/x-wings/ )

( "Swordfish"
  This is very similar to using X-Wings, in that it will allow
  you to use knowledge about rows to remove candidates from
  columns, and vice versa.

  Make sure you're happy with why X-Wings work before moving
  on to Swordfish!

  The complexity here is that you're using knowledge from
  3 rows at the same time - and that's what makes them
  harder to spot. Unlike X-Wings, they don't form a
  simple rectangle.

  https://www.sudokuoftheday.com/techniques/swordfish/ )

( ---------------------------------------- )


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
   eliminate-all-box-possibilities 
   eliminate-all-dangling
   eliminate-all-candidate-lines ;

: solve ( -- )
   begin
     eliminate-all-possibilities
     transfer-singular-possibilities
   0= until ;


( Go! )

.board

initialise-possible

solve

.board
.possibles

.s

\ bye
