using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    public enum Event { ClickedBlank, ClickedNearDanger, ClickedDanger, Win };
    public enum FlagDestroyEvents { WHENGRIDNOTACTIVE, HIDEFLAGS, DESTROYALL};

    [SerializeField] private Box BoxPrefab;
    [SerializeField] private int Width = 10;
    [SerializeField] private int Height = 10;

    [Space(10)][Header("DangerousBox Setter")]
    public int easy_DangerousBox;
    public int medium_DangerousBox;
    public int hard_DangerousBox;
    public int NumberOfDangerousBoxes;

    public static List<Box> NeighbourBoxList { get; private set; }
    public static List<Box> HighlightedNeighbourBoxList { get; private set; }
    public bool NeighbouringBoxesHighlighted { private set; get; }

    public Box[] _grid;
    public Vector2Int[] _neighbours;
    private RectTransform _rect;
    private Action<Event> _clickEvent;
    public GameManager manager;
    public List<int> dangerIndexList;
    public List<int> flagIndexList;

    public List<GameObject> BoardChilds;

    public void Setup(Action<Event> onClickEvent)
    {
        _clickEvent = onClickEvent;
        Clear();
    }
    public void Clear()
    {
        for (int row = 0; row < Height; ++row)
        {
            for (int column = 0; column < Width; ++column)
            {
                int index = row * Width + column;
                _grid[index].StandDown();
            }
        }
    }
    public void DestroyAllBoxes()
    {
        Array.Clear(_grid, 0, _grid.Length);

        foreach (var item in BoardChilds)
        {
            Destroy(item.gameObject);
        }
    }

    // Setting up the board
    public void GenerateBoxes()
    {
        Width = 10;
        Height = 10;


        // Setting the box Transforms
        _grid = new Box[Width * Height];
        _rect = transform as RectTransform;
        RectTransform boxRect = BoxPrefab.transform as RectTransform;

        _rect.sizeDelta = new Vector2(boxRect.sizeDelta.x * Width, boxRect.sizeDelta.y * Height);
        Vector2 startPosition = _rect.anchoredPosition - (_rect.sizeDelta * 0.5f) + (boxRect.sizeDelta * 0.5f);
        startPosition.y *= -1.0f;

        //To check if the neighbouring boxes are empty
        _neighbours = new Vector2Int[8]
        {
            new Vector2Int(-Width - 1, -1),
            new Vector2Int(-Width, -1),
            new Vector2Int(-Width + 1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(Width - 1, 1),
            new Vector2Int(Width, 1 ),
            new Vector2Int(Width + 1, 1)
        };

        // Generate the boxes
        for (int row = 0; row < Width; ++row)
        {
            GameObject rowObj = new GameObject(string.Format("Row{0}", row), typeof(RectTransform));
            RectTransform rowRect = rowObj.transform as RectTransform;
            rowRect.SetParent(transform);
            rowRect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, startPosition.y - (boxRect.sizeDelta.y * row));
            rowRect.sizeDelta = new Vector2(boxRect.sizeDelta.x * Width, boxRect.sizeDelta.y);
            rowRect.localScale = Vector2.one;

            for (int column = 0; column < Height; ++column)
            {
                int index = row * Width + column;
                _grid[index] = Instantiate(BoxPrefab, rowObj.transform);
                _grid[index].Setup(index, row, column);
                RectTransform gridBoxTransform = _grid[index].transform as RectTransform;
                _grid[index].name = string.Format("ID{0}, Row{1}, Column{2}", index, row, column);
                
                gridBoxTransform.anchoredPosition = new Vector2(startPosition.x + (boxRect.sizeDelta.x * column), 0.0f);
            }
        }

        // Sanity check
        for (int count = 0; count < _grid.Length; ++count)
        {
            //Debug.LogFormat("Count: {0}  ID: {1}  Row: {2}  Column: {3}", count, _grid[count].ID, _grid[count].RowIndex, _grid[count].ColumnIndex);
        }
    }
    public void GenerateBoxes(int width, int height)
    {
        BoardChilds = new List<GameObject>();
        // Setting the box Transforms
        Width = width;
        Height = height;

        _grid = new Box[Width * Height];
        _rect = transform as RectTransform;
        RectTransform boxRect = BoxPrefab.transform as RectTransform;

        _rect.sizeDelta = new Vector2(boxRect.sizeDelta.x * Width, boxRect.sizeDelta.y * Height);
        Vector2 startPosition = _rect.anchoredPosition - (_rect.sizeDelta * 0.5f) + (boxRect.sizeDelta * 0.5f);
        startPosition.y *= -1.0f;

        //To check if the neighbouring boxes are empty
        _neighbours = new Vector2Int[8]
        {
            new Vector2Int(-Width - 1, -1),
            new Vector2Int(-Width, -1),
            new Vector2Int(-Width + 1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(Width - 1, 1),
            new Vector2Int(Width, 1 ),
            new Vector2Int(Width + 1, 1)
        };

        // Generate the boxes
        for (int row = 0; row < Width; ++row)
        {
            GameObject rowObj = new GameObject(string.Format("Row{0}", row), typeof(RectTransform));
            RectTransform rowRect = rowObj.transform as RectTransform;
            rowRect.SetParent(transform);
            rowRect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, startPosition.y - (boxRect.sizeDelta.y * row));
            rowRect.sizeDelta = new Vector2(boxRect.sizeDelta.x * Width, boxRect.sizeDelta.y);
            rowRect.localScale = Vector2.one;

            BoardChilds.Add(rowObj);

            for (int column = 0; column < Height; ++column)
            {
                int index = row * Width + column;
                _grid[index] = Instantiate(BoxPrefab, rowObj.transform);
                _grid[index].Setup(index, row, column);
                RectTransform gridBoxTransform = _grid[index].transform as RectTransform;
                _grid[index].name = string.Format("ID{0}, Row{1}, Column{2}", index, row, column);
                
                gridBoxTransform.anchoredPosition = new Vector2(startPosition.x + (boxRect.sizeDelta.x * column), 0.0f);
            }
        }

        // Sanity check
        for (int count = 0; count < _grid.Length; ++count)
        {
            //Debug.LogFormat("Count: {0}  ID: {1}  Row: {2}  Column: {3}", count, _grid[count].ID, _grid[count].RowIndex, _grid[count].ColumnIndex);
        }
    }
    public void RechargeBoxes()
    {
        foreach (var item in _grid)
        {
            item.InteractableBox();
        }
    }
    public void AddDangerBoxes(Box clickedBox)
    {
        // for adding the danger boxes, first 10 boxes are added to the list and then they are suffled

        int numberOfItems = Width * Height;
        List<bool> dangerList = new List<bool>(numberOfItems);

        Debug.Log("No of dangerous boxes: " + NumberOfDangerousBoxes);
        for (int i = 0; i < numberOfItems; ++i)
        {
            dangerList.Add(i < NumberOfDangerousBoxes);
        }

        dangerList.RandomShuffle();

        //Check if the initial click is near the dangerbox than suffle it
        while (true)
        {
            int dangerCount = CountDangerNearby(dangerList, clickedBox.ID);

            if(dangerCount>0 || CheckDanger(clickedBox, dangerList))
            {
                Debug.Log("Shuffle");
                dangerList.RandomShuffle();
            }
            else if (dangerCount <= 0 || !CheckDanger(clickedBox, dangerList))
            {
                break;
            }
        }
        for (int row = 0; row < Height; ++row)
        {
            for (int column = 0; column < Width; ++column)
            {
                int index = row * Width + column;
                _grid[index].Charge(CountDangerNearby(dangerList, index), dangerList[index], OnClickedBox);
            }
        }


        // Add the danger Index to the list
        dangerIndexList.Clear();
        for (int i = 0; i < dangerList.Count; i++)
        {
            if(dangerList[i] ==true)
            {
                _grid[i].IsDangerBox = true;
                dangerIndexList.Add(_grid[i].ID);
            }
        }

        Debug.Log("Danger List count: " + dangerIndexList.Count);
    }


    // Danger Methods
    private bool CheckDanger(Box clickedBox, List<bool> dangerList)
    {
        if(dangerList[clickedBox.ID] == true)
        {
            return true;
        }
        return false;
    }
    private int CountDangerNearby(List<bool> danger, int index)
    {
        int result = 0;
        int boxRow = index / Width;

        if (!danger[index])
        {
            for (int count = 0; count < _neighbours.Length; ++count)
            {
                int neighbourIndex = index + _neighbours[count].x;
                int expectedRow = boxRow + _neighbours[count].y;
                int neighbourRow = neighbourIndex / Width;

                result += (expectedRow == neighbourRow && neighbourIndex >= 0 && neighbourIndex < danger.Count && danger[neighbourIndex]) ? 1 : 0;
            }
        }
        return result;
    }
    public void RevealAllDangers()
    {
        for (int i = 0; i < _grid.Length; i++)
        {
            if(_grid[i].IsDangerous)
            {
                _grid[i].Reveal();
                _grid[i].Danger.enabled = true;

            }
        }
    }

    //Neighbouring Boxes
    public void FindNeighBouringBoxes(Box clickedBox)
    {
        List<Box> neighbouringBoxes = new List<Box>();
        //SetDefaultButtonColor();

        for (int count = 0; count < _neighbours.Length; ++count)
        {
            int neighbourIndex = clickedBox.ID + _neighbours[count].x;
            int expectedRow = clickedBox.RowIndex + _neighbours[count].y;
            int neighbourRow = neighbourIndex / Width;

            if((expectedRow == neighbourRow && neighbourIndex >= 0))
            {
                Debug.Log("Found");
                foreach (var item in _grid)
                {
                    if(item.ID == neighbourIndex)
                    {
                        neighbouringBoxes.Add(item);
                        break;
                    }
                }
            }
        }

        NeighbourBoxList = neighbouringBoxes;
        HighlightedNeighbourBoxList = neighbouringBoxes;
    }
    public void HighlightNeighbours()
    {
        if (NeighbourBoxList != null)
        {
            foreach (var item in NeighbourBoxList)
            {
                item.btn_Image.GetComponent<Image>().color = item.manager.highlightNeighbourButtonColor;
            }
        }
        NeighbouringBoxesHighlighted = true;
    }
    public void SetDefaultButtonColor()
    {
        if (HighlightedNeighbourBoxList != null)
        {
            foreach (var item in HighlightedNeighbourBoxList)
            {
                item.btn_Image.GetComponent<Image>().color = item.manager.defaultButtonColor;
            }
        }
        NeighbouringBoxesHighlighted = false;
    }
    public void FlagDestroyer(FlagDestroyEvents flagEvent)
    {
        GameObject flag;
        switch (flagEvent)
        {
            case FlagDestroyEvents.WHENGRIDNOTACTIVE:

                for (int i = 0; i < _grid.Length; i++)
                {
                    if (!_grid[i].IsActive && (flag = _grid[i].LookForFlag()) != null)
                    {
                        Destroy(flag.gameObject);
                        GameManager.flagCount++;
                    }
                }
                break;
            case FlagDestroyEvents.HIDEFLAGS:

                flagIndexList = new List<int>();
                foreach (var item in _grid)
                {
                    if ((flag = item.LookForFlag()) != null)
                    {
                        flagIndexList.Add(item.ID);
                        Destroy(flag.gameObject);
                    }
                }
                break;
            case FlagDestroyEvents.DESTROYALL:
                foreach (var item in _grid)
                {
                    if ((flag = item.LookForFlag()) != null)
                    {
                        Destroy(flag.gameObject);
                    }
                }

                break;
        }


    }
    public void FlagHide()
    {
        FlagDestroyer(FlagDestroyEvents.HIDEFLAGS);
    }
    public void FlagShow()
    {
        foreach (var item in flagIndexList)
        {
            _grid[item].SetFlag();
        }
    }

    private void OnClickedBox(Box box)
    {
        Event clickEvent = Event.ClickedBlank;
        if (box.IsDangerous)
        {
            clickEvent = Event.ClickedDanger;
            manager.SoundPlayer(manager.gameOverSound, AudioMode.ONCE);
        }
        else if (box.DangerNearby > 0)
        {
            clickEvent = Event.ClickedNearDanger;
        }
        else
        {
            ClearNearbyBlanks(box);
            FlagDestroyer(FlagDestroyEvents.WHENGRIDNOTACTIVE);
        }

        if(!box.IsDangerous)
        {
            UpdateGold(manager.addGoldValue);
        }
        else
        {
            UpdateGold(manager.subtractGoldValue);
        }

        if (CheckForWin())
        {
            clickEvent = Event.Win;
            UpdateGold(300);
        }

        _clickEvent?.Invoke(clickEvent);
    }

    public void UpdateGold(int goldVal)
    {
        GameManager.gold += goldVal;
    }

    //Shop Canvas Scripts
    public bool minesLeftToBeDiscovered = false;
    public void RevealRandomMine()
    {
        int mineIndex = 0;
        //Check if there is any mine left to be discovered
        foreach (var item in dangerIndexList)
        {
            if(_grid[item]._button.interactable)
            {
                minesLeftToBeDiscovered = true;

                mineIndex = item;
                break;
            }
        }

        if (minesLeftToBeDiscovered)
        {
            minesLeftToBeDiscovered = false;
            if (_grid[mineIndex].IsActive)
            {
                Debug.Log("Danger is on: " + _grid[mineIndex].ID + " on index: " + _grid[mineIndex]);
                _grid[mineIndex]._button.interactable = false;
                _grid[mineIndex].Danger.enabled = true;
                manager.ui.Shop.gameObject.SetActive(false);
                manager.ResumeGame();
            }
        }
        else
        {
            manager.ui.ShowStatus("No Mines left to be discovered");
            manager.ui.Shop.gameObject.SetActive(false);
            manager.ResumeGame();
        }

        manager.ui.ShowFlags();

    }
    public void IncreaseTimeLimit()
    {
        if (manager.game._challengerMode)
        {
            manager.game._countDownTimer += 20;
            manager.ui.Shop.gameObject.SetActive(false);
            manager.ResumeGame();
        }
        else
        {
            manager.ui.Shop.gameObject.SetActive(false);
            Debug.Log("Not in challenger mode");
            manager.ui.ShowStatus("You are not in challenger mode");
            manager.ResumeGame();
        }

        manager.ui.ShowFlags();

    }

    private bool CheckForWin()
    {
        bool Result = true;

        for( int count = 0; Result && count < _grid.Length; ++count)
        {
            if(!_grid[count].IsDangerous && _grid[count].IsActive)
            {
                Result = false;
            }
        }

        return Result;
    }
    private void ClearNearbyBlanks(Box box)
    {
        RecursiveClearBlanks(box);
    }
    private void RecursiveClearBlanks(Box box)
    {
        if (!box.IsDangerous)
        {
            box.Reveal();

            if (box.DangerNearby == 0)
            {
                for (int count = 0; count < _neighbours.Length; ++count)
                {
                    int neighbourIndex = box.ID + _neighbours[count].x;
                    int expectedRow = box.RowIndex + _neighbours[count].y;
                    int neighbourRow = neighbourIndex / Width;
                    bool correctRow = expectedRow == neighbourRow;
                    bool active = neighbourIndex >= 0 && neighbourIndex < _grid.Length && _grid[neighbourIndex].IsActive;

                    if (correctRow && active)
                    {
                        RecursiveClearBlanks(_grid[neighbourIndex]);
                    }
                }
            }
        }
    }


}
