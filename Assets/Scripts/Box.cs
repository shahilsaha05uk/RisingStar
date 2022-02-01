using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Box : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Color[] DangerColors = new Color[8];
    public Image Danger;
    public Image btn_Image;

    private TMP_Text _textDisplay;
    [HideInInspector]public Button _button;
    private Action<Box> _changeCallback;
    public GameManager manager;

    public int RowIndex { get; private set; }
    public int ColumnIndex { get; private set; }
    public int ID { get; private set; }
    public int DangerNearby { get; private set; }
    public bool IsDangerBox { get; set; }
    public bool IsDangerous { get; private set; }
    public bool IsActive { get { return _button != null && _button.interactable; } }


    public void Setup(int id, int row, int column)
    {
        ID = id;
        RowIndex = row;
        ColumnIndex = column;
    }
    public void Charge(int dangerNearby, bool danger, Action<Box> onChange)
    {
        _changeCallback = onChange;
        DangerNearby = dangerNearby;
        IsDangerous = danger;
        ResetState();
    }
    public void InteractableBox()
    {
        _button.interactable = true;
    }

    public void Reveal()
    {
        if (_button != null)
        {
            _button.interactable = false;
        }

        if (_textDisplay != null)
        {
            _textDisplay.enabled = true;
        }
    }
    public void StandDown()
    {
        if (_button != null)
        {
            _button.interactable = false;
        }

        if (Danger != null)
        {
            Danger.enabled = false;
        }

        if (_textDisplay != null)
        {
            _textDisplay.enabled = false;
        }
    }
    public void OnClick()
    {
        if (GameManager.firstClick == true)
        {
            GameManager.firstClick = false;
            manager.board.AddDangerBoxes(this);
        }

        if (LookForFlag() == null)
        {

            if (_button != null)
            {
                _button.interactable = false;
            }

            if (IsDangerous && Danger != null)
            {
                Danger.enabled = true;
            }
            else if (_textDisplay != null)
            {
                _textDisplay.enabled = true;
            }

            _changeCallback?.Invoke(this);
        }
    }

    //Check if the box is already containing a flag
    public GameObject LookForFlag()
    {
        //Check for the flag Game Object
        Transform[] childObjs = GetComponentsInChildren<Transform>();
        GameObject tempFlag = null;
        foreach (var item in childObjs)
        {
            if (item.CompareTag("Flag"))
            {
                Debug.Log("Found the flag object");
                tempFlag = item.gameObject;
            }
        }
        
        return tempFlag;

    }

    //... IF the box contains a flag, than destroy the flag... ELSE Add a flag
    public void SetFlag()
    {
        GameObject tempFlag;
        tempFlag = LookForFlag();

        if (tempFlag != null)
        {
            Destroy(tempFlag.gameObject);

            GameManager.flagCount++;

        }

        if (tempFlag == null && _button.interactable && GameManager.flagCount > 0)
        {
            GameObject flag = Instantiate(manager.flagRef);
            flag.transform.position = transform.position;
            flag.transform.rotation = transform.rotation;
            flag.transform.SetParent(transform);
            GameManager.flagCount--;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Debug.Log("Set Flags");

            manager.SoundPlayer(manager.flagSound, AudioMode.ONCE);
            SetFlag();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(manager.board.NeighbouringBoxesHighlighted == false)
            {
                manager.board.SetDefaultButtonColor();
                manager.board.FindNeighBouringBoxes(this);
                manager.board.HighlightNeighbours();
            }
            else if(manager.board.NeighbouringBoxesHighlighted == true)
            {
                manager.board.SetDefaultButtonColor();
            }
        }
    }
    private void ResetState()
    {
        if (Danger != null)
        {
            Danger.enabled = false;
        }

        if (_textDisplay != null)
        {
            if (DangerNearby > 0)
            {
                _textDisplay.text = DangerNearby.ToString("D");
                _textDisplay.color = DangerColors[DangerNearby-1];
            }
            else
            {
                _textDisplay.text = string.Empty;
            }

            _textDisplay.enabled = false;
        }

        if (_button != null)
        {
            _button.interactable = true;
        }
    }


    private void Awake()
    {
        _textDisplay = GetComponentInChildren<TMP_Text>(true);
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ResetState();
    }

}
