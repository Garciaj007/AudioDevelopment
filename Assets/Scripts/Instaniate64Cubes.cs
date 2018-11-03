using UnityEngine;

public class Instaniate64Cubes : MonoBehaviour {

    public GameObject samplePrefabCube;
    public AudioVisualiser av;
    public float maxScale;
    public Color c;
    Material[] mat;
    float hue;

    GameObject[] sampleCubes;

	// Use this for initialization
	void Start () {
        sampleCubes = new GameObject[64];
        mat = new Material[64];
        hue = 0;

        for(int i = 0; i < 64; i++)
        {
            GameObject cube = Instantiate(samplePrefabCube);
            cube.transform.position = transform.position;
            cube.transform.parent = transform;
            cube.name = "Cube64 " + i;
            cube.transform.position = Vector3.right * i * 2 * 10;
            sampleCubes[i] = cube;

            mat[i] = cube.GetComponent<MeshRenderer>().materials[0];
        }

        transform.position = new Vector3(-64 * 2 * 10, 0, 0);
        transform.localScale = new Vector3(2, 2, 2);
	}
	
	// Update is called once per frame
	void Update () {
        c = Color.HSVToRGB(hue, 1, 1);

        if (hue > 1)
            hue = 0;

		for(int i = 0; i < 64; i++)
        {
            if(sampleCubes != null)
            {
                sampleCubes[i].transform.localScale = new Vector3(5, (av.audioBandBuffer[i] * maxScale + 2f), 5);
                Color color = new Color( c.r * av.audioBandBuffer[i], c.g * av.audioBandBuffer[i], c.b * av.audioBandBuffer[i]);
                mat[i].SetColor("_EmissionColor", color);
            }
        }
        hue += av.amplitudeBuffer * 0.01f;
	}
}
