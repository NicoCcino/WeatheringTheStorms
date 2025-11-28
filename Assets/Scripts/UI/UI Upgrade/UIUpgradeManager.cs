using TMPro;
using UnityEngine;
using WiDiD.UI;

public class UIUpgradeManager : Singleton<UIUpgradeManager>
{
    private UIUpgrade[] allUiUpgrades;
    [SerializeField] private GameObject panelDescription;
    [SerializeField] private TextMeshProUGUI textHoveredUpgradeDescription;
    [SerializeField] private CanvasGroupCustom canvasGroupCustom;


    private UIUpgrade hoveredUiUpgrade;
    public UIUpgrade HoveredUiUpgrade
    {
        get => hoveredUiUpgrade;
        set
        {
            if (value == hoveredUiUpgrade)
                return;
            hoveredUiUpgrade = value;
            DisplayUpgradeDescription(hoveredUiUpgrade);
        }
    }
    public void Display(bool on)
    {
        if (on)
            Timeline.Instance.SetPauseSpeed(false);
        else
            Timeline.Instance.ResumeSpeed(false); // Maybe not resume speed
        canvasGroupCustom.Fade(on);
    }
    private void Start()
    {
        allUiUpgrades = GetComponentsInChildren<UIUpgrade>();
        ComputePower.Instance.OnCP += RefreshBuyableStatus;
        panelDescription.gameObject.SetActive(false);
    }

    private void RefreshBuyableStatus(int obj)
    {
        for (int i = 0; i < allUiUpgrades.Length; i++)
        {
            if (!allUiUpgrades[i].upgrade.IsUnlocked) continue;
            allUiUpgrades[i].RefreshStatus();
        }
    }

    private void OnDisable()
    {
        ComputePower.Instance.OnCP -= RefreshBuyableStatus;
    }
    private void DisplayUpgradeDescription(UIUpgrade uiUpgrade)
    {
        if (uiUpgrade == null)
        {
            panelDescription.SetActive(false);
            textHoveredUpgradeDescription.text = "";
            return;
        }
        panelDescription.SetActive(true);
        textHoveredUpgradeDescription.text = uiUpgrade.upgrade.UpgradeData.Description;
    }

}
