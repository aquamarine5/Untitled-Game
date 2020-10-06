using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    /// <summary>
    /// This is one of the main generators in Strata, it starts from an initial location in the top/northern row of the grid
    /// and then spawns RoomTemplates in sequence, randomly walking downward through the level. 
    /// It checks the doors you've labeled in the RoomTemplate and makes sure that each room spawned is connected. This is
    /// for generating connected vertical structures that are definitely connected and well suited to platformer games. It stops 
    /// when it reaches the bottom of the level. Then you can optionally fill the rest of the grid with random rooms.This approach is
    /// heavily inspired by the level generation in Derek Yu and Andy Hull's classic roguelike platformer Spelunky.
    ///  </summary>

    [CreateAssetMenu(menuName = "Strata/Generators/Platformer Room Chain")]

    public class GeneratorPlatformerRoomChain : Generator
    {
        //Dimensions on x and y of the RoomTemplate, this has been tested with regular, equally sized rooms, YMMV with irregular rooms.
        public int roomSizeX = 10;
        public int roomSizeY = 10;

        //RoomLists of all rooms organized by their exits.
        public RoomList eastWestExits;
        public RoomList northSouthExits;
        public RoomList hasSouthExit;
        public RoomList hasNorthExit;

        //Set to true to fill the area not filled by the critical path from entrance to exit with random rooms
        public bool fillEmptySpaceWithRandomRooms = false;
        //Add rooms to this list to fill unused grid space randomly
        public RoomList randomFillRooms;

        //The RoomTemplate for the first room
        public RoomTemplate firstRoom;
        //The RoomTemplate for the last room, note if you want your player to progress bottom to top 
        // just swap this with firstRoom to have entrance on bottom, exit on top.
        public RoomTemplate lastRoom;

        //Sanity check for the generation loop, if we try 999 times to generate and fail, stop trying and show an error.
        int maximumGenerationAttempts = 999;

        //This is the function called by BoardGenerator
        public override void Generate(BoardGenerator boardGenerator)
        {
            //The number 999 is here to have a finite number of attempts to create a usable path and avoid infinite loops if we get into a failure state

            bool generationSucceeded = false;
            int generationAttempts = 0;

            while (!generationSucceeded)
            {
                RoomChain roomChainComponent = SetupRoomChainComponent(boardGenerator);

                if (BuildPath(boardGenerator, roomChainComponent))
                {
                    generationSucceeded = true;
                    break;
                }
                else
                {
                    generationSucceeded = false;
                }

                generationAttempts++;
                if (generationAttempts > maximumGenerationAttempts)
                {
                    Debug.LogError("Generation failed after " + maximumGenerationAttempts + " try to tweak your parameters to create something more likely to succeed by lowering minimum generated rooms and raising chance to continue growing.");
                    break;
                }
            }
            
        }

        public RoomChain SetupRoomChainComponent(BoardGenerator boardGenerator)
        {
            GameObject oldRoomChain = GameObject.Find("RoomChainHolder");
            if (oldRoomChain != null)
            {
                DestroyImmediate(oldRoomChain);
            }
            RoomChain roomChainComponent = new GameObject("RoomChainHolder").AddComponent<RoomChain>();
            return roomChainComponent;
        }

        //This is used to see if we have a valid space to place a RoomTemplate
        bool TestIfGridIndexIsValid(int x, int y, int gridWidthX, int gridWidthY)
        {
            if (x > gridWidthX-1 || x < 0 || y > gridWidthY-1 || y < 0 )
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        bool BuildPath(BoardGenerator boardGenerator, RoomChain roomChainComponent)
        {
            //We will return this when we have finished building our path
            bool pathBuildComplete = false;

            //This is to check if we still need to place our last or exit room, the end of our path
            bool lastRoomPlaced = false;

            //Figure out how many rooms we will need to fill the board horizontally
            int horizontalRoomsToFill = boardGenerator.boardGenerationProfile.boardHorizontalSize / roomSizeX;
            //Figure out how many rooms we need vertically
            int verticalRoomsToFill = boardGenerator.boardGenerationProfile.boardVerticalSize / roomSizeY;

            //Create a two dimensional array of RoomTemplates in a grid based on the number of rooms
            //This is slightly different from our other RoomChain approach in that we work with a grid of rooms
            //during path creation, then spawn them to tiles.
            RoomTemplate[,] roomTemplateGrid = new RoomTemplate[horizontalRoomsToFill,verticalRoomsToFill];

            //Create a variable to store the last direction we moved, this helps to avoid doubling back on the path
            RoomChain.Direction lastMoveDirection = RoomChain.Direction.NoMove;

            //Pick a random space in the room grid in the top row
            int startIndex = Random.Range(0, horizontalRoomsToFill);

            //Place first room in random position in top row
            roomTemplateGrid[startIndex, verticalRoomsToFill - 1] = firstRoom;

            //What column of our grid are we writing to? Set initially to the random one chosen in startIndex
            int columnIndex = startIndex;

            //Set our rowIndex to the top row in our grid
            int rowIndex = verticalRoomsToFill - 1;

            Vector2 roomChainPos = new Vector2(columnIndex * roomSizeX, rowIndex * roomSizeY);
            Vector2 arrowPosition = roomChainPos + new Vector2(roomSizeX * .5f, roomSizeY * .5f);
            roomChainComponent.SetupDebugObject(roomTemplateGrid[columnIndex, rowIndex], roomChainPos, arrowPosition, 0, lastMoveDirection);

            //Loop over the length of our grid minus one and place rooms.
            for (int i = 1; i < roomTemplateGrid.Length - 1; i++)
            {
                //Make sure we never try to place rooms outside the grid
                if (columnIndex > horizontalRoomsToFill || columnIndex < 0 || rowIndex < 0 || rowIndex > verticalRoomsToFill)
                {
                    break;
                }

                //Choose the direction to move in next 0,1 are right 2,3 are left 4 is move down
                int randomDir = 0;
                //If we're placing the first room, don't move down immediately
                if (i == 1)
                {
                    randomDir = Random.Range(0, 4);
                }
                //Otherwise, pick from all directions, including down
                else
                {
                    randomDir = Random.Range(0, 5);
                }
                //Use this to check if we have successfully created a room
                bool roomCreated = false;

                //Decide what to do based on our random number
                switch (randomDir)
                {
                    case 0:
                    case 1:


                        //Move Right
                        //Check if we have a valid grid index to the right of our current index, also make sure that we haven't moved east/right last move to avoid doubling back
                        if (TestIfGridIndexIsValid(columnIndex + 1, rowIndex, horizontalRoomsToFill, verticalRoomsToFill) && lastMoveDirection != RoomChain.Direction.West)
                        {
                            //Pick one of the rooms that has exits to the east and west (could rewrite for one horizontal direction but this leads to more player choice IMO, less dead ends)
                            RoomTemplate room = eastWestExits.roomList[Random.Range(0, eastWestExits.roomList.Count)];
                            //If the space in the room grid we are trying to write to is empty
                            if (roomTemplateGrid[columnIndex + 1, rowIndex] == null)
                            {
                                //Set it to the random room we chose
                                roomTemplateGrid[columnIndex + 1, rowIndex] = room;
                                //Increment the column index (moving to new location for generation)
                                columnIndex = columnIndex + 1;
                                //We successfully created a room
                                roomCreated = true;
                                //Record the direction we moved to avoid doubling back immediately
                                lastMoveDirection = RoomChain.Direction.East;
                            }
                                
                        }
                        //Exit the switch
                        break;

                    case 2:
                    case 3:

                        //Move Left
                        //Check if we have a valid grid index to the left of our current index, also make sure that we haven't moved west/left last move to avoid doubling back
                        if (TestIfGridIndexIsValid(columnIndex - 1, rowIndex, horizontalRoomsToFill, verticalRoomsToFill) && lastMoveDirection != RoomChain.Direction.East)
                        {
                            //Pick one of the rooms that has exits to the east and west (could rewrite for one horizontal direction but this leads to more player choice IMO, less dead ends)
                            RoomTemplate room = eastWestExits.roomList[Random.Range(0, eastWestExits.roomList.Count)];
                            //If the space in the room grid we are trying to write to is empty
                            if (roomTemplateGrid[columnIndex - 1, rowIndex] == null)
                            {
                                //Set it to the random room we chose
                                roomTemplateGrid[columnIndex - 1, rowIndex] = room;
                                //Decrement the column index (moving to new location for generation)
                                columnIndex = columnIndex - 1;
                                //We successfully created a room
                                roomCreated = true;
                                //Record the direction we moved to avoid doubling back immediately
                                lastMoveDirection = RoomChain.Direction.West;
                            }
                            
                        }
                        //Exit the switch
                        break;
                    case 4:
                        //Move Down
                        //Check if the space below is valid and that we didn't just move down (optional, could be removed if you want to create pits, I found them boring level design wise)
                        if (TestIfGridIndexIsValid(columnIndex, rowIndex-1, horizontalRoomsToFill, verticalRoomsToFill) && lastMoveDirection != RoomChain.Direction.South)
                        {
                            //Pick a random roomtemplate
                            RoomTemplate room = hasSouthExit.roomList[Random.Range(0, hasSouthExit.roomList.Count)];
                            //Assign it to the grid index
                            roomTemplateGrid[columnIndex, rowIndex] = room;
                            //Record the move direction
                            lastMoveDirection = RoomChain.Direction.South;

                            //Spawn the connected room below it as well, a room with entrance to north/up
                            //Make sure we're not at bottom of grid
                            if (rowIndex > 0)
                            {
                                //Decrement the row index, move down a row
                                rowIndex--;
                                //Pick a room from the northList
                                RoomTemplate roomWithNorthExit = hasNorthExit.roomList[Random.Range(0, hasNorthExit.roomList.Count)];
                                //Assign it to the grid
                                roomTemplateGrid[columnIndex, rowIndex] = roomWithNorthExit;
                            }
                            else
                            {
                                //If we try to move down but are in the bottom row, place the last room.
                                if (!lastRoomPlaced)
                                {
                                    roomTemplateGrid[columnIndex, rowIndex] = lastRoom;
                                    lastRoomPlaced = true;
                                    break;
                                }
                                
                            }
                            //Set roomCreated to true, success
                            roomCreated = true;
                        }
                        break;
                }

                //If we fell through the switch and didn't fulfill any of the above conditions meaning we got stuck, move down
                if (roomCreated == false)
                {
                    //Place a random northSouth exiting room
                    roomTemplateGrid[columnIndex, rowIndex] = northSouthExits.roomList[Random.Range(0, northSouthExits.roomList.Count)];
                    //Record the move direction
                    lastMoveDirection = RoomChain.Direction.South;

                    //If we're not at the bottom of the grid
                    if (rowIndex > 0)
                    {
                        //Go down a row
                        rowIndex--;
                        //And place the room with an north/up entrance on the bottom
                        roomTemplateGrid[columnIndex, rowIndex] = hasNorthExit.roomList[Random.Range(0, hasNorthExit.roomList.Count)];
                    }
                    //If we're in the bottom row
                    else
                    {
                        //And we haven't placed the last room
                        if (!lastRoomPlaced)
                        {
                            //Place the last room
                            roomTemplateGrid[columnIndex, rowIndex] = lastRoom;
                            //Record that we've done it so we won't do it twice
                            lastRoomPlaced = true;
                            break;
                        }
                    }
                    //We successfully created a room    
                    roomCreated = true;
                }

                roomChainPos = new Vector2(columnIndex * roomSizeX, rowIndex * roomSizeY);
                arrowPosition = roomChainPos + new Vector2(roomSizeX * .5f, roomSizeY * .5f);
                roomChainComponent.SetupDebugObject(roomTemplateGrid[columnIndex, rowIndex], roomChainPos, arrowPosition, i, lastMoveDirection);

            }            
            //Loop over the array of roomTemplates from bottom to top until we find the first room to make sure we have a complete path
            for (int j = 0; j < horizontalRoomsToFill; j++)
            {
                for (int k = 0; k < verticalRoomsToFill; k++)
                {
                    if (roomTemplateGrid[j, k] == firstRoom)
                    {
                        pathBuildComplete = true;
                    }
                }
            }

          
            //If we want to fill the rest of the grid with random rooms, do so.
            if (fillEmptySpaceWithRandomRooms)
            {
                FillGridWithRandomRooms(boardGenerator, roomTemplateGrid, horizontalRoomsToFill, verticalRoomsToFill);

            }

            //This is just used to number placeholder objects for debugging in scene view
            int roomChainNumber = 0;

            //Loop over the roomTemplateGrid and write the rooms to the board grid
            for (int x = 0; x < horizontalRoomsToFill; x++)
            {
                for (int y = 0; y < verticalRoomsToFill; y++)
                {
                   //Get the position in worldspace of this roomTemplate
                    Vector2 roomPos = new Vector2(x * roomSizeX, y * roomSizeY);
                    //Get the current roomTemplate from the roomTemplateGrid
                    RoomTemplate templateToWrite = roomTemplateGrid[x, y];

                    //If it's not null
                    if (templateToWrite != null)
                    {
                        
                        roomChainNumber++;
                        //Write the room to the boardGrid array
                        WriteChainRoomToGrid(boardGenerator, roomChainComponent, roomPos, templateToWrite, roomChainNumber, true);
                    }
                    
                }
            }
            //Return the result
            return pathBuildComplete;
        }


        public void WriteChainRoomToGrid(BoardGenerator boardGenerator, RoomChain roomChainComponent, Vector2 roomOrigin, RoomTemplate roomTemplate, int chainNumber, bool isOnPath)
        {

            //Index for the current character we're writing to the grid
            int charIndex = 0;
            //Loop over the roomTemplate
            for (int i = 0; i < roomSizeX; i++)
            {
                for (int j = 0; j < roomSizeY; j++)
                {
                    //Get the character stored in the roomTemplate
                    char selectedChar = roomTemplate.roomChars[charIndex];
                    //Make sure it's not character equivalent of null
                    if (selectedChar != '\0')
                    {
                        //Create a Vector2 position for it, offset from the origin of the room
                        Vector2 spawnPos = new Vector2(i, j) + roomOrigin;

                        //Cast those positions to ints so we can feed them to our grid
                        int x = (int)spawnPos.x;
                        int y = (int)spawnPos.y;

                        //Write the selected character to the boardGrid
                        boardGenerator.WriteToBoardGrid(x, y, selectedChar, overwriteFilledSpaces, isOnPath);
                    }
                    //Increment to get ready for the next character to be read
                    charIndex++;

                }
            }
        }

        //This is used to pick random rooms to fill the rest of the grid, beyond the path from entrance to exit
        public void FillGridWithRandomRooms(BoardGenerator boardGenerator, RoomTemplate[,] roomTemplateGrid, int horizontalRoomsToFill, int verticalRoomsToFill)
        {
            //Loop over the boardGrid
            for (int x = 0; x < horizontalRoomsToFill; x++)
            {
                for (int y = 0; y < verticalRoomsToFill; y++)
                {
                    //If the roomTemplate in the roomTemplateGrid is not null
                    if (roomTemplateGrid[x,y] == null)
                    {
                        //Pick a random room
                        RoomTemplate selectedRoom = randomFillRooms.roomList[Random.Range(0, randomFillRooms.roomList.Count)];
                        //Write it to the grid
                        roomTemplateGrid[x, y] = selectedRoom;

                    }

                }
            }
        }

    }
}

