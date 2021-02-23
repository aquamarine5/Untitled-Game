using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Mirror;
using static CatalogueScript;

public class TilemapSpawn : NetworkBehaviour
{
    [Header("GameObject")]
    [Tooltip("Tilemap's collider")] public TilemapCollider2D tilemapCollider;
    [Tooltip("Tilemap's composite collider")] public CompositeCollider2D cc2d;
    public Tilemap tilemap;
    public Tile defaultBlackTile;
    public Tilemap itemTilemap;

    [Header("UI")]
    public Text showSeedText;
    public GameObject loadingPanel;
    public SliderData[] sliderDatas;
    public Slider slider;
    public Text showTips;
    public Text showSpeed;

    [Header("Build map information")]
    public AnimationCurve mapRender;
    [Range(1,500)]public int spawnMapSpeed = 25;
    [Tooltip("地图缩放程度")][Range(0f,1f)] public float buildMapScale;
    [Range(0f, 1f)] [Tooltip("超过此数的方格将为空")] public float buildBlockScale = 0.5f;

    [Header("Build map area")]
    public Vector2Int targetSize = new Vector2Int(500, 500);
    public Vector2Int offset = new Vector2Int(-250, -500);

    [Space(10)]
    [Tooltip("whether update the collider when render a frame?")] public bool isUpdateColliderOnFrame = true;
    [Tooltip("WARNING: the value don't more than 0.5f")][Range(0f, 1f)] public float spawnTorch = 0.005f;

    float timerLoop = 0;
    int tempProgress = 0;
    readonly int timerTargetCount = 1;
    float timer = 0f;

    private static int progress = 0;
    public static Slider staticSlider;
    public static Text staticTips;
    public static BuildMapStatus buildMapStatus;
    
    public static int x, y = 0;
    public static Vector2Int _targetSize;
    public static int TargetProgress { get; set; } = 0;
    public static int Progress { get => progress;
        set {
            progress = value;
            if (Application.isPlaying)
            {
                staticSlider.value = (float)progress / TargetProgress;
            }
            staticTips.text = buildMapStatus.ConvertToString().Replace("#", Progress + "：" + TargetProgress);
        } 
    }
    private void Awake()
    {
        Random.Range(10000, 10000000).SetSeed();
        staticSlider = slider;
        staticTips = showTips;
        _targetSize = targetSize;
    }
    private void FixedUpdate()
    {
        timerLoop += Time.fixedDeltaTime;
        timer += Time.fixedDeltaTime;
        if (timer >= timerTargetCount)
        {
            showSpeed.text = (tempProgress / timerLoop).ToString("F2") + "格/秒\n" + (TargetProgress / (tempProgress / timerLoop)).ToString("F2");
            tempProgress = progress;
            timer = 0;
        }
    }
    public void BuildMap()
    {
        if (!isServer) return;
        if (!NetworkControl.S.isNetworkActive) return;

        loadingPanel.SetActive(true);
        x = 250; y = 500;
        StopCoroutine(nameof(BuildMap_v2));
        StartCoroutine(BuildMap_v2());
    }
    IEnumerator BuildMap_v2()
    {
        TargetProgress = targetSize.x * targetSize.y;
        // Disabled Physics2D Simulate
        Physics2D.simulationMode = SimulationMode2D.Script;
        if (!isUpdateColliderOnFrame) tilemapCollider.enabled = false;
        buildMapStatus = BuildMapStatus.CaveDigging;
        for (int x = 0; x < targetSize.x; x++)
        {
            for (int y = 0; y < targetSize.y; y++)
            {
                // Use now seed and add offset
                float result = Mathf.PerlinNoise(
                    RandomUtil.NowSeed + x * buildMapScale, RandomUtil.NowSeed + y * buildMapScale);

                // see also https://github.com/awesomehhhhh/Game/issues/8
                float externes = mapRender.Evaluate(1f - ((float)(y + 1) / targetSize.y));

                // use TilemapPlugin.DefaultSetTile(Tilemap, (int, int), TileBase, bool)"
                // don't load to "TilemapPlugin.tmDictionary"
                tilemap.DefaultSetTile((x + offset.x, y + offset.y),
                   result >= externes ? BlockAssetInstance.glass : BlockAssetInstance.black, true);
                // spawn torch
                if (RandomUtil.RandomRange(spawnTorch) & result < externes){
                    itemTilemap.ReSetTile((x + offset.x, y + offset.y), BlockAssetInstance.torch,true);
                }
            }
            if (x % spawnMapSpeed == 0)
            {
                Progress += targetSize.y * spawnMapSpeed;
                yield return null;
            }
        }
        if (!isUpdateColliderOnFrame) tilemapCollider.enabled = true;
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        buildMapStatus = BuildMapStatus.GlassBuilding;
        yield return StartCoroutine(PlantGlass());
        TargetProgress = 0;
        showSeedText.text += "\n" + LanguageDataInstance.RenderShapeCount + ":" + cc2d.shapeCount;
        loadingPanel.SetActive(false);
    }
    IEnumerator PlantGlass()
    {
        if (!isUpdateColliderOnFrame) tilemapCollider.enabled = false;
        TargetProgress = targetSize.x;
        Progress = 0;
        for (int i = 0; i < targetSize.x; i++)
        {
            if (tilemap.GetTile(new Vector3Int(-x + i, -2, 0)) != defaultBlackTile) 
            {
                tilemap.DefaultSetTile((-x + i, -1), BlockAssetInstance.glass_dirt,true);
                Progress++;
                if (Progress % 250 == 0) { yield return null; }
            }
        }
        if (!isUpdateColliderOnFrame) tilemapCollider.enabled = true;
    }
    public enum BuildMapStatus
    {
        CaveDigging,
        GlassBuilding,
        StartBuild
    }
}
[System.Serializable]
public class SliderData
{
    public Sprite sliderEdge;
    public Sprite sliderInside;
    public Sprite sliderBall;
}
