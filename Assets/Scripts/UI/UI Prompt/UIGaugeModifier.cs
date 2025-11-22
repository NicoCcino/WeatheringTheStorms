using UnityEngine;
using UnityEngine.UI;

public class UIGaugeModifier : MonoBehaviour
{
    [SerializeField] private Image amount;
    public void DisplayModifier(AnimationCurve curveAmountScaling, Modifier modifier)
    {
        float scaling = curveAmountScaling.Evaluate(Mathf.Abs(modifier.AddedValue));
        amount.transform.localScale = new Vector3(scaling, scaling, 1);
    }
}
