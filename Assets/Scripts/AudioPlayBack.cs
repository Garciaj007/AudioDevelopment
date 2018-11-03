using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayBack : MonoBehaviour {

    public AudioSource source;
    public float playheadTime;

    private void OnDrawGizmos()
    {
        source = GetComponent<AudioSource>();

        if(playheadTime >= source.clip.length)
        {
            throw new System.Exception("Playhead Start value exceeds songs length, please choose a shorter length");
        }
    }

    private void Start()
    {
        if(playheadTime < source.clip.length)
        source.time = playheadTime;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (source.isPlaying)
            {
                source.Pause();
            } else
            {
                source.UnPause();
            }
        }
	}
}
