using TMPro;
using UnityEngine;
using WiDiD.UI;

public class UIHoverInformationsManager : Singleton<UIHoverInformationsManager>
{
    [SerializeField] private TextMeshProUGUI textInfo;
    [SerializeField] private CanvasGroupCustom panelInfos;
    public void DisplayInformations(UIHoverInformation uIHoverInformation)
    {
        textInfo.text = uIHoverInformation.DisplayedText;
        panelInfos.Fade(true);
    }
    public void HideInformations(UIHoverInformation uIHoverInformation)
    {
        if (textInfo.text == uIHoverInformation.DisplayedText)
        {
            //  textInfo.text = "";
            panelInfos.Fade(false);
        }
    }
}
