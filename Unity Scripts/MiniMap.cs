using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MiniMap : MonoBehaviour {

    [SerializeField] GameObject fpsCamera;
    [SerializeField] GameObject navCamera;
    [SerializeField] float offset_y = 10f;
    [SerializeField] Camera miniMap;
    private int oldMask;
    private bool nav = true;
    private GameObject target;

	// Use this for initialization
	void Start () {
        oldMask = miniMap.cullingMask;
        target = navCamera;
	}
	
	// Update is called once per frame
	void Update () {
        if (nav) {
            target = navCamera;
            transform.position = target.transform.position + Vector3.up * offset_y;
        }
        else {
            target = fpsCamera;
            transform.position = target.transform.position + Vector3.up * offset_y;
        }
	}

    public void ChangeTarget(bool nav) {
        this.nav = nav;
        miniMap.cullingMask = oldMask;
    }
}
