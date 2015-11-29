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
0 , 9 , 0 , 0 , 0 , 0 , 4 , 0 , 0
,


\ Some useful constants

0 constant unassigned

3 constant box-size
9 constant board-size
board-size dup * constant cell-count


\ Puzzle access methods

: grid-cell ( n -- address )
   cells puzzle + ;

: grid@ ( n -- number )
   grid-cell @ ;

: grid! ( n number -- )
   swap grid-cell ! ;


\ Move and grid position translation methods

: move>xy ( n -- x y )
   board-size /mod ;

: move>x ( n -- x )
   move>xy drop ;

: move>y ( n -- y )
   move>xy nip ;

: xy>move ( x y -- n )
   board-size * + ;


\ Row, column and box start positions

: move>row-start ( n -- n )
   move>y 0 swap xy>move ;

: move>column-start ( n -- n )
   move>x 0 xy>move ;

: box-side-start ( n -- n )
   dup box-size mod - ;

: move>box-start ( n -- n )
   move>xy
   box-side-start swap
   box-side-start swap
   xy>move ;


\ Function: used-in-row?
\ ----------------------
\ Returns a boolean which indicates whether any assigned entry
\ in the specified row matches the given number.

\ simplified in forth by row cells being contiguious in the grid.

: used-in-row? ( n number -- f )
   swap move>row-start swap
   board-size 0 ?do
     2dup swap i + grid@ = if unloop 2drop -1 exit then
   loop 2drop 0 ;


\ Function: used-in-column?
\ -------------------------
\ Returns a boolean which indicates whether any assigned entry
\ in the specified column matches the given number.

\ Very similar to used-in-row?, with the loop increment multiplied by board-size.

: used-in-column? ( n number -- f )
   swap move>column-start swap
   board-size 0 ?do
     2dup swap i board-size * + grid@ = if unloop 2drop -1 exit then
   loop 2drop 0 ;


\ Function: used-in-box?
\ ----------------------
\ Returns a boolean which indicates whether any assigned entry
\ within the specified 3x3 box matches the given number.

\ Convert the loop into a box xy, then calculate an offset to obtain cell value.

: used-in-box? ( n number - f )
   swap move>box-start swap
   board-size 0 ?do
     2dup swap
     i box-size / board-size *
     i box-size mod + +
     grid@ = if unloop 2drop -1 exit then
   loop 2drop 0 ;


\ Function: available?
\ --------------------
\ Returns a boolean which indicates whether it will be legal to assign
\ number to the given row,column location. As assignment is legal if it that
\ number is not already used in the row, column, or box.

: available? ( n number -- f )
   2dup used-in-row? 0= >r
   2dup used-in-column? 0= >r
        used-in-box? 0=
   r> r> and and ;


\ Function: find-unassigned-location
\ ----------------------------------
\ Searches the grid to find an entry that is still unassigned. If found,
\ the reference parameters row, column will be set the location that is
\ unassigned, and true is returned. If no unassigned entries remain, false
\ is returned.

\ for the forth version, we'll just use the return stack.
\ 0 or above is a valid grid reference, -1 indicates no unnassigned
\ location is available.

: find-unassigned-location ( -- n/-1 )
   cell-count 0 ?do
     i grid@ unassigned = if i unloop exit then
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
     dup i available? if                            \ if looks promising
       dup i grid!                                  \ make tentative assignment
       solve? if
         unloop drop -1 exit                        \ recur, if success, yay!
       then
       dup unassigned grid!                         \ failure, unmake & try again
     then
   loop
   drop 0 ;                                         \ this triggers backtracking
