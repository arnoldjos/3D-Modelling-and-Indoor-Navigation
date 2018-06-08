using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollegePrefab : MonoBehaviour {

    private MyCollegeData myData;
    private CollegeInformation college;
    [SerializeField] Button thisButton;
    [SerializeField] TextMeshProUGUI collegeName;



    // Use this for initialization
    void Start () {
        college = GameObject.Find("CollegeInformation").GetComponent<CollegeInformation>();
        thisButton.onClick.AddListener(ShowCollege);
    }

    public void Setup(MyCollegeData data) {
        this.myData = data;
        this.collegeName.text = data.college_name;
    }

    private void ShowCollege() {
        college.ShowCollege(myData);
    }

}
