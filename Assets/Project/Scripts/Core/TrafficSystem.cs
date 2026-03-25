using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    [Header("API")]
    public TrafficApiService apiService; // Script de chamada da API
    [Space]
    [Header("Sistema de spawn do Tráfego")]
    public TrafficSpawner trafficSpawner; // Script de spawn do tráfego de veículos
    [Space]
    [Header("Script da HUD")]
    public UIManager uiManager; // Script que controla a HUD
    [Space]
    [Header("Script Player")]
    public PlayerController player; // Script de movimentação do player
    [Space]
    [Header("Script de VFX")]
    public WeatherVFXController weatherVFXController; // Script de efeitos do clima
    [Tooltip("Controle do status")]
    private Status currentStatus; // Armazena status atual vindos da API
    [Tooltip("Corrotina de previsões")]
    private Coroutine mainRoutine; // Armazena corrotina ativa
    [Tooltip("Lista de previsões")]
    private Queue<PredictedStatus> predictionQueue = new Queue<PredictedStatus>(); // Queue que armazena as previsões
    [Tooltip("Lista de Climas")]
    private List<string> weatherFlow = new List<string>
    {
        "sunny",
        "clouded",
        "foggy",
        "light rain",
        "heavy rain"
    };

    #region Eventos
    /// <summary>
    /// Aguarda eventos
    /// </summary>
    private void OnEnable()
    {
        GameEvents.OnGameStart += StartGame;
        GameEvents.OnResetGame += StopGame;
    }
    /// <summary>
    /// Retira eventos
    /// </summary>
    private void OnDisable()
    {
        GameEvents.OnGameStart -= StartGame;
        GameEvents.OnResetGame -= StopGame;
    }
    #endregion

    #region Controle de Jogo
    /// <summary>
    /// Inicia o tráfego chamando a corrotina
    /// </summary>
    private void StartGame()
    {
        Debug.Log("Start Traffic");
        mainRoutine = StartCoroutine(TrafficLoop());
    }
    /// <summary>
    /// Encerra o tráfego parando as corrotinas
    /// </summary>
    void StopGame()
    {
        if (mainRoutine != null) 
        {
            StopCoroutine(mainRoutine);
            mainRoutine = null;
        }

        StopAllCoroutines(); // garante o encerramento das corrotinas

        trafficSpawner.StopAllLanes();
    }
    #endregion

    #region API callback
    /// <summary>
    /// Recebimento dos dados da API
    /// </summary>
    /// <param name="data"></param>
    void OnDataReceived(TrafficResponse data)
    {
        Debug.Log("Dados recebidos!");

        //Aplica estado atual atual
        ApplyStatus(data.current_status);

        predictionQueue.Clear();

        if (data.predicted_status != null && data.predicted_status.Count > 0) 
        {
            foreach (var p in data.predicted_status) 
            {
                predictionQueue.Enqueue(p);
            }
        }
        else
        {
            Debug.LogWarning("API sem previsões!");
        }
    }
    #endregion

    #region Loop Principal
    /// <summary>
    /// Corrotina para iniciar o status do tempo e tráfego
    /// </summary>
    /// <returns></returns>
    IEnumerator TrafficLoop()
    {
        while (true)
        {
            // Buscar dados
            yield return StartCoroutine(apiService.GetTraffic(OnDataReceived));

            // Processa previsões
            while(predictionQueue.Count > 0)
            {
                var prediction = predictionQueue.Dequeue();

                yield return new WaitForSeconds(prediction.estimated_time / 1000f);

                string nextWeather = prediction.predictions.weather;

                if(!IsValidTransition(currentStatus.weather, nextWeather))
                {
                    Debug.Log("Clima inválido, corrigindo...");
                    nextWeather = GetNextWeather(currentStatus.weather);
                }

                prediction.predictions.weather = nextWeather;

                ApplyStatus(prediction.predictions);
            }

            Debug.Log("Fila finalizada -> buscando nova API...");
        }
    }
    #endregion

    #region Aplicação de Status
    /// <summary>
    /// Aplicação dos status inciais e atualizações
    /// </summary>
    /// <param name="status"></param>
    void ApplyStatus(Status status)
    {
        currentStatus = status;

        PredictedStatus nextPrediction = null;

        if(predictionQueue.Count > 0)
            nextPrediction = predictionQueue.Peek();

        uiManager.UpdateHUD(status, nextPrediction);

        // Spawn de veículos
        trafficSpawner.UpdateTraffic(status);

        // Player
        float playerMultiplier = GetWeatherMultiplier(status.weather);
        player.ApplyWeather(playerMultiplier);

        //VFX
        weatherVFXController.ApplyWeather(status.weather);
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Multiplicador de clima
    /// </summary>
    /// <param name="weather"></param>
    /// <returns></returns>
    float GetWeatherMultiplier(string weather)
    {
        return weather switch
        {
            "sunny" => 1.0f,
            "clouded" or "foggy" => 0.8f,
            "light rain" => 0.6f,
            "heavy rain" => 0.4f,
            _ => 1.0f,
        };
    }
    /// <summary>
    /// Verificação de clima para evitar bug com resposta da API
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    string GetNextWeather(string current)
    {
        int index = weatherFlow.IndexOf(current);

        if (index == -1 || index == weatherFlow.Count - 1)
            return weatherFlow[0];

        return weatherFlow[index + 1];
    }
    /// <summary>
    /// Validação do clima
    /// </summary>
    /// <param name="current"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    bool IsValidTransition(string current, string next)
    {
        return GetNextWeather(current) == next;
    }
    #endregion
}
