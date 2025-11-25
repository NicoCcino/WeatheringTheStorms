using System;
using UnityEngine;
using UnityEngine.UI;

public class UITimelineController : MonoBehaviour
{
    [SerializeField] private Toggle togglePlayPause;
    [SerializeField] private GameObject disabledGo;
    [SerializeField] private Image imageIsPlaying;
    [SerializeField] private Image imageIsPaused;
    [SerializeField] private Toggle togglex1;
    [SerializeField] private Toggle togglex2;
    [SerializeField] private Toggle togglex4;

    private void OnEnable()
    {
        togglePlayPause.onValueChanged.AddListener(OnTogglePlayPauseCallback);
        togglex1.onValueChanged.AddListener(OnToggleX1PauseCallback);
        togglex2.onValueChanged.AddListener(OnToggleX2PauseCallback);
        togglex4.onValueChanged.AddListener(OnToggleX4PauseCallback);

        PromptManager.Instance.OnPromptOpened += OnPromptOpened;

    }

    private void OnPromptOpened(Prompt prompt)
    {
        togglex1.SetIsOnWithoutNotify(true);
    }

    private void OnDisable()
    {
        togglePlayPause.onValueChanged.RemoveListener(OnTogglePlayPauseCallback);
        togglex1.onValueChanged.RemoveListener(OnToggleX1PauseCallback);
        togglex2.onValueChanged.RemoveListener(OnToggleX2PauseCallback);
        togglex4.onValueChanged.RemoveListener(OnToggleX4PauseCallback);
    }
    private void OnToggleX4PauseCallback(bool val)
    {
        if (val == true)
        {
            Timeline.Instance.SetVeryFastSpeed();
        }
    }

    private void OnToggleX2PauseCallback(bool val)
    {
        if (val == true)
        {
            Timeline.Instance.SetFastSpeed();
        }
    }

    private void OnToggleX1PauseCallback(bool val)
    {
        if (val == true)
        {
            Timeline.Instance.SetPlaySpeed();
        }
    }

    private void OnTogglePlayPauseCallback(bool val)
    {
        if (!val)
        {
            Timeline.Instance.SetPauseSpeed();
            disabledGo.SetActive(true);
            imageIsPlaying.gameObject.SetActive(false);
            imageIsPaused.gameObject.SetActive(true);
            return;
        }
        disabledGo.SetActive(false);
        imageIsPlaying.gameObject.SetActive(true);
        imageIsPaused.gameObject.SetActive(false);
        OnToggleX1PauseCallback(togglex1.isOn);
        OnToggleX2PauseCallback(togglex2.isOn);
        OnToggleX4PauseCallback(togglex4.isOn);
    }
}
