using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Templates/RoomTemplate")]
    public class RoomTemplate : ScriptableObject
    {
        public int roomSizeX = 10;
        public int roomSizeY = 10;

        public char[] roomChars = new char[100];

        public bool opensToNorth;
        public bool opensToEast;
        public bool opensToSouth;
        public bool opensToWest;

        void OnValidate()
        {
            if (roomChars.Length != roomSizeX * roomSizeY)
            {
                char[] newSizedArray = new char[roomSizeX * roomSizeY];
                roomChars = newSizedArray;
            }
        }
    }
}
