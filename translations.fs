( Move translations )

( xy <-> move )

: move>xy ( n -- x y )
   board-size /mod ;

: move>x ( n -- x )
   move>xy drop ;

: move>y ( n -- y )
   move>xy nip ;

: xy>move ( x y -- n )
   board-size * + ;

( box translations )

: move>box-xy ( n -- box-x box-y )
   move>xy box-size / swap box-size / swap ;

: box>box-xy ( box-n -- box-x box-y )
   box-size /mod ;

: box-xy>offset ( box-x box-y -- offset )
   box-size * + ;

: box>first-move ( box -- n )
   box>box-xy
   board-size box-size * * 
   swap box-xy>offset ;

: box-offset>move-offset ( n -- offset )
   box>box-xy xy>move ;

: box-offset>move ( box offset -- n )
   box-offset>move-offset swap box>first-move + ;

: box-box-xy>move ( box box-x box-y -- n )
   box-xy>offset box-offset>move ;
