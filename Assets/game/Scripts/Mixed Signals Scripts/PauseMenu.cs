using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public static bool isMenued = false;
    public GameObject pauseMenuUI;
    public GameObject gameOverMenuUI;
    public GameObject winMenuUI;

    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("Scroll 1");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FindObjectOfType<AudioManager>().Play("Select");
            if (isPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        FindObjectOfType<AudioManager>().Play("Select");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        isMenued = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        isMenued = true;
    }

    public void SelectSound()
    {
        FindObjectOfType<AudioManager>().Play("Select");
    }

    public void GameOver()
    {
        isMenued = true;
        gameOverMenuUI.SetActive(true);
        Time.timeScale = 0;
    }
    public void Win()
    {
        isMenued = true;
        winMenuUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void LoadMenu()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        isMenued = false;
        FindObjectOfType<AudioManager>().Stop("Scroll 1");
        FindObjectOfType<AudioManager>().Stop("Game Over");
        FindObjectOfType<AudioManager>().Stop("Win");
        FindObjectOfType<AudioManager>().Play("Select");
        SceneManager.LoadScene("Level 1");
    }

    public void TitleMenu()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        isMenued = false;
        FindObjectOfType<AudioManager>().Play("Select");
        Debug.Log("Loading menu");
        SceneManager.LoadScene("Title Screen");
    }

    public void QuitGame()
    {
        /*pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        isMenued = false;*/
        FindObjectOfType<AudioManager>().Play("Select");
        Debug.Log("QUIT");
        Application.Quit();
    }
}
