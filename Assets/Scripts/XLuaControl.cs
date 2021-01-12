﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class XLuaControl : MonoBehaviour
{
    public static LuaEnv luaEnv;
    void Awake()
    {
        luaEnv = new LuaEnv();
    }
    private void OnDestroy()
    {
        luaEnv.Dispose();
    }
}
