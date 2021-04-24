using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Text textVolumeSlider;
    public Toggle fullscreenToggle;
    bool fullscreen = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVolumeSlider(volumeSlider, textVolumeSlider);
    }



    public void ToggleGameobject(GameObject go)
    {
        if (go.active)
            go.SetActive(false);
        else
            go.SetActive(true);
    }

    public void UpdateResolution(Dropdown dd)
    {
        fullscreen = fullscreenToggle.isOn;
        switch (dd.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, fullscreen);
                break;
            case 1:
                Screen.SetResolution(1920, 1200, fullscreen);
                break;
            case 2:
                Screen.SetResolution(1690, 1050, fullscreen);
                break;
            case 3:
                Screen.SetResolution(1440, 900, fullscreen);
                break;
            case 4:
                Screen.SetResolution(1280, 800, fullscreen);
                break;
            case 5:
                Screen.SetResolution(1280, 720, fullscreen);
                break;
            case 6:
                Screen.SetResolution(1024, 768, fullscreen);
                break;
            case 7:
                Screen.SetResolution(800, 600, fullscreen);
                break;
            case 8:
                Screen.SetResolution(640, 480, fullscreen);
                break;
            default:
                Screen.SetResolution(1920, 1080, true);
                break;
        }
    }


    void UpdateVolumeSlider(Slider slider, Text text)
    {
        if (slider.value.ToString() != text.text)
        {
            text.text = slider.value.ToString();
            
        }
    }

}
