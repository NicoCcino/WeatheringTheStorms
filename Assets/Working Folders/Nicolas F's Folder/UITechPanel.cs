using UnityEngine;
using WiDiD.UI;

public class UITechPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private CanvasGroupCustom fadeComponent;
    private bool isUITechPanelVisible = false;

    void OnEnable()
    {
        fadeComponent = GetComponent<CanvasGroupCustom>();
    }

    public void FadeOutSelf()
    {
        fadeComponent.Fade(false);
        isUITechPanelVisible = false;
    }
    public void FadeInSelf()
    {
        fadeComponent.Fade(true);
        isUITechPanelVisible = true;
    }
    public void FadeToggle()
    {
        if (isUITechPanelVisible)
        {
            FadeOutSelf();
        }
        else
        {
            FadeInSelf();
        }
    }

    void Start()
    {
        FadeOutSelf();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
