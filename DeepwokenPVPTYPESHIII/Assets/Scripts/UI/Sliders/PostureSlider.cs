using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostureSlider : MonoBehaviour, ISliderHandler
{
    [SerializeField] Slider postureSlider;

    void Start()
    {
        postureSlider = GetComponent<Slider>();
    }
    public void ChangeValue(float currentAmount)
    {
        postureSlider.value = Mathf.Clamp(currentAmount, 0, postureSlider.maxValue);
    }

    public void SetMaxValue(float maxValue)
    {
        postureSlider.maxValue = maxValue;
    }
    
    public void SetValueToMax()
    {
        postureSlider.value = postureSlider.maxValue;
    }
}
