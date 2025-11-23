public class PromptClickable : GridClickable<Prompt>
{
    protected override void OnClick()
    {
        Timeline.Instance.SetPauseSpeed();
        PromptManager.Instance.OpenPrompt(LinkedGridObject);
    }
}