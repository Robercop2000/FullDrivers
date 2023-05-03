using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Sounds")]

    public AudioSource backSound;

    [Header("UI")]
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject HUD;
    public GameObject escMenu;
    public GameObject antiFailMenu;

    [Header("StartGame")]
    public bool GameStarted = false;
    public GameObject Countdown;
    [SerializeField] private Sprite[] countImage;

    [Header("Path")]
    public GameObject Path;
    Transform[] pathTransforms;
    private List<Transform> CheckPoints = new List<Transform>();


    [Header("Spawn")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject KartAI;
    [SerializeField] private GameObject PlayerKart;
    private Transform ActualTransform;

    public void SpawnManager()
    {
        int rnd = Random.Range(1, spawnPoints.Count);
        ActualTransform = spawnPoints[rnd - 1];
        spawnPoints[rnd - 1].gameObject.SetActive(false);
        spawnPoints.Remove(spawnPoints[rnd - 1]);
    }

    private void Awake()
    {
        pathTransforms = Path.gameObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != Path.transform)
            {
                CheckPoints.Add(pathTransforms[i]);
            }
        }

        backSound.Play();
    }

    public void StartGame(int difficulty)
    {
        SpawnManager();
        PlayerKart.GetComponent<KartController>().Spawn(ActualTransform);

        while (spawnPoints.Count != 0)
        {
            SpawnManager();
            GameObject ActualAI = Instantiate(KartAI, ActualTransform.position, ActualTransform.rotation);
            ActualAI.GetComponent<KartIA>().enabled = true;
            ActualAI.GetComponent<KartIA>().Difficulty(difficulty);
            ActualAI.GetComponent<KartIA>().Checkpoints = CheckPoints;
        }
        if (!GameStarted)
            StartCoroutine(CountDownRoutine());
    }

    public void WinGame()
    {
        if (GameStarted)
        {
            winMenu.SetActive(true);
            HUD.SetActive(false);
            GameStarted = false;
        }
    }

    public void LooseGame()
    {
        if (GameStarted)
        {
            loseMenu.SetActive(true);
            HUD.SetActive(false);
            GameStarted = false;
        }
    }

    public void Back()
    {
        SceneManager.LoadScene(1);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ReloadWinter()
    {
        SceneManager.LoadScene(5);
    }
    public void ReloadFormula1()
    {
        SceneManager.LoadScene(6);
    }
    public void ReloadRocks()
    {
        SceneManager.LoadScene(4);
    }
    public void ReloadCity()
    {
        SceneManager.LoadScene(3);
    }

    public void Pause()
    {
        if(GameStarted)
        {
            escMenu.SetActive(true);
            HUD.SetActive(false);
            Time.timeScale = 0f;
            AudioListener.volume = 0;
        }
        
    }
    public void Resume()
    {
        if (GameStarted)
        {
            escMenu.SetActive(false);
            HUD.SetActive(true);
            Time.timeScale = 1f;
            AudioListener.volume = 1;
        }
    }
    public void NoResume()
    {
        escMenu.SetActive(false);
        antiFailMenu.SetActive(true);
    }
    public void No()
    {
        antiFailMenu.SetActive(false);
        escMenu.SetActive(true);
    }

    IEnumerator CountDownRoutine()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);
        Countdown.GetComponent<Image>().sprite = countImage[0];
        Countdown.GetComponent<Image>().enabled = true;

        yield return new WaitForSecondsRealtime(1f);
        Countdown.GetComponent<Image>().sprite = countImage[1];

        yield return new WaitForSecondsRealtime(1f);
        Countdown.GetComponent<Image>().sprite = countImage[2];

        yield return new WaitForSecondsRealtime(1f);
        Countdown.GetComponent<Image>().sprite = countImage[3];

        yield return new WaitForSecondsRealtime(1f);
        Countdown.GetComponent<Image>().enabled = false;
        Time.timeScale = 1;
        GameStarted = true;
    }

    
}
