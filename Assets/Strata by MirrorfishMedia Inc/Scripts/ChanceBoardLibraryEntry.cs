using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Strata
{

    //This is used to add a second layer of randomness in each Tile spawned

    [System.Serializable]
    public class ChanceBoardLibraryEntry
    {
        //Array of ChanceChars to choose between, one of these will be randomly selected
        public ChanceChar[] chanceChars;

        //This is the array that will be filled with the specified ChanceChars and random characters will be actually selected out of
        [HideInInspector]
        public char[] outputCharArray = new char[100];

        //Build the array of characters based on the probabilities specified
        public void BuildChanceCharListProbabilities()
        {
            int arrayPercentIndex = 0;

            //Go over the array of chanceChars set in the inspector and fill the list 100 entries that we will use to pick out of
            for (int i = 0; i < chanceChars.Length; i++)
            {
                for (int j = 0; j < chanceChars[i].percentChanceToChoose; j++)
                {
                    outputCharArray[arrayPercentIndex] = chanceChars[i].outputCharIds;
                    arrayPercentIndex++;
                }

            }
        }

        //Use this to get a random chance char back from the array
        public char GetChanceCharId()
        {
            char outputChar = '0';
            if (outputCharArray.Length > 0)
            {
                outputChar = outputCharArray[Random.Range(0, outputCharArray.Length)];
            }
            return outputChar;
        }
    }

}
