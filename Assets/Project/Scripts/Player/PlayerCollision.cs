using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [Header("Script GameManager")]
    public GameManager gameManager; // Script GameManager

    #region Unity Methods
    /// <summary>
    /// Busca o objeto ao iniciar os scripts
    /// </summary>
    private void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }
    #endregion

    #region Colisão
    /// <summary>
    /// Sistema de colisão 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Vehicle"))
        {
            Debug.Log("Player Perdeu!");

            GameEvents.OnPlayerDeath?.Invoke(); // Dispara o evento de Game Over
        }

        if(other.CompareTag("Finish"))
        {
            Debug.Log("Player Venceu");

            GameEvents.OnPlayerWin?.Invoke(); // Dispara o evento de vitória
        }
    }
    #endregion
}
