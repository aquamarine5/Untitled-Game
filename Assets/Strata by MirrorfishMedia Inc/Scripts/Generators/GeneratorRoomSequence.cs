using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/Room Sequence")]

    public class GeneratorRoomSequence : Generator
    {
        
        [Tooltip("This generator will repeat, discarding it's output until it has this many rooms. Be careful not to try to generate too many rooms in too small a space.")]
        public int minimumGeneratedRooms = 10;
        [Tooltip("The highest number of rooms we want to generate")]
        public int maximumGeneratedRooms = 20;


        //Dimensions on x and y of the RoomTemplate, this has been tested with regular, equally sized rooms, YMMV with irregular rooms.
        [Tooltip("Dimensions on x and y of the RoomTemplate, this has been tested with regular, equally sized rooms, YMMV with irregular rooms.")]
        public int roomSizeX = 10;
        public int roomSizeY = 10;

        [Tooltip("List of rooms from which to choose a starting room.")]
        public RoomList startRoomList;
        public bool placeCustomEndRoom = false;
        public RoomList endRoomList;

        //RoomLists of all rooms organized by their exits.
        public RoomList hasSouthExit;
        public RoomList hasNorthExit;
        public RoomList hasEastExit;
        public RoomList hasWestExit;

        [Tooltip("If true we use a position from list, otherwise a random grid position (ex 10,20)")]
        public bool pickStartPositionFromList = false;
        public GridPosition[] startPositions;

        [Tooltip("Set to true to destroy the RoomChain GameObject when generation is complete, this is needed if you will run multiple roomchains in sequence.")]

        public bool destroyRoomChainHolderOnCompletion = true;

        //Set to true if you want to fill the grid with random rooms after generating the main path.
        public bool fillEmptySpaceWithRandomRooms;
        public RoomList randomFillRoomList;

        //Sanity check for the generation loop, if we try 999 times to generate and fail, stop trying and show an error.
        int maximumAttempts = 999;



        //This function creates a temporary GameObject with a RoomChain component to keep track of
        //generation (how many rooms generated) without storing that in the BoardGenerator. This 
        //helps to clean up repeated generation attempts, we just destroy and recreate each time.
        private RoomChain SetupRoomChainComponent(BoardGenerator boardGenerator)
        {
            //Look for a RoomChainHolder
            GameObject oldRoomChain = GameObject.Find("RoomChainHolder");
            //If one already exists..
            if (oldRoomChain != null)
            {
                //Destroy it
                DestroyImmediate(oldRoomChain);
            }
            //Then create a new GameObject with a new RoomChain component and return it so we can use it
            RoomChain roomChainComponent = new GameObject("RoomChainHolder").AddComponent<RoomChain>();
            return roomChainComponent;
        }

        //This is called by BoardGenerator to start the generation process
        public override void Generate(BoardGenerator boardGenerator)
        {
            //Figure out how many rooms we will need to fill the board horizontally
            int horizontalRoomsToFill = boardGenerator.boardGenerationProfile.boardHorizontalSize / roomSizeX;
            //Figure out how many rooms we need vertically
            int verticalRoomsToFill = boardGenerator.boardGenerationProfile.boardVerticalSize / roomSizeY;
            //Use this to sanity check the while loop of generation
            int generationAttempts = 0;

            bool generationSucceeded = false;

            //Loop and discard the generated output until we find something that meets our criteria
            while (!generationSucceeded)
            {
                generationAttempts++;
                //Create a new roomChainComponent for each attempt
                RoomChain roomChainComponent = SetupRoomChainComponent(boardGenerator);

                //Set up a grid of RoomTemplate objects, we'll work on this grid until we've generated
                //what we want, then we'll turn these into actual tiles. The only potential (minor)
                //drawback of generating is that our rooms will always start on the grid (10x10 for example)
                //IMO this is a small price to pay for the ease of reasoning about the grid that this gets us
                RoomTemplate[,] roomTemplateGrid = new RoomTemplate[horizontalRoomsToFill, verticalRoomsToFill];

                //Build the sequence of rooms into the grid, this will return a roomTemplateGrid with just the main path if successful
                roomTemplateGrid = BuildRoomSequence(roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill);

                //If roomTemplateGrid comes back null it means we've failed, so we want to retry generating
                if (roomTemplateGrid != null)
                {
                    //Turn the rooms generated in the grid into chararcters in BoardGenerator
                    //We don't write anything to BoardGenerator until we know we've got a valid
                    //roomTemplateGrid, this saves BoardGenerator having to reset during generation
                    WriteFilledRooms(boardGenerator, roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill, roomTemplateGrid);
                    generationSucceeded = true;
                    if (destroyRoomChainHolderOnCompletion)
                    {
                        Debug.Log(3);
                        if (Application.isPlaying&&Application.isEditor)
                        {
                            Debug.Log(1);
                            Destroy(roomChainComponent.gameObject);
                        }
                        else
                        {
                            Debug.Log(2);
                            DestroyImmediate(roomChainComponent.gameObject);
                        }
                        
                    }
                    break;
                }
                else
                {
                    //If we haven't met our generation criteria, set up and try again until we have
                    //or we run out of attempts
                    roomChainComponent = SetupRoomChainComponent(boardGenerator);
                    roomTemplateGrid = BuildRoomSequence(roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill);
                }

                //Do we want to fill the rest of the grid with random rooms?
                //This can help hide the main path if you want to.
                if (fillEmptySpaceWithRandomRooms)
                {
                    FillUnusedSpaceWithRandomRooms(horizontalRoomsToFill, verticalRoomsToFill, roomTemplateGrid);
                }

                //Sanity check generation and log an error if we ran out of attempts
                if (generationAttempts > maximumAttempts)
                {
                    Debug.LogError("Generation failed after " + maximumAttempts + " try to tweak your parameters to create something more likely to succeed by lowering minimum generated rooms and raising chance to continue growing.");
                    Destroy(roomChainComponent.gameObject);
                    break;
                }
            }
        }

        private void WriteFilledRooms(BoardGenerator boardGenerator, RoomChain roomChainComponent, int horizontalRoomsToFill, int verticalRoomsToFill, RoomTemplate[,] finalRoomGrid)
        {
            int roomChainNumber = 0;

            for (int x = 0; x < horizontalRoomsToFill; x++)
            {
                for (int y = 0; y < verticalRoomsToFill; y++)
                {
                    //Get the position in worldspace of this roomTemplate
                    Vector2 roomPos = new Vector2(x * roomSizeX, y * roomSizeY);
                    //Get the current roomTemplate from the roomTemplateGrid
                    RoomTemplate templateToWrite = finalRoomGrid[x, y];
                    //If it's not null
                    if (templateToWrite != null)
                    {
                        roomChainNumber++;
                        //Write the room to the boardGrid array
                        WriteChainRoomToGrid(boardGenerator, roomChainComponent, roomPos, templateToWrite, roomChainNumber, true);
                    }
                }
            }

        }

        //The main generation function for this Generator, this builds a linear sequence of roomTemplates
        private RoomTemplate[,] BuildRoomSequence(RoomChain roomChainComponent, int horizontalRoomsToFill, int verticalRoomsToFill)
        {
            //Create a two dimensional array of RoomTemplates in a grid based on the number of rooms
            RoomTemplate[,] filledGrid = new RoomTemplate[horizontalRoomsToFill, verticalRoomsToFill];

            int xStart = 0;
            int yStart = 0;
            if (!pickStartPositionFromList)
            {
                //Pick a random space in the room grid
                xStart = Random.Range(0, horizontalRoomsToFill);
                yStart = Random.Range(0, verticalRoomsToFill);
            }
            else
            {
                //Otherwise pick a pre-defined position from the startPositions array
                GridPosition randomListPosition = startPositions[Random.Range(0, startPositions.Length)];
                xStart = randomListPosition.x;
                yStart = randomListPosition.y;
            }

            //RoomTileSpace is a helper class that here stores coordinates and a list of rooms to pick from
            //In this case we're picking a random room from the startRoomList
            RoomTileSpace firstRoomTileSpace = new RoomTileSpace(xStart, yStart, startRoomList, RoomChain.Direction.NoMove);

            //Create a holder for the current coordinates and roomTemplate based on the firstRoomTileSpace
            RoomTileSpace currentRoomTileSpace = SelectNextRoomInSequence(roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill, filledGrid, firstRoomTileSpace);
            //Store the last valid roomTileSpace we got, in case it's the end
            RoomTileSpace endTileSpace = currentRoomTileSpace;
            //Loop until we have the maximum number of generated rooms or fail
            for (int i = 0; i <= maximumGeneratedRooms; i++)
            {
                //Pick new coordinates and room and store it in currentRoomTileSpace
                if (currentRoomTileSpace != null)
                {
                    currentRoomTileSpace = SelectNextRoomInSequence(roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill, filledGrid, currentRoomTileSpace);
                    if (currentRoomTileSpace != null)
                    {
                        //Store the last valid roomTileSpace we got, in case it's the end
                        endTileSpace = currentRoomTileSpace;

                        //If this worked, add it to the roomTileGrid we'll use for final spawning

                        filledGrid[currentRoomTileSpace.x, currentRoomTileSpace.y] = currentRoomTileSpace.roomTemplate;

                    }
                    else
                    {
                        if (placeCustomEndRoom)
                        {
                            //Use the last valid position stored in endTileSpace to place the end room

                            filledGrid[endTileSpace.x, endTileSpace.y] = endRoomList.RandomRoom();
                        }
                        //If it failed and came back empty, let's leave the loop so we can
                        //test if we've achieved the minimum, otherwise restart
                        break;
                    }

                    
                }
                
            }

            //if we exceeded the minimum, this meets our standards so we'll return it
            if (roomChainComponent.roomsCreated >= minimumGeneratedRooms)
            {
                
                return filledGrid;
            }
            else
            {
                //Otherwise we'll pass back null and the calling function will know we've failed
                //and that means we need to try again
                return null;
            }
        }

        //This function looks at the current room, sees what exits it has, picks one randomly
        //and then attempts to move in that direction. If it runs into a corner
        //and can't go further it will return null to tell use we need to try again.
        private RoomTileSpace SelectNextRoomInSequence(RoomChain roomChainComponent, int horizontalRoomsToFill, int verticalRoomsToFill, RoomTemplate[,] roomTemplateGrid, RoomTileSpace currentSpace)
        {
            //Pick a random roomTemplate for this space from the current RoomTileSpace
            currentSpace.SelectRoomTemplateForSpace();
            Vector2 roomPosition = new Vector2(currentSpace.x * roomSizeX, currentSpace.y * roomSizeY);
            Vector2 arrowPosition = roomPosition + new Vector2(roomSizeX * .5f, roomSizeY * .5f);
            roomChainComponent.SetupDebugObject(currentSpace.roomTemplate, roomPosition, arrowPosition, 0, currentSpace.pathDirection);
            //Set it into the roomTemplateGrid
            roomTemplateGrid[currentSpace.x, currentSpace.y] = currentSpace.roomTemplate;

            //Create a list of directions that we'll pick from to try to extend the sequence
            List<RoomChain.Direction> possibleDirections = new List<RoomChain.Direction>();

            //Check the roomTemplate for what exits it has, and add them to our list
            if (currentSpace.roomTemplate.opensToNorth)
            {
                possibleDirections.Add(RoomChain.Direction.North);
            }
            if (currentSpace.roomTemplate.opensToEast)
            {
                possibleDirections.Add(RoomChain.Direction.East);
            }
            if (currentSpace.roomTemplate.opensToSouth)
            {
                possibleDirections.Add(RoomChain.Direction.South);
            }
            if (currentSpace.roomTemplate.opensToWest)
            {
                possibleDirections.Add(RoomChain.Direction.West);
            }

            //This will become true once we find a direction to go in
            bool foundNextDirection = false;

            //We'll pick random directions and try to move until we've found one
            while (!foundNextDirection)
            {
                //If there are no directions in our list it means we've run out of space, time
                //to quit and retry
                if (possibleDirections.Count <= 0)
                {
                    return null;
                }
                else
                {
                    //Pick a random direction from our list of possible directions
                    RoomChain.Direction nextDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
                    //Remove it from the list (we might use this later to pick more than one direction)
                    possibleDirections.Remove(nextDirection);

                    //Now that we've got a direction, let's use it
                    switch (nextDirection)
                    {
                        case RoomChain.Direction.North:
                            //First check if the space we're trying to move to is in the grid, otherwise do nothing
                            if (TestIfGridIndexIsValid(currentSpace.x, currentSpace.y + 1, horizontalRoomsToFill, verticalRoomsToFill))
                            {
                                //These just make the below code more readable
                                int x = currentSpace.x;
                                int y = currentSpace.y + 1;

                                //Check to make sure the space we're trying to fill is empty
                                //This is important because it prevents doubling back and cutting
                                //off the path
                                if (roomTemplateGrid[x, y] == null)
                                {
                                    //Tell the roomChainComponent that we've added a room and made our
                                    //sequence longer
                                    roomChainComponent.roomsCreated++;
                                    //Since we've found a workable direction, we can stop looping
                                    foundNextDirection = true;
                                    //Return the result as a RoomTileSpace with coordinates and a list
                                    //of rooms to pick from that all have connected exits
                                    return new RoomTileSpace(x, y, hasSouthExit, RoomChain.Direction.North);
                                }
                            }
                            break;
                        //The other three directions below all use the same formula as above^^
                        case RoomChain.Direction.East:

                            if (TestIfGridIndexIsValid(currentSpace.x + 1, currentSpace.y, horizontalRoomsToFill, verticalRoomsToFill))
                            {
                                int x = currentSpace.x + 1;
                                int y = currentSpace.y;

                                if (roomTemplateGrid[x, y] == null)
                                {
                                    roomChainComponent.roomsCreated++;
                                    foundNextDirection = true;
                                    return new RoomTileSpace(x, y, hasWestExit, RoomChain.Direction.East);
                                }

                            }
                            break;
                        case RoomChain.Direction.South:
                            if (TestIfGridIndexIsValid(currentSpace.x, currentSpace.y - 1, horizontalRoomsToFill, verticalRoomsToFill))
                            {
                                int x = currentSpace.x;
                                int y = currentSpace.y - 1;

                                if (roomTemplateGrid[x, y] == null)
                                {
                                    roomChainComponent.roomsCreated++;
                                    foundNextDirection = true;
                                    return new RoomTileSpace(x, y, hasNorthExit, RoomChain.Direction.South);
                                }
                            }
                            break;
                        case RoomChain.Direction.West:
                            if (TestIfGridIndexIsValid(currentSpace.x - 1, currentSpace.y, horizontalRoomsToFill, verticalRoomsToFill))
                            {
                                int x = currentSpace.x - 1;
                                int y = currentSpace.y;

                                if (roomTemplateGrid[x, y] == null)
                                {
                                    roomChainComponent.roomsCreated++;
                                    foundNextDirection = true;
                                    return new RoomTileSpace(x, y, hasEastExit, RoomChain.Direction.West);
                                }

                            }
                            break;
                        case RoomChain.Direction.NoMove:
                            break;
                        default:
                            return null;
                    }
                }
            }
            //If nothing met the conditions return null and we'll know we need to try again
            return null;
        }

        //Loop over the board and place random rooms from the randomFillRoomList
        private void FillUnusedSpaceWithRandomRooms(int horizontalRoomsToFill, int verticalRoomsToFill, RoomTemplate[,] roomTemplateGrid)
        {
            for (int x = 0; x < horizontalRoomsToFill; x++)
            {
                for (int y = 0; y < verticalRoomsToFill; y++)
                {
                    if (roomTemplateGrid[x, y] == null)
                    {
                        roomTemplateGrid[x, y] = randomFillRoomList.RandomRoom();
                    }
                }
            }
        }

        //This method converts our roomTemplates into characters in our boardGrid in the BoardGenerator
        private void WriteChainRoomToGrid(BoardGenerator boardGenerator, RoomChain roomChainComponent, Vector2 roomOrigin, RoomTemplate roomTemplate, int chainNumber, bool isOnPath)
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

        //This is used to see if we have a valid space to place a RoomTemplate meaning it's inside the board
        bool TestIfGridIndexIsValid(int x, int y, int gridWidthX, int gridWidthY)
        {
            if (x > gridWidthX - 1 || x < 0 || y > gridWidthY - 1 || y < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Roll dice between 0-100, used for percentage based randomness
        public bool RollPercentage(int chanceToHit)
        {
            int randomResult = Random.Range(0, 100);
            if (randomResult < chanceToHit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
    
}
