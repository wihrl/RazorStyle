# RazorStyle
A library to allow adding CSS locally inside .razor components without duplication.

### Why?
While using `<style> ... </style>` within Blazor components works fine, each instance of the component creates a new style tag.
This results in unnecessary junk in the DOM as well as potential styling conflicts.

Using CSS isolation (`.razor.css`) fixes these issues, but having styles in a separate file is annoying.

### Usage
1. Add `@using RazorStyle` to your `_Imports.cs`. (This is necessary for CSS editor support)

2. Add the `CssRoot` component to any singleton component, for example `App.razor`. The root component creates a single `<style>` tag to be
shared between all instances of `<RazorStyle.Style>`.
```csharp
// App.razor

// ...

<CssRoot />
    
<Router AppAssembly="@typeof(App).Assembly">
    // ...
</Router>

// ...
```

3. Use `<Style>` instead of `<style>` in your components.
```csharp
// SomeComponent.razor

<h1 class="title">Foo</h1>

<Style>
.title {
    // ...
}
</Style>
````

**BEWARE:** This library does not handle CSS isolation! Make sure to use unique class names / selectors to avoid conflicts.

### AnimationTrigger and class duplication
`<Style>` blocks can be rendered twice to allow duplicating CSS. This can be useful when trying
to trigger an animation without relying on JS.

```csharp
// SomeAnimatedComponent.razor

@code {

    readonly AnimationTrigger _titleAnimation = new("fly-in");


    void Trigger()
    {
        _titleAnimation.Trigger();
        StateHasChanged();
    }

}

<button @onclick="Trigger">Trigger</button>

<h1 class="title" style="@_titleAnimation">Home</h1>

<Style>
    .title {
        animation: 1s linear;
        // ...
    }
</Style>

<Style Clone="true">
    @@keyframes @context("fly-in") {
        // ...
    }
</Style>
````
Note that *all* contents of a cloneable Style block will be rendered twice, so keep any CSS
you do not want to duplicate in a separate block.