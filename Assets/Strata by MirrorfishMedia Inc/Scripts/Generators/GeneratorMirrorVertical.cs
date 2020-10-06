using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/Mirror Vertical")]

    public class GeneratorMirrorVertical : Generator
    {

        public override void Generate(BoardGenerator boardGenerator)
        {
            MirrorGridVertical(boardGenerator);
        }

        private void MirrorGridVertical(BoardGenerator boardGenerator)
        {

            for (int i = 0; i < boardGenerator.boardGenerationProfile.boardHorizontalSize; i++)
            {
                for (int j = 0; j < boardGenerator.boardGenerationProfile.boardVerticalSize; j++)
                {
                    char charToRewrite = boardGenerator.boardGridAsCharacters[i, j];
                    boardGenerator.WriteToBoardGrid(i, boardGenerator.boardGenerationProfile.boardVerticalSize - j, charToRewrite, true, false);
                }
            }
        }
    }
}

