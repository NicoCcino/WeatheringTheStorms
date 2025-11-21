using UnityEngine;
using WiDiD.UI;

public class UITechPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private CanvasGroupCustom fadeComponent;

    public void FadeOutSelf()
    {
        fadeComponent = this.GetComponent<CanvasGroupCustom>();
        fadeComponent.Fade(false);
    }
    public void FadeInSelf()
    {
        fadeComponent = this.GetComponent<CanvasGroupCustom>();
        fadeComponent.Fade(true);
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
