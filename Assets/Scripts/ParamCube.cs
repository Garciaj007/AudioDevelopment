using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour {

    public AudioVisualiser av;
    public int band;
    public float startScale, scaleMultiplier;
    public bool useBuffer;
    public bool useAudioBand;
    Material mat;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().materials[0];
        //av.e_numOfBands = AudioVisualiser.NumBands.x8;
    }

    // Update is called once per frame
    void Update () {
        if (useBuffer && useAudioBand)
        {
            transform.localScale = new Vector3(transform.localScale.x, (av.audioBandBuffer[band] * scaleMultiplier) + startScale, transform.localScale.z);
            Color color = new Color(av.audioBandBuffer[band] * 1.2f, av.audioBandBuffer[band] * 1.2f, av.audioBandBuffer[band] * 1.2f);
            mat.SetColor("_EmissionColor", color);
        }

        if (!useBuffer && useAudioBand)
        {
            transform.localScale = new Vector3(transform.localScale.x, (av.audioBand[band] * scaleMultiplier) + startScale, transform.localScale.z);
            Color color = new Color(av.audioBand[band] * 1.2f, av.audioBand[band] * 1.2f, av.audioBand[band]* 1.2f);
            mat.SetColor("_EmissionColor", color);
        }

        ///<remarks>Dont use this code as it can contains values that exceed the emmission map</remarks>
        if (useBuffer && !useAudioBand)
        {
            transform.localScale = new Vector3(transform.localScale.x, (av.bandBuffer[band] * scaleMultiplier) + startScale, transform.localScale.z);
            Color color = new Color(av.bandBuffer[band], av.bandBuffer[band], av.bandBuffer[band]);
            mat.SetColor("_EmissionColor", color);
        }

        ///<remarks>Dont use this code as it can contains values that exceed the emmission map</remarks>
        if (!useBuffer && !useAudioBand)
        {
            transform.localScale = new Vector3(transform.localScale.x, (av.freqBands[band] * scaleMultiplier) + startScale, transform.localScale.z);
            Color color = new Color(av.freqBands[band], av.freqBands[band], av.freqBands[band]);
            mat.SetColor("_EmissionColor", color);
        }
    }
}
