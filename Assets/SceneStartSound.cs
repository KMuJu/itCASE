using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStartSound : MonoBehaviour
{
    public AudioClip nextLevelSound; // The sound to play when firingeference to the AudioSource component
    private AudioSource _audioSource;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    //private void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}


    //private void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    private void Start(Scene scene, LoadSceneMode mode)
    {
        // Ensure AudioManager is available
        if (AudioManager.Instance != null)
        {
            Debug.Log("PLAYING");
            PlaySound(nextLevelSound);
        }
    }
    
    public void PlaySound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}
