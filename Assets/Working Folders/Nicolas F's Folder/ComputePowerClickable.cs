public class ComputePowerClickable : GridClickable<ComputePowerLootData>
{
    protected override void OnClick()
    {
        ComputePower.Instance.AddComputePower(ComputePower.Instance.computePowerParameter.ComputePowerClickableGain);
    }
}
