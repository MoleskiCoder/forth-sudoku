( bitset operations )

: mask@ ( n -- mask )
   1 swap lshift ;

: bit@ ( n bit# -- masked )
   mask@ and ;

: bit-set? ( n bit# -- f )
   bit@ 0<> ;

: bit-set ( n bit# -- n )
   mask@ or ;

: bit-clear ( n bit# -- n )
   mask@ invert and ;

: bit-count ( n -- count )
   0 swap
   board-size 0 do
     dup i bit-set? if
       swap 1+ swap
     then
   loop
   drop ;

: bit-singular? ( n - f )
   bit-count 1 = ;

: first-set-bit ( n -- bit#/n )
   board-size 0 do
     dup i bit-set? if
       drop i leave
     then
   loop ;
