using UnityEngine;
using System.Collections;
using System;

public class IUtilities
{

    public static Type GetPlayerPref()
    {
#if UNITY_WP8
        return typeof(PlayerPrefs);
#else
        return typeof(Utilities.PlayerPrefs);
#endif
    }

}
