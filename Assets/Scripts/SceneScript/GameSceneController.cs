using UnityEngine;
using System.Collections;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class GameSceneController : MonoBehaviour
{

    GameplayController gamePlayController;

    public GameObject FadeObject;

    public static bool needShowFade = false;
	
	private InterstitialAd interstitial;

    // Use this for initialization
	
	private void RequestInterstitial()
    {
        string adUnitId = "ca-app-pub-9738690958049285/7560188655";
        // Create an interstitial.
        interstitial = new InterstitialAd(adUnitId);
        // Load an interstitial ad.
		AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }
	
	private void ShowInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
    }

    void Start()
    {
		RequestInterstitial();
        needShowFade = false;
        FadeObject.animation.Play("FadeIn");
        gamePlayController = GameObject.FindObjectOfType<GameplayController>();

        // Initial
        // Levelselect switch
        LevelSelectController.NewUnlockedLevel = -1;
        LevelSelectController.needUpdateMoveSquirrelIcon = false;
    }

    void Update()
    {
        if (needShowFade)
        {
            needShowFade = false;
            FadeObject.animation.Play("FadeOut");
        }
    }

    // Scene Controlling
    // Menu Popup Area
    public GameObject PauseMenu, WinMenu, EndlessWinMenu, LoseMenu, OptionMenu;
    public UnityEngine.UI.Button PauseButton;
    public void PauseGame()
    {
        if (gamePlayController.gamePaused)
        {
            gamePlayController.gamePaused = false;
            PauseMenu.gameObject.SetActive(false);
            PauseButton.enabled = true;
        }
        else
        {
			ShowInterstitial();
            gamePlayController.gamePaused = true;
            PauseMenu.gameObject.SetActive(true);
            PauseButton.enabled = false;
        }
    }

    public void OpenSetting()
    {
        PauseMenu.gameObject.SetActive(false);
        OptionMenu.gameObject.SetActive(true);
    }

    public void CloseSetting()
    {
        PauseMenu.gameObject.SetActive(true);
        OptionMenu.gameObject.SetActive(false);
    }

    public void RestartLevel()
    {
        Application.LoadLevel("GameScene");
    }

    public void NextLevel()
    {
		ShowInterstitial();
        if (GlobalData.gameMode == GlobalData.GameMode.PUZZLE_MODE)
        {
            LevelSelectController.ShowPlayDialog(GlobalData.GetCurrentLevel() + 1);
            GoToHomeScene();
        }
        else if (GlobalData.gameMode == GlobalData.GameMode.ENDLESS_MODE)
        {
            // Continue gameplay
            EndlessWinMenu.SetActive(false);

            gamePlayController.ContinuePlayEndless();
        }
    }

    public void GoToHomeScene()
    {
        GameSceneController.needShowFade = true;

        // Reset load counter
        BranchPairSetup.ResetAllValue();

        if (GlobalData.gameMode == GlobalData.GameMode.PUZZLE_MODE)
        {
            Application.LoadLevel("LevelSelect");
        }
        else if (GlobalData.gameMode == GlobalData.GameMode.ENDLESS_MODE)
        {
            Application.LoadLevel("HomeScene");
        }
    }

}
