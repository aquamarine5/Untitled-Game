using UnityEngine;
using System.Reflection;
using UnityEngine.Experimental.Rendering.Universal;

public class CustomShadowCaster2D : ShadowCaster2D
{
    public ShadowCaster2D shadowCaster;
    FieldInfo _meshField;
    FieldInfo _shapePathField;
    MethodInfo _onEnableMethod;
    void Awake()
    {
        _meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
        _shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
        _onEnableMethod = typeof(ShadowCaster2D).GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance);
        _shapePathField.SetValue(shadowCaster, null); //This won't deal with holes in the shape
        _meshField.SetValue(shadowCaster, null);
        _onEnableMethod.Invoke(shadowCaster, new object[0]);
    }
    public static void SetShadow()
    {

    }
}