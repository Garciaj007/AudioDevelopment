using UnityEngine;
using UnityEngine.Audio;

[RequireComponent (typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour {

    public bool useMicrophone;
    public string selectedDevice;
    public AudioMixerGroup mixerGroupMicrophone, mixerGroupMaster;

    AudioSource source;

    #pragma warning disable
    int minSamplingFreq, maxSamplingFreq;
    #pragma warning restore

    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();

        if (useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {
                selectedDevice = Microphone.devices[0];
                source.outputAudioMixerGroup = mixerGroupMicrophone;
                Microphone.GetDeviceCaps(selectedDevice, out minSamplingFreq, out maxSamplingFreq);
                source.clip = Microphone.Start(selectedDevice, false, 1000, maxSamplingFreq);
                source.Play();
            }
            else
            {
                useMicrophone = false;
                throw new System.Exception("No Microphone Devices Detected.");
            }
        } else
        {
            source.outputAudioMixerGroup = mixerGroupMaster;
        }
    }

    private void Update()
    {
        if (!Microphone.IsRecording(selectedDevice))
        {
            source.Stop();
            source.clip = Microphone.Start(selectedDevice, false, 10, maxSamplingFreq);
            source.Play();
        }
    }
   
}
