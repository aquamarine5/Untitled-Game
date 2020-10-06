using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is adapted from Sebastian Lague's procedural cave generation tutorial, for more information on cellular automata and the technique applied to 
 * marching squares 3d mesh generation please check out his series: https://unity3d.com/learn/tutorials/s/procedural-cave-generation-tutorial
 */


namespace Strata
{
    //Change the string here to change where this appears in the create menu and what it's called
    [CreateAssetMenu(menuName = "Strata/Generators/GenerateCellularAutomata")]
    public class GeneratorCellularAutomata : Generator
    {

       
        //Character used for positive, filled space, corresponds to charId in BoardLibrary
        public char charForFill = 'w';

        bool useLibraryDefaultEmptyCharForEmptySpace = true;
        //Character used for negative space
        public char emptySpaceChar = '\0';

        //Whether to use a random seed or not
        public bool useRandomSeed;

        //How much to fill space in the map, try values from 40-55
        [Range(0, 100)]
        public int randomFillPercent;

        //This is the function that will be called by BoardGenerator to kick off the generation process
        public override void Generate(BoardGenerator boardGenerator)
        {
            //If we want to generate empty space as defined in BoardLibrary
            if (useLibraryDefaultEmptyCharForEmptySpace)
            {
                //Set the emptySpaceChar to the default empty space character
                emptySpaceChar = boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar();
            }
            //Generate the level
            GenerateMap(boardGenerator);

        }


        
        void GenerateMap(BoardGenerator boardGenerator)
        {
            //First fill the map with random data
            RandomFillMap(boardGenerator);

            //Then post process that data and smooth it out into organic shapes
            for (int i = 0; i < 5; i++)
            {
                SmoothMap(boardGenerator);
            }
        }

        //Fill the map with random noise based on the size of the grid
        void RandomFillMap(BoardGenerator boardGenerator)
        {
            for (int x = 0; x < boardGenerator.boardGenerationProfile.boardHorizontalSize; x++)
            {
                for (int y = 0; y < boardGenerator.boardGenerationProfile.boardVerticalSize; y++)
                {
                    boardGenerator.WriteToBoardGrid(x, y, (Random.Range(0,100) < randomFillPercent) ? charForFill : emptySpaceChar, overwriteFilledSpaces, false);
                }
            }
        }

        //Go over the random noise created and check each spaces neighbors, flip it empty or filled based on the number of neighbors
        void SmoothMap(BoardGenerator boardGenerator)
        {
            for (int x = 0; x < boardGenerator.boardGenerationProfile.boardHorizontalSize; x++)
            {
                for (int y = 0; y < boardGenerator.boardGenerationProfile.boardVerticalSize; y++)
                {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y, boardGenerator);

                    if (neighbourWallTiles > 4)
                        boardGenerator.WriteToBoardGrid(x,y,charForFill,overwriteFilledSpaces, false);
                    else if (neighbourWallTiles < 4)
                        boardGenerator.WriteToBoardGrid(x, y, emptySpaceChar, overwriteFilledSpaces, false);

                }
            }
        }

        //Check how many neighbors each space has
        int GetSurroundingWallCount(int gridX, int gridY, BoardGenerator boardGenerator)
        {
            int wallCount = 0;
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                {
                    if (neighbourX >= 0 && neighbourX < boardGenerator.boardGenerationProfile.boardHorizontalSize && neighbourY >= 0 && neighbourY < boardGenerator.boardGenerationProfile.boardVerticalSize)
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            if (boardGenerator.boardGridAsCharacters[neighbourX, neighbourY] == charForFill)
                            {
                                wallCount++;
                            }
                        }
                    }
                    else
                    {
                        wallCount++;
                    }
                }
            }

            return wallCount;
        }

    }

}
