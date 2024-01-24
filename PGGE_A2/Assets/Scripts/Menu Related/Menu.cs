using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private AudioSource m_AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickSinglePlayer()
    {
        //start the coroutine
        StartCoroutine(Coroutine_OnClickSinglePlayer());
    }

    public void OnClickMultiPlayer()
    {
        //start the coroutine
        StartCoroutine(Coroutine_OnClickMultiPlayer());
    }


    IEnumerator Coroutine_OnClickSinglePlayer()
    {
        //play the button sound
        m_AudioSource.Play();

        //wait for the sound to player finish before going to the nextscene
        yield return new WaitForSeconds(m_AudioSource.clip.length);
        SceneManager.LoadScene("SinglePlayer");
    }

    IEnumerator Coroutine_OnClickMultiPlayer()
    {
        //play the button sound
        m_AudioSource.Play();

        //wait for the sound to player finish before going to the nextscene
        yield return new WaitForSeconds(m_AudioSource.clip.length);
        SceneManager.LoadScene("Multiplayer_Launcher");
    }

}
