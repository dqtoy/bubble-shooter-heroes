using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour
{
    //bg    
    public GameObject TopCloud;
    public GameObject BackButton;
    public float backButtonDisplacement;
    public static float TopCloudHeight = 57.6f;

    public GameObject BackgroundContainer;
    public float maxBg, minBg;

    public GameObject FadeInScene;
    public static float maxCam = 72.0f, minCam = 0; // Camera

    // StarCounter
    public static int StarCounter;
    public static bool FinishedSetup;
    public UnityEngine.UI.Text StarCounterDisplay;
    public static bool needUpdateStarCounterDisplay = false;

    // PlayMenu 
    public GameObject ConfirmPlayMenu, ConfirmPlayPopup;
    public UnityEngine.UI.Text LevelText, HighScoreText;
    public GameObject[] HighestStars;
    static int QueuedLevel = 0, QueuedStar = 0, QueuedHighScore = 0;
    public static bool needShowPlayDialog = false;

    // Moving Squirrel
    public GameObject SquirrelIcon;
    public static GameObject NewButton, LastButton, HighestButton;
    public static int NewUnlockedLevel = -1;
    public static bool needUpdateMoveSquirrelIcon = false;
    public bool FinishedMoveSquirrelIcon = true;
    const float ICON_OFFSET = 0.9f;
    public static bool needPlayFadeIn = false;

    // Loading
    public GameObject LoadingCanvas;
    public Image LoadingBar;

    public static int LastVisit = -1;
    public static Vector3 LastVisitLoc;

    bool FinishedPositioning = false;

    public RectTransform MapCanvas;

    // Use this for initialization
    void Awake()
    {
        // calculate scale
        float screenRate = (float)Screen.height / Screen.width;
        float min = 1.779f, max = 1.333f;

        float additionScale = ((screenRate - max) / (min - max)) * 0.3f;

        MapCanvas.localScale = new Vector3(1.3f - additionScale, 1, 1);

        FinishedPositioning = false;
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

    // Update is called once per frame
    void Update()
    {

        if (!FinishedSetup || LoadingBar.fillAmount < 1)
        {
            float displayAmount = (float)BranchPairSetup.BtnCounter / BranchPairSetup.MAXLEVEL;

            if (LoadingBar.fillAmount < displayAmount)
                LoadingBar.fillAmount = LoadingBar.fillAmount + 0.01f;
        }

        else //if (FinishedSetup)
        {
            if (needPlayFadeIn)
            {
                LoadingCanvas.SetActive(false);
                needPlayFadeIn = false;
                FadeInScene.animation.Play("FadeIn");
            }

            //GameSetting.DisableOutOfBound();
            if (needUpdateMoveSquirrelIcon)
            {
                needUpdateMoveSquirrelIcon = false;

                StartCoroutine(MovingIcon());
            }

            //Back key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!ConfirmPlayMenu.activeSelf)
                {

                    Application.LoadLevel("HomeScene");
                }
                else
                {
                    ConfirmPlayMenu.SetActive(false);
                }
            }

            if (!FinishedPositioning)
            {
                if (NewUnlockedLevel == -1)
                {
                    // Update squirrel icon
                    if (LastVisit == -1)
                    {
                        SquirrelIcon.transform.position = HighestButton.transform.position + new Vector3(0, ICON_OFFSET, 0);
                    }
                    else
                    {
                        SquirrelIcon.transform.position = LastVisitLoc + new Vector3(0, ICON_OFFSET, 0);
                    }

                    float newposy = SquirrelIcon.transform.position.y;
                    if (newposy < minCam)
                    {
                        newposy = minCam;
                    }
                    else if (newposy > maxCam)
                    {
                        newposy = maxCam;
                    }
                    transform.position = new Vector3(transform.position.x, newposy, transform.position.z);
                    BackButton.transform.position = new Vector3(BackButton.transform.position.x, transform.position.y + backButtonDisplacement, BackButton.transform.position.z);
                    FinishedPositioning = true;
                }
                else
                {
                    float newposy = SquirrelIcon.transform.position.y;
                    if (newposy < minCam)
                    {
                        newposy = minCam;
                    }
                    else if (newposy > maxCam)
                    {
                        newposy = maxCam;
                    }
                    transform.position = new Vector3(transform.position.x, newposy, transform.position.z);
                    BackButton.transform.position = new Vector3(BackButton.transform.position.x, transform.position.y + backButtonDisplacement, BackButton.transform.position.z);

                    FinishedPositioning = FinishedMoveSquirrelIcon;

                    if (FinishedPositioning)
                    {
                        NewUnlockedLevel = -1;
                        LastVisit = NewButton.GetComponent<SelectLevel>().ID;
                    }
                }
            }

            if (needUpdateStarCounterDisplay)
            {
                StarCounterDisplay.text = StarCounter + "/" + BranchPairSetup.MAXLEVEL * 3;
                needUpdateStarCounterDisplay = false;
            }

            if (needShowPlayDialog && FinishedSetup && FinishedMoveSquirrelIcon)
            {
                needShowPlayDialog = false;

                LevelText.text = QueuedLevel.ToString("00");
                HighScoreText.text = QueuedHighScore.ToString();
                for (int i = 0; i < 3; i++)
                {
                    HighestStars[i].SetActive(i < QueuedStar);
                }

                ConfirmPlayMenu.SetActive(true);
                ConfirmPlayMenu.transform.position = new Vector3(ConfirmPlayMenu.transform.position.x, Camera.main.transform.position.y, ConfirmPlayMenu.transform.position.z);

                ConfirmPlayPopup.animation.Play("CanvasScaleUp");
            }

            if (!ConfirmPlayMenu.activeSelf)
                UpdateTouchInput();
        }


    }

    IEnumerator MovingIcon()
    {
        SquirrelIcon.transform.position = LastButton.transform.position + new Vector3(0, ICON_OFFSET, 0);

        FinishedMoveSquirrelIcon = false;
        MovingQueue mq = SquirrelIcon.GetComponent<MovingQueue>();
        mq.moveTo(NewButton.transform.position + new Vector3(0, ICON_OFFSET, 0), 1.0f);

        yield return new WaitForSeconds(1.1f);

        FinishedMoveSquirrelIcon = true;
    }


    public Button PlayButton;
    public void PlayGame()
    {
        PlayButton.interactable = false;
        StartCoroutine(PlayGameCorountine());
    }

    IEnumerator PlayGameCorountine()
    {
        FadeInScene.animation.Play("FadeOut");

        yield return new WaitForSeconds(1f);
        GlobalData.gameMode = GlobalData.GameMode.PUZZLE_MODE;
        GlobalData.SetCurrentLevel(QueuedLevel);
        Application.LoadLevel("GameScene");
    }

    public void ClosePlayDialog()
    {
        ConfirmPlayMenu.SetActive(false);
    }

    public static void AddStar(int s)
    {
        StarCounter += s;
        needUpdateStarCounterDisplay = true;
    }

    public static void ShowPlayDialog(int level)
    {
        int targetLevel = level;
        if (targetLevel >= BranchPairSetup.MAXLEVEL)
            targetLevel = BranchPairSetup.MAXLEVEL;

        needShowPlayDialog = true;
        QueuedLevel = targetLevel;

        QueuedStar = Utilities.PlayerPrefs.GetInt("LevelStar-" + QueuedLevel, 0);
        QueuedHighScore = Utilities.PlayerPrefs.GetInt("LevelHighScore-" + QueuedLevel, 0);

    }

    #region Dragging
    Vector3 MouseLastPosition;
    float CamStartPosY, MouseStartPosY;

    void UpdateTouchInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;

            TouchBegan(pos);
            MouseLastPosition = pos;
            return;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 pos = Input.mousePosition;

            if (MouseLastPosition != pos)
            {
                TouchMoved(pos);
                MouseLastPosition = pos;
            }
            return;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TouchEnded(pos);
            return;
        }

        UpdateCamMovement();

        int i = 0;
        while (i < Input.touchCount)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Vector3 pos = Input.GetTouch(i).position;
                TouchBegan(pos);
                return;
            }
            if (Input.GetTouch(i).phase == TouchPhase.Moved)
            {
                Vector3 pos = Input.GetTouch(i).position;
                TouchMoved(pos);
                return;
            }
            if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                TouchEnded(pos);
                return;
            }
            if (Input.GetTouch(i).phase == TouchPhase.Canceled)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                TouchCanceled(pos);
                return;
            }
            ++i;
        }
    }

    float timeDelta;
    float velocity;
    float targetPositionY;
    bool isSwiping = false;
    void UpdateCamMovement()
    {
        if (!isSwiping)
        {
            float moveY = velocity * 0.9f;
            float newposy = transform.position.y + moveY;
            velocity = moveY;

            if (newposy < minCam)
            {
                newposy = minCam;
            }
            else if (newposy > maxCam)
            {
                newposy = maxCam;
            }

            transform.position = new Vector3(transform.position.x, newposy, transform.position.z);
            BackButton.transform.position = new Vector3(BackButton.transform.position.x, transform.position.y + backButtonDisplacement, BackButton.transform.position.z);

            float newBgRate = (newposy - minCam) / (maxCam - minCam);
            BackgroundContainer.transform.position = new Vector3(BackgroundContainer.transform.position.x, newposy + (maxBg - minBg) * newBgRate, BackgroundContainer.transform.position.z);

        }
    }

    public void BackToHomeScene()
    {
        Application.LoadLevel("HomeScene");
    }

    // Handle TouchEvent
    void TouchBegan(Vector3 touch)
    {
        MouseStartPosY = touch.y;
        CamStartPosY = transform.position.y;
        isSwiping = true;
    }

    void TouchMoved(Vector3 touch)
    {
        float newposy = CamStartPosY - (touch.y - MouseStartPosY) / Screen.height * 10;
        if (newposy < minCam)
        {
            newposy = minCam;
        }
        else if (newposy > maxCam)
        {
            newposy = maxCam;
        }

        transform.position = new Vector3(transform.position.x, newposy, transform.position.z);
        BackButton.transform.position = new Vector3(BackButton.transform.position.x, transform.position.y + backButtonDisplacement, BackButton.transform.position.z);

        float newBgRate = (newposy - minCam) / (maxCam - minCam);
        BackgroundContainer.transform.position = new Vector3(BackgroundContainer.transform.position.x, newposy + (maxBg - minBg) * newBgRate, BackgroundContainer.transform.position.z);


    }

    void TouchEnded(Vector3 touch)
    {
        isSwiping = false;

        if (Mathf.Abs(transform.position.y - CamStartPosY) > 1)
            velocity = (transform.position.y - CamStartPosY > 0) ? 0.2f : -0.2f;
        else
            velocity = 0;
    }

    void TouchCanceled(Vector3 touch)
    {

    }
    #endregion
}
