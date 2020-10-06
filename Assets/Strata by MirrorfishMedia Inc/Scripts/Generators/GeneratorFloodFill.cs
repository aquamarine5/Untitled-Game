using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/Flood Fill")]

    public class GeneratorFloodFill : Generator
    {
        public char charToBeReplaced = '0';
        public char charToReplaceWith = 't';
        public bool edgeFill;

        public Vector2 pointToFill = new Vector2(25,25);

        public override void Generate(BoardGenerator boardGenerator)
        {
            GridPosition fillPosition = new GridPosition((int)pointToFill.x, (int)pointToFill.y);
            BoardFloodFill(boardGenerator, fillPosition, charToBeReplaced, charToReplaceWith);
        }

        private void BoardFloodFill(BoardGenerator boardGenerator, GridPosition gridPosition, char targetChar, char replacementChar)
        {
            targetChar = boardGenerator.boardGridAsCharacters[gridPosition.x, gridPosition.y];

            if (targetChar == replacementChar)
            {
                return;
            }

            Stack<GridPosition> gridPositions = new Stack<GridPosition>();

            gridPositions.Push(gridPosition);

            while (gridPositions.Count != 0)
            {
                GridPosition temp = gridPositions.Pop();
                int y1 = temp.y;
                while (y1 >= 0 && boardGenerator.boardGridAsCharacters[temp.x, y1] == targetChar)
                {
                    y1--;
                }
                y1++;
                bool spanLeft = false;
                bool spanRight = false;

                while (y1 < boardGenerator.boardGenerationProfile.boardVerticalSize && boardGenerator.boardGridAsCharacters[temp.x, y1] == targetChar)
                {

                    boardGenerator.boardGridAsCharacters[temp.x, y1] = replacementChar;

                    if (!spanLeft && temp.x > 0 && boardGenerator.boardGridAsCharacters[temp.x -1 , y1] == targetChar)
                    {
                        gridPositions.Push(new GridPosition(temp.x - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.x - 1 >= 0 && boardGenerator.boardGridAsCharacters[temp.x - 1, y1] != targetChar)
                    {
                        spanLeft = false;
                    }

                    if (!spanRight && temp.x < boardGenerator.boardGenerationProfile.boardHorizontalSize- 1 && boardGenerator.boardGridAsCharacters[temp.x + 1, y1] == targetChar)
                    {
                        gridPositions.Push(new GridPosition(temp.x + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.x < boardGenerator.boardGenerationProfile.boardHorizontalSize - 1 && boardGenerator.boardGridAsCharacters[temp.x + 1, y1] != targetChar)
                    {
                        spanRight = false;
                    }
                    y1++;
                }
            }
        }
    }
}

