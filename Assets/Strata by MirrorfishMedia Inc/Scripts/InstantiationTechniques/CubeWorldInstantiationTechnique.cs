using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    //This is more of a proof of concept than a real feature, but is here to spark ideas. Very beta.
    //This naively instantiates thousands of cubes, on my machine it runs fine, draw calls batch etc but it's probably not performant
    //if you wanted to use this in production I would look at using marching cubes or some kind of mesh combining or other optimization

    [CreateAssetMenu(menuName = "Strata/CubeWorldInstantiator")]
    public class CubeWorldInstantiationTechnique : BoardInstantiationTechnique
    {
        //How tall along the Y axis should we make our previously 2D map out of 1 unity unit sized cubes
        public int mapYLayers = 3;

        //This overrides the abstract function in the base BoardInstantiationTechnique class
        public override void SpawnBoardSquare(BoardGenerator boardGenerator, Vector2 location, BoardLibraryEntry inputEntry)
        {

            if (inputEntry != null)
            {
                if (inputEntry.prefabToSpawn == null)
                {

                    //If prefab is null, do nothing, we need prefabs for this to work
                }
                else
                {
                    for (int i = 0; i < mapYLayers; i++)
                    {
                        //Create a 3D position from our 2D map based on the mapYLayers value for y height, converting x,y of the 2D map to x,z of the 3D map
                        Vector3 pos = new Vector3((int)location.x, i, (int)location.y);

                        //Spawn a cube at the position
                        SpawnPrefab(boardGenerator, pos, inputEntry.prefabToSpawn);
                    }
                }
            }
            else
            {
                Debug.LogError("Returned null from library, something went wrong when trying to draw tiles.");
            }
        }

        private void SpawnPrefab(BoardGenerator boardGenerator, Vector3 spawnPosition, GameObject prefab)
        {
            //Instantiate the specified prefab at the spawn position.
            GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity) as GameObject;
            //Add this to the list in boardGenerator so we can clear it when we regenerate
            boardGenerator.generatedObjectsToClear.Add(spawnedObject);

        }
    }

}
