using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RazorStyle;

public class Style : ComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public const string TriggerPrefix = "_triggered_";
    public const string TriggerCloneSuffix = "-2";

    static readonly char[] _nameEndChars = { ' ', '{', '\r', '\n' };


    protected override void OnInitialized()
    {
        if (ChildContent is null)
            return;

        if (StyleRoot.LockIfFragmentMissing(ChildContent))
            StyleRoot.AddFragmentAndUnlock(ChildContent, BuildTriggerAnimations());
    }

    string BuildTriggerAnimations()
    {
        var builder = new RenderTreeBuilder();
        builder.AddContent(0, ChildContent);

        var markup =
            builder.GetFrames()
                .Array.Where(x => x.MarkupContent is not null)
                .Select(x => x.MarkupContent)
                .Aggregate("", (x, y) => x + y);
        
        var markupSpan = markup.AsSpan();

        StringBuilder sb = new(markup);
        sb.AppendLine();

        var nameStartIndex = -1;
        while ((nameStartIndex = markup.IndexOf(TriggerPrefix, nameStartIndex + 1, StringComparison.Ordinal)) >= 0)
        {
            var nameEndIndex = markup.IndexOfAny(_nameEndChars, nameStartIndex);
            var bodyStartIndex = markup.IndexOf('{', nameEndIndex);
            var bodyEndIndex = bodyStartIndex + 1;
            var blockEntered = false;

            while (++bodyEndIndex < markup.Length)
            {
                var character = markup[bodyEndIndex];
                if (character == '}')
                {
                    if (!blockEntered)
                        break;

                    blockEntered = false;
                }

                if (character == '{')
                    blockEntered = true;
            }

            sb.Append("@keyframes ");
            sb.Append(markupSpan[nameStartIndex..nameEndIndex]);
            sb.AppendLine(TriggerCloneSuffix);
            sb.Append(markupSpan[bodyStartIndex..bodyEndIndex]);
            sb.AppendLine("}");
        }

        return sb.ToString();
    }
}