\
\ From: https://see.stanford.edu/materials/icspacs106b/H19-RecBacktrackExamples.pdf
\
\ A straightforward port from the original C to Forth


\ http://www.telegraph.co.uk/news/science/science-news/9359579/Worlds-hardest-sudoku-can-you-crack-it.html

create puzzle
8 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c,
0 c, 0 c, 3 c, 6 c, 0 c, 0 c, 0 c, 0 c, 0 c,
0 c, 7 c, 0 c, 0 c, 9 c, 0 c, 2 c, 0 c, 0 c,
0 c, 5 c, 0 c, 0 c, 0 c, 7 c, 0 c, 0 c, 0 c,
0 c, 0 c, 0 c, 0 c, 4 c, 5 c, 7 c, 0 c, 0 c,
0 c, 0 c, 0 c, 1 c, 0 c, 0 c, 0 c, 3 c, 0 c,
0 c, 0 c, 1 c, 0 c, 0 c, 0 c, 0 c, 6 c, 8 c,
0 c, 0 c, 8 c, 5 c, 0 c, 0 c, 0 c, 1 c, 0 c,
0 c, 9 c, 0 c, 0 c, 0 c, 0 c, 4 c, 0 c, 0 c,


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
   ]] puzzle + c@ [[ ; immediate

: grid! ( number n -- )
   ]] puzzle + c! [[ ; immediate


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
   move>row-start puzzle +
   board-size 0 ?do
     2dup i +
     c@ = if unloop 2drop -1 exit then
   loop 2drop 0 ;


\ Function: used-in-column?
\ -------------------------
\ Returns a boolean which indicates whether any assigned entry
\ in the specified column matches the given number.

\ Very similar to used-in-row?, with the loop increment multiplied by board-size.

: used-in-column? ( number n -- f )
   move>column-start puzzle +
   board-size 0 ?do
     2dup i xy>move
     c@ = if unloop 2drop -1 exit then
   loop 2drop 0 ;


\ Function: used-in-box?
\ ----------------------
\ Returns a boolean which indicates whether any assigned entry
\ within the specified 3x3 box matches the given number.

\ Convert the loop into a box xy, then calculate an offset to obtain cell value.

: used-in-box? ( number n - f )
   move>box-start puzzle +
   board-size 0 ?do
     2dup i box-size /mod xy>move +
     c@ = if unloop 2drop -1 exit then
   loop 2drop 0 ;


\ Function: available?
\ --------------------
\ Returns a boolean which indicates whether it will be legal to assign
\ number to the given row,column location. As assignment is legal if it that
\ number is not already used in the row, column, or box.

\ Because Forth doesn't seem to shortcut logical operations, we must explicitly leave early
\ if possible.

: available? ( number n -- f )
   2dup used-in-row? >r r@ if 2drop rdrop 0 exit then
   2dup used-in-column? >r r@ if 2drop rdrop rdrop 0 exit then
        used-in-box? 0=
   r> 0= r> 0= and and ;


\ Function: find-unassigned-location
\ ----------------------------------
\ Searches the grid to find an entry that is still unassigned. If found,
\ the reference parameters row, column will be set the location that is
\ unassigned, and true is returned. If no unassigned entries remain, false
\ is returned.

\ for the forth version, we'll just use the return stack.
\ 0 or above is a valid grid reference, -1 indicates no unnassigned
\ location is available.

\ For performance reasons in Forth, I've inlined the puzzle + grid offset access.

: find-unassigned-location ( -- n/-1 )
   cell-count 0 ?do
     i grid@ unassigned? if i unloop exit then
   loop -1 ;


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

: solve? ( -- f )
   recursive
   find-unassigned-location dup -1 = if exit then   \ success!
   10 1 ?do                                         \ consider digits 1 to 9
     i over available? if                           \ if looks promising
       i over grid!                                 \ make tentative assignment
       solve? if
         unloop drop -1 exit                        \ recur, if success, yay!
       then
       unassigned over grid!                        \ failure, unmake & try again
     then
   loop
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
  solve? if
    utime 2swap d-
    .board
    cr cr ." Time taken " d. ." microseconds" cr cr
  else
    2drop
    cr cr ." No solution exists"
  then ;

game
