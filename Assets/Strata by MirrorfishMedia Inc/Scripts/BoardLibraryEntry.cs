using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Strata
{
    [System.Serializable]
    public class BoardLibraryEntry
    {
        //The ASCII character ID that we will use to look this Entry up
        public char characterId;
        //Should we use this as the default empty space? Only one entry should have this as true, if we have multiple the first one will be used.
        public bool useAsDefaultEmptySpace;
        //Should we spawn a prefab from this entry? Can be left empty if no.
        public GameObject prefabToSpawn;
        //What TileBase should we spawn here? Can be a regular Sprite Tile or a Scriptable Tile like a Rule Tile or Random Tile.
        public TileBase tile;
        //Defaults to empty, fill this in if you want to randomly spawn something other than the TileBase set in the tile variable.
        public ChanceBoardLibraryEntry chanceBoardLibraryEntry;
    }
}
