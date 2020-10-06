using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    //ScriptableObject asset to store a collection of rooms, mainly used by RoomChain generators to organize rooms based on their open exits
    [CreateAssetMenu(menuName = "Strata/Collections/RoomList")]
    public class RoomList : ScriptableObject
    {
        public List<RoomTemplate> roomList = new List<RoomTemplate>();

        //Use this to get rid of any empty entries in the list while we're adding RoomTemplates to it, just a helper to clean up the lists
        public void RemoveEmptyEntriesThenAdd(RoomTemplate templateToAdd)
        {
            if (roomList.Count != 0)
            {
                for (int i = roomList.Count - 1; i >= 0; i--)
                {
                    if (roomList[i] == null)
                    {
                        roomList.RemoveAt(i);
                    }
                }
            }
            
            roomList.Add(templateToAdd);
        }

        public RoomTemplate RandomRoom()
        {
            return roomList[Random.Range(0, roomList.Count)];
        }

    }

    
}

