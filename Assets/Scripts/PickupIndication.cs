using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupIndication : MonoBehaviour
{
    public float duration;
    private float currentValue;
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public BoosterManager boosterManager;
    public string name;

    // Start is called before the first frame update
    void Start()
    {
        currentValue = duration;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float sliderVal = currentValue / duration;
        fill.color = gradient.Evaluate(sliderVal);
        slider.value = sliderVal;
        currentValue -= Time.deltaTime;

        if (currentValue < 0)
        {
            gameObject.SetActive(false);
            boosterManager.RemoveBooster(name);
        }
            
    }

    public void StartTimer(float duration)
    {
        this.duration = duration;
        currentValue = duration;
    }
}
