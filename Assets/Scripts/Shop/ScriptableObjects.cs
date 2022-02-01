using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop List", menuName = "Items", order = 1)]
public class ScriptableObjects : ScriptableObject
{
    public ItemType m_Type;

    public int ID;
    public string Name;
    public int Cost;

    public ItemType returnItemType() { return m_Type; }

}

public enum ItemType
{
    REVEAL_MINE, EXTRA_TIME
}
