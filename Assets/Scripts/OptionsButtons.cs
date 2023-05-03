using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsButtons : MonoBehaviour
{
    public GameObject controlsMenu;
    public GameObject audioMenu;

    public void Volver()
    {
        SceneManager.LoadScene(0);
    }

    public void ControlsMenu()
    {
        controlsMenu.SetActive(true);
    }

    public void AudioMenu()
    {
        audioMenu.SetActive(true);
    }

    public void VolverDeAudio()
    {
        audioMenu.SetActive(false);
    }

    public void VolverDeControles()
    {
        controlsMenu.SetActive(false);
    }
}
