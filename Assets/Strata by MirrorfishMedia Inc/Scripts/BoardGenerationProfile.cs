using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{

    //The board generation profile is an asset which holds all info needed to generate a level or world
    [CreateAssetMenu(menuName = "Strata/BoardGenerationProfile")]
    public class BoardGenerationProfile : ScriptableObject
    {
        [Tooltip("This seed allows repeatable generation of levels, or seed sharing between two players")]
        public string seedValue = "@mattmirrorfish";
        [Tooltip("The size of the board in tiles horizontally, ideally this should be 1 to 1 in Unity units in worldspace, otherwise see unityTileSizeInWorldspace to adjust")]
        public int boardHorizontalSize = 100;
        [Tooltip("The size of the board vertically")]
        public int boardVerticalSize = 100;

        [Tooltip("Normally this should be 1, meaning 1 Tile equals 1 Unity unit (worldspace integer step in Transform position for example), if tiles have a different relationship to world space, set it here.")]
        public float unityTileSizeInWorldspace = 1f;

        [Tooltip("The BoardLibrary contains the mapping of tiles to characters needed to generate levels along with lists of roomtemplates used in generation of room based levels")]
        public BoardLibrary boardLibrary;

        [Tooltip("This array of generators contains all the different generator tools which will be used to generate the level. The generators chosen and the order that they are placed in determines the character of the generated level. Drag and drop different generators into this array to generate different types of levels")]

        public List<Generator> generators;
    }

}
