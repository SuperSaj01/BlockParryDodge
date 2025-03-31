using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour, ISliderHandler
{
    [SerializeField] Slider healthSlider;
    private float smoothSpeed = 0.5f;

    private Coroutine changeCoroutine;

    void Start()
    {
        healthSlider = GetComponent<Slider>();
    }

    public void IncreaseValue(float amount)
    {
        ChangeValue(amount);
    }
    public void ReduceValue(float amount)
    {
        ChangeValue(-amount);
    }
    public void ChangeValue(float amount)
    {
        float targetValue = Mathf.Clamp(healthSlider.value + amount, 0, healthSlider.maxValue);

        //Stop current coroutine if running
        if (changeCoroutine != null) StopCoroutine(changeCoroutine);

        // Start new coroutine
        changeCoroutine = StartCoroutine(SmoothChange(targetValue));
    }

    private IEnumerator SmoothChange(float targetValue)
    {
        float startValue = healthSlider.value;
        float elapsedTime = 0f;

        while (elapsedTime < smoothSpeed)
        {
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / smoothSpeed);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until next frame
        }

        // Ensure final value is set
        healthSlider.value = targetValue;
    }
}
