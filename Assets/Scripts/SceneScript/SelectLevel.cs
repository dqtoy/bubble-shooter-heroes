using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour
{

    public int ID = 1;
    public Text MapName;
    public GameObject StarContainer;
    public GameObject[] Stars;
    public void Select()
    {

        // Save Last click level
        Utilities.PlayerPrefs.SetInt("LastVisitedLevel", ID);
        Utilities.PlayerPrefs.Flush();

        LevelSelectController.LastVisit = ID;
        LevelSelectController.ShowPlayDialog(ID);
    }

    public void SetLevel(int id)
    {
        ID = id;
        MapName.text = ID.ToString();

        // Scroll camera to this
        if (ID == LevelSelectController.LastVisit)
        {
            LevelSelectController.LastVisitLoc = transform.position;
        }

        // Set moving destination
        if (LevelSelectController.NewUnlockedLevel != -1)
        {
            if (ID == LevelSelectController.NewUnlockedLevel - 1)
            {
                LevelSelectController.LastButton = gameObject;
            }
            if (ID == LevelSelectController.NewUnlockedLevel)
            {
                LevelSelectController.NewButton = gameObject;

            }
        }
        // Get Level Data
        Button btn = GetComponent<Button>();
        btn.interactable = Utilities.PlayerPrefs.GetBool("LevelUnlocked-" + ID, false);

        if (ID == 1)
        {
            btn.interactable = true;
        }

        if (btn.IsInteractable())
        {
            MapName.gameObject.SetActive(true);
            StarContainer.SetActive(true);
            LevelSelectController.HighestButton = gameObject;
            // show Star
            int star = Utilities.PlayerPrefs.GetInt("LevelStar-" + ID, 0);
            LevelSelectController.AddStar(star);

            for (int i = 0; i < 3; i++)
            {
                Stars[i].SetActive(i < star);
            }
        }
        else
        {
            MapName.gameObject.SetActive(false);
            StarContainer.SetActive(false);
            BranchPairSetup.allAvailableButtonViewed = true;
        }
    }
}
