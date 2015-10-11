( Column eliminations )

: eliminate-column ( value x -- )
   board-size 0 do
     2dup
     i xy>move
     eliminate
   loop
   2drop ;

: eliminate-column-possibilities ( x -- )
   board-size 0 do
     dup i xy>move board-element@
     ?dup-if
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
      2dup
      i swap xy>move
      eliminate
   loop
   2drop ;

: eliminate-row-possibilities ( y -- )
   board-size 0 do
     i over xy>move board-element@
     ?dup-if
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
     2dup
     i box-offset>move
     eliminate
   loop
   2drop ;

: eliminate-box-possibilities ( box -- )
   board-size 0 do
     dup i box-offset>move board-element@
     ?dup-if
       1- over eliminate-box
     then
   loop
   drop ;

: eliminate-all-box-possibilities ( -- )
   board-size 0 do
     i eliminate-box-possibilities
   loop ;
