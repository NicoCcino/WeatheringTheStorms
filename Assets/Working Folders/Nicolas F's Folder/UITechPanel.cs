using UnityEngine;
using WiDiD.UI;

public class UITechPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private CanvasGroupCustom fadeComponent;
    private bool isUITechPanelVisible = false;

    void OnEnable()
    {

    }

    public void FadeToggle()
    {
        if (isUITechPanelVisible)
        {
            UIUpgradeManager.Instance.Display(false);
            isUITechPanelVisible = false;
        }
        else
        {
            UIUpgradeManager.Instance.Display(true);
            isUITechPanelVisible = true;
        }
    }

    void Start()
    {
        UIUpgradeManager.Instance.Display(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
