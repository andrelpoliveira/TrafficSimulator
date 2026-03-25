using System.Collections;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Lanes")]
    public LaneController[] lanes; // Quantidade de Lanes para spawn dos veículos
    
    #region Lógica do Spawn
    /// <summary>
    /// Atualização do tráfego das lanes de acordo com os dados da API
    /// </summary>
    /// <param name="density"></param>
    /// <param name="avgSpeed"></param>
    public void UpdateTraffic(Status status)
    {
        // Intervalo baseado na densidade
        float normalized = Mathf.Clamp01(status.vehicleDensity);
        float avgSpeed = status.averageSpeed;

        // Curva suave baseado nas lanes
        float baseInterval = Mathf.Lerp(3f, 0.8f, normalized);

        //Ajuste por lanes
        float spawnInterval = baseInterval * 0.5f;

        // Velocidade baseada na API
        float referenceSpeed = 10f;
        float vehicleSpeed = (avgSpeed / 100f) * referenceSpeed;

        foreach(var lane in lanes)
        {
            lane.Setup(spawnInterval, vehicleSpeed, normalized);
        }
    }
    /// <summary>
    /// Para o spawn de veículos para encerrar o game
    /// </summary>
    public void StopAllLanes()
    {
        foreach (var lane in lanes) 
        {
            lane.StopSpawning();
        }
    }
    #endregion
}
