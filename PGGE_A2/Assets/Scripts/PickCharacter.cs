using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickCharacter : MonoBehaviour
{
    public Text text;

    private string scifiString;

    private string shrekString;


    private void Start()
    {
        scifiString = "Scifi Robot";
        shrekString = "Shrek";

        text.text = scifiString;

    }

    public void NextCharacter()
    {
        if (text.text == scifiString)
            text.text = shrekString;
        else
            text.text = scifiString;
    }

}




