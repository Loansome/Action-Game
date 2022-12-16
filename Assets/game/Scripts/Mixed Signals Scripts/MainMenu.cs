using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<AudioManager>().Stop("Scroll 1");
        FindObjectOfType<AudioManager>().Stop("Game Over");
        FindObjectOfType<AudioManager>().Stop("Win");
        FindObjectOfType<AudioManager>().Play("Title");
    }

    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().Stop("Title");
        FindObjectOfType<AudioManager>().Play("Select");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SelectSound()
    {
        FindObjectOfType<AudioManager>().Play("Select");
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("Select");
        Debug.Log("QUIT");
        Application.Quit();
    }
}
