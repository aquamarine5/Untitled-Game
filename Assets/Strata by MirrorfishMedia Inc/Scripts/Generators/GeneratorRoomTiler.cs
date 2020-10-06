using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/Generator RoomTiler")]


    public class GeneratorRoomTiler : Generator
    {

        [Tooltip("How many times does our structure try to spread out via open exits?")]
        public int growthIterations = 5;
        [Tooltip("Chance to continue growing means that when the structure has an open exit it will roll percentage chance against this value to see if it should spread in this direction. Setting this to 100 produces more regular shapes. Very low chance to grow require more attempts and can result in slow or failed generation")]
        [Range(15, 100)]
        public int chanceToContinueGrowing = 50;
        [Tooltip("If we create less than this many rooms we'll retry until we've exceeded the maximum attempts, default is 999")]
        public int minimumGeneratedRooms = 10;
        [Tooltip("Dimensions on x and y of the RoomTemplate, this has been tested with regular, equally sized rooms, YMMV with irregular rooms.")]
        public int roomSizeX = 10;
        public int roomSizeY = 10;

        [Tooltip("We'll pick our first room from this list")]
        public RoomList firstRoomList;

        [Tooltip("Once we start generating we'll pick rooms from these lists")]
        public RoomList hasSouthExit;
        public RoomList hasNorthExit;
        public RoomList hasEastExit;
        public RoomList hasWestExit;

        private int maximumAttempts = 999;

        [Tooltip("If true we use a position from list, otherwise a random grid position (ex 10,20)")]
        public bool pickStartPositionFromList = false;
        public GridPosition[] startPositions;


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
            //Used to sanity check generation and fail if we get stuck
            int generationAttempts = 0;

            //We'll loop until this is true
            bool generationSucceeded = false;

            //Repeatedly attempt to generate desired output until criteria are met or maximumAttempts exceeded
            while (!generationSucceeded)
            {
                generationAttempts++;
                RoomChain roomChainComponent = SetupRoomChainComponent(boardGenerator);
                RoomTemplate[,] roomTemplateGrid = new RoomTemplate[horizontalRoomsToFill, verticalRoomsToFill];

                roomTemplateGrid = TileRooms(boardGenerator, roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill);

                if (roomTemplateGrid != null)
                {
                    WriteFilledRooms(boardGenerator, roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill, roomTemplateGrid);
                    generationSucceeded = true;
#if UNITYEDITOR
                    DestroyImmediate(roomChainComponent.gameObject);
#endif
                    if (roomChainComponent.gameObject != null)
                    {
                        Destroy(roomChainComponent.gameObject);
                    }
                    break;
                }
                else
                {
                    roomChainComponent = SetupRoomChainComponent(boardGenerator);
                    roomTemplateGrid = TileRooms(boardGenerator, roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill);
                }

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

        RoomTemplate[,] TileRooms(BoardGenerator boardGenerator, RoomChain roomChainComponent, int horizontalRoomsToFill, int verticalRoomsToFill)
        {

            //Create a two dimensional array of RoomTemplates in a grid based on the number of rooms
            RoomTemplate[,] filledGrid = new RoomTemplate[horizontalRoomsToFill, verticalRoomsToFill];
            RoomList[,] roomListGrid = new RoomList[horizontalRoomsToFill, verticalRoomsToFill];
            RoomTileSpace[,] roomTileSpaces = new RoomTileSpace[horizontalRoomsToFill, verticalRoomsToFill];
            List<RoomTileSpace> roomTileSpacesToFill = new List<RoomTileSpace>();


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
            
            roomListGrid[xStart, yStart] = firstRoomList;

            RoomTileSpace startingSpace = new RoomTileSpace(xStart, yStart, firstRoomList, RoomChain.Direction.NoMove);
            roomTileSpaces[xStart, yStart] = startingSpace;

            FillSpaceFromList(boardGenerator, roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill, roomTileSpaces, roomTileSpacesToFill, filledGrid, roomTileSpaces[xStart, yStart]);

            for (int i = 0; i < growthIterations; i++)
            {
                for (int j = roomTileSpacesToFill.Count - 1; j > -1; j--)
                {
                    FillSpaceFromList(boardGenerator, roomChainComponent, horizontalRoomsToFill, verticalRoomsToFill, roomTileSpaces, roomTileSpacesToFill, filledGrid, roomTileSpacesToFill[j]);
                    roomTileSpacesToFill.RemoveAt(j);
                }
            }
            if (roomChainComponent.roomsCreated >= minimumGeneratedRooms)
            {
                return filledGrid;
            }
            else
            {
                return null;
            }
        }

        private void FillSpaceFromList(BoardGenerator boardGenerator, RoomChain roomChainComponent, int horizontalRoomsToFill, int verticalRoomsToFill, RoomTileSpace[,] roomTileSpaces, List<RoomTileSpace> roomTileSpacesToFill, RoomTemplate[,] roomTemplateGrid, RoomTileSpace currentSpace)
        {
            currentSpace.SelectRoomTemplateForSpace();
            roomTemplateGrid[currentSpace.x, currentSpace.y] = currentSpace.roomTemplate;


            if (RollPercentage(chanceToContinueGrowing))
            {
                if (TestIfGridIndexIsValid(currentSpace.x + 1, currentSpace.y, horizontalRoomsToFill, verticalRoomsToFill))
                {
                    int x = currentSpace.x + 1;
                    int y = currentSpace.y;

                    if (currentSpace.roomTemplate.opensToEast)
                    {
                        if (roomTileSpaces[x, y] == null)
                        {
                            RoomTileSpace roomTileSpace = new RoomTileSpace(x, y, hasWestExit, RoomChain.Direction.East);
                            roomTileSpaces[x, y] = roomTileSpace;
                            roomTileSpacesToFill.Add(roomTileSpace);
                            roomChainComponent.roomsCreated++;
                        }
                    }

                }
            }

            if (RollPercentage(chanceToContinueGrowing))
            {
                if (TestIfGridIndexIsValid(currentSpace.x - 1, currentSpace.y, horizontalRoomsToFill, verticalRoomsToFill))
                {
                    int x = currentSpace.x - 1;
                    int y = currentSpace.y;

                    if (currentSpace.roomTemplate.opensToWest)
                    {
                        if (roomTileSpaces[x, y] == null)
                        {
                            RoomTileSpace roomTileSpace = new RoomTileSpace(x, y, hasEastExit, RoomChain.Direction.West);
                            roomTileSpaces[x, y] = roomTileSpace;
                            roomTileSpacesToFill.Add(roomTileSpace);
                            roomChainComponent.roomsCreated++;
                        }
                    }

                }
            }

            if (RollPercentage(chanceToContinueGrowing))
            {
                if (TestIfGridIndexIsValid(currentSpace.x, currentSpace.y + 1, horizontalRoomsToFill, verticalRoomsToFill))
                {
                    int x = currentSpace.x;
                    int y = currentSpace.y + 1;

                    if (currentSpace.roomTemplate.opensToNorth)
                    {
                        if (roomTileSpaces[x, y] == null)
                        {
                            RoomTileSpace roomTileSpace = new RoomTileSpace(x, y, hasSouthExit, RoomChain.Direction.North);
                            roomTileSpaces[x, y] = roomTileSpace;
                            roomTileSpacesToFill.Add(roomTileSpace);
                            roomChainComponent.roomsCreated++;
                        }
                    }
                }
            }

            if (RollPercentage(chanceToContinueGrowing))
            {
                if (TestIfGridIndexIsValid(currentSpace.x, currentSpace.y - 1, horizontalRoomsToFill, verticalRoomsToFill))
                {
                    int x = currentSpace.x;
                    int y = currentSpace.y - 1;

                    if (currentSpace.roomTemplate.opensToSouth)

                    {

                        if (roomTileSpaces[x, y] == null)
                        {
                            RoomTileSpace roomTileSpace = new RoomTileSpace(x, y, hasNorthExit, RoomChain.Direction.South);
                            roomTileSpaces[x, y] = roomTileSpace;
                            roomTileSpacesToFill.Add(roomTileSpace);
                            roomChainComponent.roomsCreated++;
                        }
                    }

                }
            }
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

        //This is used to see if we have a valid space to place a RoomTemplate
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
