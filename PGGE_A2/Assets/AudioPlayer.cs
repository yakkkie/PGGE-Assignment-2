using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource dingSound;


    public void PlaySound()
    {
        dingSound.Play();
    }
}
