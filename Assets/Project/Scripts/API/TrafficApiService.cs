using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TrafficApiService : MonoBehaviour
{
    [Tooltip("URL de chamada da API")]
    private string url = "http://localhost:3001/v1/traffic/status"; // endpoint da API

    #region Chamada da API
    /// <summary>
    /// Corrotina para chamada da API com os dados
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetTraffic(System.Action<TrafficResponse> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url)) 
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) 
            {
                string json = request.downloadHandler.text;

                Debug.Log("API Response: " + json);

                TrafficResponse data = JsonUtility.FromJson<TrafficResponse>(json);

                callback?.Invoke(data);
            }
            else
            {
                Debug.Log("Erro na API: " + request.error);
            }
        }
    }
    #endregion
}
