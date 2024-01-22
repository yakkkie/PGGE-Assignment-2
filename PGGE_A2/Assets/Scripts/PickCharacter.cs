using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickCharacter : MonoBehaviour
{
    private Text text;

    private string defaultCharacter;
    private string textScifi;
    private string textShrek;

    private string playerPref_ScifiString;
    private string playerPref_ShrekString;

    private string playerPref_CharacterKey = "Character";

    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        text = GetComponent<Text>();
        playerPref_ScifiString = "Prefabs/SciFiPlayer_Networked";
        playerPref_ShrekString = "Prefabs/Shrek_Connected";
        textScifi = "Scifi Robot";
        textShrek = "Shrek";
        defaultCharacter = string.Empty;

        if (text != null)
        {
            if (PlayerPrefs.HasKey(playerPref_CharacterKey))
            {
                defaultCharacter = PlayerPrefs.GetString(playerPref_CharacterKey);


                if (defaultCharacter == playerPref_ScifiString)
                    text.text = textScifi;


                else if (defaultCharacter == playerPref_ShrekString)
                    text.text = textShrek;
                
            }
        }

    }

    public void NextCharacter()
    {
        audioSource.Play();
        if (text.text == textScifi)
        {
            text.text = "Shrek";
            
            PlayerPrefs.SetString(playerPref_CharacterKey, playerPref_ShrekString);
        }


        else
        {
            text.text = "Scifi Robot";
            
            PlayerPrefs.SetString(playerPref_CharacterKey, playerPref_ScifiString);
        }
            
    }

}




