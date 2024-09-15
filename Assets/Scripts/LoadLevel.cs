using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LoadLevel : MonoBehaviour
{
    public string LevelName;
    
    private AudioSource _audioSource; // R
    public AudioClip deathSound; // The sound to play when firingeference to the AudioSource component
    public AudioClip nextLevelSound; // The sound to play when firingeference to the AudioSource component
    public bool death = false;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(LevelName);
        }
    }
}
