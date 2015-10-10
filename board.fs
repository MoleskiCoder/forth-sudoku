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


( Current boards, start -> solved. 1 - 9, zero is an entry in the possible set )

: board-element ( n -- addr )
   cells board + ;

: board-element@ ( n -- value )
   board-element @ ;

: board-element! ( value n -- )
   board-element ! ;
