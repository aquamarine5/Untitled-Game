using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{

    /// <summary>
    ///This generator is used to place an entrance and exit (or player and exit) using two randomly selected contiguous, connected spaces made by previous generators.
    /// </summary>
    [CreateAssetMenu(menuName = "Strata/Generators/PlaceEntranceExit")]

    public class GenerateRandomContiguousItems : Generator
    {
        //Set this to a character corresponding to your exit or goal object to place in the level
        public char exitChar;

        //Set this to a character corresponding to your entrance or player to place in the level
        public char playerChar;

        //This is the function called by BoardGenerator during Generation
        public override void Generate(BoardGenerator boardGenerator)
        {
            PlaceExit(boardGenerator);
            PlaceStartLocation(boardGenerator);
        }

        //Places the exit character at a random empty grid position
        void PlaceExit(BoardGenerator boardGenerator)
        {
            GridPosition openConnectedPosition = boardGenerator.GetRandomEmptyGridPositionFromLastEmptySpaceGeneratorInStack(boardGenerator);
            boardGenerator.WriteToBoardGrid(openConnectedPosition.x, openConnectedPosition.y, exitChar, true, false);
        }

        //Places the player character at a random empty grid position
        void PlaceStartLocation(BoardGenerator boardGenerator)
        {
            GridPosition openConnectedPosition = boardGenerator.GetRandomEmptyGridPositionFromLastEmptySpaceGeneratorInStack(boardGenerator);
            boardGenerator.WriteToBoardGrid(openConnectedPosition.x, openConnectedPosition.y, playerChar, true, false);
        }


    }
}
