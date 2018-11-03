using UnityEngine;

public class ScaleAmplitude : MonoBehaviour {

    public AudioVisualiser av;
    public float startScale, maxScale;
    public bool useBuffer;
    public Color color;
    Material mat;

	// Use this for initialization
	void Start () {
        mat = GetComponent<MeshRenderer>().materials[0];
        //av.e_numOfBands = AudioVisualiser.NumBands.x8;
	}
	
	// Update is called once per frame
	void Update () {
        if (!useBuffer)
        {
            transform.localScale = new Vector3((av.amplitude * maxScale) + startScale, (av.amplitude * maxScale) + startScale, (av.amplitude * maxScale) + startScale);
            Color _color = new Color(color.r * av.amplitude, color.g * av.amplitude, color.b * av.amplitude);
            mat.SetColor("_EmissionColor", _color);
        }

        if (useBuffer)
        {
            transform.localScale = new Vector3((av.amplitudeBuffer * maxScale) + startScale, (av.amplitudeBuffer * maxScale) + startScale, (av.amplitudeBuffer * maxScale) + startScale);
            Color _color = new Color(color.r * av.amplitudeBuffer, color.g * av.amplitudeBuffer, color.b * av.amplitudeBuffer);
            mat.SetColor("_EmissionColor", _color);
        }
    }
}
