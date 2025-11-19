using UnityEngine;
using TMPro;

public class UIChoice : MonoBehaviour
{

    public TextMeshProUGUI textLabel;


    public void UpdateDisplay(Choice choice)
    {
        // Update le titre
        textLabel.text = choice.Label;
        // Update la description
        // Update 
    }

}
