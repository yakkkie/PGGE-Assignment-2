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
        StartCoroutine(Coroutine_Back());
    }

    IEnumerator Coroutine_Back()
    {

        m_AudioSource.Play();

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Menu");
    }


}
