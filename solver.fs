( Solver )

: initialise-possible ( -- )
   element-count 0 do
     i board-element@ 0= if -1 else 0 then
     i possible!
   loop ;

include eliminator.fs
include dangling.fs
include candidate_line.fs



( -------------- Medium ------------------ )



( "Double Pairs"
  This technique relies on spotting two pairs of
  candidates for a value, and using these to rule
  out candidates from other boxes.

  https://www.sudokuoftheday.com/techniques/double-pairs/ )

( "Multiple Lines"
  This is very similar to the Double Pairs Test, but
  is a little harder to spot. It works in the same way,
  but the candidates that occupy the lines could be spread
  across two of the blocks, and there could be several
  candidates in each line.

  https://www.sudokuoftheday.com/techniques/multiple-lines/ )

( -------------- Advanced ------------------ )

( "Naked Pairs/Triples"
  This technique is known as "Naked Subset" or "Disjoint Subset"
  in general, and works by looking for candidates that can be removed
  from other cells. Naked Pairs are when there are just two candidates
  being looked for, Naked Triple when there are three, and Naked Quads
  when there are four.

  https://www.sudokuoftheday.com/techniques/naked-pairs-triples/ )

( "Hidden Pairs/Triples"
  Hopefully you've got the hang of finding Naked Pairs and Triples - if
  not, practise looking for those before trying to understand the
  hidden equivalent!

  Hidden pairs and triples are quite a bit trickier to spot - they're
  hiding after all!

  This technique is also known as Hidden Subset or Unique Subset,
  in general.

  https://www.sudokuoftheday.com/techniques/hidden-pairs-triples/ )

( -------------- Master ------------------ )

( "X-Wings"
  X-Wings are fairly easy to spot, but a little harder
  to understand than some other techniques. Like others
  it relies on using positions of pencilmarks to infer
  enough to allow you to eliminate some other candidates.

  X-Wings are when there are two lines, each having the
  same two positions for a number.

  https://www.sudokuoftheday.com/techniques/x-wings/ )

( "Swordfish"
  This is very similar to using X-Wings, in that it will allow
  you to use knowledge about rows to remove candidates from
  columns, and vice versa.

  Make sure you're happy with why X-Wings work before moving
  on to Swordfish!

  The complexity here is that you're using knowledge from
  3 rows at the same time - and that's what makes them
  harder to spot. Unlike X-Wings, they don't form a
  simple rectangle.

  https://www.sudokuoftheday.com/techniques/swordfish/ )

( ---------------------------------------- )


( Global eliminator )

: transfer-singular-possibilities ( -- solved-count )
   0
   element-count 0 do
     i possible@ dup bit-singular if
       first-set-bit 1+ i board-element!
       0 i possible!
       1+
     else
       drop
     then
   loop ;

: eliminate-all-possibilities ( -- )
   eliminate-all-row-possibilities
   eliminate-all-column-possibilities
   eliminate-all-box-possibilities 
   eliminate-all-dangling
   eliminate-all-candidate-lines ;

: solve ( -- )
   begin
     eliminate-all-possibilities
     transfer-singular-possibilities
   0= until ;
