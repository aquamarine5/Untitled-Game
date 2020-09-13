using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    public AttackType at;
    public Test[] ts;
    public int a;
    public enum AttackType
    {
        Melee=1
    }
    public struct Test
    {
        public int a;
        string b;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
