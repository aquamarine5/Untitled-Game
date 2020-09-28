using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
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
    private void Start()
    {
        
    }
    private void Awake()
    {
        
    }
}
