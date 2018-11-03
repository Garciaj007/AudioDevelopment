using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attract : MonoBehaviour {

    Rigidbody _rigidbody;
    public Transform attractTo;
    public float strengthOfAttraction, maxMagnitude;

	// Use this for initialization
	void Start () {
        _rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if(attractTo != null)
        {
            Vector3 direction = attractTo.position - transform.position;
            _rigidbody.AddForce(strengthOfAttraction * direction);

            if(_rigidbody.velocity.magnitude > maxMagnitude)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * maxMagnitude;
            }
        }
	}
}
