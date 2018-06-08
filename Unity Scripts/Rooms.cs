using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct MyRoomData {
    public int id;
    public string room_name;
    public string room_desc;
    public string location;
    public float xCoord;
    public float yCoord;
    public float zCoord;
    public string college;
}

[System.Serializable]
public struct MyRooms {
    public MyRoomData[] rooms;
}

[System.Serializable]
public struct MyPersonData {
    public int id;
    public string first_name;
    public string last_name;
    public string job_position;
    public int assigned_office_id;
    public string assigned_office_name;
}

public struct Personnel {
    public MyPersonData[] personnel;
}

[System.Serializable]


public class Rooms : MonoBehaviour {

    private MyRooms officeList;
    private Personnel personList;
    private List<GameObject> list;
    [SerializeField] GameObject officePrefab;
    [SerializeField] Transform contentPanel;
    [SerializeField] Transform searchPanel;
    [SerializeField] TMP_InputField searchText;
    [SerializeField] CanvasGroup nothingFound;   
    [SerializeField] UIController contenControls;
    [SerializeField] UIController searchControl;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] MyNavScript navScript;

	// Use this for initialization
	IEnumerator Start () {
        officeList = new MyRooms();
        personList = new Personnel();
        list = new List<GameObject>();        
        string url = "http://localhost:5000/room/";
        WWWForm form = new WWWForm();
        form.AddField("username", "asd");
        form.AddField("password", "asd");
        var headers = form.headers;
        byte[] rawData = form.data;
        headers["Authorization"] = "kLVHntU5YCsQku6GPf1Ehz9cPJ4lwO7krUOgICtSOefIiJ2QGZkbrlXiAXPH";

        using (WWW www = new WWW(url, rawData, headers)) {
            yield return www;
            if (www.error == null) {
                officeList = JsonUtility.FromJson<MyRooms>(www.text);

                foreach (MyRoomData room in officeList.rooms) {
                    GameObject newOffice = (GameObject)GameObject.Instantiate(officePrefab);
                    newOffice.transform.SetParent(contentPanel, false);

                    RoomPrefab prefab = newOffice.GetComponent<RoomPrefab>();
                    prefab.Setup(room);
                    list.Add(newOffice);
                }               
            }
            else {
                Debug.Log("ERROR: " + www.error);
            }
        }

        url = "http://localhost:5000/personnel/";
        using (WWW www = new WWW(url, rawData, headers)) {
            yield return www;
            if (www.error == null) {
                personList = JsonUtility.FromJson<Personnel>(www.text);
                navScript.GetPersonnelList(this.personList);
            }
            else {
                Debug.Log("ERROR: " + www.error);               
            }
        }
    }

    void OnGUI() {
        if (searchText.isFocused && searchText.text != "" && Input.GetKey(KeyCode.Return)) {
            Search();
        }
    }

    public void Search() {
        if (searchControl.isActiveAndEnabled) {
            searchControl.Show();
        }
        HideNothingFound();
        ReloadRooms();        
        bool match = false;
        string query = searchText.text.ToString().ToLower();
        if (query.Length > 0) {
            if(list != null) {
                foreach (GameObject obj in list) {
                    var room = obj.GetComponent<RoomPrefab>().mydata.room_name;
                    if (room.ToLower().Contains(query)) {
                        obj.transform.SetParent(searchPanel, false);
                        match = true;
                    }
                    foreach (MyPersonData person in personList.personnel) {
                        var name = person.first_name + person.last_name;
                        if (name.ToLower().Contains(query) && person.assigned_office_name == room) {
                            obj.transform.SetParent(searchPanel, false);
                            match = true;
                            break;
                        }
                    }
                }
            }

        }
        if (!match) {
            ShowNothingFound();
        }
        if (contenControls.isShow) {
            contenControls.Hide();
        }
        searchControl.Show();
        searchText.text = "";
    }

    public void ReloadRooms() {
        if (list.Count > 0) {
            foreach (GameObject obj in list) {
                if (obj.transform.parent != contentPanel) {
                    obj.transform.SetParent(contentPanel, false);
                    obj.transform.SetSiblingIndex(list.IndexOf(obj));
                }
            }
        }
    }

    public void ShowNothingFound() {
        nothingFound.alpha = 1;
    }

    public void HideNothingFound() {
        nothingFound.alpha = 0;
    }

    public void SetScrollRectToTop() {
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
