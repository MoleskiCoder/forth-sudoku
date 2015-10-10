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
     2dup
     i swap xy-possible? if
       i dangle
     then
   loop
   2drop
   dangling? ;

: eliminate-dangling-in-row ( value row -- )
   2dup
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
     2dup
     i xy-possible? if
       i dangle
     then
   loop
   2drop
   dangling? ;

: eliminate-dangling-in-column ( value column -- )
   2dup
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
     2dup
     i box-offset-possible? if
       i dangle
     then
   loop
   2drop
   dangling? ;

: eliminate-dangling-in-box ( value box -- )
   2dup
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
