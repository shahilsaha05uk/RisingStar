using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private CanvasGroup Menu;
    [SerializeField] private CanvasGroup Game;
    [SerializeField] private CanvasGroup Result;
    public CanvasGroup Board;
    [SerializeField] private CanvasGroup Background;
    [SerializeField] private CanvasGroup Pause;
    [SerializeField] private CanvasGroup Difficulty;
    public CanvasGroup Settings;
    public TextMeshProUGUI Status;
    public GameObject Shop;
    public GameObject ControlsList;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private TMP_Text ResultText;
    public TextMeshProUGUI flagTxt;
    public TextMeshProUGUI goldTxt;

    public Button ResetButton;
    public Button NextLevelButton;

    public Dropdown dropdown;
    public ScrollRect controlList;

    public bool ResetCanvasStatus { get; private set; }
    public bool paused { get; private set; }
    private static readonly string[] ResultTexts = { "Game Over!", "You Win!!" };
    private static readonly float AnimationTime = 0.5f;
    public GameManager manager;
    public bool BoardCanvasStatus;


    private void Start()
    {
        manager.pause = false;
        manager.resume = true;
        HidePause();
        HideStatus();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Awake()
    {
        if (Menu != null)
        {
            Menu.alpha = 0.0f;
            Menu.interactable = false;
            Menu.blocksRaycasts = false;
        }

        if (Game != null)
        {
            Game.alpha = 0.0f;
            Game.interactable = false;
            Game.blocksRaycasts = false;
        }

        if (Result != null)
        {
            Result.alpha = 0.0f;
            Result.interactable = false;
            Result.blocksRaycasts = false;
        }
    }

    #region Show Methods
    public void ShowMenu()
    {
        StartCoroutine(ShowCanvas(Menu, 1.0f));
        HideSettings();
        manager.game._challengerMode = false;
    }
    public void ShowFlags()
    {
        manager.board.FlagShow();
    }
    public void ShowSettings()
    {
        StartCoroutine(ShowCanvas(Settings, 1.0f));
    }
    public void ShowPause()
    {
        Pause.gameObject.SetActive(true);
        manager.pause = true;
    }
    public void ShowGame()
    {
        StartCoroutine(ShowCanvas(Game, 1.0f));
    }
    public void ShowBoard()
    {
        ShowGame();

        StartCoroutine(ShowCanvas(Board, 1.0f));
        StartCoroutine(ShowCanvas(Background, 0.0f));

        BoardCanvasStatus = true;
    }
    public void ShowDifficulty()
    {
        StartCoroutine(ShowCanvas(Difficulty, 1.0f));
    }
    public void ShowStatus(string text)
    {
        StartCoroutine(StatusCanvas(text));
    }

    public void ShowResult(bool success)
    {
        HideBoard();
        if (ResultText != null)
        {
            ResultText.text = ResultTexts[success ? 1 : 0];
        }
        manager.board.SetDefaultButtonColor();
        StartCoroutine(ShowCanvas(Result, 1.0f));
        ResetCanvasStatus = true;
    }
    #endregion

    #region Hide Methods
    public void HideMenu()
    {
        StartCoroutine(ShowCanvas(Menu, 0.0f));
    }
    public void HideFlags()
    {
        manager.board.FlagHide();
    }

    public void HideSettings()
    {
        StartCoroutine(ShowCanvas(Settings, 0.0f));
    }
    public void HidePause()
    {
        Pause.gameObject.SetActive(false);
        manager.resume = true;
    }

    public void HideStatus()
    {
        Status.gameObject.SetActive(false);
    }
    public void HideBoard()
    {
        StartCoroutine(ShowCanvas(Board, 0.0f));
        HideGame();
        StartCoroutine(ShowCanvas(Background, 1.0f));
        BoardCanvasStatus = false;
        manager.SoundPlayer(manager.backgroundSound, AudioMode.LOOP);
    }
    public void HideGame()
    {
        StartCoroutine(ShowCanvas(Game, 0.0f));
    }
    public void HideResult()
    {
        StartCoroutine(ShowCanvas(Result, 0.0f));
        ResetCanvasStatus = false;
    }
    public void HideDifficulty()
    {
        StartCoroutine(ShowCanvas(Difficulty, 0.0f));
        ShowBoard();
    }
    #endregion

    public void UpdateTimer(double gameTime)
    {
        if (TimerText != null)
        {
            TimerText.text = FormatTime(gameTime);
        }
    }
    private static string FormatTime(double seconds)
    {
        float m = Mathf.Floor((int)seconds / 60);
        float s = (float)seconds - (m * 60);
        string mStr = m.ToString("00");
        string sStr = s.ToString("00.000");
        return string.Format("{0}:{1}", mStr, sStr);
    }
    private IEnumerator ShowCanvas(CanvasGroup group, float target)
    {
        if (group != null)
        {
            float startAlpha = group.alpha;
            float t = 0.0f;

            group.interactable = target >= 1.0f;
            group.blocksRaycasts = target >= 1.0f;

            while (t < AnimationTime)
            {
                t = Mathf.Clamp(t + Time.deltaTime, 0.0f, AnimationTime);
                group.alpha = Mathf.SmoothStep(startAlpha, target, t / AnimationTime);
                yield return null;
            }
        }
    }
    private IEnumerator StatusCanvas(string text)
    {
        Status.text = text;
        Status.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        Status.gameObject.SetActive(false);
    }
}
