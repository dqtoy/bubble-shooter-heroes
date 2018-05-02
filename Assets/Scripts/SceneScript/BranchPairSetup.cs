using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BranchPairSetup : MonoBehaviour
{

    public static List<GameObject> BranchList = new List<GameObject>();

    public const int MAXLEVEL = 120;
    public static int BtnCounter = 0, LoadCounter = 0;
    public Button[] ButtonList;
    public GameObject BranchPrefab;

    public static bool allAvailableButtonViewed = false;
    void Start()
    {
        StartCoroutine(StartLoad());
    }

    IEnumerator StartLoad()
    {
        for (int i = 0; i < ButtonList.GetLength(0); i++)
        {
            Button btn = ButtonList[i];

            if (BtnCounter < MAXLEVEL)
            {
                BtnCounter++;
                SelectLevel slv = btn.GetComponent<SelectLevel>();
                slv.SetLevel(BtnCounter);
                btn.gameObject.SetActive(true);
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }

        LoadCounter++;

        if (BtnCounter < MAXLEVEL)
        {
            //yield return null;
        }
        else
        {
            LevelSelectController.FinishedSetup = true;
            LevelSelectController.needPlayFadeIn = true;
        }

        yield return new WaitForSeconds(0.1f);
    }

    public static void ResetAllValue()
    {
        BranchList.Clear();
        BtnCounter = 0;
        LoadCounter = 0;
        allAvailableButtonViewed = false;
        LevelSelectController.FinishedSetup = false;
        LevelSelectController.StarCounter = 0;
    }
}
