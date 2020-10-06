using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Strata
{
    //This is one of the simplest generators, it just generates Perlin noise and fills the map with it. May be useful as a base starting point for other stuff.
    [CreateAssetMenu(menuName = "Strata/Generators/Perlin Noise Fill")]
    public class GeneratorPerlinNoise : Generator
    {

        [Tooltip("Use this to scale the noise pattern. Low values give big blobs, high values lots of little bits")]
        public float noiseScaler = .5f;
        [Tooltip("Should we use the default empty character in BoardLibary (0 by default) for empty space?")]
        public bool useDefaultEmptySpaceChar = true;
        [Tooltip("Specify an alternate character for the negative space in the noise pattern.")]
        public char alternateEmptyChar = '0';
        [Tooltip("Specify what character to use for the fill in the noise pattern.")]
        public char filledSpaceChar = 'm';

        public override void Generate(BoardGenerator boardGenerator)
        {
            int point;
            char charToWrite = '0';
            //Loop over the board and write noise
            for (int x = 0; x < boardGenerator.boardGenerationProfile.boardHorizontalSize; x++)
            {
                for (int y = 0; y < boardGenerator.boardGenerationProfile.boardVerticalSize; y++)
                {

                    //Pick two random offsets from which to sample our noise, these will allow the noise to be effected by random seeding
                    int randomOffsetX = Random.Range(0, 1000);
                    int randomOffsetY = Random.Range(0, 1000);

                    //Pick a rounded point using perlin noise, scale it by our scaling value
                    point = Mathf.RoundToInt(Mathf.PerlinNoise(x + randomOffsetX * noiseScaler, y + randomOffsetY * noiseScaler));
                    if (point == 0)
                    {
                        if (useDefaultEmptySpaceChar)
                        {
                            charToWrite = boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultEmptyChar();

                        }
                        else
                        {
                            charToWrite = alternateEmptyChar;
                        }
                    }
                    else if (point == 1)
                    {
                        charToWrite = filledSpaceChar;
                    }

                    boardGenerator.WriteToBoardGrid(x, y, charToWrite, overwriteFilledSpaces, generatesEmptySpace);
                    
                }
            }
        }
    }

}

