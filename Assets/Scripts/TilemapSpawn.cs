using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilemapSpawn : MonoBehaviour
{
    public CatalogueScript catalogue;
    public Strata.BoardGenerator sbg;
    public CompositeCollider2D cc2d;
    public Text showSeedText;
    public GameObject loadingPanel;
    public SliderData[] sliderDatas;
    public Slider slider;
    public Tilemap tilemap;
    public Tile defaultBlackTile;
    public Text showTips;
    public Text showSpeed;

    float timerLoop = 0;
    int tempProgress = 0;
    [SerializeField]int timerTargetCount = 1;
    float timer = 0f;

    private static int progress = 0;
    public static Slider staticSlider;
    public static Text staticTips;
    public static BuildMapStatus buildMapStatus;
    public static int x, y = 0;

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
        int seed = Random.Range(0, 10000000);
        showSeedText.text = seed.ToString();
        Random.InitState(seed);
        loadingPanel.SetActive(true);
        x = 250; y = 500;
        StartCoroutine(StartBuildMap());
        
    }
    IEnumerator StartBuildMap()
    {
        yield return StartCoroutine(sbg.BuildLevel());
        showSeedText.text += "\n"+catalogue.languageData.RenderShapeCount+":"+cc2d.shapeCount;
        buildMapStatus = BuildMapStatus.GlassBuilding;
        yield return StartCoroutine(PlantGlass());
        loadingPanel.SetActive(false);
    }
    IEnumerator PlantGlass()
    {
        TargetProgress = sbg.boardGenerationProfile.boardHorizontalSize;
        Progress = 0;
        for (int i = 0; i < sbg.boardGenerationProfile.boardHorizontalSize; i++)
        {
            if (tilemap.GetTile(new Vector3Int(-x + i, -2, 0)) != defaultBlackTile) 
            {
                tilemap.SetTile(new Vector3Int(-x + i, -1, 0), catalogue.blockAsset.glass_dirt);
                Progress++;
                if (Progress % 250 == 0) { yield return null; }
            }
        }
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
