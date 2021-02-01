using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class RandomUtil
{
    static int nowSeed;
    public static int NowSeed
    {
        get => nowSeed; set
        {
            nowSeed = value;
            Random.InitState(nowSeed);
            //CatalogueScript.S.seedText.text = $"Seed:{value}";
        }
    }
    public static int SetSeed(this int seed)
    {
        NowSeed = seed;
        return seed;
    }
    public static void Choice()
    {

    }
    public static bool RandomRange(float scale = 0.5f)
    {
        float value = Random.value;
        if (value<=scale) return true;
        else return false;
    }
}