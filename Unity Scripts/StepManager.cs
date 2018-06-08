using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class StepManager : MonoBehaviour {

    private int number = 0;
    [SerializeField] TextMeshProUGUI step_number;
    [SerializeField] TextMeshProUGUI step_desc;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject nextButton;
    [SerializeField] MenuController menuControls;
    Vector3 target;
    private Queue<MyStepsData> steps;
    private MyStepsData current;


    private void Update() {
        if (number > 0) {
            if (!agent.pathPending) {
                if (agent.remainingDistance <= agent.stoppingDistance) {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                        nextButton.GetComponent<Button>().interactable = true;
                        if (current.step_type == "Direction") {
                            RotateToTarget(target);
                        }                     
                    }
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        steps = new Queue<MyStepsData>();
	}


    public void StartService (MyServiceData data) {
        menuControls.BackToEntrance();
        nextButton.SetActive(true);
        nextButton.GetComponent<Button>().interactable = true;
        steps.Clear();
        foreach(MyStepsData step in data.steps) {
            steps.Enqueue(step);
        }
        step_number.text = data.service_name;
        step_desc.text = data.service_desc;
    }

    public void DisplayNextStep() {
        if (steps.Count == 0) {
            number = 0;
            StopAgent();
            step_number.text = "";
            nextButton.SetActive(false);
            step_desc.text = "Thank you! Please Come Again";
            return;
        }
        number++;
        step_number.text = "Step " + number;
        current = steps.Dequeue();
        step_desc.text = current.step_desc;
        //StartCoroutine(ChangeText());
        if (current.step_type == "Direction") {
            agent.isStopped = false;
            target = new Vector3(current.xCoord, current.yCoord, current.zCoord);
            agent.destination = target;
        }
        else {
            agent.isStopped = true;
        }
        nextButton.GetComponent<Button>().interactable = false;
    }

    IEnumerator ChangeText() {
        step_desc.text = "";
        foreach(char letter in current.step_desc.ToCharArray()) {
            step_desc.text += letter;
            yield return null;
        }
    }

    private void RotateToTarget(Vector3 target) {
        Vector3 direction = (target - agent.transform.position).normalized;
        if (direction.x != 0 && direction.z != 0) {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * 2.5f);
        }
    }

    public void StopAgent() {
        agent.isStopped = true;
        number = 0;
    }
}
