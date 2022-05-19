using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hazardPrefab;
    [SerializeField]
    private int maxHazardsToSpawn = 3;
    [SerializeField]
    private TMPro.TextMeshProUGUI scoreText;
    [SerializeField]
    private Image pauseMenu;
    [SerializeField]
    private GameObject mainVCam;
    [SerializeField]
    private GameObject zoomVCam;
    [SerializeField]
    private GameObject gameOverMenu;
    [SerializeField]
    private GameObject player;

    private int highScore;
    private int score;
    private float timer;
    private Coroutine hazardCoroutine;
    private bool gameOver;

    private static GameManager instance;
    private const string HighScorePreferenceKey = "HighScore";

    public static GameManager Instance => instance;
    
    public int HighScore => highScore;

    void Start()
    {
        instance = this; 

        highScore = PlayerPrefs.GetInt(HighScorePreferenceKey);       
    }

    private void OnEnable()
    {        
        player.SetActive(true);

        zoomVCam.SetActive(false);
        mainVCam.SetActive(true);

        gameOver = false;
        scoreText.text = "0";
        score = 0;
        timer = 0;

        hazardCoroutine = StartCoroutine(SpawnHazards());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                Resume();
            }
            if (Time.timeScale == 1)
            {
                Pause();
            }
        }

        if (gameOver)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            score++;
            scoreText.text = score.ToString();

            timer = 0;
        }
    }

    private void Pause()
    {
        LeanTween.value(1, 0, 0.3f)
                        .setOnUpdate(SetTimeScale)
                        .setIgnoreTimeScale(true);
        pauseMenu.gameObject.SetActive(true);
    }

    private void Resume()
    {
        LeanTween.value(0, 1, 0.3f)
                        .setOnUpdate(SetTimeScale)
                        .setIgnoreTimeScale(true);
        pauseMenu.gameObject.SetActive(false);
    }

    private void SetTimeScale(float value)
    {
        Time.timeScale = value;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    IEnumerator ScaleTime(float start, float end, float duration)
    {
        float lastTime = Time.realtimeSinceStartup;
        float timer = 0.0f;

        while (timer < duration)
        {
            Time.timeScale = Mathf.Lerp(start, end, timer / duration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            timer += (Time.realtimeSinceStartup - lastTime);
            lastTime = Time.realtimeSinceStartup;
            yield return null;
        }

        Time.timeScale = end;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private IEnumerator SpawnHazards()
    {
        var hazardToSpawn = Random.Range(1, maxHazardsToSpawn);

        for (int i = 0; i < hazardToSpawn; i++)
        {
            var x = Random.Range(-7, 7);
            var drag = Random.Range(0f, 2f);

            var hazard = Instantiate(hazardPrefab, new Vector3(x, 11, 0), Quaternion.identity);
            hazard.GetComponent<Rigidbody>().drag = drag;
        }

        var timeToWait = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(timeToWait);

        yield return SpawnHazards();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void GameOver()
    {
        StopCoroutine(hazardCoroutine);
        gameOver = true;

        if (Time.timeScale < 2)
        {
            Resume();
        }

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScorePreferenceKey, highScore);
            Debug.Log(highScore);
        }

        mainVCam.SetActive(false);
        zoomVCam.SetActive(true);

        gameObject.SetActive(false);
        gameOverMenu.SetActive(true);
    }



}
