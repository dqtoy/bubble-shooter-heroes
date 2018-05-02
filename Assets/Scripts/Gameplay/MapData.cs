using UnityEngine;
using System.Collections.Generic;

public class MapData
{

    public bool isStartWithLeft; // Check if the last row is left 
    public int TargetNo1, TargetNo2, TargetNo3; // Target to get Stars
    public int BubbleNumber; // Number of Bubble to shoot
    public int MapSizeY;
    public List<int[]> BubbleData; // x, y, id 

    public void LoadData(int level)
    {
        int[,] rawData = MyUtilities.ParseFile("MapData/config_level_data_" + level, 21);

        MapSizeY = rawData.GetLength(0);

        // check left row
        isStartWithLeft = false;
        if (rawData[0, 20] != 0) isStartWithLeft = false;
        else
        {
            for (int i = 20; i > 1; i--)
            {
                if (rawData[0, i] != rawData[0, i - 1])
                {
                    isStartWithLeft = (i % 2 != 0);
                    break;
                }
            }
        }

        // put data to array
        BubbleData = new List<int[]>();

        for (int i = rawData.GetLength(0) - 1; i >= 0; i--)
        {
            int j = 20;
            if (isStartWithLeft == ((MapSizeY - i) % 2 == 0)) j = 19;

            for (; j >= 0; j -= 2)
            {
                if (rawData[i, j] != 0)
                {
                    int[] data = new int[] { Mathf.FloorToInt((20 - j) / 2), i - MapSizeY + 1, rawData[i, j] };
                    BubbleData.Add(data);
                }
            }
        }
    }

    // Set default parameter for endless mode
    public void LoadEndlessMapData()
    {
        isStartWithLeft = false;
        MapSizeY = 0;
    }

}