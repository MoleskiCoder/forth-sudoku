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
   element-count 0 do
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
