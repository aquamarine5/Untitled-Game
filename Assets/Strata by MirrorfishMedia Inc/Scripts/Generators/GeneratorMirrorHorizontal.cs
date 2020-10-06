using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/Mirror Horizontal")]

    public class GeneratorMirrorHorizontal : Generator
    {

        public override void Generate(BoardGenerator boardGenerator)
        {
            MirrorGridHorizontal(boardGenerator);
        }

        private void MirrorGridHorizontal(BoardGenerator boardGenerator)
        {
            for (int i = 0; i < boardGenerator.boardGenerationProfile.boardHorizontalSize; i++)
            {
                for (int j = 0; j < boardGenerator.boardGenerationProfile.boardVerticalSize; j++)
                {
                    char charToRewrite = boardGenerator.boardGridAsCharacters[i, j];
                    boardGenerator.WriteToBoardGrid(boardGenerator.boardGenerationProfile.boardHorizontalSize - i, j, charToRewrite, true, false);
                }
            }
        }
    }
}


