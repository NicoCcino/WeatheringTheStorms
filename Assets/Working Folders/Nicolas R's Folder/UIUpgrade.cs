using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UIUpgradeParameter : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] public Button button;

    [Header("Upgrade data References")]
    [SerializeField] public UpgradeData upgradeData;

    private Upgrade upgrade;

    private TextMeshProUGUI buttonText;

    private void Start()
    {
        upgrade = new Upgrade(upgradeData);
        if (button != null && upgradeData != null)
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

        // Subscribe to ComputePower OnCP event
        if (ComputePower.Instance != null)
        {
            ComputePower.Instance.OnCP += OnComputePowerChanged;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from ComputePower OnCP event to prevent memory leaks
        if (ComputePower.Instance != null)
        {
            ComputePower.Instance.OnCP -= OnComputePowerChanged;
        }
    }

    private void OnComputePowerChanged(int newValue)
    {
        // Callback when compute power value changes
        // Add your logic here to update UI based on the new compute power value
        //Debug.Log($"Compute Power changed to: {newValue}");
        if (upgrade != null && button != null && buttonText != null)
        {
            if (upgrade.IsUnlocked)
            {
                return;
            }
            bool IsValid = newValue >= upgrade.UpgradeData.Cost;
            // Enable button if valid, disable if not
            button.interactable = IsValid;
            // Change button text color to black if valid, grey disabled if not
            if (IsValid)
            {
                buttonText.color = Color.black;
            }
            else
            {
                buttonText.color = Color.grey;
            }
        }
    }

    public void OnClick()
    {
        if (upgrade == null)
        {
            Debug.LogWarning("UIUpgrade: OnClick called but upgrade is null!");
            return;
        }
        
        //Debug.Log("UIUpgrade: OnClick: " + upgrade.UpgradeData.Label);
        upgrade.Unlock();
        // Change button text color to green
        if (buttonText != null)
        {
            buttonText.color = Color.green;
        }
    }
}
