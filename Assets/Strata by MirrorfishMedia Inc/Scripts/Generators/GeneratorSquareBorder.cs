using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{

    [CreateAssetMenu(menuName = "Strata/Generators/GenerateWallBorder")]
    public class GeneratorSquareBorder : Generator
    {
        public char borderChar = 'w';

        public override void Generate(BoardGenerator boardGenerator)
        {
            BuildBorder(boardGenerator);
        }

        public void BuildBorder(BoardGenerator boardGenerator)
        {
            // The outer walls are one unit left, right, up and down from the board.
            float leftEdgeX = -1f;
            float rightEdgeX = boardGenerator.boardGenerationProfile.boardHorizontalSize + 0f;
            float bottomEdgeY = -1f;
            float topEdgeY = boardGenerator.boardGenerationProfile.boardVerticalSize + 0f;

            // Instantiate both vertical walls (one on each side).
            InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY, boardGenerator);
            InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY, boardGenerator);

            // Instantiate both horizontal walls, these are one in left and right from the outer walls.
            InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY, boardGenerator);
            InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY, boardGenerator);
        }

        void InstantiateVerticalOuterWall(float xCoord, float startingY, float endingY, BoardGenerator boardGenerator)
        {
            // Start the loop at the starting value for Y.
            float currentY = startingY;

            // While the value for Y is less than the end value...
            while (currentY <= endingY)
            {
                // ... instantiate an outer wall tile at the x coordinate and the current y coordinate.
                //InstantiateFromArray(wall, xCoord, currentY);
                Vector2 spawnPos = new Vector2(xCoord, currentY);
                boardGenerator.CreateMapEntryFromGrid(borderChar, spawnPos);
                currentY++;
            }
        }


        void InstantiateHorizontalOuterWall(float startingX, float endingX, float yCoord, BoardGenerator boardGenerator)
        {
            // Start the loop at the starting value for X.
            float currentX = startingX;

            // While the value for X is less than the end value...
            while (currentX <= endingX)
            {
                // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
                //InstantiateFromArray (wall, currentX, yCoord);
                Vector2 spawnPos = new Vector2(currentX, yCoord);
                boardGenerator.CreateMapEntryFromGrid(borderChar, spawnPos);
                currentX++;
            }
        }
    }

}
