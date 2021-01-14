using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilemapSpawn : MonoBehaviour
{
    public CatalogueScript catalogue;
    [System.Obsolete] public Strata.BoardGenerator sbg;
    [Tooltip("Tilemap's collider")] public TilemapCollider2D tilemapCollider;
    [Tooltip("Tilemap's composite collider")] public CompositeCollider2D cc2d;
    [Tooltip("Human's rigidbody")] public Rigidbody2D rigidbody2d;
    public Text showSeedText;
    public GameObject loadingPanel;
    public SliderData[] sliderDatas;
    public Slider slider;
    public Tilemap tilemap;
    public Tile defaultBlackTile;
    public Text showTips;
    public Text showSpeed;
    public bool isUpdateColliderOnFrame = true;
    [Range(1,500)]public int spawnMapSpeed = 25;
    [Tooltip("地图乘积")][Range(1f,20f)] public float buildMapSize;
    [Tooltip("地图缩放程度")][Range(10,1000)] public float buildMapScale;
    public Vector2Int targetSize = new Vector2Int(500, 500);
    public Vector2Int offset = new Vector2Int(-250, -500);

    float timerLoop = 0;
    int tempProgress = 0;
    [SerializeField] int timerTargetCount = 1;
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
        Random.Range(10000, 10000000).SetSeed();
        loadingPanel.SetActive(true);
        x = 250; y = 500;
        StartCoroutine(BuildMap_v2());
    }
    [System.Obsolete("Use Buildmap_v2 to instead")]
    IEnumerator StartBuildMap()
    {
        yield return StartCoroutine(sbg.BuildLevel());
        showSeedText.text += "\n"+catalogue.languageData.RenderShapeCount+":"+cc2d.shapeCount;
        buildMapStatus = BuildMapStatus.GlassBuilding;
        yield return StartCoroutine(PlantGlass());
        loadingPanel.SetActive(false);
    }
    IEnumerator BuildMap_v2()
    {
        TargetProgress = 0;
        Physics2D.simulationMode = SimulationMode2D.Script;
        if (!isUpdateColliderOnFrame) tilemapCollider.enabled = false;
        buildMapStatus = BuildMapStatus.CaveDigging;
        float mapScale = RandomSeedPlugin.NowSeed / buildMapScale;
        TargetProgress = targetSize.x * targetSize.y;
        for (int x = 0; x < targetSize.x; x++)
        {
            for (int y = 0; y < targetSize.y; y++)
            {
                tilemap.ReSetTile(new Vector2Int(x, y)+offset, Mathf.PerlinNoise(
                    mapScale + x / buildMapSize, mapScale + y / buildMapSize)
                    >= 0.5f ? catalogue.blockAsset.glass : catalogue.blockAsset.black);
            }
            if (x % spawnMapSpeed == 0)
            {
                Progress += targetSize.y * spawnMapSpeed;
                if(isUpdateColliderOnFrame) Physics2D.Simulate(Time.fixedDeltaTime);
                yield return null;
            }
        }
        if (!isUpdateColliderOnFrame) tilemapCollider.enabled = true;
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        buildMapStatus = BuildMapStatus.GlassBuilding;
        yield return StartCoroutine(PlantGlass());
        TargetProgress = 0;
        showSeedText.text += "\n" + catalogue.languageData.RenderShapeCount + ":" + cc2d.shapeCount;
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
                tilemap.ReSetTile(new Vector2Int(-x + i, -1), catalogue.blockAsset.glass_dirt);
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