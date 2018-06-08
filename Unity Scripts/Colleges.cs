using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


//College Struct
[System.Serializable]
public struct MyProgramData {
    public string program_name;
    public string program_years;
    public string department;
}
[System.Serializable]
public struct MyCollegeData {
    public string college_name;
    public string vision;
    public string mission;
    public string goal;
    public MyProgramData[] programs;
}

public struct College {
    public MyCollegeData[] colleges;
}

public class Colleges : MonoBehaviour {

    private College collegeList;
    [SerializeField] GameObject collegePrefab;
    [SerializeField] Transform contentPanel;
        

	// Use this for initialization
	IEnumerator Start () {

        string url = "http://localhost:5000/college/";
        WWWForm form = new WWWForm();
        form.AddField("username", "asd");
        form.AddField("password", "asd");
        var headers = form.headers;
        byte[] rawData = form.data;
        headers["Authorization"] = "kLVHntU5YCsQku6GPf1Ehz9cPJ4lwO7krUOgICtSOefIiJ2QGZkbrlXiAXPH";

        using (WWW www = new WWW(url, rawData, headers)) {
            yield return www;
            if (www.error == null) {
                collegeList = JsonUtility.FromJson<College>(www.text);
                if (collegeList.colleges.Length > 0) {
                    foreach (MyCollegeData college in collegeList.colleges) {
                        GameObject newCollege = (GameObject)GameObject.Instantiate(collegePrefab);
                        newCollege.transform.SetParent(contentPanel, false);

                        CollegePrefab prefab = newCollege.GetComponent<CollegePrefab>();
                        prefab.Setup(college);
                    }
                }
            }
            else {
                Debug.Log("ERROR: " + www.error);
            }
        }

    }
	
}
