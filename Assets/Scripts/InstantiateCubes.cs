using UnityEngine;

public class InstantiateCubes : MonoBehaviour {

    public GameObject _sampleCubePrefab;
    public AudioVisualiser av;
    public float _maxScale;

    GameObject[] _sampleCubes;

    const float ANGLE = 360f / 512f;

	// Use this for initialization
	void Start () {
        _sampleCubes = new GameObject[512];

		for(int i = 0; i < 512; i++)
        {
            GameObject cube = Instantiate(_sampleCubePrefab);
            cube.transform.position = transform.position;
            cube.transform.parent = transform;
            cube.name = "Cube" + i;
            cube.layer = 9;
            transform.eulerAngles = new Vector3(0, -ANGLE * i, 0);
            cube.transform.position = Vector3.forward * 150;
            _sampleCubes[i] = cube;
        }
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < 512; i++)
        {
            if(_sampleCubes != null)
            {
                _sampleCubes[i].transform.localScale = new Vector3(1, (av.samplesLeft[i] * _maxScale) + 2f, 1);
            }
        }
	}
}
