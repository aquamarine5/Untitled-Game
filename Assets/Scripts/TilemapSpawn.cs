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
    public Animator anim;
    public BlockAsset blockAsset;
    public SliderData[] sliderDatas;
    public Slider slider;
    public Tilemap tilemap;
    public Text showTips;
    public static Slider staticSlider;
    public static Text staticTips;
    public static BuildMapStatus buildMapStatus;
    public static int TargetProgress { get { return TargetProgress; }
        set {
            TargetProgress = value;
        }
    }
    public static int Progress { get { return Progress; } 
        set {
            Progress = value;
            staticSlider.value = Progress / TargetProgress;
            staticTips.text = buildMapStatus.ConvertToString();
        } 
    }
    public static int x, y = 0;
    private void Awake()
    {
        staticSlider = slider;
        staticTips = showTips;
    }
    public void BuildMap()
    {
        int seed = Random.Range(0, 2333333);
        showSeedText.text = seed.ToString();
        Random.InitState(seed);
        loadingPanel.SetActive(true);
        x = 250; y = 500;
        
        StartCoroutine(StartBuildMap());
        
    }
    IEnumerator StartBuildMap()
    {
        anim.SetBool("isRuning", true);
        yield return StartCoroutine(sbg.BuildLevel());
        anim.SetBool("isRuning", false);
        loadingPanel.SetActive(false);
        showSeedText.text += "\n"+catalogue.languageData.RenderShapeCount+":"+cc2d.shapeCount;
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
