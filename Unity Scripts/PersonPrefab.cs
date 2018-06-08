using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PersonPrefab : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI fullname;
    public MyPersonData data;


    public void Setup(MyPersonData data) {
        this.data = data;
        fullname.text ="<u>" + data.first_name + " " + data.last_name + "</u>" +"\n" + data.job_position;
    }

}
