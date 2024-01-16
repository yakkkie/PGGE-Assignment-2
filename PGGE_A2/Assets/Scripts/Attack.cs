using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject shrek;
    ShrekPlayer shrekPlayer;
    
    void Start()
    {
        Debug.Log("start!!!");
        shrekPlayer = shrek.GetComponent<ShrekPlayer>();
    }
    
    public void MinusAttack() //called in animation event when attacking
    {
        //Made this just to decrease remaining attacks when the animation stops 
        shrekPlayer.mAttacksRemaining -= 1;
        shrekPlayer.ResetAttack();
        Debug.Log(shrekPlayer.mAttacksRemaining);
        
    }

}
