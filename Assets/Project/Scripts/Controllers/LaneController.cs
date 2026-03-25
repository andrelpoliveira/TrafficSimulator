using System.Collections;
using UnityEngine;

public class LaneController : MonoBehaviour
{
    [Header("OBJs")]
    public Transform spawnPoint; // Ponto de Spawn dos veículos
    public GameObject[] vehiclesPrefab; // Prefab dos veículos
    [Space]
    [Header("Direção e Espaço do spwan")]
    public int direction = 1; // 1 = direita, -1 = esquerda 
    public float minSpacing = 3f; // Espaço entre os veiculos
    [Space]
    [Header("Gap Settings")]
    public float gapChance = 0.25f; // 25% de chance de gap
    public float minGapTime = 1.5f; // Gap mínimo
    public float maxGapTime = 3.5f; // Gap máximo
    [Tooltip("Controle interno de velocidade")]
    private float spawnInterval = 2f; // Intervalo inicial do spawn
    private float vehicleSpeed = 5f; // Velocidade inicial dos veículos
    [Tooltip("Corrotina de spawn")]
    private Coroutine spawnRoutine; // Armazena corrotina ativa
    [Tooltip("OBJ para verificação de veículo")]
    private GameObject lastVehicle; // Armazena o último veículo spawnado

    #region Controle da Lane
    /// <summary>
    /// Recebe as informações de status para aplicar as condições de clima
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="speed"></param>
    public void Setup(float interval, float speed, float density)
    {
        spawnInterval = interval;
        vehicleSpeed = speed;

        // gap adaptivo
        gapChance = Mathf.Lerp(0.4f, 0.15f, density);

        if(spawnRoutine != null) 
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnLoop());
    }
    /// <summary>
    /// Corrotina para o loop do spawn de veículos
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnLoop()
    {
        // Delay inicial para evitar sincronização
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        while(true)
        {
            // Aguarda spawn para evitar veículos sobrepostos
            yield return StartCoroutine(WaitForSafeSpawn());

            SpawnVehicle();

            // Chance de criar GAP
            if(Random.value < gapChance)
            {
                float gapTime = Random.Range(minGapTime, maxGapTime);

                yield return new WaitForSeconds(gapTime);
            }
            else
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
    /// <summary>
    /// Adiciona o prefab aleatório e seta as informações de velocidade e direção
    /// </summary>
    void SpawnVehicle()
    {
        // Aleatoridade de veículo
        int vehicleRandom = Random.Range(0, vehiclesPrefab.Length);

        GameObject vehicle = Instantiate(vehiclesPrefab[vehicleRandom], spawnPoint.position, Quaternion.identity);

        var movement = vehicle.GetComponent<VehicleMovement>();

        //Variação de velocidade por veículo
        float laneSpeed = vehicleSpeed * Random.Range(0.8f, 1.2f);
        movement.Setup(direction, laneSpeed);

        // Gaurda último veículo spawnado
        lastVehicle = vehicle;
    }
    /// <summary>
    /// Corrotina para spawn seguro sem sobrepor veículos
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForSafeSpawn()
    {
        while (lastVehicle != null)
        {
            // Cálculo de distância base entre veículos
            float distance = Vector3.Distance(spawnPoint.position, lastVehicle.transform.position);
            // Cálculo de espaço dinâmico entre os veículos
            float dinamicSpacing = minSpacing + (vehicleSpeed * 0.3f);

            if (distance >= dinamicSpacing)
                break;

            yield return null;
        }
    }
    /// <summary>
    /// Encerra o loop de spawn quando o jogo encerra
    /// </summary>
    public void StopSpawning()
    {
        if(spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }
    #endregion
}
