using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;




public class CollegeInformation : MonoBehaviour {

    [SerializeField] TextMeshProUGUI collegeName;
    [SerializeField] TextMeshProUGUI departments;
    [SerializeField] TextMeshProUGUI programs;
    [SerializeField] TextMeshProUGUI vision;
    [SerializeField] TextMeshProUGUI mission;
    [SerializeField] TextMeshProUGUI goal;
    [SerializeField] UIController controls;
    [SerializeField] ScrollRect scrollRect;





    public void ShowCollege(MyCollegeData data) {
        departments.text = "";
        this.programs.text = "";
        this.collegeName.text = data.college_name;

        if (data.college_name == "College of Education") {
            departments.text += "Department of Elementary Education \nDepartment of Secondary Education";
        }
        else if (data.college_name == "College of Engineering and Computer Studies") {
            departments.text += "Department of Engineering \nDepartment of Computer Studies";
        }
        else if (data.college_name == "College of Business and Accountancy") {
            departments.text += "Department of Business Administration \nDepartment of Accountancy";
        }

        foreach (MyProgramData program in data.programs) {
            programs.text += program.program_name + "-" + program.program_years +"\n";
        }
        this.vision.text = data.vision;
        this.mission.text = data.mission;
        this.goal.text = data.goal;
        SetScrollRectToTop();
        ShowScrollRect();
        controls.Hide();
    }

    public void SetScrollRectToTop() {
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void HideScrollRect() {
        scrollRect.vertical = false;
    }

    public void ShowScrollRect() {
        scrollRect.vertical = true;
    }
}
