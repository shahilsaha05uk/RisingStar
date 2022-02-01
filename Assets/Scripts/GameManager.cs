using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum AudioMode { LOOP, ONCE, STOP};
public enum KeyControls { NONE,CLICK, SELECT, FLAG, PAUSE};
public struct KeyBindings
{
    public string name;
    public KeyControls controls;
    public KeyCode key;
}

public class GameManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public GameObject flagRef;
    public Image flagImg;
    public Board board;
    public UI ui;
    public Game game;

    public Color defaultButtonColor;
    public Color highlightNeighbourButtonColor;

    public static bool firstClick;
    public static int flagCount;
    public static int gold;
    public int addGoldValue;
    public int subtractGoldValue;


    public bool pause;
    public bool resume;

    [Space(10)][Header("Shop")]
    public List<ScriptableObjects> shopItems;
    public ShopButtonScript shopButtonPrefab;
    public Transform scrollContent;

    [Space(10)][Header("Sounds")]
    public AudioSource flagSound;
    public AudioSource gameOverSound;
    public AudioSource backgroundSound;
    public AudioSource clickSound;
    public SoundClass soundClass;
    public AudioMode audioMode;

    [Space(10)][Header("DropDown")]
    public List<ResolutionClass> resClass = new List<ResolutionClass>();
    List<Dropdown.OptionData> dataList = new List<Dropdown.OptionData>();
    Dropdown.OptionData optionData;

    [Space(10)][Header("DefaultValues")]
    public double defaultCountdownTimer;
    public int defaultGold;
    public int defaultFlags;

    public void SetDefault()
    {
        defaultCountdownTimer = game._countDownTimer;
        defaultGold = gold;
        defaultFlags = flagCount;
    }

    public void ResetValues()
    {
        game._countDownTimer = defaultCountdownTimer;
        gold = defaultGold;
        flagCount = defaultFlags;
    }

    #endregion
    private void Awake()
    {
        flagCount = 10;
        soundClass = SoundClass.Instance();
        gold = 4000;
        SetDefault();

    }

    private void Start()
    {
        pause = false;
        resume = true;
        ui.Shop.gameObject.SetActive(false);

        DropDownResolution();
        ui.dropdown.onValueChanged.AddListener(ChangeRes);

        StartCoroutine(Updater());
    }
    private IEnumerator Updater()
    {
        while (true)
        {
            ui.flagTxt.text = $"{flagCount}";
            ui.goldTxt.text = $"{gold}";

            if(Input.GetMouseButtonDown(0))
            {
                audioMode = AudioMode.ONCE;
                SoundPlayer(clickSound, AudioMode.ONCE);
            }

            if(Input.GetKeyDown(KeyCode.Escape) && !pause && ui.BoardCanvasStatus == true)
            {
                ui.ShowPause();
                ui.HideFlags();
                PauseGame();
            }
            else if(Input.GetKeyDown(KeyCode.Escape) && !resume && ui.BoardCanvasStatus ==true)
            {
                ui.HidePause();
                ui.ShowFlags();
                ResumeGame();
            }

            if(Input.GetKeyDown(KeyCode.B) && !ui.Shop.gameObject.activeInHierarchy && ui.BoardCanvasStatus ==true)
            {
                ui.Shop.gameObject.SetActive(true);
                ui.HideFlags();
                PauseGame();
            }
            else if(Input.GetKeyDown(KeyCode.B) && ui.Shop.gameObject.activeInHierarchy && ui.BoardCanvasStatus == true)
            {
                ui.Shop.gameObject.SetActive(false);
                ui.ShowFlags();
                ResumeGame();
            }


            yield return null;
        }
    }
    public void PauseGame()
    {
        Debug.Log("Paused");
        resume = false;
        pause = true;

        ui.Board.blocksRaycasts = false;
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Debug.Log("Resumed");
        resume = true;
        pause = false;

        ui.Board.blocksRaycasts = true;
        Time.timeScale = 1;
    }
    public void SoundPlayer(AudioSource clip, AudioMode mode)
    {
        if(clip!= null)
        {
            switch (mode)
            {
                case AudioMode.LOOP:
                    soundClass.SoundPlayLoop(clip);
                    break;
                case AudioMode.ONCE:
                    soundClass.SoundPlayOnce(clip);
                    break;
                case AudioMode.STOP:
                    soundClass.StopSound(clip);
                    break;
            }
        }
    }

    private void DropDownResolution()
    {
        for (int i = 0; i < resClass.Count; i++)
        {
            optionData = new Dropdown.OptionData();
            optionData.text = $"{resClass[i].horizontal} x {resClass[i].vertical}";

            dataList.Add(optionData);

            if (i == resClass.Count - 1)
            {
                optionData = new Dropdown.OptionData();
                optionData.text = "Full Screen";
                dataList.Add(optionData);
            }
        }

        ui.dropdown.AddOptions(dataList);

    }
    private void ChangeRes(int dropMenuCount)
    {
        if (dataList[dropMenuCount].text == "Full Screen")
        {
            Debug.Log("Full screen");
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
        else
        {
            int horizontal = resClass[dropMenuCount].horizontal;
            int vertical = resClass[dropMenuCount].vertical;

            Screen.SetResolution(horizontal, vertical, false);

        }

    }
}
[System.Serializable]
public class ResolutionClass
{
    public int horizontal, vertical;
}
[System.Serializable]
public class KeyBindingClass
{
    private KeyBindings keyBind;

    public KeyCode key;
    public KeyControls controls;
    public string name;

    public KeyBindingClass()
    {
        keyBind = new KeyBindings();

        keyBind.controls = controls;
        keyBind.key = key;
        keyBind.name = name;
    }
}
public class SoundClass
{
    static SoundClass instance;

    private SoundClass()
    {

    }
    public static SoundClass Instance()
    {
        if(instance== null)
        {
            instance = new SoundClass();
        }

        Debug.Log("Sound Class Instantiated");
        return instance;
    }

    public void SoundPlayOnce(AudioSource clipToPlay)
    {
        if (clipToPlay != null)
        {
            clipToPlay.PlayOneShot(clipToPlay.clip);
        }
    }

    public void SoundPlayLoop(AudioSource clipToPlay)
    {
        if (clipToPlay != null)
        {
            clipToPlay.Play();
            clipToPlay.loop = true;
        }
    }

    public void StopSound(AudioSource clipToStop)
    {
        if (clipToStop != null)
        {
            clipToStop.Stop();
        }
    }
}