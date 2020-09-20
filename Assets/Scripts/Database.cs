using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public Database DefaultDatabase;
    public struct Weapon
    {
        public Chalk chalk;
    }
    public struct Chalk
    {
        public float damage;
    }
    private void Start()
    {
        
    }
    public Database NowDatabase
    {
        get
        {
            return NowDatabase;
        }
        set
        {
            NowDatabase = value;
        }
    }
    private void Awake()
    {
        
    }
}
