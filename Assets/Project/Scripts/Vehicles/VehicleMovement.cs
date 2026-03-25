using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [Tooltip("Controle de velocidade e direção")]
    private float speed; // Velocidade do veículo
    private int direction; // Direção do veículo
    [Header("Controle de distância dos veículos")]
    public float detectionDistance = 3f; //Verifica a distância dos veículos à frente
    public LayerMask vehicleLayer; // Layer dos veículos
    public float minSpeedFactor = 0.3f; // Velocidade mínima ao frear

    #region Eventos
    /// <summary>
    /// Aguarda evento
    /// </summary>
    private void OnEnable()
    {
        GameEvents.OnResetGame += HandleReset;
    }
    /// <summary>
    /// Retira evento
    /// </summary>
    private void OnDisable()
    {
        GameEvents.OnResetGame -= HandleReset;
    }
    /// <summary>
    /// Apaga os veículos quando terminar o game, seja Game Over, Vitória, etc...
    /// </summary>
    void HandleReset()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Unity Methods
    /// <summary>
    /// Atualiza a velocidade dos veículos com os dados da API 
    /// Controla Layer para evitar colisão entre veículos
    /// Ajusta a direção dos veículos de acordo com a lane
    /// </summary>
    private void Update() 
    {
        float adjustSpeed = speed;
        vehicleLayer = LayerMask.GetMask("Vehicle");

        RaycastHit hit;

        Vector3 dir = direction == 1 ? Vector3.right : Vector3.left;

        if(Physics.Raycast(transform.position, dir, out hit, detectionDistance, vehicleLayer))
        {
            if (hit.collider.CompareTag("Vehicle"))
            {
                float distance = hit.distance;

                // Reduzir velocidade proporcional a distância
                float factor = Mathf.Clamp(distance / detectionDistance, minSpeedFactor, 1f); 

                adjustSpeed *= factor;
            }
        }
        transform.Translate(direction * adjustSpeed * Time.deltaTime * Vector3.right); 
    }
    #endregion

    #region Configurações do veículo
    /// <summary>
    /// Configuração inicial do veículo
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="spd"></param>
    public void Setup(int dir, float spd)
    {
        direction = dir;
        speed = spd;
    }
    /// <summary>
    /// Destrói o veículo quando não mais visível
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DestroyVehicle")) Destroy(gameObject);
    }
    #endregion

    #region Gizmos de Orientação
    /// <summary>
    /// Desenha Gizmos para melhor visualização e controle da distância
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 dir = direction == 1 ?  Vector3.right : Vector3.left;

        Gizmos.DrawLine(transform.position, transform.position + dir * detectionDistance);
    }
    #endregion
}
