using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/Cave Digger")]

    public class GeneratorCaveDigger : Generator
    {

        [Range(1, 100)]
        public int mapPercentageToClear = 50;
        public bool useDefaultEmptyChar = true;
        public char charToWriteEmpty = '0';

        public override void Generate(BoardGenerator boardGenerator)
        {
            //Choose a random start position inside the grid
            GridPosition digPosition = boardGenerator.GetRandomGridPosition();

            int digX = digPosition.x;
            int digY = digPosition.y;
            
            
            //Calculate what percentage of the grid we want as empty space
            int targetDugSpaces = ((boardGenerator.boardGenerationProfile.boardHorizontalSize * boardGenerator.boardGenerationProfile.boardVerticalSize) * mapPercentageToClear) / 100;
            //Count how many spaces we've dug out
            int dugOutSpaceCount = 0;

            //What character are we writing to the grid?
            if (useDefaultEmptyChar)
            {
                charToWriteEmpty = boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar();
            }

            //Write the first empty space to the grid
            boardGenerator.WriteToBoardGrid(digX, digY, charToWriteEmpty, overwriteFilledSpaces, generatesEmptySpace);
            //count that space as part of our total
            dugOutSpaceCount++;

            //Loop until we reach the desired number of dug spaces
            while (dugOutSpaceCount < targetDugSpaces)
            {

                //Pick a random direction north, south, east, west
                int randomDirection = Random.Range(0, 4);

                switch (randomDirection)
                {
                    case 0: //North
                            //Make sure the target space is still within the grid before digging
                        if ((digY + 1) < boardGenerator.boardGenerationProfile.boardVerticalSize - 1)
                        {
                            //Move one space north
                            digY++;

                            //Validate that that space is not yet our empty tile
                            if (boardGenerator.boardGridAsCharacters[digX, digY] != charToWriteEmpty)
                            {
                                //Change it to our empty tile
                                boardGenerator.boardGridAsCharacters[digX, digY] = charToWriteEmpty;
                                //Count it
                                dugOutSpaceCount++;
                            }
                        }
                        break;
                    case 1: //South
                            //Ensure that the edges are still tiles
                        if ((digY - 1) > 1)
                        {
                            //Move the y down one
                            digY--;
                            //Validate that that space is not yet our empty tile
                            if (boardGenerator.boardGridAsCharacters[digX, digY] != charToWriteEmpty)
                            {
                                //Change it to our empty tile
                                boardGenerator.boardGridAsCharacters[digX, digY] = charToWriteEmpty;
                                //Count it
                                dugOutSpaceCount++;
                            }
                        }
                        break;
                    case 2: //East
                            //Ensure that the edges are still tiles
                        if ((digX + 1) < boardGenerator.boardGenerationProfile.boardHorizontalSize - 1)
                        {
                            //Move the x to the east
                            digX++;
                            //Validate that that space is not yet our empty tile
                            if (boardGenerator.boardGridAsCharacters[digX, digY] != charToWriteEmpty)
                            {
                                //Change it to our empty tile
                                boardGenerator.boardGridAsCharacters[digX, digY] = charToWriteEmpty;
                                //Count it
                                dugOutSpaceCount++;
                            }
                        }
                        break;
                    case 3: //West
                            //Ensure that the edges are still tiles
                        if ((digX - 1) > 1)
                        {
                            //Move the x to the west
                            digX--;
                            //Validate that that space is not yet our empty tile
                            if (boardGenerator.boardGridAsCharacters[digX, digY] != charToWriteEmpty)
                            {
                                //Change it to our empty tile
                                boardGenerator.boardGridAsCharacters[digX, digY] = charToWriteEmpty;
                                //Count it
                                dugOutSpaceCount++;
                            }
                        }
                        break;
                }
            }
        }

    }
}
