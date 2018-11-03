using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerBehaviour : MonoBehaviour {
    public AudioVisualiser av;
    public AudioMixer mixer;

    private void Start()
    {
        mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
    }

    private void Update()
    {
        //mixer.SetFloat("LowPassCutoff", av.amplitudeBuffer * 10000);
        //mixer.SetFloat("ChorusDryMix", av.amplitudeBuffer * 0.75f);
        //mixer.SetFloat("ChorusDepth", av.amplitudeBuffer * 0.75f);
    }
}
