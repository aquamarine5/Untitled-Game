using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/Generator Space Divider")]

    public class GeneratorSpaceDivider : Generator
    {
        [Tooltip("The smallest size rect that can be created")]
        public int minimumDivisionSize = 5;
        [Tooltip("The chance to split vertically for rects that are not 1.25 times larger on horizontal or vertical to start")]
        public int chanceToSplitVertically = 50;
        [Tooltip("The character to write for walls")]
        public char wallCharToWrite = 'm';
        [Tooltip("How many times to attempt to divide the map into sub-rects")]
        public int iterations = 10;
        [Tooltip("How wide should doors between rects be")]
        public int wallGapSize = 4;
        [Tooltip("How thick should walls dividing rects be")]
        public int wallThickness = 2;
        public int initialRectMinX = 20;
        public int initialRectMaxX = 50;
        public int initialRectMinY = 20;
        public int initialRectMaxY = 50;
        public bool randomizeInitialRectOrigin;
        public bool drawBorder = true;
        public int numOpeningsInBorder = 1;
        public bool clearInternalEmptySpace;
        public char tileForInternalEmptySpace;

        //This is set to prevent endless loops, if you prefer you could switch the main for loop to a while, but this works
        int numberOfAttempts = 999;

        public override void Generate(BoardGenerator boardGenerator)
        {
            //Create a rect to represent the initial starting point, in this case the full map
            RoomRect fullRect = new RoomRect();

            

            if (randomizeInitialRectOrigin)
            {
                fullRect.rectOrigin.x = Random.Range(0 + wallThickness, boardGenerator.boardGenerationProfile.boardHorizontalSize - initialRectMaxX - wallThickness) ;
                fullRect.rectOrigin.y = Random.Range(0 + wallThickness, boardGenerator.boardGenerationProfile.boardVerticalSize - initialRectMaxY - wallThickness);
            }
            else
            {
                fullRect.rectOrigin.x = 0;
                fullRect.rectOrigin.y = 0;
            }


            //Create a list of RoomRects which we'll randomly pick from and subdivide
            List<RoomRect> roomRectList = new List<RoomRect>();
            

            if (initialRectMaxX < boardGenerator.boardGenerationProfile.boardHorizontalSize || initialRectMaxY < boardGenerator.boardGenerationProfile.boardVerticalSize)
            {
                fullRect.xSize = Random.Range(initialRectMinX, initialRectMaxX);
                fullRect.ySize = Random.Range(initialRectMinY, initialRectMaxY);
            }
            else
            {
                fullRect.xSize = boardGenerator.boardGenerationProfile.boardHorizontalSize;
                fullRect.ySize = boardGenerator.boardGenerationProfile.boardVerticalSize;
            }


            if (clearInternalEmptySpace)
            {
                for (int x = 0; x < fullRect.xSize; x++)
                {
                    for (int y = 0; y < fullRect.ySize; y++)
                    {
                        boardGenerator.WriteToBoardGrid(x + fullRect.rectOrigin.x, y + fullRect.rectOrigin.y, boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar(), true, generatesEmptySpace);
                    }
                }
            }

            if (drawBorder)
            {
                BuildBorder(boardGenerator, fullRect);
            }

            roomRectList.Add(fullRect);

            for (int i = 0; i < numberOfAttempts; i++)
            {

                if (roomRectList.Count > iterations)
                {
                    break;
                }
                RoomRect randomRect = roomRectList[Random.Range(0, roomRectList.Count)];
                if (randomRect.Split(boardGenerator, minimumDivisionSize, wallCharToWrite, overwriteFilledSpaces, wallGapSize, wallThickness, chanceToSplitVertically))
                {
                    if (randomRect.childRectA != null)
                    {
                        roomRectList.Add(randomRect.childRectA);
                    }

                    if (randomRect.childRectB != null)
                    {
                        roomRectList.Add(randomRect.childRectB);
                    }

                }

            }

        }


        public void BuildBorder(BoardGenerator boardGenerator, RoomRect rect)
        {
            // The outer walls are one unit left, right, up and down from the board.
            int leftEdgeX = rect.rectOrigin.x - 1;
            int rightEdgeX = rect.rectOrigin.x + rect.xSize;
            int bottomEdgeY = rect.rectOrigin.y - 1;
            int topEdgeY = rect.rectOrigin.y + rect.ySize;

            // Instantiate both vertical walls (one on each side).
            InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY, boardGenerator);
            InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY, boardGenerator);

            // Instantiate both horizontal walls, these are one in left and right from the outer walls.
            InstantiateHorizontalOuterWall(bottomEdgeY, leftEdgeX + 1, rightEdgeX - 1, boardGenerator);
            InstantiateHorizontalOuterWall(topEdgeY, leftEdgeX + 1, rightEdgeX - 1, boardGenerator);
            CutDoorInBorder(boardGenerator, rect);
        }

        void CutDoorInBorder(BoardGenerator boardGenerator, RoomRect rect)
        {

            for (int i = 0; i < numOpeningsInBorder; i++)
            {
                int pickBorderWallForDoor = Random.Range(0, 4);
                int doorX = 0;
                int doorY = 0;
                {
                    switch (pickBorderWallForDoor)
                    {
                        case 0:
                            //Make a hole in the top border wall
                            doorX = rect.rectOrigin.x + Random.Range(0, rect.xSize);
                            doorY = rect.rectOrigin.y + rect.ySize;
                            break;

                        case 1:
                            doorX = rect.rectOrigin.x + Random.Range(0, rect.xSize);
                            doorY = rect.rectOrigin.y - 1;
                            break;

                        case 2:
                            doorX = rect.rectOrigin.x + rect.xSize;
                            doorY = rect.rectOrigin.y + Random.Range(0, rect.ySize);
                            break;

                        case 3:
                            doorX = rect.rectOrigin.x - 1;
                            doorY = rect.rectOrigin.y + Random.Range(0, rect.ySize);
                            break;


                        default:
                            break;
                    }
                }

                for (int x = 0; x < wallGapSize + 1; x++)
                {
                    for (int y = 0; y < wallThickness + 1; y++)
                    {
                        boardGenerator.WriteToBoardGrid(doorX + x, doorY + y, boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar(), overwriteFilledSpaces, true);
                    }
                }

            }

           
        }

        void InstantiateVerticalOuterWall(int xCoord, int startingY, int endingY, BoardGenerator boardGenerator)
        {

            // While the value for Y is less than the end value...

            for (int i = startingY; i < endingY+wallThickness; i++)
            {
                for (int j = 0; j < wallThickness; j++)
                {

                    boardGenerator.WriteToBoardGrid(xCoord + j, i, wallCharToWrite, overwriteFilledSpaces, false);
                }

            }
        }

        void InstantiateHorizontalOuterWall(int yCoord, int startingX, int endingX, BoardGenerator boardGenerator)
        {

            // While the value for Y is less than the end value...

            for (int i = startingX; i < endingX + wallThickness; i++)
            {
                for (int j = 0; j < wallThickness; j++)
                {

                    boardGenerator.WriteToBoardGrid(i, yCoord + j, wallCharToWrite, overwriteFilledSpaces, false);
                }

            }
        }
        
    }
    
}
