using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Informações do player")]
    public Transform player; // Player
    public Vector3 startPosition; // Posição inicial do player
    [Space]
    [Header("Countdown")]
    public float gameTime = 60f; // Tempo de jogo
    [Tooltip("Corrotina")]
    private Coroutine timerRoutine; // Armazena a corrotina

    #region Eventos
    /// <summary>
    /// Aguarda os eventos
    /// </summary>
    private void OnEnable()
    {
        GameEvents.OnGameStart += StartGame;
        GameEvents.OnPlayerDeath += GameOver;
        GameEvents.OnPlayerWin += Win;
    }
    /// <summary>
    /// Retira os eventos
    /// </summary>
    private void OnDisable()
    {
        GameEvents.OnGameStart -= StartGame;
        GameEvents.OnPlayerDeath -= GameOver;
        GameEvents.OnPlayerWin -= Win;
    }
    #endregion

    #region Unity Methods
    /// <summary>
    /// Status inicial do Player
    /// </summary>
    void Start()
    {
        startPosition = player.position;
    }
    #endregion

    #region Controle de tempo do game
    /// <summary>
    /// Inicia o timer do game
    /// </summary>
    public void StartGame()
    {
        timerRoutine = StartCoroutine(GameTimer());
    }
    /// <summary>
    /// Corrotina para a contagem regressiva
    /// </summary>
    /// <returns></returns>
    IEnumerator GameTimer()
    {
        float timer = gameTime;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            GameEvents.OnTimeUpdated?.Invoke(timer);
            yield return null;
        }

        Debug.Log("Tempo esgotado");

        GameOver();
    }
    #endregion

    #region Métodos Game Over e Vitória
    /// <summary>
    /// Fim do game por colisão
    /// </summary>
    public void GameOver()
    {
        Invoke(nameof(Restart), 3f);
    }
    /// <summary>
    /// Vitória do jogador
    /// </summary>
    public void Win()
    {
        Invoke(nameof(Restart), 3f);
    }
    /// <summary>
    /// Reinicia partida
    /// </summary>
    void Restart()
    {
        GameEvents.OnResetGame?.Invoke();
        player.position = startPosition;
        gameTime = 60f;
        timerRoutine = null;
    }
    #endregion

}
