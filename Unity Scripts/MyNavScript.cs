using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class MyNavScript : MonoBehaviour {


    [SerializeField] float rotationSpeed;
    [SerializeField] TextMeshProUGUI roomName;
    [SerializeField] TextMeshProUGUI roomDesc;
    [SerializeField] MenuController menuController;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject personPrefab;
    [SerializeField] Transform activePanel;
    [SerializeField] Transform inactivePanel;
    [SerializeField] Camera miniMap;
    [SerializeField] GameObject basementFurniture;
    [SerializeField] GameObject groundFurniture;
    [SerializeField] GameObject secondFurniture;
    private int oldMask;
    private MyRoomData roomData;
    private Transform target;
    private List<GameObject> personObjects;
    private bool groundF = true;
    private bool secondF = false;

    private void Start() {
        personObjects = new List<GameObject>();
        oldMask = miniMap.cullingMask;
    }

    // Update is called once per frame
    void Update () {
        if (target != null) {    
            if (!agent.pathPending) {
                if (agent.remainingDistance <= agent.stoppingDistance) {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                        RotateToTarget(target);
                    }
                }
            }
        }
    }

    public void MoveToDestination(Transform target, MyRoomData data) {
        ReloadPersonnel();
        agent.isStopped = false;
        this.target = target;
        this.roomData = data;
        roomName.text = data.room_name;
        roomDesc.text = data.room_desc;
        agent.SetDestination(target.position);
        if (CheckPersonnelCount(data)) {
            menuController.ShowPersonnelPanel();
            Assignpersonnel(data);
        }
        else {
            menuController.HidePersonnelPanel();
        }
    }

    public void GetPersonnelList(Personnel personList) {
        if (personList.personnel.Length > 0) {
            foreach (MyPersonData person in personList.personnel) {
                GameObject newPerson = (GameObject)GameObject.Instantiate(personPrefab);
                newPerson.transform.SetParent(inactivePanel, false);

                PersonPrefab prefab = newPerson.GetComponent<PersonPrefab>();
                prefab.Setup(person);
                personObjects.Add(newPerson);
            }
        }
    }

    private void Assignpersonnel(MyRoomData roomData) {
        foreach (GameObject person in personObjects) {
            var personData = person.GetComponent<PersonPrefab>().data;
            if(roomData.room_name == personData.assigned_office_name) {
                person.transform.SetParent(activePanel, false);
            }
        }
    }

    private bool CheckPersonnelCount(MyRoomData data) {
        int count = 0;
        foreach (GameObject person in personObjects) {
            var personData = person.GetComponent<PersonPrefab>().data;
            if(data.room_name == personData.assigned_office_name) {
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
            if (roomData.id == other.gameObject.GetComponent<RoomObject>().data.id) {
                menuController.ShowRoomInfo();
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
            secondF = false;
            ShowFurniture("Ground");
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            if (!menuController.CheckRoomInfoActive() && !menuController.CheckRoomListActive() && menuController.CheckCancelMenu() &&
                roomData.id == other.gameObject.GetComponent<RoomObject>().data.id && menuController.RoomInfoAnimationState()) {
                menuController.ShowRoomInfo();
            }
        }
    }

    private void RotateToTarget(Transform target) {
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction.x != 0 && direction.z != 0) {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }     
    }

    public void SetTargetNull() {
        target = null;
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
