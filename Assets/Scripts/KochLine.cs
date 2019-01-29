using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KochLine : KochGenerator {

    //[Range(0.0f, 1.0f)]
    //public float lerpAmount;
    public float generateMultiplier;

    [Header("Audio")]
    public AudioVisualiser av;
    public int[] audioBand;
    public Material material;
    public Color color;
    public int audioBandMaterial;
    public float emissionMultiplier;

    private Vector3[] lerpPositions;
    private float[] lerpAudio;
    private LineRenderer lineRenderer;
    private Material matInstance;

	// Use this for initialization
	void Start () {
        lerpAudio = new float[initatorPointAmount];

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = position.Length;
        lineRenderer.SetPositions(position);
        lineRenderer.enabled = true;

        lerpPositions = new Vector3[position.Length];

        matInstance = new Material(material);

        lineRenderer.material = matInstance;
    }
	
	// Update is called once per frame
	void Update () {
        matInstance.SetColor("_EmissionColor", color * av.audioBandBuffer[audioBandMaterial] * emissionMultiplier);

        if(generationCount != 0)
        {
            int count = 0;
            for(int i = 0; i < initatorPointAmount; i++)
            {
                lerpAudio[i] = av.audioBandBuffer[audioBand[i]];
                for(int j = 0; j < (position.Length - 1) / initatorPointAmount; j++)
                {
                    lerpPositions[count] = Vector3.Lerp(position[count], targetPosition[count], lerpAudio[i]);
                    count++;
                }
            }
            lerpPositions[count] = Vector3.Lerp(position[count], targetPosition[count], lerpAudio[initatorPointAmount - 1]);

            if (useBezierCurves)
            {
                bezierPosition = BezierCurve(lerpPositions, bezierVertexCount);
                lineRenderer.positionCount = bezierPosition.Length;
                lineRenderer.SetPositions(bezierPosition);
            }
            else
            {
                lineRenderer.positionCount = lerpPositions.Length;
                lineRenderer.SetPositions(lerpPositions);
            }
        }
	}
}
