using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    //This class is used to create a character that has a random chance of spawning, used together with ChanceBoardLibraryEntry
    [System.Serializable]
    public class ChanceChar
    {
        //An array of characters that we will pick a random result from
        public char outputCharIds;

        //What is the percent chance (out of 100) that we will select this entry?
        public int percentChanceToChoose = 33;
    }
}
