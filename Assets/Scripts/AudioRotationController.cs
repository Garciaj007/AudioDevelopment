using UnityEngine;

public class AudioRotationController : MonoBehaviour {

    public AudioVisualiser av;
    [Header("Rotation Axis")]
    public bool x;
    public bool y;
    public bool z;
    [Range(-360.0f, 360.0f)]
    public float rotationSpeed;

    private Vector3 axis;
	
	// Update is called once per frame
	void Update () {
        axis = new Vector3(x == true ? 1 : 0, y == true ? 1 : 0, z == true ? 1 : 0);
        float angle = av.amplitudeBuffer * rotationSpeed * Mathf.Deg2Rad;
        transform.Rotate(axis, angle);
	}
}
