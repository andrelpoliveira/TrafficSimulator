using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class WeatherVFXController : MonoBehaviour
{
    [Header("Compontente Volume")]
    public Volume globalVolume; // Componente Global Volume do Post Processing
    [Tooltip("Componentes para ajustes")]
    private ColorAdjustments colorAdjustments; // Override Color Adjustment
    private Vignette vignette; // Override Vignette

    #region Unity Methods
    /// <summary>
    /// Inicia as variáveis
    /// </summary>
    void Start()
    {
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out colorAdjustments);

        vignette.active = false;
    }
    #endregion

    #region Efeitos
    /// <summary>
    /// Aplicação dos efetiso de acordo com o clima vindo da API
    /// </summary>
    /// <param name="weather"></param>
    public void ApplyWeather(string weather)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionWeather(weather));
    }
    /// <summary>
    /// Corrotina para aplicação dos efeitos 
    /// </summary>
    /// <param name="weather"></param>
    /// <returns></returns>
    IEnumerator TransitionWeather(string weather)
    {
        float duration = 2f;
        float t = 0f;

        float targetExposure = 0f;
        float targetSaturation = 0f;

        switch (weather)
        {
            case "sunny":
                targetExposure = 0.2f;
                targetSaturation = 2f;
                vignette.active = false;
                break;
            case "clouded":
            case "foggy":
                targetExposure = -0.5f;
                targetSaturation = -20f;
                vignette.active = false;
                break;
            case "light rain":
                targetExposure = -1f;
                targetSaturation = -40f;
                vignette.active = false;
                break;
            case "heavy rain":
                targetExposure = -2f;
                targetSaturation = -60f;
                vignette.active = true;
                break;
        }

        float startExposure = colorAdjustments.postExposure.value;
        float startSaturation = colorAdjustments.saturation.value;

        while (t < duration)
        {
            t += Time.deltaTime;

            float lerp = t / duration;

            colorAdjustments.postExposure.value = Mathf.Lerp(startExposure, targetExposure, lerp);
            colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, lerp);

            yield return null;
        }
    }
    #endregion
}
