using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Strata
{
    //This is the main InstantiationTechnique for Strata, it matches an array of ASCII characters to TileBase objects and draws them on a Tilemap
    [CreateAssetMenu(menuName = "Strata/TileMapInstantiator")]
    public class TilemapInstantiationTechnique : BoardInstantiationTechnique
    {
        public override void SpawnBoardSquare(BoardGenerator boardGenerator, Vector2 location, BoardLibraryEntry inputEntry)
        {
            if (inputEntry != null)
            {
                if (inputEntry.prefabToSpawn == null)
                {
                    Vector3Int pos = new Vector3Int((int)location.x - TilemapSpawn.x, (int)location.y - TilemapSpawn.y, 0);
                    //Write the Tile in the BoardLibraryEntry to the Tilemap
                    boardGenerator.tilemap.SetTile(pos, inputEntry.tile);
                }
                else
                {
                    //If there is a prefab to spawn, spawn that along with the default empty tile under it
                    Vector3Int pos = new Vector3Int((int)location.x - TilemapSpawn.x, (int)location.y - TilemapSpawn.y, 0);
                    TileBase defaultTile = boardGenerator.boardGenerationProfile.boardLibrary.GetDefaultTile();
                    boardGenerator.tilemap.SetTile(pos, defaultTile);
                    GameObject spawnedPrefab = Instantiate(inputEntry.prefabToSpawn, location, Quaternion.identity);
                    //Add the spawned object to the list of GameObjects to destroy when we clear and rebuild the level
                    boardGenerator.generatedObjectsToClear.Add(spawnedPrefab);
                }

            }
            else
            {
                Debug.LogError("Returned null from library, something went wrong when trying to draw tiles.");
            }
        }
    }

}
