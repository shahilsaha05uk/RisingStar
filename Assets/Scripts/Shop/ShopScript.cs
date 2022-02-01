using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    public ScriptableObjects[] itemsInShop;
    public ShopButtonScript[] itemButtons;
    public GameManager manager;


    private void OnEnable()
    {
        //itemsInShop = manager.shopItems;
        itemButtons = new ShopButtonScript[itemsInShop.Length];

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i] = Instantiate(manager.shopButtonPrefab, manager.scrollContent.transform);
            itemButtons[i].Init(itemsInShop[i]);

            Debug.Log("Shop Item: " + itemButtons[i].name);
            itemButtons[i].items += OnBuyItem;
        }
    }
    private void OnDisable()
    {
        for (int i = itemButtons.Length - 1; i >= 0; i--)
        {
            itemButtons[i].items -= OnBuyItem;
            Destroy(itemButtons[i].gameObject);
        }
    }
    public void OnBuyItem(ScriptableObjects item)
    {
        if (GameManager.gold > item.Cost)
        {
            switch (item.m_Type)
            {
                case ItemType.REVEAL_MINE:
                    Debug.Log("Reveal a mine");
                    manager.board.RevealRandomMine();

                    break;
                case ItemType.EXTRA_TIME:
                    Debug.Log("Buy extra time");
                    manager.board.IncreaseTimeLimit();
                    break;
            }

            GameManager.gold -= item.Cost;
        }
        else
        {
            manager.ui.ShowStatus("You dont have enough coins to buy this item");
            this.gameObject.SetActive(false);
            manager.ResumeGame();
        }
    }
}
