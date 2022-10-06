using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySudoku
{
    public class SudokuTechniques
    {
        public enum Techniques
        {
            SINGLE_CANDIDATE,
            SINGLE_POSITION,
            CANDIDATE_LINES,
            DOUBLE_PAIRS,
            MULTIPLE_LINES,
            NAKED_PAIR,
            HIDDEN_PAIR,
            NAKED_TRIPLE,
            HIDDEN_TRIPLE,
            X_WING,
            FORCING_CHAINS,
            NAKED_QUAD,
            HIDDEN_QUAD,
            SWORDFISH
        }

        public enum SudokuLevel
        {
            BEGGINER,
            EASY,
            MEDIUM,
            TRICKY,
            FIENDISH,
            DIABOLICAL
        }

        /* Sudoku Techniques:
            1. Single Candidate - https://www.sudokuoftheday.com/techniques/single-candidate
            2. Single Position - https://www.sudokuoftheday.com/techniques/single-position
            3. Candidate Lines - https://www.sudokuoftheday.com/techniques/candidate-lines
            4. Double Pairs - https://www.sudokuoftheday.com/techniques/double-pairs
            5. Multiple Lines - https://www.sudokuoftheday.com/techniques/multiple-lines
            6. Naked Pairs/Triples - https://www.sudokuoftheday.com/techniques/naked-pairs-triples
            7. Hidden Pairs/Triples - https://www.sudokuoftheday.com/techniques/hidden-pairs-triples
            8. X-Wings - https://www.sudokuoftheday.com/techniques/x-wings
            9. Swordfish - https://www.sudokuoftheday.com/techniques/swordfish
            10. Forcing Chains - https://www.sudokuoftheday.com/techniques/forcing-chains
            11. Nishio - https://www.sudokuoftheday.com/techniques/nishio

            Technique               Cost   Cost for subsequent use
            Easy:
            1. Single Candidate -   100 -> 100
            2. Single Position  -   100 -> 100

            Medium:
            3. Candidate Lines  -   350 -> 200
            4. Double Pairs     -   500 -> 250
            5. Multiple Lines   -   700 -> 400

            Advanced:
            6. Naked Pair       -   750 -> 500
            7. Hidden Pair      -   1500 -> 1200
            8. Naked Triple     -   2000 -> 1400
            9. Hidden Triple    -   2400 -> 1600

            Master:
            10. X-Wing          -   2800 -> 1600
           *10. Y-Wing          -   3000 -> 1800
            11. Forcing Chains  -   4200 -> 2100
            12. Naked Quad      -   5000 -> 4000
            13. Hidden Quad     -   7000 -> 5000
            14. Swordfish       -   8000 -> 6000

            Level             Lowest Diff.  Highest Diff. - score
            Beginner        - 3600          4500
            Easy            - 4300          5500
            Medium          - 5300          6900
            Tricky          - 6500          9300
            Fiendish        - 8300          14000
            Diabolical      - 11000         25000
         */

        /* Solving rules & techniques 

        3. "Last possible number"    : Check what numbers are present in the box, column and row, place the missing one (100% only)


        11. "Pointing pairs"         : the same note appears twice in the same row or column, 
                                       remove all identical notes in the same row or column in adjacent boxes.
        12. "Pointing triples"       : the same note appears thrice in the same row or column, 
                                       remove all identical notes in the same row or column  in adjacent boxes.
        14. "Y-wing"                 : Find a cell (pivot) with only two notes, find two other cells (pincers) in the same column and row 
                                       with only two notes where at least one note is the same with one of the notes in the pivot cells.
                                       Two pincer cells should have at least one identical note that is not present in the pivot cell.
                                       Find the cross cell between two pincer cells and remove the common note from that cell.
        */
    }
}