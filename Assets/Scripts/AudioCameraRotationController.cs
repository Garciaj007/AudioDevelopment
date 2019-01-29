using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCameraRotationController : MonoBehaviour {

    public AudioVisualiser av;
    public Vector3 rotateAxis, rotateSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.GetChild(0).transform.LookAt(transform);
        transform.Rotate(rotateAxis.x * rotateSpeed.x * Time.deltaTime * av.amplitudeBuffer, rotateAxis.y * rotateSpeed.y * Time.deltaTime * av.amplitudeBuffer, rotateAxis.z * rotateSpeed.z * Time.deltaTime * av.amplitudeBuffer);
	}
}
