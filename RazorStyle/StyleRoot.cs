using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RazorStyle;

public class StyleRoot : ComponentBase, IDisposable
{
    private static readonly ReaderWriterLockSlim _lock = new();
    private static readonly Dictionary<object, string> _fragments = new();
    private static event Action? FragmentAdded;

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

    internal static bool LockIfFragmentMissing(object key)
    {
        _lock.EnterUpgradeableReadLock();
        
        if (!_fragments.ContainsKey(key))
        {
            _lock.EnterWriteLock();
            return true;
        }

        _lock.ExitUpgradeableReadLock();
        return false;
    }

    internal static void AddFragmentAndUnlock(object key, string fragment)
    {
        _fragments.Add(key, fragment);
        
        _lock.ExitWriteLock();
        _lock.ExitUpgradeableReadLock();
        
        FragmentAdded?.Invoke();
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