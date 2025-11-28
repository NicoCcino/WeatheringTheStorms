public class PromptClickable : GridClickable<Prompt>
{
    protected override void OnClick()
    {
        Timeline.Instance.SetPauseSpeed(false);
        PromptManager.Instance.OpenPrompt(LinkedGridObject);
    }
}