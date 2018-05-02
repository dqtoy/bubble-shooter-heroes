using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class HomeScene : MonoBehaviour
{

    public static bool EnterFirstTime = true;
    public GameObject LoadingText;
    public Image LoadingBar;
    float currentLoadPercent = 0;
    public GameObject UIs, Gang;
    public GameObject OptionMenu;

    public bool LoadDataFinished = false;

    void Start()
    {
        SetQuality();

        if (GlobalData.DataLoaded == false)
            StartCoroutine(LoadData());
        else
        {
            LoadDataFinished = true;
        }
    }

    void SetQuality()
    {
#if UNITY_IPHONE
        Application.targetFrameRate = 60;
#endif
    }

    void Update()
    {
        if (UIs.activeSelf == false)
        {
            UpdateLoading();
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
			Application.OpenURL("https://play.google.com/store/apps/details?id=fruit.splash.saga");
            Application.Quit();
        }
    }

    void UpdateLoading()
    {
        if (currentLoadPercent >= 100)
        {
            LoadingText.gameObject.SetActive(false);
            UIs.SetActive(true);
            Gang.SetActive(true);
            // Audio
            if (BaseManager.globalGameMusic != null)
            {
                StartCoroutine(AudioHelper.FadeAudioObject(BaseManager.globalGameMusic, -1.0f));
            }
            if (BaseManager.globalMenuMusic == null)
            {
                // create and return the Intro Scene music audio
                BaseManager.globalMenuMusic = AudioHelper.CreateGetFadeAudioObject(BaseManager.GetInstance().menuMusic, true, BaseManager.GetInstance().fadeClip, "Audio-MenuMusic");
                // play the clip
                StartCoroutine(AudioHelper.FadeAudioObject(BaseManager.globalMenuMusic, 0.5f));
            }
        }
        else
        {
            float loadpercent = 20;
            if (LoadDataFinished) loadpercent += 80;

            // update loading bar
            if (currentLoadPercent < loadpercent)
            {
                currentLoadPercent += 1;
                LoadingBar.fillAmount = currentLoadPercent / 100;
            }
        }
    }

    IEnumerator LoadData()
    {
        GlobalData.LoadLevelData();
        yield return new WaitForSeconds(1.0f);
        LoadDataFinished = true;
    }

    public void ShowOptionMenu()
    {
        OptionMenu.SetActive(true);
    }

    public void HideOptionMenu()
    {
        OptionMenu.SetActive(false);
    }

    public void GotoLevelScene()
    {
        // Reset load counter
        BranchPairSetup.ResetAllValue();

        Application.LoadLevel("LevelSelect");
    }

    public void StartEndlessMode()
    {
        // Reset load counter
        BranchPairSetup.ResetAllValue();

        GlobalData.gameMode = GlobalData.GameMode.ENDLESS_MODE;
        GlobalData.SetCurrentLevel(1);

        Application.LoadLevel("GameScene");
    }
}
