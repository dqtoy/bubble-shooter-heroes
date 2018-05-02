using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalData
{

    public enum GameMode
    {
        PUZZLE_MODE = 0,
        ENDLESS_MODE = 1
    } ;

    public const int ENDLESS_TARGET_EACHLEVEL = 200000;
    public const int LEVELUP_RATE = 6;

    // GLOBAL 
    public static bool REDUCED_VERSION = false;

    // Fixed Data
    public static Dictionary<int, int> LevelRequire = new Dictionary<int, int>();
    public static Dictionary<int, int[]> StarTarget = new Dictionary<int, int[]>();
    public static Dictionary<int, int[]> PreBubble = new Dictionary<int, int[]>();
    public static int[,] PointReward = new int[15, 3];
    public static bool DataLoaded = false;
    // Temporal Data
    public static GameMode gameMode = GameMode.PUZZLE_MODE;

    static int Level = 1;

    public static int GetCurrentLevel()
    {
        return Level;
    }
    public static void SetCurrentLevel(int lvl)
    {
        Level = lvl;
    }

    public static void LoadLevelData()
    {
        // PreLoad Allthings
        if (DataLoaded) return;
        // Level pass require
        int[,] rawData = MyUtilities.ParseFile("MapData/config_aim_data", 6);

        for (int i = 0; i < rawData.GetLength(0); i++)
        {
            LevelRequire.Add(i + 1, rawData[i, 2]);
        }

        // Level star target
        rawData = MyUtilities.ParseFile("MapData/config_star_evaluate_data", 5);
        for (int i = 0; i < rawData.GetLength(0); i++)
        {
            StarTarget.Add(i + 1, new int[] { rawData[i, 1], rawData[i, 2], rawData[i, 3], rawData[i, 4] });
        }

        // Pre load bubble
        rawData = MyUtilities.ParseFile("MapData/pre_five_bubble_data", 6);
        for (int i = 0; i < rawData.GetLength(0); i++)
        {
            PreBubble.Add(i + 1, new int[] { rawData[i, 1], rawData[i, 2], rawData[i, 3], rawData[i, 4], rawData[i, 5] });
        }

        // Points from combo
        PointReward = MyUtilities.ParseFile("MapData/ScoreRulesData", 3);

        DataLoaded = true;
    }
}
