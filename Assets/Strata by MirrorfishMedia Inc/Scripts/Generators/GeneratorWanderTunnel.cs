using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{

    [CreateAssetMenu(menuName = "Strata/Generators/Generator Wander Tunnel")]

    public class GeneratorWanderTunnel : Generator
    {
        public int tunnelMinLength = 20;
        public int tunnelMaxLength = 100;
        public bool useDefaultEmptyChar = true;
        public char alternateCharToUse = '0';
        public int turnPercentChance = 10;
        public int turnNoiseValue;
        public int tunnelWidth = 1;
        public int roomSpawnChance = 10;
        public bool spawnRandomRoomsOnTunnel;
        public bool spawnRoomOnTunnelEnd;
        public bool spawnRoomsOnTunnelTurn;
        public RoomTemplate[] roomTemplates;

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

            for (int i = 0; i < tunnelMaxLength; i++)
            {
                if (RollPercentage(turnPercentChance))
                {
                    targetPosition = boardGenerator.GetRandomGridPosition();
                }

                if (spawnRandomRoomsOnTunnel)
                {
                    if (RollPercentage(roomSpawnChance))
                    {
                        RoomTemplate randTemplate = roomTemplates[Random.Range(0, roomTemplates.Length)];
                        boardGenerator.DrawTemplate(currentPosition.x, currentPosition.y, randTemplate, overwriteFilledSpaces, generatesEmptySpace);
                    }
                }


                Dig(boardGenerator, currentPosition, targetPosition, charToWrite);
            }

            if (spawnRoomOnTunnelEnd)
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
