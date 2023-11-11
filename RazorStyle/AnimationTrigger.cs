namespace RazorStyle;

public class AnimationTrigger
{
    private readonly string _a, _b;
    
    public AnimationTrigger(string animationName)
    {
        AnimationName = animationName;
        _a = animationName.StartsWith(RazorStyle.Style.TriggerPrefix) ? animationName : RazorStyle.Style.TriggerPrefix + animationName;
        _b = _a + RazorStyle.Style.TriggerCloneSuffix;
    }

    public string AnimationName { get; }
    public string ActiveClass => State ? _b : _a;
    public bool State { get; private set; }

    public string Style => $"animation-name: {ActiveClass};";

    public void Trigger() => State = !State;
    public override string ToString() => Style;
}