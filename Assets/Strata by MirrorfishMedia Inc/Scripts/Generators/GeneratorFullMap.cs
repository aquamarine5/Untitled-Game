using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{

    //This just grabs the entire loaded Tilemap and writes it into a RoomTemplate, if for example you wanted to author a large background and then
    //generate over it. Nothing procedural here, just save/load whatever is in the current Tilemap.

    [CreateAssetMenu(menuName = "Strata/Generators/GenerateSavedFullMap")]
    public class GeneratorFullMap : Generator
    {

        public RoomTemplate templateToSpawn;

        public override void Generate(BoardGenerator boardGenerator)
        {
            boardGenerator.DrawTemplate(0, 0, templateToSpawn, true, false);


        }

    }
}
