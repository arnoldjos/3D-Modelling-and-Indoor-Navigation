using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MenuController : MonoBehaviour {

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject cancelPanel;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject explorePerson;
    [SerializeField] private GameObject navigatePerson;
    [SerializeField] private GameObject entranceLookat;
    [SerializeField] private Toggle roomButton;
    [SerializeField] private Toggle collegeButton;
    [SerializeField] private Toggle serviceButton;
    [SerializeField] private Button exploreButton;
    [SerializeField] private Button entranceButton;
    [SerializeField] private Animator mainCanvasAnimator;
    [SerializeField] private Animator roomAnimator;
    [SerializeField] private Animator enterAnimator;
    [SerializeField] private UIController roomControl;
    [SerializeField] private UIController searchControl;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject entrance;
    [SerializeField] private MiniMap miniMap;
    [SerializeField] private Transform exploreCam;
    [SerializeField] CanvasGroup personnelPanel;

    public void DisableMenu() {
        background.SetActive(true);
    }

    public void EnableMenu() {
        background.SetActive(false);
    }

    public void RoomButtonToggle() {
        collegeButton.interactable = false;
        serviceButton.interactable = false;
        exploreButton.interactable = false;
        entranceButton.interactable = false;
    }

    public void CollegeButtonToggle() {
        roomButton.interactable = false;
        serviceButton.interactable = false;
        exploreButton.interactable = false;
        entranceButton.interactable = false;
    }

    public void ServiceButtonToggle() {
        roomButton.interactable = false;
        collegeButton.interactable = false;
        exploreButton.interactable = false;
        entranceButton.interactable = false;
    }

    public void EnableAllMenuButtons() {
        roomButton.interactable = true;
        collegeButton.interactable = true;
        serviceButton.interactable = true;
        exploreButton.interactable = true;
        entranceButton.interactable = true;
    }

    public void HideMainCanvas() {
        roomControl.Hide();
        mainCanvasAnimator.SetTrigger("HideCanvas");
    }

    public void ShowMainCanvas() {
        roomControl.Show();
        mainCanvasAnimator.SetTrigger("ShowCanvas");
    }


    public void HidePersonnelPanel() {
        personnelPanel.alpha = 0;
        personnelPanel.interactable = false;
        personnelPanel.blocksRaycasts = false;
    }

    public void ShowPersonnelPanel() {
        personnelPanel.alpha = 1;
        personnelPanel.interactable = true;
        personnelPanel.blocksRaycasts = true;
    }

    public void ShowRoomInfo() {
        roomAnimator.SetTrigger("FadeIn");
    }

    public void HideRoomInfo() {
        roomAnimator.SetTrigger("FadeOut");
    }

    public void BackButton() {
        agent.isStopped = true;
        if (roomAnimator.gameObject.GetComponent<CanvasGroup>().alpha > 0) {
            HideRoomInfo();
        }
        ShowMainCanvas();
    }

    public bool CheckRoomInfoActive() {
        if (roomAnimator.gameObject.GetComponent<CanvasGroup>().alpha == 0) {
            return false;
        }
        return true;
    }

    public bool CheckRoomListActive() {
        if (!roomControl.isShow) {
            return false;
        }
        return true;
    }

    public bool CheckCancelMenu() {
        if (cancelPanel.GetComponent<CanvasGroup>().alpha == 0){
            return false;
        }
        return true;
    }

    public bool RoomInfoAnimationState() {
        if (roomAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !roomAnimator.IsInTransition(0)) {
            return true;
        }
        return false;
    }

    public void ActivateFreeRoam() {
        exploreCam.transform.localRotation = Quaternion.Euler(0, 0, 0);
        navigatePerson.SetActive(false);
        explorePerson.SetActive(true);
        BackToEntrance();
        miniMap.ChangeTarget(false);
    }
    public void DeactivateFreeRoam() {
        explorePerson.transform.position = entrance.transform.position;
        explorePerson.transform.rotation = Quaternion.Euler(0, -177.716f, 0);      
        explorePerson.SetActive(false);
        navigatePerson.SetActive(true);
        miniMap.ChangeTarget(true);       
    }

    public void CheckSearchActive() {
        if (searchControl.isShow) {
            searchControl.Hide();
        }
    }

    public void BackToEntrance() {
        if (agent.pathPending) {
            agent.isStopped = true;
        }      
        agent.Warp(entrance.transform.position);
        agent.transform.rotation = Quaternion.Euler(0, -177.716f, 0);
    }

    public void StopAgent() {
        agent.isStopped = true;
    }

    public void EnterMenu() {
        enterAnimator.SetTrigger("Enter");
    }
}
