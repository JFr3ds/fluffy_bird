using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    // Singleton del manager, de esta manera tengo la configuracion basica de los obstaculos
    // en un solo lugar y de facil acceso.
    public static GameManager Instance;

    // Pool de prefabs que poblan el juego
    [Header("Obstacles Pool")] [SerializeField]
    private GameObject pref_obstacle;

    [SerializeField] private Transform obstaclesParent;
    [SerializeField] private List<GameObject> obstacles;
    private int obstacleIndex = 0;

    // Configuracion basica de los settings de los obstaculos
    [Header("Obstacles Config")] public float movementSpeed;
    public float minHeight, maxHeight;
    public float maxLenght;

    // Frecuencia con que van a aparecer los obstaculos
    [Range(.5f, 2f)] [SerializeField] private float frecuencyObstacles;
    private float lastObstacle;

    [SerializeField] private GameObject powerUp;
    [SerializeField] private float frecuencyPowerUp;
    private float lastPowerUp;
    [SerializeField] public float minHeightPowerUp;
    [SerializeField] public float maxHeightPowerUp;
    bool canCreatePowerUp = false;
    private int probabilityPowerUp = 0;


    [SerializeField] private TMP_Text counter;
    [SerializeField] private GameObject btn_pause;
    [SerializeField] private GameObject btn_retry;

    [SerializeField] private GameObject title;


    private int ammountOfObstacles;

    public enum GameState
    {
        Menu,
        Play,
        Dead,
        Pause
    }

    public GameState actualGameState;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        ActionsController.OnCheckObstacle += UpdateObstacles;
        ActionsController.OnObstacle += OnDeathPlayer;
        SetGame();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && actualGameState == GameState.Menu)
        {
            actualGameState = GameState.Play;
            btn_pause.SetActive(true);
            counter.gameObject.SetActive(true);
            title.SetActive(false);
        }

        if (actualGameState != GameState.Play)
        {
            return;
        }

        if (lastObstacle > frecuencyObstacles)
        {
            lastObstacle = 0;
            OnNewObstacle();
        }

        if (!powerUp.activeInHierarchy)
        {
            if (lastPowerUp > frecuencyPowerUp)
            {
                lastPowerUp = 0;
                float randomPowerUp = Random.Range(0.0f, 1.0f);
                if (randomPowerUp <= (probabilityPowerUp / 100f))
                {
                    probabilityPowerUp = 0;
                    canCreatePowerUp = true;
                }
            }
        }

        lastPowerUp += Time.deltaTime;
        lastObstacle += Time.deltaTime;
    }

    public void SetGame()
    {
        actualGameState = GameState.Menu;
        btn_pause.SetActive(false);
        counter.gameObject.SetActive(false);
        btn_retry.SetActive(false);
        ammountOfObstacles = -1;
        UpdateObstacles();
        title.SetActive(true);
    }

    private void OnNewObstacle()
    {
        if (obstacles == null)
        {
            obstacles = new List<GameObject>();
        }

        if (obstacles.Count > 0)
        {
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (!obstacles[i].activeInHierarchy)
                {
                    obstacles[i].transform.localPosition = Vector3.zero;
                    obstacles[i].SetActive(true);
                    obstacles[i].GetComponent<ObstacleController>().OnInitialize();
                    if (canCreatePowerUp)
                    {
                        canCreatePowerUp = false;
                        Invoke("CreatePowerUp", frecuencyObstacles / 2f);
                    }

                    return;
                }
            }

            CreateNewObstacle();
        }
        else
        {
            CreateNewObstacle();
        }
    }

    private void CreateNewObstacle()
    {
        GameObject go = Instantiate(pref_obstacle, obstaclesParent);
        go.name = "obstacle " + obstacleIndex.ToString();
        obstacles.Add(go);
        go.GetComponent<ObstacleController>().OnInitialize();
        obstacleIndex++;
    }

    private void UpdateObstacles()
    {
        ammountOfObstacles++;
        counter.text = ammountOfObstacles.ToString();
        probabilityPowerUp++;
    }

    private void OnDeathPlayer()
    {
        actualGameState = GameState.Dead;
        btn_pause.SetActive(false);
        btn_retry.SetActive(true);
    }

    public void OnPause()
    {
        if (actualGameState == GameState.Play)
        {
            actualGameState = GameState.Pause;
        }
        else
        {
            actualGameState = GameState.Play;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        ActionsController.OnCheckObstacle -= UpdateObstacles;
        ActionsController.OnObstacle -= OnDeathPlayer;
    }

    private void CreatePowerUp()
    {
        powerUp.transform.localPosition = Vector3.zero;
        powerUp.SetActive(true);
        powerUp.GetComponent<PowerUp>().OnInitialize();
    }
}