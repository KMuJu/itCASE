using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    
    private bool _isPauseActive = false;
    private LineRenderer _lineRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isPauseActive = !_isPauseActive;

            if (_isPauseActive)
            {
                Pause();
                _lineRenderer.enabled = false;
            }
            else
            {
                Unpause();
                _lineRenderer.enabled = true;
            }
            
            pauseMenu.SetActive(_isPauseActive);
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
    }

    private void Unpause()
    {
        Time.timeScale = 1;
    }
}
