( "Candidate line"
  Find where a candidate within a box is
  in a single row or column, and eliminate
  other boxes in the row or column line.

  https://www.sudokuoftheday.com/techniques/candidate-lines/ )

create eliminator-data 3 cells allot
variable eliminator-box

: erase-eliminator
   eliminator-data 3 cells erase ;

: eliminator++ ( offset -- )
   cells eliminator-data + 1 swap +! ;

: eliminator@ ( offset -- value )
   cells eliminator-data @ ;

: singular-eliminator ( -- offset/-1 )
   0 -1 ( count last )
   box-size 0 do
     eliminator@ 0> if
       drop 1+ i ( count last )
     then
   loop
   swap ( last count )
   1 <> if
     drop -1
   then ;

: eliminate-box-candidate-row-value ( row value -- )

2dup
." Box:" eliminator-box @ . ." Value:" . ." Row:" . cr

   erase-eliminator
   box-size 0 do
     2dup
     eliminator-box @
     rot i box-box-xy>move
     possible? if
." +"
       i eliminator++
     then
   loop
   singular-eliminator dup -1 = if
     drop
   else
     . cr
     \ I've got a column available to check for elimination!
   then
   2drop ;

: eliminate-box-candidate-column-value ( column value -- )
   erase-eliminator
   2drop ;

: eliminate-box-candidate-row ( row -- )
   board-size 0 do
     dup i eliminate-box-candidate-row-value
   loop drop ;

: eliminate-box-candidate-column ( column -- )
   board-size 0 do
     dup i eliminate-box-candidate-column-value
   loop drop ;

: eliminate-box-candidate-rows ( -- )
   box-size 0 do
     i eliminate-box-candidate-row
   loop ;

: eliminate-box-candidate-columns ( -- )
   box-size 0 do
     i eliminate-box-candidate-column
   loop ;

: eliminate-box-candidate-lines ( -- )
   eliminate-box-candidate-rows
   eliminate-box-candidate-columns ;

: eliminate-all-candidate-lines ( -- )
   board-size 0 do
     i eliminator-box !
     eliminate-box-candidate-lines
   loop ;
