using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

[System.Serializable]
public class UIUpgrade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button References")]
    [SerializeField] public Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI textCost;
    [SerializeField] private GameObject gameObjectLocked;
    [SerializeField] private GameObject gameObjectBought;

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
        upgrade.OnUnlocked += RefreshStatus;

        Display(upgrade);
        RefreshStatus();
        SubscribeToParentsBought();
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClickCallback);
        upgrade.OnUnlocked -= RefreshStatus;
        UnsubscribeFromParentsBought();
    }
    private void OnButtonClickCallback()
    {
        if (!upgrade.IsUnlocked) return;

        upgrade.Buy();
        RefreshStatus();
    }
    public void RefreshStatus()
    {
        gameObjectLocked.SetActive(!upgrade.TryUnlock());
        gameObjectBought.SetActive(upgrade.IsBought);
        button.interactable = upgrade.IsBuyable();
    }
    private void Display(Upgrade upgrade)
    {
        buttonText.text = upgrade.UpgradeData.Label;
        textCost.text = upgrade.UpgradeData.Cost.ToString();
    }
    private void SubscribeToParentsBought()
    {
        foreach (var u in upgrade.ParentUpgrades)
        {
            u.OnBought += OnParentBoughtCallback;
        }
    }
    private void UnsubscribeFromParentsBought()
    {
        foreach (var u in upgrade.ParentUpgrades)
        {
            u.OnBought -= OnParentBoughtCallback;
        }
    }
    private void OnParentBoughtCallback()
    {
        RefreshStatus();
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIUpgradeManager.Instance.HoveredUiUpgrade = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UIUpgradeManager.Instance.HoveredUiUpgrade == this)
            UIUpgradeManager.Instance.HoveredUiUpgrade = null;
    }
}
