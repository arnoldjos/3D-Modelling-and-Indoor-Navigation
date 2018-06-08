using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixToggle : MonoBehaviour {
    private Toggle toggle;

    private void Start() {
        toggle = GetComponent<Toggle>();
        if (toggle != null) return;
        Debug.Log("ButtonReset:Awake - Couldn't find the _button.");
    }

    public void ResetState() {
        //Debug.Log("button name is " + toggle.gameObject.name);

        toggle.enabled = false;
        toggle.enabled = true;
    }
}
