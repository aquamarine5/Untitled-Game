using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using LitJson;

public class TilemapUtil : MonoBehaviour
{
    public Tilemap Glass_Tilemap;
    public Tilemap Railing_Timemap;
    public Tile Glass;
    public RuleTile Railing;
    void Start()
    {
        CreateDefaultJSON();
    }
    
    void CreateDefaultJSON()
    {

    }
    void Update()
    {
        
    }
}
