using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace Strata
{

    //This is the main EditorWindow for Strata. It is what we use to take Tilemap data from the Scene view and convert it into ASCII data and store
    //it into ScriptableObject assets for use in level generation. It handles a few other setup functions as well.
    public class StrataContentEditorWindow : EditorWindow
    {
        //The currently loaded RoomTemplate which we are loading and saving from/to.
        public RoomTemplate roomTemplate;
        //The currently loaded BoardLibrary which we are reading from to match Tiles to Characters.
        public BoardLibrary boardLibrary;

        //Store a reference to the Dictionary of BoardLibrary which we use to match TileBase objects to BoardLibraryEntry objects (which contain ASCII characterIds)
        private Dictionary<TileBase, BoardLibraryEntry> libraryDictionary;

        //Set up the Window
        [MenuItem("Tools/Strata Content Editor")]
        static void Init()
        {
            EditorWindow.GetWindow(typeof(StrataContentEditorWindow)).Show();
        }

        //Redraw the EditorWindow
        void OnGUI()
        {
            //Set up the Serialized Objects we need to draw in the Window and apply their properties when modified
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedRoomTemplateProperty = serializedObject.FindProperty("roomTemplate");
            EditorGUILayout.PropertyField(serializedRoomTemplateProperty, true);

            SerializedProperty serializedBoardLibraryProperty = serializedObject.FindProperty("boardLibrary");
            EditorGUILayout.PropertyField(serializedBoardLibraryProperty, true);

            serializedObject.ApplyModifiedProperties();


            //Draw the Buttons and call appropriate functions when they are pressed
            if (GUILayout.Button("Load Room"))
            {
                LoadTileMapFromRoomTemplate();
            }

            if (GUILayout.Button("Add To Enter From North List"))
            {
                FlagWithNorthAndAddToList();
            }

            if (GUILayout.Button("Add To Enter From East List"))
            {
                FlagWithEastAndAddToList();
            }

            if (GUILayout.Button("Add To Enter From South List"))
            {
                FlagWithSouthAndAddToList();
            }

            if (GUILayout.Button("Add To Enter From West List"))
            {
                FlagWithWestAndAddToList();
            }

            if (GUILayout.Button("Save Room"))
            {
                SaveTilemapToRoomTemplate();
            }

            if (GUILayout.Button("Duplicate Loaded Room"))
            {
                DuplicateLoadedRoomTemplateAndLoadCopy();
            }


            //Make sure we have a roomTemplate loaded, if so, draw a black square (or other default tile) in it's outline
            if (roomTemplate != null)
            {
                if (GUILayout.Button("Clear & Draw Empty " + roomTemplate.roomSizeX + " x " + roomTemplate.roomSizeY))
                {
                    ClearTilemap();
                }
            }

            //This sets up a new BoardGenerationProfile, set of RoomLists and a bunch more, use during initial setup
            if (GUILayout.Button("Create New Profile & Supporting Files"))
            {
                CreateNewProfile();
            }

            //Use to create a new RoomTemplate for authoring
            if (GUILayout.Button("Create New RoomTemplate"))
            {
                CreateNewRoomTemplateAsset();
            }
        }

        //Subscribe to the delegates for scene drawing, used for drawing the red box guides in the Scene view
        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        }

        //Draw the red box in the shape of the loaded RoomTemplate in the Scene view
        void OnSceneGUI(SceneView sceneView)
        {
            if (roomTemplate != null)
            {
                Handles.BeginGUI();
                Debug.DrawLine(Vector3.zero, new Vector3(roomTemplate.roomSizeX, 0, 0), Color.red);
                Debug.DrawLine(Vector3.zero, new Vector3(0, roomTemplate.roomSizeY, 0), Color.red);
                Debug.DrawLine(new Vector3(roomTemplate.roomSizeX, roomTemplate.roomSizeY, 0), new Vector3(roomTemplate.roomSizeX, 0, 0), Color.red);
                Debug.DrawLine(new Vector3(roomTemplate.roomSizeX, roomTemplate.roomSizeY, 0), new Vector3(0, roomTemplate.roomSizeY, 0), Color.red);

                HandleUtility.Repaint();
                Handles.EndGUI();
            }
            
        }

        //This is used to set up all the needed assets and components when getting started with Strata
        public void CreateNewProfile()
        {
            //Create a new BoardGenerationProfile asset
            BoardGenerationProfile profile = CreateAsset<BoardGenerationProfile>("New") as BoardGenerationProfile;
            
            //Create a new BoardLibrary and assign it in the BoardGenerationProfile we created
            profile.boardLibrary = CreateAsset<BoardLibrary>("New") as BoardLibrary;
            boardLibrary = profile.boardLibrary;

            //Create a new InstantiationTechnique asset, defaults to TilemapInstantiator
            profile.boardLibrary.instantiationTechnique = CreateAsset<TilemapInstantiationTechnique>("TilemapInstantiator") as TilemapInstantiationTechnique;

            //Create all the roomlists used by RoomChain and assign them in the BoardLibrary
            profile.boardLibrary.canBeEnteredFromNorthList = CreateAsset<RoomList>("Exits North") as RoomList;
            profile.boardLibrary.canBeEnteredFromWestList = CreateAsset<RoomList>("Exits East") as RoomList;
            profile.boardLibrary.canBeEnteredFromSouthList = CreateAsset<RoomList>("Exits South") as RoomList;
            profile.boardLibrary.canBeEnteredFromEastList = CreateAsset<RoomList>("Exits West") as RoomList;

            //Setup the default tile to be the included black, no collider Tile which comes with Strata
            profile.boardLibrary.SetDefaultTileOnProfileCreation(LoadAndSetDefaultTile());

            Tilemap tilemap = SelectTilemapInScene();

            //Check if we have already setup a BoardGenerator component in the scene, if not create a new one and assign it
            BoardGenerator boardGenerator;
            boardGenerator = tilemap.gameObject.GetComponent<BoardGenerator>();
            if (boardGenerator != null)
            {
                boardGenerator.tilemap = Selection.activeGameObject.GetComponent<Tilemap>();
                boardGenerator.boardGenerationProfile = profile;
            }

            //Save all the new assets we've created
            AssetDatabase.SaveAssets();
        }

        //Use this to create and load a new RoomTemplate asset for authoring
        public void CreateNewRoomTemplateAsset()
        {
            roomTemplate = CreateAsset<RoomTemplate>("Room") as RoomTemplate;
        }

        //Helper function for creating all the ScriptableObject assets we'll need
        public static ScriptableObject CreateAsset<T>(string assetName) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(asset, assetName + " " + typeof(T).Name + ".asset");
            return asset;
        }

        //Loads and sets the default black tile included with Strata, does this based on path so if you move it you'll need to set this manually
        TileBase LoadAndSetDefaultTile()
        {
            string pathToDefaultTile = "Assets/Strata by MirrorfishMedia Inc/Tiles/BlackEmptyNoCollisionTile.asset";
            TileBase defaultBlackEmptyTile = AssetDatabase.LoadAssetAtPath<TileBase>(pathToDefaultTile);

            if (defaultBlackEmptyTile == null)
            {
                Debug.LogError("During initial BoardProfile creation we failed to load default empty tile at intended path " + pathToDefaultTile + 
                    " did the folder structure change? Please fix the path in this script or manually add a default empty tile, with the boolean flag set to true in your BoardLibrary.");
            }

            return defaultBlackEmptyTile;
        }

        //Clear the Tilemap
        public void ClearTilemap()
        {
            Tilemap tilemap = SelectTilemapInScene();
            tilemap.ClearAllTiles();
            tilemap.ClearAllEditorPreviewTiles();
            WriteTemplateSquare();
        }

        // and draw a black rectangle in the shape of the loaded RoomTemplate
        public void WriteTemplateSquare()
        {
            Tilemap tilemap = SelectTilemapInScene();
            Vector3Int origin = tilemap.origin;

            for (int x = 0; x < roomTemplate.roomSizeX; x++)
            {
                for (int y = 0; y < roomTemplate.roomSizeY; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, 0) + origin;
                    tilemap.SetTile(tilePos, boardLibrary.GetDefaultEntry().tile);
                }
            }
        }

        //Create a duplicate of the loaded RoomTemplate, zero out it's exit flags so that we can set them manually
        public void DuplicateLoadedRoomTemplateAndLoadCopy()
        {
            //Create a path to the object we will duplicate
            string pathSource = AssetDatabase.GetAssetPath(roomTemplate);
            //Create a path to where the new copy will be adding an incremental number to end
            string pathDestination = AssetDatabase.GenerateUniqueAssetPath(pathSource);

            //Copy the asset from the source to the destination path
            AssetDatabase.CopyAsset(pathSource, pathDestination);

            //Load that as the loaded RoomTemplate
            roomTemplate = AssetDatabase.LoadAssetAtPath(pathDestination, typeof(RoomTemplate)) as RoomTemplate;

            //Set all the exits to false, we need to set this back by hand and add it to the relevant RoomLists using the buttons in the EditorWindow
            roomTemplate.opensToNorth = false;
            roomTemplate.opensToEast = false;
            roomTemplate.opensToSouth = false;
            roomTemplate.opensToWest = false;
            Debug.Log("Created duplicate room " + roomTemplate.name + " and set all exits to false, add it to room lists");
            
        }

        //This method saves Tilemap to RoomTemplate
        public void SaveTilemapToRoomTemplate()
        {
            //Build the LibraryDictionary so that we can match Tiles to characters
            libraryDictionary = boardLibrary.BuildTileKeyLibraryDictionary();

            //Get a reference to the Tilemap in the scene, find one if not selected
            Tilemap tilemap = SelectTilemapInScene();

            //An int to store the index as we loop through the RoomTemplate
            int charIndex = 0;
            for (int x = 0; x < roomTemplate.roomSizeX; x++)
            {
                for (int y = 0; y < roomTemplate.roomSizeY; y++)
                {
                    //Get the Tile from the Tilemap as a TileBase, this allows us to use other types of Tiles including RuleTiles, RandomTiles and other
                    //scripted tiles, as opposed to just Sprite based tiles.
                    TileBase foundTile = GetTileFromTilemap(x, y, tilemap) as TileBase;
                    
                    
                    if (foundTile == null)
                    {
                        //If tilemap is blank inside grid, write in default empty space character defined in board library, usually 0
                        roomTemplate.roomChars[charIndex] = boardLibrary.GetDefaultEmptyChar();
                        charIndex++;
                    }
                    else
                    {
                        //Get the BoardLibraryEntry that matches this TileBase
                        BoardLibraryEntry entry;
                        entry = boardLibrary.CheckLibraryForTile(foundTile, libraryDictionary);

                        if (entry == null)
                        {
                            //If we don't find a matching Entry, we need one, so let's create it in the BoardLibrary
                            entry = boardLibrary.AddBoardLibraryEntryIfTileNotYetEntered(foundTile);
                            //Set this dirty because we need to save it
                            EditorUtility.SetDirty(boardLibrary);

                        }
                        //Set the character into the RoomTemplate to record it
                        roomTemplate.roomChars[charIndex] = entry.characterId;
                        charIndex++;
                    }
                }
            }
            //Set the RoomTemplate dirty to make sure we'll save changes
            EditorUtility.SetDirty(roomTemplate);
            //Save the AssetDatabase to write changes back to disk
            AssetDatabase.SaveAssets();
        
            Debug.Log("Success. Tilemap written to RoomTemplate");
        }

        //This is used to convert the ASCII data stored in the RoomTemplate back into Tile data for display and editing in the Tilemap in the Scene view
        public void LoadTileMapFromRoomTemplate()
        {
            //Build up the Dictionary from the Library since we need it to match characters to Tiles
            libraryDictionary = boardLibrary.BuildTileKeyLibraryDictionary();

            //Make sure the Tilemap is selected
            Tilemap tilemap = SelectTilemapInScene();

            //Clear it out
            tilemap.ClearAllTiles();

            //Loop through the ASCII roomChars stored in the RoomTemplate and match them to entries in the BoardLibrary, load those Tiles
            int charIndex = 0;
            for (int x = 0; x < roomTemplate.roomSizeX; x++)
            {
                for (int y = 0; y < roomTemplate.roomSizeY; y++)
                {
                    //Get the tile to match the character found from BoardLibrary
                    TileBase tileToSet = boardLibrary.GetTileFromChar(roomTemplate.roomChars[charIndex]);
                    if (tileToSet == null)
                    {
                        Debug.LogError("Attempting to load empty tiles, draw and save something first");
                    }

                    Vector3Int pos = new Vector3Int(x, y, 0) + tilemap.origin;
                    tilemap.SetTile(pos, tileToSet);
                    charIndex++;
                }
            }
        }

        //All four of these functions set the boolean on the RoomTemplate to record that it exits in that direction and add it to the RoomList in the BoardLibrary
        public void FlagWithNorthAndAddToList()
        {
            roomTemplate.opensToNorth = true;
            boardLibrary.canBeEnteredFromSouthList.RemoveEmptyEntriesThenAdd(roomTemplate);
            Debug.Log("Added " + roomTemplate + " to North list and set it's Opens To North bool to true");

        }

        public void FlagWithEastAndAddToList()
        {
            roomTemplate.opensToEast = true;
            boardLibrary.canBeEnteredFromWestList.RemoveEmptyEntriesThenAdd(roomTemplate);
            Debug.Log("Added " + roomTemplate + " to East list and set it's Opens To East bool to true");

        }

        public void FlagWithSouthAndAddToList()
        {
            roomTemplate.opensToSouth = true;
            boardLibrary.canBeEnteredFromNorthList.RemoveEmptyEntriesThenAdd(roomTemplate);
            Debug.Log("Added " + roomTemplate + " to South list and set it's Opens To South bool to true");

        }

        public void FlagWithWestAndAddToList()
        {
            roomTemplate.opensToWest = true;
            boardLibrary.canBeEnteredFromEastList.RemoveEmptyEntriesThenAdd(roomTemplate);
            Debug.Log("Added " + roomTemplate + " to West list and set it's Opens To West bool to true");

        }



        //This is used to make sure that the Tilemap is selected so that we can load, save etc without having to constantly click it
        public Tilemap SelectTilemapInScene()
        {

            if (Selection.activeGameObject == null)
            {
                return FindTilemapOrCreateOne();
            }
            else
            {
                Tilemap tilemap = Selection.activeGameObject.GetComponent<Tilemap>();

                if (tilemap != null)
                {
                    return tilemap;
                }
                else
                {
                    return FindTilemapOrCreateOne();
                }
            }
        }

        //This searches the scene for a Tilemap, returns the first one it finds or creates one if there is none
        //Note: Probably not good to have more than one Tilemap in the scene you're authoring tiles in since this will just return the first one it finds.
        Tilemap FindTilemapOrCreateOne()
        {
            //Search for a Tilemap
            Tilemap tilemap = FindObjectOfType<Tilemap>();
            if (tilemap == null)
            {
                //If Tilemap not found, create one
                tilemap = AddTilemapToScene();
                Selection.activeGameObject = tilemap.gameObject;
                return tilemap;
            }

            Selection.activeGameObject = tilemap.gameObject;
            return tilemap;
        }
        
        //We looked for a Tilemap and couldn't find one, so we will helpfully create you a new one
        Tilemap AddTilemapToScene()
        {
            Debug.Log("No Tilemap found in scene. Creating a new one.");
            //Create a Grid, required for Tilemap
            GameObject grid = new GameObject("Strata Grid");
            GameObject tilemapGameObject = new GameObject("Strata Tilemap");

            //Parent the new object to the grid
            tilemapGameObject.transform.SetParent(grid.transform);

            //Add the Grid component
            grid.AddComponent<Grid>();

            //Select it
            Selection.activeGameObject = grid;

            //Add the Tilemap component
            Tilemap tilemap = tilemapGameObject.AddComponent<Tilemap>();

            //Add the Tilemap Renderer component needed to display tiles
            tilemapGameObject.AddComponent<TilemapRenderer>();

            //Add the BoardGenerator as well while we're at it
            tilemapGameObject.AddComponent<BoardGenerator>();

            return tilemap;
        }

        //Helper function to get a tile from the Tilemap based on x,y coordinates
        TileBase GetTileFromTilemap(int x, int y, Tilemap tilemap)
        {
            Vector3Int pos = new Vector3Int(x, y, 0) + tilemap.origin;
            TileBase tile = tilemap.GetTile(pos);
            return tile;
        }
    }
}

