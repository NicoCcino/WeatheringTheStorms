using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UIUpgradeParameter : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] public Button button;

    [Header("Upgrade References")]
    [SerializeField] public Upgrade upgrade;

    private void Start()
    {
        TextMeshProUGUI buttonText = null;
        if (button != null && upgrade != null)
        {
            // Find the button's text component
            buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = upgrade.UpgradeData.Label;
            }

            // Remove any existing listeners to prevent duplicates
            button.onClick.RemoveListener(OnClick);
            // Set up click listener
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogWarning("UIUpgradeButton: Button or Upgrade is not assigned!");
        }
    }

    public void OnClick()
    {
        if (upgrade == null)
        {
            Debug.LogWarning("UIUpgrade: OnClick called but upgrade is null!");
            return;
        }
        
        Debug.Log("UIUpgrade: OnClick: " + upgrade.UpgradeData.Label);
        if (upgrade.IsValid == false)
        {
            Debug.LogWarning("UIUpgrade: Upgrade is not valid!");
            return;
        }
        upgrade.Unlock();
    }
}
