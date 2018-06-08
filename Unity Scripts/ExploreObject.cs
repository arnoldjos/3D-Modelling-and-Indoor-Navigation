using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExploreObject : MonoBehaviour {

    private List<GameObject> personObjects;
    [SerializeField] Transform activePanel;
    [SerializeField] Transform inactivePanel;
    [SerializeField] MenuController menuController;
    [SerializeField] GameObject roomInfo;
    [SerializeField] TextMeshProUGUI roomName;
    [SerializeField] TextMeshProUGUI roomDesc;
    [SerializeField] UIController controls;
    [SerializeField] Camera miniMap;
    [SerializeField] GameObject basementFurniture;
    [SerializeField] GameObject groundFurniture;
    [SerializeField] GameObject secondFurniture;
    private int oldMask;
    private bool done = false;
    private bool groundF = true;
    private bool secondF = false;



    private void Update() {
        if (Input.GetKeyDown("x")) {
            controls.Hide();
            menuController.DeactivateFreeRoam();
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            if (roomInfo.GetComponent<CanvasGroup>().alpha != 0) {
                menuController.HideRoomInfo();
            }
            secondF = false;
            groundF = true;
        }
    }

    private void Start() {
        personObjects = new List<GameObject>();
        oldMask = miniMap.cullingMask;
    }


    public void GetPersonnelList(List<GameObject> personObjects) {
        this.personObjects = personObjects;
    }

    private void Assignpersonnel(MyRoomData roomData) {
        foreach (GameObject person in personObjects) {
            var personData = person.GetComponent<PersonPrefab>().data;
            if (roomData.room_name == personData.assigned_office_name) {
                person.transform.SetParent(activePanel, false);
            }
        }
    }

    private bool CheckPersonnelCount(MyRoomData data) {
        int count = 0;
        foreach (GameObject person in personObjects) {
            var personData = person.GetComponent<PersonPrefab>().data;
            if (data.room_name == personData.assigned_office_name) {
                count++;
            }
        }
        if (count > 0) {
            return true;
        }
        else {
            return false;
        }
    }

    private void ReloadPersonnel() {
        if (personObjects.Count > 0) {
            foreach (GameObject person in personObjects) {
                person.transform.SetParent(inactivePanel, false);
                person.transform.SetSiblingIndex(personObjects.IndexOf(person));
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            if (!done) {
                MyRoomData otherData = other.gameObject.GetComponent<RoomObject>().data;
                if (CheckPersonnelCount(otherData)) {
                    foreach (GameObject person in personObjects) {
                        MyPersonData personData = person.GetComponent<PersonPrefab>().data;
                        if (otherData.id == personData.id) {
                            person.transform.SetParent(activePanel, false);
                        }
                    }
                    menuController.ShowPersonnelPanel();
                }
                else {
                    menuController.HidePersonnelPanel();
                }
                roomName.text = otherData.room_name;
                roomDesc.text = otherData.room_desc;
                menuController.ShowRoomInfo();
                done = true;
            }
        }
        else if (other.gameObject.tag == "GroundFloor") {
            if (groundF) {
                var newMask = oldMask & ~(1 << 13);
                newMask = newMask & ~(1 << 9);
                newMask = newMask | (1 << 8);
                newMask = newMask | (1 << 12);
                miniMap.cullingMask = newMask;
                groundF = false;
                ShowFurniture("Basement");
            }
            else {
                var newMask = oldMask & ~(1 << 8);
                newMask = newMask & ~(1 << 12);
                newMask = newMask | (1 << 9);
                newMask = newMask | (1 << 13);
                miniMap.cullingMask = newMask;
                groundF = true;
                ShowFurniture("Ground");
            }
        }
        else if (other.gameObject.tag == "SecondFloor") {
            if (secondF) {
                var newMask = oldMask & ~(1 << 10);
                newMask = newMask & ~(1 << 14);
                newMask = newMask | (1 << 9);
                newMask = newMask | (1 << 13);
                miniMap.cullingMask = newMask;
                groundF = true;
                secondF = false;
                ShowFurniture("Ground");
            }
            else {
                var newMask = oldMask & ~(1 << 13);
                newMask = newMask & ~(1 << 9);
                newMask = newMask | (1 << 10);
                newMask = newMask | (1 << 14);
                miniMap.cullingMask = newMask;
                groundF = false;
                secondF = true;
                ShowFurniture("Second");
            }
        }
        else if (other.gameObject.tag == "Entrance") {
                var newMask = oldMask & ~(1 << 8);
                newMask = newMask & ~(1 << 12);
                newMask = newMask | (1 << 9);
                newMask = newMask | (1 << 13);
                miniMap.cullingMask = newMask;
                groundF = true;
                ShowFurniture("Ground");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            done = false;
            ReloadPersonnel();
            menuController.HideRoomInfo();
        }
    }

    private void ShowFurniture(string furn) {
        if (furn == "Basement") {
            if (!basementFurniture.activeInHierarchy) {
                basementFurniture.SetActive(true);
                if (groundFurniture.activeInHierarchy) {
                    groundFurniture.SetActive(false);
                }
                if (secondFurniture.activeInHierarchy) {
                    secondFurniture.SetActive(false);
                }
            }
        }
        else if (furn == "Ground") {
            if (!groundFurniture.activeInHierarchy) {
                groundFurniture.SetActive(true);
                if (basementFurniture.activeInHierarchy) {
                    basementFurniture.SetActive(false);
                }
                if (secondFurniture.activeInHierarchy) {
                    secondFurniture.SetActive(false);
                }
            }
        }
        else if (furn == "Second") {
            if (!secondFurniture.activeInHierarchy) {
                secondFurniture.SetActive(true);
                if (basementFurniture.activeInHierarchy) {
                    basementFurniture.SetActive(false);
                }
                if (groundFurniture.activeInHierarchy) {
                    groundFurniture.SetActive(false);
                }
            }
        }
    }
}
