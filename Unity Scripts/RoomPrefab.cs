using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using System;

[System.Serializable]
public struct OfficeReport {
    public string date;
    public string room_id;
}

public class RoomPrefab : MonoBehaviour {

    public MyRoomData mydata;
    [SerializeField] Button thisButton;
    [SerializeField] TextMeshProUGUI roomName;
    [SerializeField] GameObject targetPrefab;
    Transform targetDestination;
    GameObject navAgent;
    MenuController menuController;


    // Use this for initialization
    void Start() {
        menuController = GameObject.Find("MenuController").GetComponent<MenuController>();
        navAgent = GameObject.Find("NavAgent");
        thisButton.onClick.AddListener(ShowOffice);
    }

    private void Update() {
    }

    public void Setup(MyRoomData data) {
        this.mydata = data;
        this.roomName.text = data.room_name;
        GameObject newOffice = (GameObject)GameObject.Instantiate(targetPrefab);
        newOffice.transform.position = new Vector3(mydata.xCoord, mydata.yCoord, mydata.zCoord);
        targetDestination = newOffice.transform;
        RoomObject prefab = newOffice.GetComponent<RoomObject>();
        prefab.Setup(data);

        GameObject child = newOffice.transform.GetChild(0).gameObject;
        TextMeshPro roomName = child.GetComponent<TextMeshPro>();
        roomName.text = data.room_name;
        if (data.location == "B") {
            newOffice.layer = 12;
            child.layer = 12;
        }
        else if (data.location == "G") {
            newOffice.layer = 13;
            child.layer = 13;
        }
        else if (data.location == "S") {
            newOffice.layer = 14;
            child.layer = 14;
        }
    }

    private void ShowOffice() {
        StartCoroutine(PostOfficeReports());
        navAgent.GetComponent<MyNavScript>().MoveToDestination(targetDestination, mydata);
        menuController.HideMainCanvas();    
    }

    public IEnumerator PostOfficeReports() {
        string url = "http://localhost:5000/roomreport/set/";

        WWWForm form = new WWWForm();
        form.AddField("report_date", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        form.AddField("room", mydata.id);
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

}
