using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameSetting
{

    public GameObject PlayPopup;

    public static void SetVolume(float f)
    {
        AudioListener.volume = f;
    }
}
