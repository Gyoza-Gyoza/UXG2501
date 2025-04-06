using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] bGMs;

    private AudioSource source;
    public static BGM Instance; 
    private void Awake()
    {
        source = GetComponent<AudioSource>();
        if (Instance == null) Instance = this;
    }
    public void PlayClip(int clip)
    {
        source.clip = bGMs[clip];
        source.Play();
    }
}
