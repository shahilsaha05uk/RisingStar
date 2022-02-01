using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBind : MonoBehaviour
{
    public KeyCode key;
    public KeyControls controls;
    public string keyDescription;

    public KeyBind(KeyBindings keyBind)
    {
        keyBind = new KeyBindings();

        keyBind.controls = controls;
        keyBind.key = key;
        keyBind.name = keyDescription;
    }

}
