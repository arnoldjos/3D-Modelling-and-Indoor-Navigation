using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixButton : MonoBehaviour {
    private Button _button;

    private void Start() {
        _button = GetComponent<Button>();
        if (_button != null) return;
        Debug.Log("ButtonReset:Awake - Couldn't find the _button.");
    }

    public void ResetState() {
        Debug.Log("button name is " + _button.gameObject.name);

        _button.enabled = false;
        _button.enabled = true;
    }
}
