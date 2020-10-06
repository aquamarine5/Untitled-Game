using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{

    [CreateAssetMenu(menuName = "Strata/Generators/Generator Tunnel Brancher")]

    public class GeneratorTunnelBrancher : Generator
    {
        public int tunnelLength = 100;
        public bool useDefaultEmptyChar = true;
        public char alternateCharToUse = '0';
        public int turnPercentChance = 10;
        public int turnNoiseValue = 50;
        public int tunnelWidth = 1;
        public int roomSpawnChance = 10;
        public bool spawnRandomRoomsOnTunnel;
        public RoomTemplate[] roomTemplates;
        public int branchPercentChance = 20;
        public GeneratorWanderTunnel branchTunnel;

        public Vector2[] startLocations;

        public override void Generate(BoardGenerator boardGenerator)
        {
            char charToWrite;

            if (!useDefaultEmptyChar)
            {
                charToWrite = alternateCharToUse;
            }
            else
            {
                charToWrite = boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar();
            }

            Vector2 startLocation = startLocations[Random.Range(0, startLocations.Length)];
            GridPosition startPosition = boardGenerator.Vector2ToGridPosition(startLocation);
            GridPosition targetPosition = boardGenerator.GetRandomGridPosition();
            GridPosition currentPosition = startPosition;

            for (int i = 0; i < tunnelLength; i++)
            {
                if (RollPercentage(turnPercentChance))
                {
                    GridPosition turnPosition = boardGenerator.GetRandomGridPosition();
                    while (turnPosition.x == targetPosition.x || turnPosition.y == targetPosition.y)
                    {
                        turnPosition = boardGenerator.GetRandomGridPosition();
                    }

                    targetPosition = turnPosition;
                }

                if (spawnRandomRoomsOnTunnel)
                {
                    if (RollPercentage(roomSpawnChance))
                    {
                        RoomTemplate randTemplate = roomTemplates[Random.Range(0, roomTemplates.Length)];
                        boardGenerator.DrawTemplate(currentPosition.x, currentPosition.y, randTemplate, overwriteFilledSpaces, generatesEmptySpace);
                    }
                }

                List<GeneratorWanderTunnel> branchTunnelerList = new List<GeneratorWanderTunnel>();

                if (RollPercentage(branchPercentChance))
                {
                    GeneratorWanderTunnel wanderTunneler = ScriptableObject.CreateInstance<GeneratorWanderTunnel>();

                    wanderTunneler.tunnelMaxLength = branchTunnel.tunnelMaxLength;
                    wanderTunneler.overwriteFilledSpaces = branchTunnel.overwriteFilledSpaces;
                    wanderTunneler.useDefaultEmptyChar = branchTunnel.useDefaultEmptyChar;
                    wanderTunneler.alternateCharToUse = branchTunnel.alternateCharToUse;
                    wanderTunneler.startLocations = new Vector2[1];
                    wanderTunneler.startLocations[0] = new Vector2(currentPosition.x, currentPosition.y);
                    wanderTunneler.roomTemplates = branchTunnel.roomTemplates;
                    wanderTunneler.spawnRoomsOnTunnelTurn = branchTunnel.spawnRoomsOnTunnelTurn;
                    wanderTunneler.spawnRoomOnTunnelEnd = branchTunnel.spawnRoomOnTunnelEnd;
                    wanderTunneler.turnNoiseValue = branchTunnel.turnNoiseValue;
                    branchTunnelerList.Add(wanderTunneler);
                }

                for (int j = 0; j < branchTunnelerList.Count; j++)
                {
                    branchTunnelerList[j].Generate(boardGenerator);
                }


                Dig(boardGenerator, currentPosition, targetPosition, charToWrite);
            }

            if (spawnRandomRoomsOnTunnel)
            {
                RoomTemplate randTemplate = roomTemplates[Random.Range(0, roomTemplates.Length)];
                boardGenerator.DrawTemplate(currentPosition.x, currentPosition.y, randTemplate, overwriteFilledSpaces, generatesEmptySpace);
            }

        }

        private void Dig(BoardGenerator boardGenerator, GridPosition currentPosition, GridPosition targetPosition, char charToWrite)
        {
            if (RollPercentage(turnNoiseValue))
            {
                //Dig favoring horizontal
                if (currentPosition.x > targetPosition.x)
                {
                    currentPosition.x--;
                }
                else if (currentPosition.x < targetPosition.x)
                {
                    currentPosition.x++;
                }

                else if (currentPosition.x == targetPosition.x)
                {
                    if (currentPosition.y > targetPosition.y)
                    {
                        currentPosition.y--;
                    }
                    else if (currentPosition.y < targetPosition.y)
                    {
                        currentPosition.y++;
                    }
                }
            }
            else
            {
                //Dig favoring vertical
                if (currentPosition.y > targetPosition.y)
                {
                    currentPosition.y--;
                }
                else if (currentPosition.y < targetPosition.y)
                {
                    currentPosition.y++;
                }

                else if (currentPosition.y == targetPosition.y)
                {
                    if (currentPosition.x > targetPosition.x)
                    {
                        currentPosition.x--;
                    }
                    else if (currentPosition.x < targetPosition.x)
                    {
                        currentPosition.x++;
                    }
                }
            }


            for (int j = 0; j < tunnelWidth; j++)
            {
                boardGenerator.WriteToBoardGrid(currentPosition.x + j, currentPosition.y + j, charToWrite, overwriteFilledSpaces, generatesEmptySpace);
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
