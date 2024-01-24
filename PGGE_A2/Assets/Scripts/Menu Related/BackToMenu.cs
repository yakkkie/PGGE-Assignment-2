using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Back()
    {
        //start the coroutine
        StartCoroutine(Coroutine_Back());
    }

    IEnumerator Coroutine_Back()
    {

        //play the button sound
        m_AudioSource.Play();

        //wait for the sound to play finish before going to the nextscene
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Menu");
    }


}
