( Possible handlers.  Possible set is an array of bitsets )


create possible-moves
  element-count cells allot


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
