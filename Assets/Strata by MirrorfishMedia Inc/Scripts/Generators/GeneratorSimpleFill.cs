using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/Simple Fill")]

    public class GeneratorSimpleFill : Generator
    {
        public char charToFill = 'm';
        public override void Generate(BoardGenerator boardGenerator)
        {
            for (int x = 0; x < boardGenerator.boardGenerationProfile.boardHorizontalSize; x++)
            {
                for (int y = 0; y < boardGenerator.boardGenerationProfile.boardVerticalSize; y++)
                {
                    boardGenerator.WriteToBoardGrid(x, y, charToFill, overwriteFilledSpaces, generatesEmptySpace);
                }
            }
        }
    }

}
