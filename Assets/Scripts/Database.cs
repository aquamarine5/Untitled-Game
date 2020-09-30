using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    #region Data
    public struct Data
    {
        public Weapon weapon;
    }
    public struct Weapon
    {
        public Chalk chalk;
    }
    public struct Chalk
    {
        public float damage;
    }
    #endregion
    #region Text
    public struct Text
    {
        public UI ui;
    }
    public struct UI 
    {
        public string text_FPS;
    }
    #endregion
    private void Start()
    {
        
    }
    private void Awake()
    {
        
    }
}
