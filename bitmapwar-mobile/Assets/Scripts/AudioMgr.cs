using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    public AudioSource source;
    public AudioClip normalClip;
    public AudioClip battleClip;
    
    public static AudioMgr inst;

    private void Awake()
    {
        inst = this;
    }

    public void PlayBattle()
    {
        source.clip = battleClip;
        source.Play();
    }
    
    public void PlayNormal()
    {
        source.clip = normalClip;
        source.Play();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
