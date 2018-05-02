using UnityEngine;
using System.Collections;

public class MyUtilities
{

    public static int[,] ParseFile(string filename, int arrayLength)
    {

        TextAsset textFile = (TextAsset)Resources.Load(filename, typeof(TextAsset));


        string input = textFile.ToString();
        string[] lines = input.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        int[,] spaces = new int[lines.Length, arrayLength];

        for (int i = 0; i < lines.Length; i++)
        {
            string st = lines[i];
            string[] nums = st.Split(new[] { '\t' });
            if (nums.Length != arrayLength)
            {
                Debug.Log("Misforned input on line " + i + 1);
            }
            for (int j = 0; j < Mathf.Min(nums.Length, arrayLength); j++)
            {
                int val;
                if (int.TryParse(nums[j], out val))
                    spaces[i, j] = val;
                else
                    spaces[i, j] = -1;

            }
        }
        return spaces;
    }

    public static bool isErrorImage(Texture tex)
    {
        //The "?" image that Unity returns for an invalid www.texture has these consistent properties:
        //(we also reject null.)
        return (tex && tex.name == "" && tex.height == 8 && tex.width == 8 && tex.filterMode == FilterMode.Bilinear && tex.anisoLevel == 1 && tex.wrapMode == TextureWrapMode.Repeat && tex.mipMapBias == 0);
    }
}
