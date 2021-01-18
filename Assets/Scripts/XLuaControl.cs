using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using Mirror;

public class XLuaControl : NetworkBehaviour
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
