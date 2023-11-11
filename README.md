# RazorStyle

A library to allow adding CSS locally inside .razor components without duplication.

![Nuget](https://img.shields.io/nuget/v/wihrl.RazorStyle)
![Nuget](https://img.shields.io/nuget/dt/wihrl.RazorStyle)

### Why?

While using `<style> ... </style>` within Blazor components works fine, each instance of the component creates a new
style tag.
This results in unnecessary junk in the DOM as well as potential styling conflicts.

Using CSS isolation (`.razor.css`) fixes these issues, but having styles in a separate file sucks.

### Usage

1. Add `@using RazorStyle` to your `_Imports.cs`. (This is necessary for IDE support)

2. Add the `StyleRoot` component to any singleton component, for example `App.razor`. The root component creates a
   single `<style>` tag to be
   shared between all instances of `<RazorStyle.Style>`.

```csharp
// App.razor

// ...

<StyleRoot />
    
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

**BEWARE:** This library does not handle CSS isolation! Make sure to use
unique class names / selectors to avoid conflicts between components.

### Triggered animations

Adding a `_triggered_` prefix to `@keyframes` blocks will duplicate it with a different name.
This can be used in combination with `AnimationTrigger` to replay an animation programatically without relying on JS.

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
        // ! do NOT include the animation name ! 
        animation: 1s linear;
        // ...
    }

    @@keyframes _triggered_fly-in {
        // ...
    } 
</Style>
````