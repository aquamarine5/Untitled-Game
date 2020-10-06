using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Strata
{
    public class RoomChain : MonoBehaviour
    {
        //Variables used by RoomChain generators

        //The number of rooms created on the current RoomChain path
        public int roomsCreated;

        //The RoomTemplate of the current room being generated
        [HideInInspector]
        public RoomTemplate currentChainRoom;


        //Enumeration for directions to improve code readability
        public enum Direction { North, East, South, West, NoMove };

#if UNITY_EDITOR

        //This generates empty GameObjects which we can use to review and visualize placement of RoomChains and make sure that they are connected properly
        //This only runs in the Unity Editor.
        public GameObject GenerateRoomPlaceHolderGameObject(BoardGenerator boardGenerator, Vector2 roomOrigin, RoomTemplate roomTemplate, int chainNumber, bool isOnPath, string namePrefix)
        {
            GameObject roomMarker;
            if (isOnPath)
            {
                roomMarker = new GameObject(namePrefix + "Path Room " + chainNumber + " " + roomTemplate.name);
            }
            else
            {
                roomMarker = new GameObject(namePrefix + "Random fill Room " + roomTemplate.name);
            }

            roomMarker.transform.position = roomOrigin;
            roomMarker.transform.SetParent(this.transform);

            return roomMarker;
        }

#endif

        public void SetupDebugObject(RoomTemplate roomTemplate, Vector2 roomOrigin, Vector2 nodePosition, int loopIteration, Direction arrowDirection)
        {

#if UNITY_EDITOR
            //This generates GameObjects for each room in a room chain, it's useful in editor to be able to record the generation path and see if room generation is working correctly.
            //This does not run during the build of your game and can be safely removed if desired.

            GameObject roomMarker = new GameObject(roomTemplate.name + " " + loopIteration);
            
            roomMarker.transform.position = roomOrigin;
            PathVisualizer pathVisualizer = roomMarker.AddComponent<PathVisualizer>();
            pathVisualizer.nodePosition = nodePosition;
            switch (arrowDirection)
            {
                case Direction.North:
                    pathVisualizer.nodeDirection = new Vector2(0, 1);
                    pathVisualizer.pathDirection = Direction.North;
                    break;
                case Direction.East:
                    pathVisualizer.nodeDirection = new Vector2(1, 0);
                    pathVisualizer.pathDirection = Direction.East;
                    break;
                case Direction.South:
                    pathVisualizer.nodeDirection = new Vector2(0, -1);
                    pathVisualizer.pathDirection = Direction.South;
                    break;
                case Direction.West:
                    pathVisualizer.nodeDirection = new Vector2(-1, 0);
                    pathVisualizer.pathDirection = Direction.West;
                    break;
                case Direction.NoMove:
                    pathVisualizer.nodeDirection = new Vector2(0, 0);
                    pathVisualizer.pathDirection = Direction.NoMove;
                    break;
                default:
                    break;
            }
            string handleText = roomTemplate.name + " " + loopIteration;

            
            pathVisualizer.debugString = handleText;
            roomMarker.transform.SetParent(this.transform);

#endif
        }

    }

}
