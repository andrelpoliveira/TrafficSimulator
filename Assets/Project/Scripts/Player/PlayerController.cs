using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Controle de Movimento")]
    public float moveDistance = 1.5f; // Distãncia do movimento em grid
    public float moveDuration = 0.2f; // Duração da movimentação
    [Tooltip("Estado do movimento")]
    private bool isMoving = false; // Booleana para conferir se está em movimento ou não
    private bool canMove = false; // Booleana para habilitar e desabilitar a movimentação do player nos casos de Start, GameOver, Vitória e etc.
    [Tooltip("Multiplicador")]
    private float speedMultiplier = 1f; // Multiplicador par controle do movimento de acordo com os dados da API
    [Tooltip("Input Action")]
    private Vector2 inputDirection; // Direção do Input System
    private PlayerInputActions inputActions; // Input System
    [Space]
    [Header("Bounds")]
    public float minX = -12.5f; // Mínimo movimento no eixo X
    public float maxX = 12.5f; // Máximo movimetno no eixo X
    public float minZ = 0f; // Mínimo movimento no eixo Z
    public float maxZ = 17.5f; // Máximo movimento no eixo Z

    #region Eventos
    /// <summary>
    /// Habilita inputs e aguarda os eventos
    /// </summary>
    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        GameEvents.OnGameStart += HandleStart;
        GameEvents.OnPlayerDeath += HandleStop;
        GameEvents.OnPlayerWin += HandleStop;
        GameEvents.OnResetGame += HandleStop;
    }
    /// <summary>
    /// Desabilita inputs e retira os eventos
    /// </summary>
    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;

        inputActions.Disable();

        GameEvents.OnGameStart -= HandleStart;
        GameEvents.OnPlayerDeath -= HandleStop;
        GameEvents.OnPlayerWin -= HandleStop;
        GameEvents.OnResetGame -= HandleStop;
    }
    /// <summary>
    /// Desabilita a movimentação do jogador quando termina o game
    /// </summary>
    void HandleStop()
    {
        SetCanMove(false);
    }
    /// <summary>
    /// Habilita a movimentação do jogador quando inicia o game
    /// </summary>
    void HandleStart()
    {
        SetCanMove(true);
    }
    /// <summary>
    /// Recebe Input system
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        if (!canMove || isMoving) return;

        inputDirection = context.ReadValue<Vector2>();

        Vector3 moveDir = Vector3.zero;

        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
        {
            moveDir = inputDirection.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            moveDir = inputDirection.y > 0 ? Vector3.forward : Vector3.back;
        }

        Move(moveDir);
    }
    #endregion

    #region Unity Methods
    /// <summary>
    /// Adiciona novo input ao carregar
    /// </summary>
    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }
    #endregion

    #region Controle de Estado
    /// <summary>
    /// Verifica o estado e aplica o multiplicador correspondente na movimentação do player
    /// </summary>
    /// <param name="weather"></param>
    public void ApplyWeather(float multiplier)
    {
        speedMultiplier = multiplier;
    }
    #endregion

    #region Movimento
    /// <summary>
    /// Movimentação do player no estilo Frog por grid
    /// </summary>
    /// <param name="direction"></param>
    void Move(Vector3 direction)
    {
        Vector3 targetPos = transform.position + direction * moveDistance;

        targetPos = ClampPosition(targetPos);

        StartCoroutine(SmoothMove(targetPos));
    }
    /// <summary>
    /// Clamp de posição do player para evitar sair da tela
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    Vector3 ClampPosition(Vector3 target)
    {
        float clampedX = Mathf.Clamp(target.x, minX, maxX);
        float clampedZ = Mathf.Clamp(target.z, minZ, maxZ);

        return new Vector3(clampedX, target.y, clampedZ);
    }
    /// <summary>
    /// Corrotina para suavização da movimentação do player
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    IEnumerator SmoothMove(Vector3 target)
    {
        isMoving = true;

        Vector3 start = transform.position;
        float elapsed = 0f;

        float duration = moveDuration / speedMultiplier;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;

        isMoving = false;
    }
    /// <summary>
    /// Controle de movimentação do estado do game (Win, Colisão ou Tempo esgotado)
    /// </summary>
    /// <param name="value"></param>
    public void SetCanMove(bool value)
    {
        canMove = value;
    }
    #endregion
}
