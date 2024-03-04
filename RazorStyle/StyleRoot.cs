using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RazorStyle;

public class StyleRoot : ComponentBase, IDisposable
{
    private static readonly ReaderWriterLockSlim _lock = new();
    private static readonly Dictionary<object, string> _fragments = new();
    private static event Action? FragmentAdded;
    
    public static bool EnableHotReload { get; set; }

    protected override void OnInitialized()
    {
        FragmentAdded += Invalidate;
        base.OnInitialized();
    }

    public void Dispose()
    {
        FragmentAdded -= Invalidate;
    }

    void Invalidate() => InvokeAsync(StateHasChanged);

    internal static void AddIfMissing(object key, Func<string> fragmentFactory)
    {
        _lock.EnterUpgradeableReadLock();

        if (EnableHotReload || !_fragments.ContainsKey(key))
        {
            _lock.EnterWriteLock();
            _fragments[key] = fragmentFactory();
            _lock.ExitWriteLock();
            
            FragmentAdded?.Invoke();
        }

        _lock.ExitUpgradeableReadLock();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        _lock.EnterReadLock();

        var seq = 0;
        builder.OpenElement(seq++, "style");
        foreach (var fragment in _fragments.Values) builder.AddMarkupContent(seq++, fragment);
        builder.CloseElement();

        _lock.ExitReadLock();
    }
}