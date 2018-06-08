using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


[System.Serializable]
public struct ServiceReport {
    public string date;
    public string service_id;
}


public class ServicePrefab : MonoBehaviour {

    private MyServiceData myData;
    private StepManager stepManager;
    private UIController controls;
    [SerializeField] TextMeshProUGUI serviceName;
    [SerializeField] Button thisButton;


    private void Start() {
        thisButton.onClick.AddListener(StartStep);
        stepManager = GameObject.Find("StepManager").GetComponent<StepManager>();
        controls = GameObject.Find("Container").GetComponent<UIController>();
    }


    public void Setup(MyServiceData data) {
        this.myData = data;
        this.serviceName.text = data.service_name;
    }

    public IEnumerator PostServiceReports() {
        string url = "http://localhost:5000/servicereport/set/";

        WWWForm form = new WWWForm();
        form.AddField("report_date", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        form.AddField("service", myData.id);
        byte[] rawData = form.data;
        var headers = form.headers;
        headers["Authorization"] = "kLVHntU5YCsQku6GPf1Ehz9cPJ4lwO7krUOgICtSOefIiJ2QGZkbrlXiAXPH";
        using (WWW www = new WWW(url, rawData, headers)) {
            yield return www;

            if (www.error == null) {
                Debug.Log("The File has been uploaded");
            }
            else {
                Debug.Log(www.error);
            }
        }
    }

    public void StartStep() {
        controls.Hide();
        stepManager.StartService(myData);
        StartCoroutine(PostServiceReports());
    }

}
