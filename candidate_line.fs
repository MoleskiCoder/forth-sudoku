( "Candidate line"
  Find where a candidate within a box is
  in a single row or column, and eliminate
  other boxes in the row or column line.

  https://www.sudokuoftheday.com/techniques/candidate-lines/ )

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
