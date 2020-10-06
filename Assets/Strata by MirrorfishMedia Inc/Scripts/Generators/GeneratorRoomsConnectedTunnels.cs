using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    [CreateAssetMenu(menuName = "Strata/Generators/Generator RoomConnectedTunnels")]
    public class GeneratorRoomsConnectedTunnels : Generator
    {

        public int numRoomsToSpawn;
        public RoomTemplate[] roomTemplates;
        public GeneratorTunneler tunnelTemplate;

        public override void Generate(BoardGenerator boardGenerator)
        {
            SpawnRooms(boardGenerator);
        }

        void SpawnRooms(BoardGenerator boardGenerator)
        {

            List<GridPosition> roomGridPositions = new List<GridPosition>();

            GridPosition firstRoomPosition = boardGenerator.GetRandomGridPosition();
            boardGenerator.DrawTemplate(firstRoomPosition.x, firstRoomPosition.y, roomTemplates[Random.Range(0, roomTemplates.Length)], overwriteFilledSpaces, true);

            for (int i = 0; i < numRoomsToSpawn - 1; i++)
            {
                GridPosition randRoomPosition = boardGenerator.GetRandomGridPosition();
                roomGridPositions.Add(randRoomPosition);
                boardGenerator.DrawTemplate(randRoomPosition.x, randRoomPosition.y, roomTemplates[Random.Range(0, roomTemplates.Length)], overwriteFilledSpaces, true);
            }
            ConnectRooms(boardGenerator, firstRoomPosition, roomGridPositions);

        }

        void ConnectRooms(BoardGenerator boardGenerator, GridPosition firstRoomPosition, List<GridPosition> roomPositions)
        {
            for (int i = 0; i < roomPositions.Count; i++)
            {
                tunnelTemplate.DigTunnel(boardGenerator, firstRoomPosition, roomPositions[i]);
            }

            tunnelTemplate.DigTunnel(boardGenerator, roomPositions[roomPositions.Count - 1], firstRoomPosition);

        }
    }
}
