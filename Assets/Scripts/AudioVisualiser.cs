using UnityEngine;

public class AudioVisualiser : MonoBehaviour
{
    public float[] samplesLeft;
    public float[] samplesRight;
    public float[] freqBands;
    public float[] bandBuffer;
    public float[] audioBand;
    public float[] audioBandBuffer;
    public float amplitude, amplitudeBuffer; //RMS, RMSBuffered;
    public float audioProfile;

    AudioSource source;
    float[] bufferDecrease;
    float[] freqBandHighest;
    float amplitudeHighest;
    int I_NUMOFBANDS;
    const int FFT_SIZE = 512;

    public enum SourceType {AudioListener, AudioSource}
    public enum Channel { Stereo, Left, Right };
    public enum NumBands { x8, x64 };
    public SourceType e_sourceType = new SourceType();
    public Channel e_channel = new Channel();
    public NumBands e_numOfBands = new NumBands();

    private void Awake()
    {
        source = FindObjectOfType<AudioSource>();
    }

    // Use this for initialization
    void Start()
    {
        samplesLeft = new float[FFT_SIZE];
        samplesRight = new float[FFT_SIZE];

        if (e_numOfBands == NumBands.x8)
            I_NUMOFBANDS = 8;
        if (e_numOfBands == NumBands.x64)
            I_NUMOFBANDS = 64;

        freqBands = new float[64];
        bandBuffer = new float[64];
        bufferDecrease = new float[64];
        freqBandHighest = new float[64];
        audioBand = new float[64];
        audioBandBuffer = new float[64];

        AudioProfile(audioProfile);
    }

    // Update is called once per frame
    void Update()
    {
        if (e_numOfBands == NumBands.x8)
            I_NUMOFBANDS = 8;
        if (e_numOfBands == NumBands.x64)
            I_NUMOFBANDS = 64;

        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();
        //GetRMS();
    }

    void AudioProfile(float _audioProfile)
    {
        for (int i = 0; i < I_NUMOFBANDS; i++)
        {
            freqBandHighest[i] = _audioProfile;
        }
    }

    void GetSpectrumAudioSource()
    {
        if (e_sourceType == SourceType.AudioListener)
        {
            AudioListener.GetSpectrumData(samplesLeft, 0, FFTWindow.BlackmanHarris);
            AudioListener.GetSpectrumData(samplesRight, 1, FFTWindow.BlackmanHarris);
        } else
        {
            source.GetSpectrumData(samplesLeft, 0, FFTWindow.BlackmanHarris);
            source.GetSpectrumData(samplesRight, 1, FFTWindow.BlackmanHarris);
        }
    }

    void MakeFrequencyBands()
    {
        if (e_numOfBands == NumBands.x8)
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                float avg = 0;
                int sampleCount = (int)Mathf.Pow(2, i) * 2;
                if (i == 7)
                {
                    sampleCount += 2;
                }
                for (int j = 0; j < sampleCount; j++)
                {
                    if (e_channel == Channel.Stereo)
                        avg += (samplesLeft[count] + samplesRight[count]) * (count + 1);
                    if (e_channel == Channel.Left)
                        avg += samplesLeft[count] * (count + 1);
                    if (e_channel == Channel.Right)
                        avg += samplesRight[count] * (count + 1);
                    count++;
                }
                avg /= count;
                freqBands[i] = avg * 10;
            }
        }

        if (e_numOfBands == NumBands.x64)
        {
            int count = 0;
            int sampleCount = 1;
            int power = 0;

            for (int i = 0; i < 64; i++)
            {
                float avg = 0;
                if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
                {
                    power++;
                    sampleCount = (int)Mathf.Pow(2, power);
                    if (power == 3)
                        sampleCount -= 2;
                }
                for (int j = 0; j < sampleCount; j++)
                {
                    if (e_channel == Channel.Stereo)
                        avg += (samplesLeft[count] + samplesRight[count]) * (count + 1);
                    if (e_channel == Channel.Left)
                        avg += samplesLeft[count] * (count + 1);
                    if (e_channel == Channel.Right)
                        avg += samplesRight[count] * (count + 1);
                    count++;
                }
                avg /= count;
                freqBands[i] = avg * 80;
            }
        }
    }

    void BandBuffer()
    {
        for (int i = 0; i < I_NUMOFBANDS; i++)
        {
            if (freqBands[i] > bandBuffer[i])
            {
                bandBuffer[i] = freqBands[i];
                bufferDecrease[i] = 0.005f;
            }

            if (freqBands[i] < bandBuffer[i])
            {
                bandBuffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f;
            }
        }
    }

    void CreateAudioBands()
    {

        for (int i = 0; i < I_NUMOFBANDS; i++)
        {
            if (freqBands[i] > freqBandHighest[i])
            {
                freqBandHighest[i] = freqBands[i];
            }
            audioBand[i] = (freqBands[i] / freqBandHighest[i]);
            audioBandBuffer[i] = (bandBuffer[i] / freqBandHighest[i]);
        }

    }

    void GetAmplitude()
    {
        float currentAmplitude = 0, currentAmplitudeBuffer = 0;

        for (int i = 0; i < I_NUMOFBANDS; i++)
        {
            currentAmplitude += audioBand[i];
            currentAmplitudeBuffer += audioBandBuffer[i];
        }

        if (currentAmplitude > amplitudeHighest)
        {
            amplitudeHighest = currentAmplitude;
        }
        amplitude = currentAmplitude / amplitudeHighest;
        amplitudeBuffer = currentAmplitudeBuffer / amplitudeHighest;
    }

    /* In development
    void GetRMS()
    {
        RMS = 0;
        RMSBuffered = 0;

        for (int i = 0; i < amplitude; i++)
        {
            if (channel == Channel.Stereo)
                RMS += Mathf.Pow(samplesLeft[i] + samplesRight[i], 2);
            if (channel == Channel.Left)
                RMS += Mathf.Pow(samplesLeft[i], 2);
            if (channel == Channel.Right)
                RMS += Mathf.Pow(samplesRight[i], 2);
        }

        for(int i = 0; i < amplitudeBuffer; i++)
        {
            if (channel == Channel.Stereo)
                RMS += Mathf.Pow(samplesLeft[i] + samplesRight[i], 2);
            if (channel == Channel.Left)
                RMS += Mathf.Pow(samplesLeft[i], 2);
            if (channel == Channel.Right)
                RMS += Mathf.Pow(samplesRight[i], 2);
        }

        RMS /= FFT_SIZE;
        RMS = Mathf.Sqrt(RMS);
    }
    */
}
