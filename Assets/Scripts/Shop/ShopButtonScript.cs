using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopButtonScript : MonoBehaviour
{
    public event Action<ScriptableObjects> items;
    private ScriptableObjects buttonData;
    public TextMeshProUGUI btn_Text;
    public TextMeshProUGUI btn_Cost;

    public string Name;
    public int Cost;
    public int ID;

    public void Init(ScriptableObjects item)
    {
        buttonData = item;

        item.Cost = buttonData.Cost;
        item.Name = buttonData.Name;
        item.ID = buttonData.ID;
        btn_Text.text = buttonData.Name;
        btn_Cost.text = buttonData.Cost.ToString();
    }


    public void BuyItem()
    {
        items?.Invoke(buttonData);
    }

}
