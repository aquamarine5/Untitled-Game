using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public Tilemap t;
    public AnimatedTile tt;
    void Start()
    {
        var a = new Grid();
        Debug.Log(t.GetTile(new Vector3Int(1,1,0)));
        t.SetEditorPreviewTile(new Vector3Int(1, 1, 0), tt);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
