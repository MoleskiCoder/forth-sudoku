\
\ From: https://see.stanford.edu/materials/icspacs106b/H19-RecBacktrackExamples.pdf
\
\ A straightforward port from the original C to Forth


\ http://www.telegraph.co.uk/news/science/science-news/9359579/Worlds-hardest-sudoku-can-you-crack-it.html

create puzzle
8 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 ,
0 , 0 , 3 , 6 , 0 , 0 , 0 , 0 , 0 ,
0 , 7 , 0 , 0 , 9 , 0 , 2 , 0 , 0 ,
0 , 5 , 0 , 0 , 0 , 7 , 0 , 0 , 0 ,
0 , 0 , 0 , 0 , 4 , 5 , 7 , 0 , 0 ,
0 , 0 , 0 , 1 , 0 , 0 , 0 , 3 , 0 ,
0 , 0 , 1 , 0 , 0 , 0 , 0 , 6 , 8 ,
0 , 0 , 8 , 5 , 0 , 0 , 0 , 1 , 0 ,
0 , 9 , 0 , 0 , 0 , 0 , 4 , 0 , 0 ,


\ Some useful constants

0 constant unassigned

3 constant box-size
9 constant board-size
board-size dup * constant cell-count


\ Rather than compare against zero, make the code more meaningful to readers.

: unassigned? ( -- )
   ]] 0= [[ ; immediate


\ Puzzle access methods

: grid@ ( n -- number )
   ]] cells puzzle + @ [[ ; immediate

: grid! ( number n -- )
   ]] cells puzzle + ! [[ ; immediate


\ Move and grid position translation methods

: move>xy ( n -- x y )
   ]] board-size /mod [[ ; immediate

: move>x ( n -- x )
   ]] board-size mod [[ ; immediate

: move>y ( n -- y )
   ]] board-size / [[ ; immediate

: xy>move ( x y -- n )
   ]] board-size * + [[ ; immediate


\ Row, column and box start positions

: move>row-start ( n -- n )
   ]] move>y board-size * [[ ; immediate

: move>column-start ( n -- n )
   ]] move>x [[ ; immediate

: box-side-start ( n -- n )
   ]] dup box-size mod - [[ ; immediate

: move>box-start ( n -- n )
   ]] move>xy
   box-side-start swap
   box-side-start swap
   xy>move [[ ; immediate


\ Function: used-in-row?
\ ----------------------
\ Returns a boolean which indicates whether any assigned entry
\ in the specified row matches the given number.

\ simplified in Forth by row cells being contiguious in the grid.

: used-in-row? ( number n -- f )
   move>row-start cells puzzle +
   board-size 0 ?do
     2dup
     @ = if unloop drop exit then
     cell+
   loop 2drop 0 ;


\ Function: used-in-column?
\ -------------------------
\ Returns a boolean which indicates whether any assigned entry
\ in the specified column matches the given number.

\ Very similar to used-in-row?, with the offset incrementing by the board-size

: used-in-column? ( number n -- f )
   move>column-start cells puzzle +
   board-size 0 ?do
     2dup
     @ = if unloop drop exit then
     board-size cells +
   loop 2drop 0 ;


\ Function: used-in-box?
\ ----------------------
\ Returns a boolean which indicates whether any assigned entry
\ within the specified 3x3 box matches the given number.

\ Convert the loop into a box xy, then calculate an offset to obtain cell value.

: used-in-box? ( number n - f )
   move>box-start cells puzzle +
   board-size 0 ?do
     2dup i box-size /mod xy>move cells +
     @ = if unloop drop exit then
   loop 2drop 0 ;


\ Function: available?
\ --------------------
\ Returns a boolean which indicates whether it will be legal to assign
\ number to the given row,column location. As assignment is legal if it that
\ number is not already used in the row, column, or box.

\ Because Forth doesn't seem to shortcut logical operations, we must explicitly leave early
\ if possible.

: available? ( number n -- f )
   2dup used-in-row?    if 2drop 0 exit then
   2dup used-in-column? if 2drop 0 exit then
        used-in-box? 0= ;


\ Function: solve?
\ ----------------
\ Takes a partially filled-in grid and attempts to assign values to all
\ unassigned locations in such a way to meet the requirements for sudoku
\ solution (non-duplication across rows, columns, and boxes). The function
\ operates via recursive backtracking: it finds an unassigned location with
\ the grid and then considers all digits from 1 to 9 in a loop. If a digit
\ is found that has no existing conflicts, tentatively assign it and recur
\ to attempt to fill in rest of grid. If this was successful, the puzzle is
\ solved. If not, unmake that decision and try again. If all digits have
\ been examined and none worked out, return false to backtrack to previous
\ decision point.

: solve? ( n -- f )
   recursive
   dup cell-count = if drop -1 exit then            \ success!
   dup grid@ if 1+ solve? exit then                 \ if it's already assigned, skip
   10 1 ?do                                         \ consider digits 1 to 9
     i over available? if                           \ if looks promising
       i over grid!                                 \ make tentative assignment
       dup 1+ solve? if
         unloop drop -1 exit                        \ recur, if success, yay!
       then
     then 
   loop
   unassigned over grid!                            \ failure, unmake & try again
   drop 0 ;                                         \ this triggers backtracking


\ Board display

: .board-element ( n -- )
   ."  "
   grid@ dup unassigned? if drop ." - " else .  then
   ."  " ;

: .box-break-vertical ( -- )
   ." |" ;

: .box-break-horizontal ( -- )
   ." ------------+------------+-----------" ;

: .board ( -- )
   cell-count 0 do
     i move>x unassigned? if
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

: game ( -- )
  utime
  0 solve? if
    utime 2swap d-
    .board
    cr cr ." Time taken " d. ." microseconds" cr cr
  else
    2drop
    cr cr ." No solution exists"
  then ;

game
