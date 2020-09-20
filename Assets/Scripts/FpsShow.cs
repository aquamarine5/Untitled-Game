
using UnityEngine;
using UnityEngine.UI;
class FpsShow : MonoBehaviour
{
    public Text FpsText;
    private float time;
    private int frameCount;
    private void Update()
    {
        time += Time.unscaledDeltaTime;
        frameCount++;
        if (time >= 1 && frameCount >= 1)
        {
            float fps = frameCount / time;
            time = 0;
            frameCount = 0;
            FpsText.text = fps.ToString("f2");//#0.00
            FpsText.color = fps >= 24 ? Color.green : (fps > 15 ? Color.yellow : Color.red);
        }
    }
}