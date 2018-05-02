using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour
{

    // Global
    public int _FlyingSpeedDiv = 7;
    public int CurrentScore = 0;
    public int EndlessLastScore, EndlessTargetScore;
    public int CurrentSquirrel = 0;
    public int ComboCount = 0;

    public int RequiredSquirrel = 1;
    public int RemainBubbleShoot = 69;

    public int[] StarTargets = new int[] { 0, 0, 0 };
    public GameObject[] StarTargetFlags;
    public GameObject[] StarDisplay;

    // base bubble
    public GameObject BubblePrefab;
    // Positioning 
    public Transform LoadedPosition, WaitingPosition;
    public static float LeftWall = -2.24f, RightWall = 2.20f, TopWall = 4, BotWall = -4, BotLimit = -0.2f, TopLimit = 3.0f;
    public float LowestBubblePos;
    // Objects
    Bubble _loadedBubble = null, _waitingBubble = null;

    //prefabs 
    public GameObject ModePuzzlePrefab, ModeEndlessPrefab;
    public IBubbleBoard _bubbleBoard;

    public GameObject PlayerSquirrel;
    public Collider2D touchableArea, swapBubbleArea;

    public GameObject SplitBar; // endless only

    // Arrow
    public ArrowController _shootArrow;
    public bool _usingArrow, _displayingArrow;
    bool canShowArrow;
    public bool needUpdateArrowPosition = true;

    public List<Bubble> _flyingBubbles;
    // input
    Vector3 MouseLastPosition;

    // global
    public bool gameStarted, gameEnded, gamePaused, allLoadingDone;
    public bool needUpdateStar = true, needUpdateBullet = false, needUpdateHUD = true;
    // HUD
    public GameObject BubbleRemainBoard, SquirrelCounter, HUDBGEndless;
    public RectTransform RECT_ScoreBoard, RECT_StarBar, RECT_StarRealBar;
    public Sprite longStarBarSprite;
    public UnityEngine.UI.Text txtCounter, txtScore, txtWinScore, txtEndlessWinScore, txtWinHighScore;
    public UnityEngine.UI.Text[] txtLevelLabels;
    public TextMesh txtRemain;

    // Scene
    GameSceneController gameSceneController;

    void Start()
    {

        gameSceneController = GameObject.FindObjectOfType<GameSceneController>();

        // BGM
        if (BaseManager.globalMenuMusic != null)
        {
            StartCoroutine(AudioHelper.FadeAudioObject(BaseManager.globalMenuMusic, -1f));
        }
        if (BaseManager.globalGameMusic == null)
        {
            // create and return the Intro Scene music audio
            BaseManager.globalGameMusic = AudioHelper.CreateGetFadeAudioObject(BaseManager.GetInstance().gameMusic, true, BaseManager.GetInstance().fadeClip, "Audio-GameMusic");
            // play the clip
            StartCoroutine(AudioHelper.FadeAudioObject(BaseManager.globalGameMusic, 0.5f));
        }
        // Load data (if not done yet)
        GlobalData.LoadLevelData();


        if (GlobalData.gameMode == GlobalData.GameMode.PUZZLE_MODE)
        {
            // create board
            GameObject board = Instantiate(ModePuzzlePrefab) as GameObject;
            _bubbleBoard = board.GetComponent<IBubbleBoard>();

            // Set Data
            RequiredSquirrel = GlobalData.LevelRequire[GlobalData.GetCurrentLevel()];
            RemainBubbleShoot = GlobalData.StarTarget[GlobalData.GetCurrentLevel()][0];

            if (GlobalData.GetCurrentLevel() >= 40)
            {
                RemainBubbleShoot += 15;

            }

            StarTargets[0] = GlobalData.StarTarget[GlobalData.GetCurrentLevel()][1];
            StarTargets[1] = GlobalData.StarTarget[GlobalData.GetCurrentLevel()][2];
            StarTargets[2] = GlobalData.StarTarget[GlobalData.GetCurrentLevel()][3];

            float maxPoint = StarTargets[2] + StarTargets[2] * 15 / 100;
            for (int i = 0; i < 3; i++)
            {
                RectTransform tf = StarTargetFlags[i].GetComponent<RectTransform>();
                tf.anchoredPosition = new Vector2(StarTargets[i] / maxPoint * 266f, tf.anchoredPosition.y);
            }


            // Levelselect switch
            LevelSelectController.NewUnlockedLevel = -1;
            LevelSelectController.needUpdateMoveSquirrelIcon = false;
        }
        else if (GlobalData.gameMode == GlobalData.GameMode.ENDLESS_MODE)
        {
            LittleSquirrelController.publicList.Clear();

            // create board
            GameObject board = Instantiate(ModeEndlessPrefab) as GameObject;
            _bubbleBoard = board.GetComponent<IBubbleBoard>();

            // Set Data
            RequiredSquirrel = -1;
            RemainBubbleShoot = -1;

            BubbleRemainBoard.SetActive(false);
            SquirrelCounter.SetActive(false);
            HUDBGEndless.SetActive(true);
            SplitBar.SetActive(true);

            RECT_ScoreBoard.anchoredPosition = new Vector2(-134, -16);
            RECT_StarBar.anchoredPosition = new Vector2(-122, -52);
            RECT_StarRealBar.sizeDelta = new Vector2(470, 12.8f);
            RECT_StarRealBar.GetComponent<Image>().sprite = longStarBarSprite;

            for (int i = 0; i < 3; i++)
            {
                StarTargetFlags[i].SetActive(false);
            }

            EndlessLastScore = 0;
            EndlessTargetScore = GlobalData.ENDLESS_TARGET_EACHLEVEL;
        }

        // InitHUD

        foreach (UnityEngine.UI.Text txt in txtLevelLabels)
        {
            txt.text = GlobalData.GetCurrentLevel().ToString();
        }

        // Finish InitHUD


        UpdateHUD();

        gameStarted = false;
        gameEnded = false;
        allLoadingDone = false;
        _bubbleBoard.InitBubbleBoard();

        InitBubbles();
        _usingArrow = false;

        _shootArrow.CreateDots();
        _shootArrow.UpdateArrow();
    }

    // for endless mode only
    public void ContinuePlayEndless()
    {
        gameEnded = false;
        gamePaused = false;

        gameSceneController.PauseButton.interactable = true;

        EndlessTargetScore = EndlessLastScore + GlobalData.ENDLESS_TARGET_EACHLEVEL;
        GlobalData.SetCurrentLevel(GlobalData.GetCurrentLevel() + 1);

        foreach (UnityEngine.UI.Text txt in txtLevelLabels)
        {
            txt.text = GlobalData.GetCurrentLevel().ToString();
        }

        if (GlobalData.GetCurrentLevel() > 1 && GlobalData.GetCurrentLevel() % GlobalData.LEVELUP_RATE == 0)
        {
            _bubbleBoard.increaseBubbleType();
        }
    }

    void InitBubbles()
    {
        CreateBubbleBullet();
    }

    void CreateBubbleBullet(int id = 0)
    {
        if (RemainBubbleShoot != 0)
        {
            int color = _bubbleBoard.GetRandomColor();
            if (id != 0)
                color = id;

            if (color != -1)
            {
                _waitingBubble = (Instantiate(BubblePrefab, WaitingPosition.position + new Vector3(0, -1f, 0), WaitingPosition.rotation) as GameObject).GetComponent("Bubble") as Bubble;
                _waitingBubble.MotionDelegate = onBubbleMove;
                _waitingBubble.SetBubbleColor(color);
                _waitingBubble.gameObject.tag = "flyingbubble";
                _waitingBubble.isMoving = true;
                _waitingBubble.moveTo(WaitingPosition.position, 0.2f);

                RemainBubbleShoot--;
            }
            else
            {
                _waitingBubble = null;
            }
        }
    }

    void LoadBubbleBullet()
    {
        if (_waitingBubble == null) return;

        _loadedBubble = _waitingBubble;

        _loadedBubble.moveTo(LoadedPosition.position, 0.3f, new Vector3(0, 2.5f, 0));

        _waitingBubble = null;
        CreateBubbleBullet();

        _shootArrow.UpdateDotColor(_loadedBubble.ID);
    }

    // Update is called once per frame
    void Update()
    {
        // Update Logic
        if (gameStarted && !gameEnded && !gamePaused)
        {
            if (_waitingBubble == null)
            {
                CreateBubbleBullet();
            }
            if (_loadedBubble == null && _waitingBubble != null)
            {
                if (_waitingBubble.isMoving == false)
                {
                    LoadBubbleBullet();
                }
            }

            UpdateFlyingBubbles();

            if (needUpdateBullet) UpdateBulletBubbles();

            UpdateTouchInput();
            UpdateArrow();

        }

        // Update Loading and finish
        else if (_bubbleBoard.isDoneLoading() && !allLoadingDone)
        {
            StartCoroutine(StartTheGamePhase1());
        }

        else if (allLoadingDone && startGamePhase1Ended == false)
        {
            bool runNextPhase = false;
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (touchableArea.collider2D == Physics2D.OverlapPoint(pos))
                {
                    runNextPhase = true;
                }
            }
            int i = 0;
            while (i < Input.touchCount)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                    if (touchableArea.collider2D == Physics2D.OverlapPoint(pos))
                    {
                        runNextPhase = true;
                    }
                    break;
                }
                ++i;
            }
            if (runNextPhase)
            {
                startGamePhase1Ended = true;
                StopAllCoroutines();
                StartCoroutine(StartTheGamePhase2());
            }
        }

        // Update execution
        if (KillList.Count > 0 || DropList.Count > 0)
        {
            ExecuteBubbleLists();
        }

        UpdateHUD();

        // Key back
        if (gameStarted && !gameEnded)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameSceneController.PauseGame();
            }
    }

    public GameObject ReadyGoText;
    bool startGamePhase1Ended = false;
    float camMoveTime = 0;
    IEnumerator StartTheGamePhase1()
    {
        allLoadingDone = true;
        yield return new WaitForSeconds(0.5f);

        // MOVE THE CAM
        MovingQueue mq = Camera.main.GetComponent<MovingQueue>();
        camMoveTime = _bubbleBoard.getMapData().MapSizeY * 0.15f;
        mq.moveTo(new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z), camMoveTime);

        yield return new WaitForSeconds(camMoveTime);
        startGamePhase1Ended = true;
        StartCoroutine(StartTheGamePhase2());
    }

    IEnumerator StartTheGamePhase2()
    {
        MovingQueue mq = Camera.main.GetComponent<MovingQueue>();
        mq.StopAllCoroutines();
        float newMoveTime = (camMoveTime - mq.TimeMoved) / 2.5f;
        mq.moveTo(new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z), newMoveTime);
        yield return new WaitForSeconds(newMoveTime);
        // READY-GO
        ReadyGoText.animation.Play("ReadyGoAppear");
        AudioHelper.CreatePlayAudioObject(BaseManager.GetInstance().sfxReadyGo);
        // =========
        yield return new WaitForSeconds(0.5f);
        gameStarted = true;
        CreateBubbleBullet();
    }

    void UpdateFlyingBubbles()
    {
        foreach (Bubble b in _flyingBubbles)
        {
            b.UpdateMove();
        }
    }

    void UpdateTouchInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            TouchBegan(pos);
            MouseLastPosition = pos;
            return;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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

        int i = 0;
        while (i < Input.touchCount)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                TouchBegan(pos);
                return;
            }
            if (Input.GetTouch(i).phase == TouchPhase.Moved)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
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

    public void UpdateHUD()
    {
        if (GlobalData.gameMode == GlobalData.GameMode.PUZZLE_MODE)
            txtCounter.text = CurrentSquirrel + "/" + RequiredSquirrel;
        else if (GlobalData.gameMode == GlobalData.GameMode.ENDLESS_MODE)
            txtCounter.text = CurrentSquirrel.ToString();

        txtScore.text = CurrentScore.ToString();

        int realRemain = RemainBubbleShoot + (_waitingBubble != null ? 1 : 0) + (_loadedBubble != null ? 1 : 0);
        txtRemain.text = realRemain.ToString();

        if (needUpdateStar)
        {
            StarBarProcess();
            needUpdateStar = false;
        }
    }

    public UnityEngine.UI.Image Starbar;
    void StarBarProcess()
    {
        float scale = 0;
        if (GlobalData.gameMode == GlobalData.GameMode.PUZZLE_MODE)
            scale = (float)CurrentScore / (StarTargets[2] + StarTargets[2] * 10 / 100);
        else if (GlobalData.gameMode == GlobalData.GameMode.ENDLESS_MODE)
            scale = (float)(CurrentScore - EndlessLastScore) / (EndlessTargetScore - EndlessLastScore);

        if (scale > 1) scale = 1;

        Starbar.fillAmount = scale;

    }

    public void GiveComboReward(bool drop = false, int size = 1)
    {
        if (!drop)
        {
            if (ComboCount <= 14)
                CurrentScore += GlobalData.PointReward[ComboCount, 1];
            else
            {
                CurrentScore += GlobalData.PointReward[14, 1];
                CurrentScore += (ComboCount - 14) * 2000;
            }
        }
        else
        {
            if (size <= 14)
                CurrentScore += GlobalData.PointReward[size, 2] * size;
            else
            {
                CurrentScore += GlobalData.PointReward[14, 2] * 14;
                CurrentScore += (size - 14) * 1000;
            }
        }

        needUpdateStar = true;
        needUpdateHUD = true;
    }

    public void GiveBubblePoint(int p = 10)
    {
        CurrentScore += p;

        needUpdateStar = true;
        needUpdateHUD = true;
    }

    public void IncreaseSquirrel()
    {
        CurrentSquirrel++;
        needUpdateHUD = true;
        if (CurrentSquirrel >= RequiredSquirrel && GlobalData.gameMode == GlobalData.GameMode.PUZZLE_MODE)
        {
            // Send message to drop everything
            _bubbleBoard.DropEveryThing();
        }
    }

    public void SetComboCount(int inc)
    {
        ComboCount = inc;

        // Change shoot bubble
        if (ComboCount > 0 && ComboCount % 6 == 0)
        {
            if (_loadedBubble != null)
            {
                _loadedBubble.SetBubbleColor(Random.Range(0, 2) == 0 ? 19 : 20);
            }
        }
    }

    void UpdateArrow()
    {
        if (_displayingArrow)
        {
            if (needUpdateArrowPosition)
            {
                needUpdateArrowPosition = false;
                _shootArrow.UpdateArrow();
            }
            _shootArrow.gameObject.SetActive(true);
        }
        else
        {
            _shootArrow.gameObject.SetActive(false);
        }
    }

    void UpdateBulletBubbles()
    {
        if (_bubbleBoard.getBubbleList().Count <= 0) return;
        // waiting
        if (_waitingBubble != null)
        {

            bool waitingOk = false;
            if (_waitingBubble.ID == 19 || _waitingBubble.ID == 20) waitingOk = true;
            else foreach (Bubble b in _bubbleBoard.getBubbleList())
                {
                    if (_waitingBubble.ID == b.ID)
                    {
                        waitingOk = true;
                        break;
                    }
                }

            if (!waitingOk) _waitingBubble.SetBubbleColor(_bubbleBoard.GetRandomColor());
        }
        // loaded
        if (_loadedBubble != null)
        {

            bool loadedOk = false;
            if (_loadedBubble.ID == 19 || _loadedBubble.ID == 20) loadedOk = true;
            else foreach (Bubble b in _bubbleBoard.getBubbleList())
                {
                    if (_loadedBubble.ID == b.ID)
                    {
                        loadedOk = true;
                        break;
                    }
                }

            if (!loadedOk) _loadedBubble.SetBubbleColor(_bubbleBoard.GetRandomColor());
        }
        needUpdateBullet = false;

        _shootArrow.UpdateDotColor(_loadedBubble.ID);
    }

    // Handle TouchEvent
    void TouchBegan(Vector3 touch)
    {

        if (touchableArea.collider2D == Physics2D.OverlapPoint(touch))
        {
            _displayingArrow = true;
            var dir = touch - LoadedPosition.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _shootArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            _usingArrow = true;

            _shootArrow.UpdateArrow();
            needUpdateArrowPosition = true;
        }
        else if (swapBubbleArea.collider2D == Physics2D.OverlapPoint(touch))
        {
            // Check if can swap gem
            if (_waitingBubble != null && _loadedBubble != null)
                if (!_waitingBubble.isMoving && !_loadedBubble.isMoving)
                {
                    Bubble t = _waitingBubble;
                    _waitingBubble = _loadedBubble;
                    _loadedBubble = t;

                    _waitingBubble.moveTo(WaitingPosition.transform.position, 0.3f, new Vector3(0, 2.5f, 0));
                    _loadedBubble.moveTo(LoadedPosition.transform.position, 0.3f, new Vector3(0, 2.5f, 0));

                    AudioHelper.CreatePlayAudioObject(BaseManager.GetInstance().sfxSwap);

                    _shootArrow.UpdateDotColor(_loadedBubble.ID);
                }
        }


    }

    void TouchMoved(Vector3 touch)
    {
        if (_usingArrow)
        {
            if (touchableArea.collider2D == Physics2D.OverlapPoint(touch))
            {
                _displayingArrow = true;
                var dir = touch - LoadedPosition.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _shootArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                needUpdateArrowPosition = true;
            }
            else
            {
                _displayingArrow = false;
            }
        }
    }

    void TouchEnded(Vector3 touch)
    {
        _displayingArrow = false;
        if (_loadedBubble == null || _loadedBubble.isMoving || !_usingArrow)
        {

            return;
        }
        else if (touchableArea.collider2D == Physics2D.OverlapPoint(touch))
        {

            ShootBubble(touch);
        }
        else
        {

        }
        _usingArrow = false;


    }

    void TouchCanceled(Vector3 touch)
    {

    }

    // Processing
    void ShootBubble(Vector3 pos)
    {
        var dir = pos - LoadedPosition.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _shootArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float diffx, diffy;
        float rangle = (-angle + 90) * Mathf.PI / 180;
        diffx = Mathf.Sin(rangle);
        diffy = Mathf.Cos(rangle);
        Vector2 velocity = new Vector2(diffx / _FlyingSpeedDiv, diffy / _FlyingSpeedDiv);

        _loadedBubble.velocity = velocity;
        _loadedBubble.isMoving = true;

        _flyingBubbles.Add(_loadedBubble);

        _loadedBubble = null;

        // Animation
        Animator animator = PlayerSquirrel.GetComponent<Animator>();
        animator.Play("Player-Throw");
        AudioHelper.CreatePlayAudioObject(BaseManager.GetInstance().sfxThrow);


    }

    public Transform subTextRoot;
    public GameObject subBubbleTextPrefab;
    public void ReduceShootTrigger()
    {
        GameObject subt = Instantiate(subBubbleTextPrefab) as GameObject;
        subt.transform.SetParent(subTextRoot, false);

        RemainBubbleShoot -= 1;
        if (RemainBubbleShoot < 0) RemainBubbleShoot = 0;

        UpdateHUD();
    }

    public bool HasAnyChance()
    {
        return (_flyingBubbles.Count > 0 || _loadedBubble != null || _waitingBubble != null);
    }

    // Delegate
    int onBubbleMove(Vector3 position, Bubble b)
    {
        // type : 0 = Normal, 1 = excess LeftLimit, 2 = excess RightLimit , 3 =  excess TopLimit, 4 = excess BottomLimit
        if (position.y > TopWall - Bubble.BUBBLE_RADIUS / 2)
            return 3;
        if (position.y < BotWall + Bubble.BUBBLE_RADIUS / 2)
        {
            // Kill that Bitch here
            _flyingBubbles.Remove(b);
            return 4;
        }

        if (position.x < LeftWall + Bubble.BUBBLE_RADIUS / 2)
            return 1;
        if (position.x > RightWall - Bubble.BUBBLE_RADIUS / 2)
            return 2;


        return 0;
    }

    public GameObject FireworkPrefab;

    public void AnnounceVictory()
    {
        if (GlobalData.gameMode == GlobalData.GameMode.PUZZLE_MODE)
        {
            gameEnded = true;
            gameSceneController.PauseButton.interactable = false;
            StartCoroutine(StartVictorySequence());
            StartCoroutine(SpitFirework(3.1f));
        }
        else if (GlobalData.gameMode == GlobalData.GameMode.ENDLESS_MODE)
        {

            gamePaused = true;
            gameEnded = true;
            gameSceneController.PauseButton.interactable = false;
            StartCoroutine(StartEndlessVictorySequence());
            StartCoroutine(SpitFirework(3.1f, 20));
        }


    }

    IEnumerator SpitFirework(float waitDelay, int times = -1)
    {
        yield return new WaitForSeconds(waitDelay);
        while (gameEnded)
        {
            GameObject fw = Instantiate(FireworkPrefab) as GameObject;
            fw.transform.position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), -5);
            Animator anitor = fw.GetComponent<Animator>();
            anitor.Play("Firework");
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator StartVictorySequence()
    {
        // Hide All flying bubble
        if (_flyingBubbles.Count > 0)
        {
            foreach (Bubble b in _flyingBubbles)
            {
                b.gameObject.SetActive(false);
                RemainBubbleShoot++;
            }
        }
        _flyingBubbles.Clear();

        // Hide Bullets
        if (_loadedBubble != null)
        {
            _loadedBubble.gameObject.SetActive(false);
            _loadedBubble = null;
            RemainBubbleShoot++;
        }
        if (_waitingBubble != null)
        {
            _waitingBubble.gameObject.SetActive(false);
            _waitingBubble = null;
            RemainBubbleShoot++;
        }
        if (needUpdateHUD)
        {
            UpdateHUD();
            needUpdateHUD = false;
        }

        yield return new WaitForSeconds(3f);

        // Shoot remain bubbles Up
        int pointEachBubble = GlobalData.PointReward[RemainBubbleShoot > 14 ? 14 : RemainBubbleShoot, 2];
        while (RemainBubbleShoot > 0)
        {
            Bubble b = (Instantiate(BubblePrefab, WaitingPosition.position, WaitingPosition.rotation) as GameObject).GetComponent("Bubble") as Bubble;
            b.SetBubbleColor(Random.Range(1, 10));

            StartCoroutine(b.shootUp(pointEachBubble));

            RemainBubbleShoot--;
            //UpdateHUD();
            yield return new WaitForSeconds(0.1f);
        }
        //

        // Play squirrel win animation
        Animator animator = PlayerSquirrel.GetComponent<Animator>();
        animator.Play("Victory");
        AudioHelper.CreatePlayAudioObject(BaseManager.GetInstance().sfxVictory);

        yield return new WaitForSeconds(2f);

        // Result calculate
        int highScore = Utilities.PlayerPrefs.GetInt("LevelHighScore-" + GlobalData.GetCurrentLevel(), 0);
        if (CurrentScore > highScore)
        {
            highScore = CurrentScore;
        }

        int star = 0;
        if (CurrentScore >= StarTargets[2])
        {
            star = 3;
        }
        else if (CurrentScore >= StarTargets[1])
        {
            star = 2;
        }
        else
        {
            star = 1;
        }

        int highStar = Utilities.PlayerPrefs.GetInt("LevelStar-" + GlobalData.GetCurrentLevel(), 0);
        if (star > highStar)
        {
            highStar = star;
        }

        int nextLevel = GlobalData.GetCurrentLevel() + 1;
        // Update Preference
        Utilities.PlayerPrefs.SetInt("LevelHighScore-" + GlobalData.GetCurrentLevel(), highScore);
        Utilities.PlayerPrefs.SetInt("LevelStar-" + GlobalData.GetCurrentLevel(), highStar);
        // Set icon switch\
        if (nextLevel <= BranchPairSetup.MAXLEVEL)
        {
            Utilities.PlayerPrefs.SetBool("LevelUnlocked-" + (GlobalData.GetCurrentLevel() + 1), true);

            LevelSelectController.NewUnlockedLevel = GlobalData.GetCurrentLevel() + 1;
            LevelSelectController.needUpdateMoveSquirrelIcon = true;
        }

        Utilities.PlayerPrefs.Flush();



        // Update Text & Star
        txtWinScore.text = CurrentScore.ToString();
        txtWinHighScore.text = highScore.ToString();

        for (int i = 0; i < star; i++)
        {
            StarDisplay[i].SetActive(true);

        }
        gameSceneController.WinMenu.gameObject.SetActive(true);

        for (int i = 0; i < star; i++)
        {
            MovingQueue moac = StarDisplay[i].GetComponent<MovingQueue>();
            moac.moveToLocal(new Vector3(0, 0, 0), .5f, new Vector3((i - 1) * 300, 0, 0));
            yield return new WaitForSeconds(.5f);
        }
    }

    IEnumerator StartEndlessVictorySequence()
    {
        // Update score
        EndlessLastScore = CurrentScore;

        // Result calculate
        int highScore = Utilities.PlayerPrefs.GetInt("EndlessHighScore", 0);
        if (CurrentScore > highScore)
        {
            highScore = CurrentScore;
        }

        Utilities.PlayerPrefs.SetInt("EndlessHighScore", highScore);

        // Update Text & Star
        txtEndlessWinScore.text = CurrentScore.ToString();
        //txtWinHighScore.text = highScore.ToString();

        // Shoot random
        for (int i = 0; i < 10; i++)
        {
            Bubble b = (Instantiate(BubblePrefab, LoadedPosition.position, LoadedPosition.rotation) as GameObject).GetComponent("Bubble") as Bubble;
            b.SetBubbleColor(Random.Range(1, 10));

            StartCoroutine(b.shootUp(0));

            yield return new WaitForSeconds(0.1f);
        }

        // Play squirrel win animation
        Animator animator = PlayerSquirrel.GetComponent<Animator>();
        animator.Play("Victory");
        AudioHelper.CreatePlayAudioObject(BaseManager.GetInstance().sfxVictory);

        yield return new WaitForSeconds(2.0f);

        gameSceneController.EndlessWinMenu.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);
    }

    public void AnnounceDefeated()
    {
        gameEnded = true;

        gameSceneController.PauseButton.interactable = false;

        StartCoroutine(StartDefeatSequence());
    }

    IEnumerator StartDefeatSequence()
    {
        // Drop All remain thing if on endless mode
        if (GlobalData.gameMode == GlobalData.GameMode.ENDLESS_MODE)
            _bubbleBoard.DropEveryThing();


        yield return new WaitForSeconds(2f);

        UpdateHUD();

        AudioHelper.CreatePlayAudioObject(BaseManager.GetInstance().sfxFailed);


        yield return new WaitForSeconds(0.5f);

        gameSceneController.LoseMenu.gameObject.SetActive(true);
    }

    #region IndependentKiller
    public List<Bubble> KillList = new List<Bubble>();
    public List<Bubble> DropList = new List<Bubble>();

    public void AddBubbleToKillList(Bubble b)
    {
        if (!KillList.Contains(b) && !DropList.Contains(b))
            KillList.Add(b);
    }

    public void AddBubbleToDropList(Bubble b)
    {
        if (!KillList.Contains(b) && !DropList.Contains(b))
            DropList.Add(b);
    }

    public void ExecuteBubbleLists()
    {
        // kill list
        foreach (Bubble b in KillList)
        {
            StartCoroutine(b.kill());
        }

        KillList.Clear(); // finish

        // drop list
        foreach (Bubble b in DropList)
        {
            StartCoroutine(b.drop());
        }

        DropList.Clear(); // finish

    }

    #endregion

}
