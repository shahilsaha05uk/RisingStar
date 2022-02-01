using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum DifficultyLevel { EASY, MEDIUM, HARD };
public class Game : MonoBehaviour
{
    private Board _board;
    private UI _ui;
    private double _gameStartTime;
    public bool _gameInProgress;
    public bool _challengerMode;
    public double _countDownTimer;
    public double defaultCountDownTimer;
    public double currentTime;
    public GameManager manager;

    public int maxDangers;
    public int minDangers;
    public int level_Height;
    public int level_Width;
    public int level;
    public bool LevelStatus { get; set; }
    public DifficultyLevel difficultyLevel;
    public bool buttonTobeReveal;


    public void SetDefault()
    {
        _countDownTimer = defaultCountDownTimer;
    }

    #region Button Click

    // Main menu
    public void OnClickedNewGame()
    {
        _ui.ShowDifficulty();
        _ui.HideMenu();
    }
    public void OnCLickChallenger()
    
    {
        level = 1;
        level_Height = 5;
        level_Width = 5;
        minDangers = 0;
        maxDangers = 2;
        manager.ResetValues();

        ChallengerMode(minDangers, maxDangers, level_Width, level_Height, level);
        _ui.ResetButton.gameObject.SetActive(false);
        _ui.NextLevelButton.gameObject.SetActive(false);
    }
    public void OnClickSettings()
    {
        _ui.ShowSettings();
        _ui.HideMenu();
    }
    public void OnClickedExit()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

    //Result menu

    public void OnClickResume()
    {
        _ui.HidePause();
        manager.ResumeGame();
    }
    public void OnClickExit()   //During GamePlay
    {
        if (_board != null)
        {
            _ui.HideBoard();
            _board.Clear();
        }

        if (_ui != null)
        {
            manager.ResetValues();
            _ui.HidePause();
            _ui.HideResult();
            _ui.HideStatus();
            _ui.ShowMenu();
            manager.ResumeGame();
        }
    }

    //Settings menu
    public void OnClickBack()
    {
        _ui.HideSettings();
        manager.ResetValues();

        _ui.ShowMenu();
    }
    public void OnShowControlsChecked()
    {
        if (!_ui.ControlsList.activeInHierarchy)
        {
            _ui.ControlsList.SetActive(true);
        }
        else
        {
            _ui.ControlsList.SetActive(false);
        }
    }

    // Result menu
    public void OnClickedReturnBackToMainMenu()
    {
        if (_board != null)
        {
            _board.Clear();
        }

        if (_ui != null)
        {
            _ui.HideStatus();
            _ui.HideResult();
            _ui.HideBoard();
            
            _ui.ShowMenu();
            
            manager.ResetValues();

        }
    }
    public void OnPlayAgainButtonClicked()
    {
        GameManager.flagCount = manager.defaultFlags;
        _countDownTimer = manager.defaultCountdownTimer;
        _ui.HideResult();

        if (_challengerMode)
        {
            ChallengerMode(minDangers, minDangers, level_Width, level_Height, level);
        }
        else
        {
            DifficultyLevelLoader();
            StartCoroutine(UpdateNormalTimer());
        }
    }
    public void OnNextLevelButtonClick()
    {
        if (_challengerMode)
        {
            if (LevelStatus == true)
            {
                if (level < 3)
                {
                    LevelStatus = false;
                    
                    level++;
                    level_Height++;
                    level_Width++;
                    minDangers++;
                    maxDangers++;

                    _ui.HideResult();
                    _board.DestroyAllBoxes();
                    _board.GenerateBoxes(level_Width, level_Height);
                    ChallengerMode(minDangers, minDangers, level_Width, level_Height, level);

                }
            }
        }

    }

