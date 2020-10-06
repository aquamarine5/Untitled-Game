using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    public class RoomRect
    {
        //If a rect has already split, we'll skip it
        public bool splitAlready = false;

        //Gridposition used to hold the lower left corner of the rect we're working with
        public GridPosition rectOrigin = new GridPosition(0, 0);

        //Horizontal size of the rect
        public int xSize;
        //Vertical size of the rect
        public int ySize;

        //Sub rects, children of this one which will hold the products of splitting
        public RoomRect childRectA;
        public RoomRect childRectB;
        

        //This function is called by the generator and will split this rect and start a recursive process of splitting until we can't split any further
        public bool Split(BoardGenerator boardGenerator, int minimumDivisionSize, char wallCharToWrite, bool overwriteFilledSpaces, int wallGapSize, int wallThickness, int verticalSplitChance)
        {
            //If a rect has already split, we'll skip it
            if (splitAlready)
            {
                return false;
            }

            //Safety to return if for some reason we have sizes of 0
            if (xSize == 0 || ySize == 0)
            {
                return false;
            }

            //There's a fifty percent chance to split horizontally or vertically
            bool splitVertical = boardGenerator.RollPercentage(verticalSplitChance);


            //Before we decide randomly to split, check if the rect we're splitting is much bigger along one axis and use that instead
            if (xSize > ySize && xSize / ySize >= 1.25f)
            {
                splitVertical = true;
            }
            else if (ySize > xSize && ySize / xSize >= 1.25f)
            {
                splitVertical = false;
            }

            //Otherwise use the random decision we previously rolled

            //Determine the maximum size that our rect is allowed to be, based on whether we're splitting vertically or horizontally
            int maxSize = (splitVertical ? xSize : ySize) - minimumDivisionSize;

            //If the maxSize is smaller than or equal to the minimumSize, then stop
            if (maxSize <= minimumDivisionSize)
            {
                return false;
            }

            //This will hold the index along the dividing wall at which we'll place an opening to ensure connectivity
            int splitPoint = 0;

            //Two new RoomRects which will hold the children and we'll fill with data as we split
            childRectA = new RoomRect();
            childRectB = new RoomRect();


            //If we've decided to split vertically and our rect is larger than the minimumDivisionSize
            if (splitVertical && xSize > minimumDivisionSize)
            {
                //Pick a random point to split the rect that is between the minimumDivision size and the maxSize we calculated
                splitPoint = Random.Range(minimumDivisionSize, maxSize);

                //Create the first new rect which is the same vertical size but only as wide as the splitpoint
                childRectA.ySize = ySize;
                childRectA.xSize = splitPoint;

                //Create the second new rect which is the same vertical size but only as big as the remainder of the parent rect after split
                childRectB.ySize = ySize;
                childRectB.xSize = xSize - splitPoint;

                //Set the origin of the A to the origin of the parent
                childRectA.rectOrigin = rectOrigin;
                //Set the origin of B to the splitpoint
                childRectB.rectOrigin = new GridPosition(splitPoint + rectOrigin.x, rectOrigin.y);

                //Calculate an index for where to place the opening, it should be far enough away from the start and end of both ends of the wall
                int randomDoorIndex = Random.Range(minimumDivisionSize / 2 + wallThickness, ySize - minimumDivisionSize / 2 - wallThickness);
                //Make sure that during calculation we haven't produced a negative number or a number that's greater than our dividing wall length
                Mathf.Clamp(randomDoorIndex, 0, ySize - 1);
                //This bool is there for error reporting in case we input inspector values that lead to a door not being produced
                bool madeDoor = false;

                //Loop over the vertical length of the parent rect
                for (int i = 0; i < ySize; i++)
                {
                    //This nested loop will repeat the wall based on wallThickness, if we want walls that are multiple units thick
                    for (int j = 0; j < wallThickness; j++)
                    {
                        //This checks if our loop has reached the index to leave a space for a door and allows for doors that are multiple units wide
                        if (i >= randomDoorIndex && i < randomDoorIndex + wallGapSize)
                        {
                            //Write empty space to the boardGrid where the door should be (note you could place door tiles here if you wanted, just replace the third argument with a door tile)
                            boardGenerator.WriteToBoardGrid(splitPoint + childRectA.rectOrigin.x + j, childRectA.rectOrigin.y + i, boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar(), overwriteFilledSpaces, false);
                            //We successfully made at least one door space so set this to true, we won't throw up a warning
                            madeDoor = true;

                        }
                        else
                        {
                            //Now we've placed empty space for our door, place filled space.
                            //We want to make sure that we don't have walls intersecting other doors, so we check ahead of the wall if it's at the end
                            //This is to avoid creating a 'T' shape where a divider blocks a door
                            if (childRectA.rectOrigin.y + (i + wallGapSize) < boardGenerator.boardGenerationProfile.boardVerticalSize)
                            {
                                //If we're about to join up the T and block the door, just exit the loop
                                if (i >= ySize - wallGapSize)
                                {
                                    if (boardGenerator.boardGridAsCharacters[splitPoint + childRectA.rectOrigin.x + j, childRectA.rectOrigin.y + (i + wallGapSize)] == boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar())
                                    {
                                        break;
                                    }
                                }

                                //If none of those corner case conditions are met, place the filled wall tiles for our dividing wall
                                boardGenerator.WriteToBoardGrid(splitPoint + childRectA.rectOrigin.x + j, childRectA.rectOrigin.y + i, wallCharToWrite, overwriteFilledSpaces, false);

                            }
                        }
                    }
                    
                }
                //If we failed to make a door when we should have, log an error to the console.
                if (madeDoor == false)
                {
                    Debug.Log("<color=red> Split vertical : failed to make door </color>" + childRectA.rectOrigin.x + " " + childRectA.rectOrigin.y + " xSize " + childRectA.xSize + " ySize " + childRectA.ySize);
                }
                //Set split already to true so we don't try to split again and return true, that we succeeded
                splitAlready = true;
                
                return true;
            }
            else if (ySize > minimumDivisionSize)
            {
                //This code duplicates the code above but flipped horizontally, please refer to the above code comments, it will all be the same
                // but with an inversion of x and y. At some point I may try to abstract this into a single function but for now it works with
                // the code duplication.

                splitPoint = Random.Range(minimumDivisionSize, maxSize);
                childRectA.ySize = splitPoint;
                childRectA.xSize = xSize;

                childRectB.ySize = ySize - splitPoint;
                childRectB.xSize = xSize;

                childRectA.rectOrigin = rectOrigin;
                childRectB.rectOrigin = new GridPosition(rectOrigin.x, rectOrigin.y + splitPoint);


                int randomDoorIndex = Random.Range(minimumDivisionSize / 2 + wallThickness, xSize - minimumDivisionSize / 2 - wallThickness);
                Mathf.Clamp(randomDoorIndex, 0, xSize-1);
                bool madeDoor = false;
                for (int i = 0; i < xSize; i++)
                {
                    for (int j = 0; j < wallThickness; j++)
                    {
                        if (i >= randomDoorIndex && i < randomDoorIndex + wallGapSize)
                        {
                            boardGenerator.WriteToBoardGrid(childRectA.rectOrigin.x + i, splitPoint + childRectA.rectOrigin.y + j, boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar(), overwriteFilledSpaces, false);
                            madeDoor = true;
                        }
                        else
                        {
                            if (childRectA.rectOrigin.x + (i + wallGapSize) < boardGenerator.boardGenerationProfile.boardHorizontalSize)
                            {
                                if (i >= xSize - wallGapSize && boardGenerator.boardGridAsCharacters[childRectA.rectOrigin.x + (i + wallGapSize), splitPoint + childRectA.rectOrigin.y + j] == boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar())
                                {
                                    break;
                                }
                                else
                                {
                                    boardGenerator.WriteToBoardGrid(childRectA.rectOrigin.x + i, splitPoint + childRectA.rectOrigin.y + j, wallCharToWrite, overwriteFilledSpaces, false);
                                }
                            }
                            
                        }
                    }
                    

                    

                }
               
                if (madeDoor == false)
                {
                    Debug.Log("<color=red> failed to make door </color>" + childRectA.rectOrigin.x + " " + childRectA.rectOrigin.y + " xSize " + childRectA.xSize + " ySize " + childRectA.ySize);
                }
                splitAlready = true;
                return true;
            }
            //This should happen extremely rarely but if for some reason we failed to be able to split in either direction, log to console.
            //If you see this, you probably should try to adjust your inspector values.
            else
            {
                Debug.Log("failed to split vertical or horizontal");
                return false;
            }
            
        }
    }


}
