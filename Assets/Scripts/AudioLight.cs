using UnityEngine;

[RequireComponent (typeof(Light))]
public class AudioLight : MonoBehaviour {

    public AudioVisualiser av;
    public int band;
    public float minIntensity, maxIntensity;

    new Light light;

	// Use this for initialization
	void Start () {
        light = GetComponent<Light>();
        //av.e_numOfBands = AudioVisualiser.NumBands.x8;
	}
	
	// Update is called once per frame
	void Update () {
        light.intensity = (av.audioBandBuffer[band] * (maxIntensity - minIntensity) + minIntensity);
	}
}
