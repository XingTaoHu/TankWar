using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

    private void OnGUI()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (GUI.Button(new Rect(50, 50, 100, 50), "播放"))
        {
            audioSource.Play();
        }
        if (GUI.Button(new Rect(200, 50, 100, 50), "停止"))
        {
            audioSource.Stop();
        }
    }
}