    // Difficulty menu
    public void OnEasyClick()
    {
        difficultyLevel = DifficultyLevel.EASY;
        _board.NumberOfDangerousBoxes = _board.easy_DangerousBox;
        LoadGame(5,5);
        StartCoroutine(UpdateNormalTimer());
    }
    public void OnMediumClick()
    {
        difficultyLevel = DifficultyLevel.MEDIUM;
        _board.NumberOfDangerousBoxes = _board.medium_DangerousBox;
        LoadGame(8,8);
        StartCoroutine(UpdateNormalTimer());
    }
    public void OnHardClick()
    {
        difficultyLevel = DifficultyLevel.HARD;
        _board.NumberOfDangerousBoxes = _board.hard_DangerousBox;
        LoadGame(10,10);
        StartCoroutine(UpdateNormalTimer());
    }

    public void DifficultyLevelLoader()
    {
        switch (difficultyLevel)
        {
            case DifficultyLevel.EASY:
                OnEasyClick();
                break;
            case DifficultyLevel.MEDIUM:
                OnMediumClick();
                break;
            case DifficultyLevel.HARD:
                OnHardClick();
                break;
        }
    }
    /*----------------------------------------------------------*/
    #endregion

    //Game play
    public void LoadGame(int width, int height)
    {
        _ui.HideDifficulty();

            GameManager.firstClick = true;
        if (_board.transform.childCount == 0)
        {
            _board.GenerateBoxes(width, height);
            _board.Setup(BoardEvent);
            _board.RechargeBoxes();
        }
        else
        {
            _board.Setup(BoardEvent);
            _board.RechargeBoxes();
        }

        if (_ui != null)
        {
            _ui.HideMenu();
            _ui.ShowGame();
        }

        manager.SoundPlayer(manager.backgroundSound, AudioMode.STOP);

    }
    public void ChallengerMode(int danger_Min, int danger_Max, int width, int height, int level)
    {
        // Load the boxes, and the player needs to finish the game within the given time frame
        _board.NumberOfDangerousBoxes = UnityEngine.Random.Range(danger_Min, danger_Max);
        _challengerMode = true;
        this.level = level;

        LoadGame(width, height);
        _ui.UpdateTimer(_countDownTimer);
        currentTime = 0;

        StartCoroutine(UpdateChallengerCountDownTimer());

    }

    private void BoardEvent(Board.Event eventType)
    {
        if(eventType == Board.Event.ClickedDanger && _ui != null)
        {
            _ui.ShowResult(success: false);
            _board.FlagDestroyer(Board.FlagDestroyEvents.DESTROYALL);
            _board.RevealAllDangers();
            Debug.Log("You clicked Danger");
        }

        if (eventType == Board.Event.Win && _ui != null && level!=2)
        {
            _board.FlagDestroyer(Board.FlagDestroyEvents.DESTROYALL);

            _ui.HideGame();
            _ui.ShowResult(success: true);
        }

        if (!_gameInProgress)
        {
            _gameInProgress = true;
            _gameStartTime = Time.deltaTime;
        }
    }

