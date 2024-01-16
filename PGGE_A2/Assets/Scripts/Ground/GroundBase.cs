using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundBase : MonoBehaviour
{
    
    protected AudioClip[] concreteFootSteps;

    public GroundBase(AudioClip[] footSteps)
    {
        concreteFootSteps = footSteps;
    }


}
