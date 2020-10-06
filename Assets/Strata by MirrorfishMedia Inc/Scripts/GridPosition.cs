using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{

    //Simple little helper class to store and work with X,Y positions as ints
    [System.Serializable]
    public class GridPosition
    {
        public int x;
        public int y;

        public GridPosition(int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }


        
    }
}