    //Function to check if all the buttons are clicked

    
    private bool ButtonsLeftToBeRevealed()
    {
        bool b = false;
        Debug.Log("Button Left to be revealed Function");
        bool arrayExistCheck = Array.Exists(_board._grid, x => x != null);

        if (arrayExistCheck)
        {
            foreach (var item in _board._grid)
            {
                if (!item.IsDangerous && item.IsActive)
                {
                    Debug.Log("Found a dangerous box");
                    b = true;
                }
            }
        }
        return b;
    }
    public void ChallengeGameStatusChecker()
    {
        buttonTobeReveal = ButtonsLeftToBeRevealed();


        if (_gameInProgress == true && !buttonTobeReveal)
        {
            Debug.Log("You won the game");
            LevelStatus = true;
            _gameInProgress = false;
            _board.FlagDestroyer(Board.FlagDestroyEvents.DESTROYALL);
            ResetTimer();
            manager.ResetValues();

            if (level == 2)
            {
                StartCoroutine(WaitUntilStatusDisabled());
            }
            else
            {
                _ui.ShowResult(true);

                _ui.ResetButton.gameObject.SetActive(false);
                _ui.NextLevelButton.gameObject.SetActive(true);

                _ui.NextLevelButton.GetComponent<LayoutElement>().ignoreLayout = false;
                _ui.ResetButton.GetComponent<LayoutElement>().ignoreLayout = true;

            }

        }

        else if (!_gameInProgress && buttonTobeReveal)
        {
            Debug.Log("You Lost the game");
            LevelStatus = false;

            _gameInProgress = false;
            _board.FlagDestroyer(Board.FlagDestroyEvents.DESTROYALL);
            currentTime = 0;

            _ui.UpdateTimer(currentTime);
            _ui.ShowResult(false);
            ResetTimer();
            manager.ResetValues();

            _ui.ResetButton.gameObject.SetActive(true);
            _ui.NextLevelButton.gameObject.SetActive(false);

            _ui.NextLevelButton.GetComponent<LayoutElement>().ignoreLayout = true;
            _ui.ResetButton.GetComponent<LayoutElement>().ignoreLayout = false;

        }
    }

    //Timer Methods
    public IEnumerator UpdateChallengerCountDownTimer()
    {
        ResetTimer();
        manager.ResetValues();

        while (GameManager.firstClick != false)
        {
            yield return null;
        }

        if (GameManager.firstClick == false)
        {
            float elapsedTime = 0;
            _gameInProgress = true;

            currentTime = _countDownTimer;
            Debug.Log("Challenger Timer started");
            while (currentTime >= 0)
            {
                Debug.Log("Challenger working");

                elapsedTime += Time.deltaTime;
                if (_ui != null && _challengerMode)
                {
                    currentTime = _countDownTimer - elapsedTime;
                    _ui.UpdateTimer(_gameInProgress ? currentTime : 0.0);
                }
                ChallengeGameStatusChecker();


                if (currentTime <= 0 || _ui.ResetCanvasStatus == true)
                {
                    Debug.Log("Update Challenge enum break");
                    _gameInProgress = false;
                    break;
                }


                yield return null;
            }
            ChallengeGameStatusChecker();
        }
    }
    public IEnumerator UpdateNormalTimer()
    {
        ResetTimer();
        while (GameManager.firstClick != false)
        {
            yield return null;
        }


        if (GameManager.firstClick == false)
        {
            float elapsedTime = 0;
            while (true)
            {
                elapsedTime += Time.deltaTime;
                if (_ui != null && _ui.ResetCanvasStatus == false)
                {
                    Debug.Log("Not in the challenger mode");
                    _ui.UpdateTimer(_gameInProgress ? elapsedTime - _gameStartTime : 0.0);
                }
                else if(_ui != null && _ui.ResetCanvasStatus == true)
                {
                    Debug.Log("Main game exit");
                    yield break;
                }

                yield return null;
            }
        }
    }
    public IEnumerator WaitUntilStatusDisabled()
    {
        Debug.Log("Level Complete");

        SetDefault();
        _board.DestroyAllBoxes();
        _ui.HideBoard();
        _ui.ShowStatus("You Won the Challenge");
        GameManager.firstClick = true;
        while (_ui.Status.gameObject.activeInHierarchy)
        {
            yield return null;
        }

        _ui.ShowMenu();

    }
    public void ResetTimer()
    {
        _ui.UpdateTimer(0);
    }



    private void Awake()
    {
        _board = transform.parent.GetComponentInChildren<Board>();
        _ui = transform.parent.GetComponentInChildren<UI>();
        _gameInProgress = false;

        _ui.HideDifficulty();

    }
    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _countDownTimer = defaultCountDownTimer;

        if (_ui != null)
        {
            _ui.ShowMenu();
            _ui.HideBoard();
            manager.ResetValues();

        }
    }
}
