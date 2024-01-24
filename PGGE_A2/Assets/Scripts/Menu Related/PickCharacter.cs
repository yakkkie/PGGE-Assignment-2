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

        //set the strings at the start
        //playerPref strings are to set the playerpref value later
        playerPref_ScifiString = "Prefabs/SciFiPlayer_Networked";
        playerPref_ShrekString = "Prefabs/Shrek_Connected";

        //text strings are for displaying on the screen
        textScifi = "Scifi Robot";
        textShrek = "Shrek";

        defaultCharacter = string.Empty;

        if (text != null)
        {
            //checks if the player has chosen a character before
            if (PlayerPrefs.HasKey(playerPref_CharacterKey))
            {
                //if yes then set the character chosen to be the same as the character they chose before
                defaultCharacter = PlayerPrefs.GetString(playerPref_CharacterKey);

                //if the character was the scifi robot, set the text to show the player he is selecting the scifi robot
                if (defaultCharacter == playerPref_ScifiString)
                    text.text = textScifi;

                //if the character was the shrek, set the text to show the player he is selecting shrek
                else if (defaultCharacter == playerPref_ShrekString)
                    text.text = textShrek;
                
            }
        }

    }

    //used on a button to let the player cycle through the characters
    public void NextCharacter()
    {
        //plays a button sound
        audioSource.Play();

        //set the text to show the player he is selecting shrek and set the player pref value to shrek
        if (text.text == textScifi)
        {
            text.text = "Shrek";
            
            PlayerPrefs.SetString(playerPref_CharacterKey, playerPref_ShrekString);
        }

        //set the text to show the player he is selecting scifi robot and set the player pref value to scifirobot
        else
        {
            text.text = "Scifi Robot";
            
            PlayerPrefs.SetString(playerPref_CharacterKey, playerPref_ScifiString);
        }
            
    }

}




