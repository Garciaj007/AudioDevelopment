using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PluginEditor : MonoBehaviour {

    public AudioMixer mixer;

    public void Start()
    {
        mixer.SetFloat("Gain", 0.0f);
    }

    public void SliderChanged(Slider s)
    {
        mixer.SetFloat("Gain", Mathf.Pow(10, (s.value / 20.0f)));
    }
     
}
