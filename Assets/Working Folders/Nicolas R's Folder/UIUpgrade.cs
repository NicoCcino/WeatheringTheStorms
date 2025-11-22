using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class UIUpgrade : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] public Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject gameObjectLocked;

    [Header("Upgrade data References")]
    [SerializeField] public Upgrade upgrade;

    private void OnEnable()
    {
        if (upgrade == null)
        {
            Debug.LogError($"You forget to fill the upgrade field on one of the upgrade button.Name of the button : {button.gameObject.name}");
            return;
        }
        button.interactable = false;
        button.onClick.AddListener(OnButtonClickCallback);

        Display(upgrade);
        RefreshStatus();
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClickCallback);
    }
    private void OnButtonClickCallback()
    {
        if (!upgrade.IsUnlocked) return;

        upgrade.Buy();
    }
    private void RefreshStatus()
    {
        button.interactable = upgrade.IsBuyable();
    }
    private void Display(Upgrade upgrade)
    {
        buttonText.text = upgrade.UpgradeData.Label;
    }
    public void OnClick()
    {
        if (upgrade == null)
        {
            Debug.LogWarning("UIUpgrade: OnClick called but upgrade is null!");
            return;
        }
        if (buttonText != null)
        {
            buttonText.color = Color.green;
        }
    }
}
