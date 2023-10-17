namespace RazorStyle;

public class AnimationTrigger
{
    public AnimationTrigger(string animationName)
    {
        AnimationName = animationName;
    }

    public string AnimationName { get; }
    public string ActiveName => State ? AnimationName + Style.CloneSuffix : AnimationName;
    public string CSS => $"animation-name: {ActiveName};";
    public bool State { get; private set; }

    public void Trigger() => State = !State;

    public override string ToString() => CSS;
}