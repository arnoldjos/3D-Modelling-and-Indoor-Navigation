using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyToggle : MonoBehaviour {

    [SerializeField] MenuController myControls;
    [SerializeField] UIController uiControls;

    public void ToggleMyButton() {
        var toggle = GetComponent<Toggle>();

        if (toggle.isOn) {
            uiControls.Show();
            if (this.name == "RoomButton") {
                myControls.RoomButtonToggle();
            }
            else if (this.name == "CollegeButton") {
                myControls.CollegeButtonToggle();
            }
            else if (this.name == "ServiceButton") {
                myControls.ServiceButtonToggle();
            }
            //myControls.DisableMenu();
        }
        else  if (!toggle.isOn) {
            uiControls.Hide();
            //myControls.EnableMenu();
            myControls.EnableAllMenuButtons();
        }
    }

    public void ToggleReset() {
        var toggle = GetComponent<Toggle>();
        toggle.isOn = false;
    }

}
