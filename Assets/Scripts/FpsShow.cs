using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FpsShow : MonoBehaviour
{
    public CatalogueScript cs;
    public Text FpsText;
    float time=0;
    int frameCount=0;
    void Update()
    {
        time += Time.unscaledDeltaTime;
        frameCount++;
        if (time >= 1 && frameCount >= 1)
        {
            float fps = frameCount / time;
            time = 0;
            frameCount = 0;
            FpsText.text = cs.languageData.FPSShow + fps.ToString("f2");
            FpsText.color = fps >= 24 ? Color.green : (fps > 15 ? Color.yellow : Color.red);
        }
    }
}