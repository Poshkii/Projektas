using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceSettings : MonoBehaviour
{
    [SerializeField] private Slider fpsSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("FPS"))
        {
            LoadFPS();
        }
        else
        {
            SetFPS();
        }
        
    }

    public void LoadFPS()
    {
		int savedFPS = PlayerPrefs.GetInt("FPS", 60);
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = savedFPS;
		fpsSlider.value = GetSliderValue(savedFPS);

        SetFPS();
	}

    public void SetFPS()
    {
        int selectedFPS = GetFPSFromSliderValue((int)fpsSlider.value);
        Application.targetFrameRate = selectedFPS;
        PlayerPrefs.SetInt("FPS", selectedFPS);
        PlayerPrefs.Save();
    }

    float GetSliderValue(int fps)
    {
        switch (fps)
        {
            case 30:
                return 0;
            case 60:
                return 0.5f;
            case 120:
                return 1;
            default:
                return 0.5f;
        }
    }

    int GetFPSFromSliderValue(float sliderValue)
    {
        if (sliderValue <= 0.25f)
            return 30;
        else if (sliderValue <= 0.75f)
            return 60;
        else
            return 120;
    }
}
