using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicAttraction : MonoBehaviour
{
    public GameObject atom, attractor;
    public AudioVisualiser av;
    public Gradient gradient;
    public Material atomMaterial;
    public AnimationCurve animCurve;
    public Vector3 spacingDirection;
    public Vector3 destination;
    public Vector2 atomScaleMinMax;
    public int[] attractPoints;
    public int posAnimBand;
    [Range(0, 80)]
    public int amountOfAtomsPerAttractor;
    [Range(0, 100)]
    public float spacingBetweenAttractPoints;
    [Range(0, 50)]
    public float scaleAttractPoints;
    [Range(0, 1)]
    public float tresholdEmission;
    public float strengthOfAttraction, maxMagnitude, randomPosDistance, audioScaleMultiplier, audioEmissionMultiplier, animSpeed;
    public bool useGravity;

    public enum AnimPosition {NoAnimation, Animate, Buffered};
    public AnimPosition e_animatePosition = new AnimPosition();
    public enum AnimAttractor {NoAnimation, Animate, Buffered};
    public AnimAttractor e_animateAttractor = new AnimAttractor();
    public enum EmmisionThreshold {Buffered, NoBuffer};
    public EmmisionThreshold e_emmisionThreshold = new EmmisionThreshold();
    public enum EmmisionColor { Buffered, NoBuffer };
    public EmmisionColor e_emmisionColor = new EmmisionColor();
    public enum AtomScale { Buffered, NoBuffer };
    public AtomScale e_atomScale = new AtomScale();

    GameObject[] attractors, atoms;
    Material[] sharedMaterial;
    Color[] sharedColor;
    Vector3 startPoint;

    float animTimer;
    float[] atomScale;
    float[] audioBandEmissionThreshold;
    float[] audioBandEmissionColor;
    float[] audioBandScale;


    private void OnDrawGizmos()
    {
        for (int i = 0; i < attractPoints.Length; i++)
        {
            float evaluteStep = 0.125f;
            Color color = gradient.Evaluate(Mathf.Clamp(evaluteStep * attractPoints[i], 0, 7));
            Gizmos.color = color;

            Vector3 pos = new Vector3(transform.position.x + (spacingBetweenAttractPoints * i * spacingDirection.x), transform.position.y + (spacingBetweenAttractPoints * i * spacingDirection.y), transform.position.z + (spacingBetweenAttractPoints * i * spacingDirection.z));

            Gizmos.DrawSphere(pos, scaleAttractPoints);
        }

        Gizmos.color = new Color(1, 1, 1);
        Vector3 startpoint = transform.localPosition;
        Vector3 endPoint = transform.localPosition + destination;
        Gizmos.DrawLine(startPoint, endPoint);
    }

    // Use this for initialization
    void Start()
    {
        startPoint = transform.position;
        attractors = new GameObject[attractPoints.Length];
        atoms = new GameObject[attractPoints.Length * amountOfAtomsPerAttractor];
        atomScale = new float[attractPoints.Length * amountOfAtomsPerAttractor];

        audioBandEmissionThreshold = new float[8];
        audioBandEmissionColor = new float[8];
        audioBandScale = new float[8];

        sharedMaterial = new Material[8];
        sharedColor = new Color[8];

        int countAtom = 0;

        for(int i = 0; i < attractPoints.Length; i++)
        {
            GameObject attractorInstance = Instantiate(attractor);
            attractors[i] = attractorInstance;

            attractorInstance.transform.position = new Vector3(
                transform.position.x + (spacingBetweenAttractPoints * i * spacingDirection.x),
                transform.position.y + (spacingBetweenAttractPoints * i * spacingDirection.y),
                transform.position.z + (spacingBetweenAttractPoints * i * spacingDirection.z));

            attractorInstance.transform.parent = transform;
            attractorInstance.transform.localScale = new Vector3(scaleAttractPoints, scaleAttractPoints, scaleAttractPoints);

            //colors
            Material matInstance = new Material(atomMaterial);
            matInstance.name = "MaterialInstance" + i;
            sharedMaterial[i] = matInstance;
            sharedColor[i] = gradient.Evaluate(0.125f * i);

            for(int j = 0; j < amountOfAtomsPerAttractor; j++)
            {
                GameObject atomInstance = Instantiate(atom);
                atoms[countAtom] = atomInstance;
                atomInstance.GetComponent<Attract>().attractTo = attractors[i].transform;
                atomInstance.GetComponent<Attract>().strengthOfAttraction = strengthOfAttraction;
                atomInstance.GetComponent<Attract>().maxMagnitude = maxMagnitude;
                if (useGravity)
                {
                    atomInstance.GetComponent<Rigidbody>().useGravity = true;
                } else
                {
                    atomInstance.GetComponent<Rigidbody>().useGravity = false;
                }

                atomInstance.transform.position = new Vector3(
                    attractors[i].transform.position.x + Random.Range(-scaleAttractPoints - randomPosDistance, scaleAttractPoints + randomPosDistance),
                    attractors[i].transform.position.y + Random.Range(-scaleAttractPoints - randomPosDistance, scaleAttractPoints + randomPosDistance),
                    attractors[i].transform.position.z + Random.Range(-scaleAttractPoints - randomPosDistance, scaleAttractPoints + randomPosDistance));

                float randomScale = Random.Range(atomScaleMinMax.x, atomScaleMinMax.y);
                atomScale[countAtom] = randomScale;
                atomInstance.transform.localScale = new Vector3(atomScale[countAtom], atomScale[countAtom], atomScale[countAtom]);
                atomInstance.transform.parent = transform.parent;

                atomInstance.GetComponent<MeshRenderer>().material = sharedMaterial[i];

                countAtom++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        SelectAudioValues();
        AtomBehaviour();
        AttractorBehaviour();
        AnimatePosition();
    }

    void SelectAudioValues()
    {
        if(e_emmisionThreshold == EmmisionThreshold.Buffered)
        {
            for(int i = 0; i < 8; i++)
            {
                audioBandEmissionThreshold[i] = av.audioBandBuffer[i];
            }
        }
        if (e_emmisionThreshold == EmmisionThreshold.NoBuffer)
        {
            for (int i = 0; i < 8; i++)
            {
                audioBandEmissionThreshold[i] = av.audioBand[i];
            }
        }
        if (e_emmisionColor == EmmisionColor.Buffered)
        {
            for (int i = 0; i < 8; i++)
            {
                audioBandEmissionColor[i] = av.audioBandBuffer[i];
            }
        }
        if (e_emmisionColor == EmmisionColor.NoBuffer)
        {
            for (int i = 0; i < 8; i++)
            {
                audioBandEmissionColor[i] = av.audioBand[i];
            }
        }
        if (e_atomScale == AtomScale.Buffered)
        {
            for (int i = 0; i < 8; i++)
            {
                audioBandScale[i] = av.audioBandBuffer[i];
            }
        }
        if (e_atomScale == AtomScale.NoBuffer)
        {
            for (int i = 0; i < 8; i++)
            {
                audioBandScale[i] = av.audioBand[i];
            }
        }
    }

    void AnimatePosition()
    {
        if (e_animatePosition != AnimPosition.NoAnimation)
        {
            if (e_animatePosition == AnimPosition.Buffered)
            {
                animTimer += Time.deltaTime * av.audioBandBuffer[posAnimBand] * animSpeed;
            }
            else 
            {
                animTimer += Time.deltaTime * av.audioBand[posAnimBand] * animSpeed;
            }
            if(animTimer >= 1)
            {
                animTimer -= 1f;
            }
            

            float alphaTime = animCurve.Evaluate(animTimer);
            Vector3 endpoint = destination + startPoint;
            transform.position = Vector3.Lerp(startPoint, endpoint, alphaTime);
        }
    }

    void AttractorBehaviour()
    {
        if (e_animateAttractor != AnimAttractor.NoAnimation)
        {
            for (int i = 0; i < 8; i++)
            {
                if (e_animateAttractor == AnimAttractor.Buffered)
                {
                    attractors[i].transform.localScale = new Vector3(av.audioBandBuffer[i] * scaleAttractPoints, av.audioBandBuffer[i] * scaleAttractPoints, av.audioBandBuffer[i] * scaleAttractPoints) * 50;
                } else
                {
                    attractors[i].transform.localScale = new Vector3(av.audioBand[i] * scaleAttractPoints, av.audioBand[i] * scaleAttractPoints, av.audioBand[i] * scaleAttractPoints) * 50;
                }
            }
        }
    }

    void AtomBehaviour()
    {
        int countAtom = 0; 
        for(int i = 0; i < attractPoints.Length; i++)
        {
            if(audioBandEmissionThreshold[attractPoints[i]] >= tresholdEmission)
            {
                Color audioColor = new Color(
                    sharedColor[i].r * audioBandEmissionColor[attractPoints[i]] * audioEmissionMultiplier,
                    sharedColor[i].g * audioBandEmissionColor[attractPoints[i]] * audioEmissionMultiplier,
                    sharedColor[i].b * audioBandEmissionColor[attractPoints[i]] * audioEmissionMultiplier, 1);
                sharedMaterial[i].SetColor("_EmissionColor", audioColor);
            }
            else
            {
                Color audioColor = new Color(0, 0, 0, 1);
                sharedMaterial[i].SetColor("_EmisionColor", audioColor);
            }
            for(int j = 0; j < amountOfAtomsPerAttractor; j++)
            {
                atoms[countAtom].transform.localScale = new Vector3(
                    atomScale[countAtom] + audioBandScale[attractPoints[i]] * audioScaleMultiplier,
                    atomScale[countAtom] + audioBandScale[attractPoints[i]] * audioScaleMultiplier,
                    atomScale[countAtom] + audioBandScale[attractPoints[i]] * audioScaleMultiplier);
                countAtom++;
            }
        }
    }
}
