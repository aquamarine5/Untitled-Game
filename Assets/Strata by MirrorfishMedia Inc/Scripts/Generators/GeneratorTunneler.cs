using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/GeneratorTunneler")]
    public class GeneratorTunneler : Generator
    {
        //How many tunnels to spawn
        public int numTunnels = 4;
        //Maximum possible length per tunnel
        public int maxLengthPerTunnel = 1000;
        //How wide is the tunnel
        [Range (0,100)]
        public int tunnelWidth = 1;

        //Should we spawn RoomTemplates at tunnel ends?
        public bool spawnRoomsAtTunnelEnds = false;

        //Room templates to spawn at tunnel ends if spawnRoomsAtTunnelEnds is true
        public RoomTemplate[] tunnelEndTemplates;

        //Should we choose random start positions for tunnels?
        public bool useRandomTunnelStartPositions = false;

        //Or should we pick random empty spaces from each generator and connect them with tunnels 
        public bool connectLastStrataLayer = true;

        //Should we use a character besides the default empty char (to draw rivers or lava flows for example)
        public bool useCustomEmptySpaceCharForTunnels = false;

        //If so, use this character for the empty space inside the tunnels
        public char customEmptySpaceChar = '0';

        //Generation stuff goes here
        public override void Generate(BoardGenerator boardGenerator)
        {
            //Start position for our tunnels, currently empty
            GridPosition startPos = null;

            //If we are generating random tunnels
            if (useRandomTunnelStartPositions)
            {
                //Get a random position for the start position
                startPos = boardGenerator.GetRandomGridPosition();

                //Repeat until we have the desired number of tunnels
                for (int i = 0; i < numTunnels; i++)
                {
                    //Get a new random position
                    GridPosition randomGoalPosition = boardGenerator.GetRandomGridPosition();
                    //If we are spawning rooms, spawn one
                    if (spawnRoomsAtTunnelEnds)
                    {
                        SpawnRoomTemplateAtTunnelEnd(boardGenerator, randomGoalPosition);
                    }
                    //Now dig the tunnel from the current start position to the current random goal position
                    DigTunnel(boardGenerator, startPos, randomGoalPosition);
                }
            }
            //If we are using this tunneler to connect the previous Generators in the BoardProfile...
            else if(connectLastStrataLayer)
            {
                //Create a list of the positions we want to get to based on previously recorded empty spaces in the BoardManager
                //This is where the generatesEmptySpace boolean is used, to keep track of connectible empty spaces for this process
                List<GridPosition> goalPositions = BuildTunnelGoalList(boardGenerator);
                

                for (int i = 0; i < goalPositions.Count; i++)
                {
                    //Set startpos to the current position in the loop
                    startPos = goalPositions[i];
                    //Loop over the array without going out of range since we don't know how long it will be (not all generators create empty space)
                    int loopingGoalPositionIndex = ((i + 1) % goalPositions.Count);
                    //Set targetPosition for our tunnel to the current position from our Generators
                    GridPosition targetPosition = goalPositions[loopingGoalPositionIndex];
                    //Dig the tunnel
                    DigTunnel(boardGenerator, startPos, targetPosition);
                }
            }
        }

        //This method creates a list of empty spaces from each Generator that generates empty space in our BoardProfile
        private List<GridPosition> BuildTunnelGoalList(BoardGenerator boardGenerator)
        {
            //Create a new list
            List<GridPosition> goalPositions = new List<GridPosition>();

            //Loop over all of the emptySpaceLists stored in the generator. These are populated when each Generator
            //that has the boolean generatesEmptySpace set to true runs it's generation process.
            for (int i = 0; i <= boardGenerator.currentGeneratorIndexIdForEmptySpaceTracking; i++)
            {
                for (int j = 0; j < numTunnels; j++)
                {
                    if (boardGenerator.emptySpaceLists[i].gridPositionList.Count > 0)
                    {
                        //Pick a random index number from the empty space list from this Generator
                        int index = Random.Range(0, boardGenerator.emptySpaceLists[i].gridPositionList.Count);
                        //Store it in a GridPosition
                        GridPosition emptyPosition = boardGenerator.emptySpaceLists[i].gridPositionList[index];
                        //Remove it from the list so we can't pick it again
                        boardGenerator.emptySpaceLists[i].gridPositionList.RemoveAt(index);
                        //Add it to our list of goalPositions to tunnel to
                        goalPositions.Add(emptyPosition);
                    }
                }
            }

            return goalPositions;
        }

        //This method does the digging
        public void DigTunnel(BoardGenerator boardGenerator, GridPosition startPosition, GridPosition tunnelGoal)
        {
            //Start digging here
            GridPosition currentDigPosition = startPosition;

            //Loop until we've exceeded the max length per tunnel or reached the goal
            for (int i = 0; i < maxLengthPerTunnel; i++)
            {
                //Check if we are to right of our goal
                if (currentDigPosition.x < tunnelGoal.x)
                {
                    //Increment the position to move right
                    currentDigPosition.x++;
                }
                //Otherwise if we're to the left
                else if (currentDigPosition.x > tunnelGoal.x)
                {
                    //Decrement to move left
                    currentDigPosition.x--;
                }
                else
                {
                    //If we're not right or left, it must be equal, so it's time to turn the tunnel
                    //these corners in the tunnel are a good spot to spawn rooms, so let's do that if we're going to
                    if (spawnRoomsAtTunnelEnds)
                    {
                        SpawnRoomTemplateAtTunnelEnd(boardGenerator, currentDigPosition);
                    }
                    
                    break;
                }

                //If our tunnel is wider than 1
                for (int j = 0; j < tunnelWidth; j++)
                {
                    //We'll dig out some extra nearby spaces to widen out the tunnel
                    boardGenerator.WriteToBoardGrid(currentDigPosition.x, currentDigPosition.y + j, GetCharToWriteForTunnel(boardGenerator), true, true);

                }

            }
            //Now it's time to move vertically
            for (int k = 0; k < maxLengthPerTunnel; k++)
            {
                //If we're below the tunnel goal
                if (currentDigPosition.y < tunnelGoal.y)
                {
                    //Move up toward it
                    currentDigPosition.y++;
                }
                // If we're above it
                else if (currentDigPosition.y > tunnelGoal.y)
                {
                    //Move down
                    currentDigPosition.y--;
                }
                else
                {
                    //Otherwise, we must be on it and therefore at our goal, so spawn a RoomTemplate if we're going to
                    if (spawnRoomsAtTunnelEnds)
                    {
                        SpawnRoomTemplateAtTunnelEnd(boardGenerator, currentDigPosition);
                    }
                    break;
                }
                for (int s = 0; s < tunnelWidth; s++)
                {
                    //Again, widen it out if we need to
                    boardGenerator.WriteToBoardGrid(currentDigPosition.x + s, currentDigPosition.y, GetCharToWriteForTunnel(boardGenerator), true, true);
                }
            }

        }

        //Simple function to get the character we're actually going to write, if it's custom or default
        char GetCharToWriteForTunnel(BoardGenerator boardGenerator)
        {
            char charToWrite;
            if (useCustomEmptySpaceCharForTunnels)
            {
                charToWrite = customEmptySpaceChar;
            }
            else
            {
                charToWrite = boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar();
            }
            return charToWrite;
        }
        
        //Simple function that spawns a RoomTemplate if needed
        void SpawnRoomTemplateAtTunnelEnd(BoardGenerator boardGenerator, GridPosition spawnPosition)
        {
            if (tunnelEndTemplates.Length > 0)
            {
                RoomTemplate templateToSpawn = tunnelEndTemplates[Random.Range(0, tunnelEndTemplates.Length)];
                boardGenerator.DrawTemplate(spawnPosition.x, spawnPosition.y, templateToSpawn, overwriteFilledSpaces, true);
            }
        }
    }
}

