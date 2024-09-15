using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public string LevelName;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(LevelName);
        }
    }
}
