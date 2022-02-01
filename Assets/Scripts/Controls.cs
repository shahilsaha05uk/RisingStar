using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    private GameManager manager;
    public Dictionary<string, KeyCode> keyBinding = new Dictionary<string, KeyCode>();
    
    [Space(2)] public Text click, select, flag, buy, pause;
    [Space(2)] public Text lblClick, lblSelect, lblFlag, lblBuy, lblPause;

    void Start()
    {
        manager = GetComponent<GameManager>();
        KeyBind();

    }

    public void KeyBind()
    {
        keyBinding.Add("Click", KeyCode.Mouse0);
        keyBinding.Add("Select", KeyCode.Mouse1);
        keyBinding.Add("Flag", KeyCode.Mouse2);
        keyBinding.Add("Buy", KeyCode.B);
        keyBinding.Add("Pause", KeyCode.Escape);

        click.text = keyBinding["Click"].ToString();
        select.text = keyBinding["Select"].ToString();
        flag.text = keyBinding["Flag"].ToString();
        buy.text = keyBinding["Buy"].ToString();
        pause.text = keyBinding["Pause"].ToString();

        lblClick.text = "Click";
        lblSelect.text = "Select";
        lblFlag.text = "Flag";
        lblBuy.text = "Buy";
        lblPause.text = "Pause";

    }


}
