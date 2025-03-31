using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthSlider : MonoBehaviour, ISliderHandler
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text text; // Declare a TMP_Text variable for TextMesh Pro
    private float smoothSpeed = 0.5f;

    private Coroutine changeCoroutine;

    void Start()
    {
        healthSlider = GetComponent<Slider>();
    }

    public void ChangeValue(float currentAmount)
    {
        float targetValue = Mathf.Clamp(currentAmount, 0, healthSlider.maxValue);

        //Stop current coroutine if running
        if (changeCoroutine != null) StopCoroutine(changeCoroutine);

        // Start new coroutine
        changeCoroutine = StartCoroutine(SmoothChange(targetValue));
    }

    public void SetMaxValue(float maxValue)
    {
        healthSlider.maxValue = maxValue;
        healthSlider.value = maxValue; // Set the current value to max as well
    }
    

    public void SetValueToMax() 
    {
        healthSlider.value = healthSlider.maxValue;
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

    private void Update()
    {
        // Update the text to show the current value of the slider
        text.text = Mathf.RoundToInt(healthSlider.value) + "/" + Mathf.RoundToInt(healthSlider.maxValue);
    }
}
