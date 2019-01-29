using UnityEngine;

public class PhyllotaxisTunnelController : MonoBehaviour {

    public Transform tunnel, cam;
    public AudioVisualiser audioVisualiser;
    public float tunnelSpeed, cameraDistance;
    [Header("Rotation Animation")]
    public bool animateRotation;
    [Range(-1.0f, 1.0f)]
    public float rotationSpeed = 0;

    private float angle = 0;
	
	// Update is called once per frame
	void Update () {

        if (animateRotation)
        {
            angle += audioVisualiser.amplitude;
            tunnel.Rotate(new Vector3(0, 0, 1), rotationSpeed * angle * Mathf.Deg2Rad);
        }

        tunnel.position = new Vector3(tunnel.position.x, tunnel.position.y, tunnel.position.z + audioVisualiser.amplitudeBuffer * tunnelSpeed);
        cam.position = new Vector3(cam.position.x, cam.position.y, tunnel.position.z + cameraDistance);
	}
}
