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
        postureSlider.value = Mathf.Clamp(postureSlider.value + amount, 0, postureSlider.maxValue);
    }
}
