using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//College Struct
[System.Serializable]
public struct MyStepsData {
    public string step_type;
    public string step_desc;
    public float xCoord;
    public float yCoord;
    public float zCoord;
}
[System.Serializable]
public struct MyServiceData {
    public string id;
    public string service_name;
    public string service_desc;
    public MyStepsData[] steps;
}

public struct Service {
    public MyServiceData[] services;
}

public class Services : MonoBehaviour {

    private Service serviceList;
    [SerializeField] Transform contentPanel;
    [SerializeField] GameObject servicePrefab;

    IEnumerator Start() {

        string url = "http://localhost:5000/service/";
        WWWForm form = new WWWForm();
        form.AddField("username", "asd");
        form.AddField("password", "asd");
        var headers = form.headers;
        byte[] rawData = form.data;
        headers["Authorization"] = "kLVHntU5YCsQku6GPf1Ehz9cPJ4lwO7krUOgICtSOefIiJ2QGZkbrlXiAXPH";

        using (WWW www = new WWW(url, rawData, headers)) {
            yield return www;
            if (www.error == null) {
                serviceList = JsonUtility.FromJson<Service>(www.text);
                if (serviceList.services.Length > 0) {
                    foreach (MyServiceData service in serviceList.services) {
                        GameObject newService = (GameObject)GameObject.Instantiate(servicePrefab);
                        newService.transform.SetParent(contentPanel, false);

                        ServicePrefab prefab = newService.GetComponent<ServicePrefab>();
                        prefab.Setup(service);
                    }
                }
            }
            else {
                Debug.Log("ERROR: " + www.error);
            }
        }
    }

}
