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
        StartCoroutine(Coroutine_OnClickSinglePlayer());
    }

    public void OnClickMultiPlayer()
    {
        StartCoroutine(Coroutine_OnClickMultiPlayer());
    }


    IEnumerator Coroutine_OnClickSinglePlayer()
    {
        m_AudioSource.Play();

        yield return new WaitForSeconds(m_AudioSource.clip.length);
        SceneManager.LoadScene("SinglePlayer");
    }

    IEnumerator Coroutine_OnClickMultiPlayer()
    {
        m_AudioSource.Play();

        yield return new WaitForSeconds(m_AudioSource.clip.length);
        SceneManager.LoadScene("Multiplayer_Launcher");
    }

}
