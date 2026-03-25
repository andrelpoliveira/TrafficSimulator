using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Paineis do Game")]
    public GameObject startPanel; // Painel Start
    public GameObject hudPanel; // Painel do Game
    public GameObject losePanel; // Painel Game Over
    public GameObject winPanel; // Painel de Vitória
    [Space]
    [Header("Paineis Internos das Huds")]
    public GameObject countdownPanel; // Painel de Countdown do start
    public GameObject gameStatusPanel; // Painel de status do game
    public GameObject gameCountdownPanel; // Painel de Countdown do game
    [Space]
    [Header("Texts")]
    public TMP_Text countDownText; // Texto de contagem regressiva para início do game
    public TMP_Text weatherText; // Texto do Clima atual
    public TMP_Text speedText; // Texto da velocidade dos veículos
    public TMP_Text nextWeatherText; // Texto da previsão
    public TMP_Text nextWeatherTimeText; // Texto do tempo para o próximo clima
    public TMP_Text countDownGameText; // Texto para a contagem regressiva de tempo do jogo
    public TMP_Text currentLevelText; // Texto para o level atual do jogo

    #region Eventos
    /// <summary>
    /// Aguarda eventos
    /// </summary>
    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += GameOver;
        GameEvents.OnPlayerWin += Win;
        GameEvents.OnResetGame += StartGame;
        GameEvents.OnTimeUpdated += UpdateTimer;
        GameEvents.OnTimeNextWeather += UpdateTimerNextWeather;
        GameEvents.OnNextLevel += NextLevel;
    }
    /// <summary>
    /// Retira eventos
    /// </summary>
    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= GameOver;
        GameEvents.OnPlayerWin -= Win;
        GameEvents.OnResetGame -= StartGame;
        GameEvents.OnTimeUpdated -= UpdateTimer;
        GameEvents.OnTimeNextWeather -= UpdateTimerNextWeather;
        GameEvents.OnNextLevel -= NextLevel;
    }
    #endregion

    #region Unity Methods
    /// <summary>
    /// Método Start para início da HUD
    /// </summary>
    void Start()
    {
        StartGame();
    }
    #endregion
    #region Controle de start do Game
    /// <summary>
    /// Controla os painéis iniciais do game
    /// </summary>
    public void StartGame()
    {
        startPanel.SetActive(true);
        hudPanel.SetActive(false);
        countdownPanel.SetActive(false);
        gameStatusPanel.SetActive(false);
        gameCountdownPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        UpdateLevel(1);
    }
    /// <summary>
    /// Função do botão de início do game
    /// </summary>
    public void BtnStartGame()
    {
        startPanel.SetActive(false);
        hudPanel.SetActive(true);
        countdownPanel.SetActive(true);
        GameEvents.OnGameStart?.Invoke();
        StartCoroutine(StartCountdown());
    }
    /// <summary>
    /// Corrotina de contagem regressiva para o início do game
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCountdown()
    {
        for(int i = 3; i > 0; i--)
        {
            countDownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countDownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownPanel.SetActive(false);
        gameStatusPanel.SetActive(true);
        gameCountdownPanel.SetActive(true);
    }
    #endregion

    #region HUD
    /// <summary>
    /// Atualização da HUD
    /// </summary>
    /// <param name="current"></param>
    /// <param name="next"></param>
    public void UpdateHUD(Status current, PredictedStatus next)
    {
        // Status
        weatherText.text = $"{current.weather}";
        speedText.text = $"{current.averageSpeed} km/h";
       
        //Color
        weatherText.color = GetWeatherColor(current.weather);
        

        if(next != null && next.predictions != null)
        {
            nextWeatherText.text = $"{next.predictions.weather}";
            nextWeatherText.color = GetWeatherColor(next.predictions.weather);

            float seconds = next.estimated_time / 1000f;

            nextWeatherTimeText.text = $"{seconds:0}s";
        }
        else
        {
            nextWeatherText.text = "Sem previsão";
            nextWeatherTimeText.text = "Sem previsão";
        }
    }
    /// <summary>
    /// Atualiza o contador de tempo para concluir o game
    /// </summary>
    /// <param name="time"></param>
    public void UpdateTimer(float time)
    {
        GetTimerColor(time);

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        countDownGameText.text = $"{minutes:00}:{seconds:00}";
    }
    /// <summary>
    /// Atualiza o tempo para o próximo clima
    /// </summary>
    /// <param name="time"></param>
    public void UpdateTimerNextWeather(float time)
    {
        if(time < 0) time = 0;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        nextWeatherTimeText.text = $"{minutes:00}:{seconds:00}";
    }
    /// <summary>
    /// Atualiza o level do jogo
    /// </summary>
    /// <param name="nextLevel"></param>
    public void UpdateLevel(int nextLevel)
    {
        currentLevelText.text = nextLevel.ToString();
    }
    /// <summary>
    /// Sistema de cores do TMP_Text por clima
    /// </summary>
    /// <param name="weather"></param>
    /// <returns></returns>
    Color GetWeatherColor(string weather)
    {
        return weather switch
        {
            "sunny" => Color.yellow,
            "clouded" or "foggy" => Color.gray,
            "light rain" => Color.cyan,
            "heavy rain" => Color.blue,
            _ => Color.white,
        };
    }
    /// <summary>
    /// Painel de Game Over
    /// </summary>
    public void GameOver()
    {
        losePanel.SetActive(true);
        gameStatusPanel.SetActive(false);
        UpdateTimerNextWeather(0f);
    }
    /// <summary>
    /// Painel de Vitória
    /// </summary>
    public void Win()
    {
        winPanel.SetActive(true);
        gameStatusPanel.SetActive(false);
        UpdateTimerNextWeather(0f);
    }
    /// <summary>
    /// Avanço de level
    /// </summary>
    /// <param name="level"></param>
    public void NextLevel(int level)
    {
        gameCountdownPanel.SetActive(false);
        winPanel.SetActive(false);
        BtnStartGame();
        UpdateLevel(level);
    }
    /// <summary>
    /// Sistema de cores do TMP_Text do contador
    /// </summary>
    /// <param name="timeGame"></param>
    /// <returns></returns>
    Color GetTimerColor(float timeGame)
    {
        Color textColor;

        if(timeGame >= 30)
        {
            textColor = Color.white;
        }
        else if(timeGame < 30 && timeGame >= 10)
        {
            textColor = Color.orange;
        }
        else
        {
            textColor = Color.red;
        }

        return textColor;
    }
    #endregion
}
