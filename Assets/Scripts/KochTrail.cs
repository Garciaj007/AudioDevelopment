using System.Collections.Generic;
using UnityEngine;


public class KochTrail : KochGenerator {

    public class TrailObject
    {
        public GameObject G { get; set; }
        public TrailRenderer Trail { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Color EmissionColor { get; set; }
        public int CurrentTargetNum { get; set; }
    }

    public List<TrailObject> trails;

    [Header("Trail Properties")]
    public GameObject trailPrefab;
    public AnimationCurve trailWidthCurve;
    public Material trailMaterial;
    public Gradient trailColor;
    public float colorMultiplier;
    [Range(0, 8)]
    public int trailEndCapVerticies;

    [Header("Audio")]
    public AudioVisualiser av;
    public Vector2 speedMinMax, widthMinMax, trailTimeMinMax;
    public int[] audioBand;

    private float lerpPosSpeed, distanceSnap;
    private Color startColor, endColor;

	// Use this for initialization
	void Start () {

        startColor = new Color(0,0,0,0);
        endColor = new Color(0,0,0,1);
        trails = new List<TrailObject>();

        for(int i = 0; i < initatorPointAmount; i++)
        {
            GameObject trailInstance = Instantiate(trailPrefab, transform.position, Quaternion.identity, transform) as GameObject;
            TrailObject trailObjectInstance = new TrailObject();
            trailObjectInstance.G = trailInstance;
            trailObjectInstance.Trail = trailInstance.GetComponent<TrailRenderer>();
            trailObjectInstance.Trail.material = new Material(trailMaterial);
            trailObjectInstance.EmissionColor = trailColor.Evaluate(i * (1.0f / initatorPointAmount));
            trailObjectInstance.Trail.numCapVertices = trailEndCapVerticies;
            trailObjectInstance.Trail.widthCurve = trailWidthCurve;

            Vector3 instancePosition;
            if(generationCount > 0)
            {
                int step;
                if (useBezierCurves)
                {
                    step = bezierPosition.Length / initatorPointAmount;
                    instancePosition = bezierPosition[i * step];
                    trailObjectInstance.CurrentTargetNum = (i * step) + 1;
                    trailObjectInstance.TargetPosition = bezierPosition[trailObjectInstance.CurrentTargetNum];
                }
                else
                {
                    step = position.Length / initatorPointAmount;
                    instancePosition = position[i * step];
                    trailObjectInstance.CurrentTargetNum = (i * step) + 1;
                    trailObjectInstance.TargetPosition = position[trailObjectInstance.CurrentTargetNum];
                }
            }
            else
            {
                instancePosition = position[i];
                trailObjectInstance.CurrentTargetNum = i + 1;
                trailObjectInstance.TargetPosition = position[trailObjectInstance.CurrentTargetNum];
            }

            trailObjectInstance.G.transform.localPosition = instancePosition;
            trails.Add(trailObjectInstance);
        }
	}

    void Movement()
    {
        lerpPosSpeed = Mathf.Lerp(speedMinMax.x, speedMinMax.y, av.amplitude);

        for(int i = 0; i < trails.Count; i++)
        {
            distanceSnap = Vector3.Distance(trails[i].G.transform.localPosition, trails[i].TargetPosition);

            if (distanceSnap < 0.05f)
            {
                trails[i].G.transform.localPosition = trails[i].TargetPosition;

                if (useBezierCurves && generationCount > 0)
                {
                    if(trails[i].CurrentTargetNum < bezierPosition.Length - 1)
                        trails[i].CurrentTargetNum += 1;
                    else
                        trails[i].CurrentTargetNum = 1;

                    trails[i].TargetPosition = bezierPosition[trails[i].CurrentTargetNum];
                }
                else
                {
                    if (trails[i].CurrentTargetNum < position.Length - 1)
                        trails[i].CurrentTargetNum += 1;
                    else
                        trails[i].CurrentTargetNum = 1;

                    trails[i].TargetPosition = targetPosition[trails[i].CurrentTargetNum];
                }
            }
            trails[i].G.transform.localPosition = Vector3.MoveTowards(trails[i].G.transform.localPosition, trails[i].TargetPosition, Time.deltaTime * lerpPosSpeed);
        }
    }

    void AudioBehaviour()
    {
        for(int i = 0; i < initatorPointAmount; i++)
        {
            Color colorLerp = Color.Lerp(startColor, trails[i].EmissionColor * colorMultiplier, av.audioBandBuffer[audioBand[i]]);
            trails[i].Trail.material.SetColor("_EmissionColor", colorLerp);
            colorLerp = Color.Lerp(startColor, endColor, av.audioBandBuffer[audioBand[i]]);
            trails[i].Trail.material.SetColor("_Color", colorLerp);

            float widthLerp = Mathf.Lerp(widthMinMax.x, widthMinMax.y, av.audioBandBuffer[audioBand[i]]);
            trails[i].Trail.widthMultiplier = widthLerp;

            float timeLerp = Mathf.Lerp(trailTimeMinMax.x, trailTimeMinMax.y, av.audioBandBuffer[audioBand[i]]);
            trails[i].Trail.time = timeLerp;
        }
    }
	
	// Update is called once per frame
	void Update () {
        Movement();
        AudioBehaviour();
	}
}
