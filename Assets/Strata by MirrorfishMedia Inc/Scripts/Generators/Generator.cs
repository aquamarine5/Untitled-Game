using UnityEngine;

namespace Strata
{

    //Abstract base class from which all generators are derived, inherit from this to create new generators with different functionality
    [System.Serializable]
    public abstract class Generator : ScriptableObject
    {
        
        //Should we overwrite filled characters (anything but empty space, as defined by default empty char in BoardLibrary?)
        public bool overwriteFilledSpaces;
        //Does this add spaces to our empty space lists for potential connection later?
        public bool generatesEmptySpace;

        //This is the function that all Generators must override, and will be called by BoardGenerator at Generation time.
        public abstract void Generate(BoardGenerator boardGenerator);
    }
}
