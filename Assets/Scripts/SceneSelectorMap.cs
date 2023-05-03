using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelectorMap : MonoBehaviour
{
    public void Volver()
    {
        SceneManager.LoadScene(0);
    }

    public void City()
    {
        SceneManager.LoadScene(3);
    }

    public void Rocks()
    {
        SceneManager.LoadScene(4);
    }

    public void Winter()
    {
        SceneManager.LoadScene(5);
    }

    public void Formula1()
    {
        SceneManager.LoadScene(6);
    }
}
