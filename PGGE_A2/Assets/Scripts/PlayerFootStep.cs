using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{
    public   AudioClip[] curFootSteps; //gets the footstep from player after checking the ground check
    [SerializeField]
    private AudioSource footSource;

    public AudioClip[] concreteFootSteps, dirtFootSteps, sandFootSteps, metalFootSteps, woodFootSteps;

    public LayerMask groundMask;


    float walkVol = 0.3f;
    float walkPitch = 0.5f;
    float runVol = 0.5f;
    float runPitch = 1.0f;


    public void WalkFootStep() //this method is called in animation events for walking
    {
        
        Debug.DrawRay(transform.position + transform.up + transform.up, transform.forward * 5, Color.red, 0.1f);
        CheckGroundType();
        AudioClip footClip = curFootSteps[Random.Range(0, curFootSteps.Length)];
        walkVol = Random.Range(0.2f, 0.4f);
        footSource.volume = walkVol;
        walkPitch = Random.Range(0.3f, 0.7f);
        footSource.pitch = walkPitch;
        footSource.PlayOneShot(footClip);
    }

    public void RunFootStep()
    {
        //this method is called in animation events for running 
        Debug.DrawRay(transform.position + transform.up + transform.up, transform.forward * 5, Color.red, 0.1f);
        CheckGroundType();
        AudioClip footClip = curFootSteps[Random.Range(0, curFootSteps.Length)];
        runVol = Random.Range(0.6f, 0.9f);
        footSource.volume = runVol;
        runPitch = Random.Range(0.9f, 1.2f);
        footSource.pitch = runPitch;
        footSource.PlayOneShot(footClip);
    }




    public void CheckGroundType()
    {
        Physics.Raycast(transform.position + transform.up, -transform.up , out RaycastHit hit, 10f, groundMask); //cast a ray below the player to check what ground type it is on
        Debug.DrawRay(transform.position + transform.up, -transform.up, Color.magenta, 0.1f);
        
        //checks the tag for the material
        if (hit.collider.tag == "Concrete")
        {
            curFootSteps = concreteFootSteps;
        }
        else if (hit.collider.tag == "Dirt")
        {
            curFootSteps = dirtFootSteps;
        }
        else if (hit.collider.tag == "Sand")
        {
            curFootSteps = sandFootSteps;
        }
        else if (hit.collider.tag == "Metal")
        {
            curFootSteps = metalFootSteps;
        }
        else if (hit.collider.tag == "Wood")
        {
            curFootSteps = woodFootSteps;
        }
    }
}
