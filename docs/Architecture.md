# 🧩 Project Architecture with Prism (WPF + Unity)

- [Back to main](../README.md)

At a certain point, I realized that keeping all launcher logic inside `MainWindow` is a dead-end approach.

As the project grows, everything starts piling up in one place: game launch, updates, settings, tray logic, config handling, services, UI state, and tons of event handlers.

So I migrated the project structure to **Prism + Unity** to separate responsibilities, clean up the codebase, and prepare the project for further scaling.

---

## 📁 Base Project Structure

```text
📂 LizeriumLauncher
│   App.xaml
│   App.xaml.cs        // PrismApplication entry point
│
├── 📂 Bootstrapper / Infrastructure
│   ├── UnityConfig.cs // dependency registration
│   ├── RegionNames.cs // region constants
│   └── ...
│
├── 📂 Services
│   ├── IGameService.cs
│   ├── GameService.cs
│   ├── ISettingsService.cs
│   ├── SettingsService.cs
│   └── ...
│
├── 📂 Models
│   ├── GameButtonData.cs
│   └── ...
│
├── 📂 Views
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── SettingsGameWindow.xaml
│   ├── UpdatesWindow.xaml
│   └── ...
│
├── 📂 ViewModels
│   ├── MainWindowViewModel.cs
│   ├── SettingsGameWindowViewModel.cs
│   ├── UpdatesWindowViewModel.cs
│   └── ...
│
└── 📂 Modules
    ├── GameModule
    │   ├── GameModule.cs
    │   ├── Views/...
    │   ├── ViewModels/...
    │   └── Services/...
    │
    ├── SettingsModule
    │   ├── SettingsModule.cs
    │   ├── Views/...
    │   ├── ViewModels/...
    │   └── Services/...
    │
    └── ...
```

---

# ⚙️ Why I chose Prism + Unity

I use **Prism** as the architectural foundation for the WPF application, and **Unity Container** for dependency management.

This gives me several key advantages:

- I’m no longer tied to a single `MainWindow`
- I can split functionality into modules
- I can properly inject dependencies
- UI logic moves into **ViewModels**, instead of code-behind
- the application becomes easier to scale, maintain, and test

In short — this is no longer just a “window with buttons”, but a **properly structured client application**.

---

# 🧠 What changed after the migration

Previously, a lot of logic lived directly inside `MainWindow.xaml.cs`:

- game launching
- file updates
- config handling
- window navigation
- tray logic
- application state management

After switching to Prism, I started organizing everything by responsibility.

---

## What goes into `Services/`

This is where I place all logic that **does not belong in the UI**.

For example:

- launching the game
- working with file paths
- reading/saving settings
- update logic
- file validation
- launcher core logic

Example:

```csharp
public interface IGameService
{
    void LaunchGame();
}
```

```csharp
public class GameService : IGameService
{
    public void LaunchGame()
    {
        // game launch logic
    }
}
```

---

## What goes into `ViewModels/`

ViewModels contain **UI state and commands**, not business logic.

For example:

- button text
- state flags
- selected options
- commands (launch, update, open windows)

Example:

```csharp
public class MainWindowViewModel : BindableBase
{
    private readonly IGameService _gameService;

    public DelegateCommand LaunchGameCommand { get; }

    public MainWindowViewModel(IGameService gameService)
    {
        _gameService = gameService;
        LaunchGameCommand = new DelegateCommand(OnLaunchGame);
    }

    private void OnLaunchGame()
    {
        _gameService.LaunchGame();
    }
}
```

So:

- `ViewModel` defines **what to do**
- `Service` defines **how it is done**

---

## What stays in `Views/`

Views contain only the visual layer:

- XAML
- layout
- bindings
- visual containers
- Prism regions

The window should **render**, not “own the entire application logic”.

---

## What goes into `Modules/`

When functionality becomes large, I isolate it into modules.

Examples:

- settings module
- update module
- game management module
- diagnostics module
- content/news module

This prevents the project from turning into a mess of hundreds of files in one place.

---

# 🔄 Application lifecycle (Prism)

With Prism, the application startup becomes structured and predictable.

## Flow:

1. `App.xaml.cs` starts
2. Application inherits from `PrismApplication`
3. Prism calls `RegisterTypes()`
4. Services and dependencies are registered
5. Prism calls `CreateShell()`
6. Main window (`MainWindow`) is created
7. Modules are loaded
8. Views are injected into regions

---

# 🏗 Key concepts in Prism + Unity

## 1. `App.xaml.cs` inherits from `Prism.Unity.PrismApplication`

The application runs as a **Prism Shell application**, not a plain WPF app.

Example:

```csharp
public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IGameService, GameService>();
        containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
    }
}
```

---

## 2. Centralized dependency registration

Instead of manually calling `new SomeService()` everywhere, everything is registered in the container:

```csharp
containerRegistry.RegisterSingleton<IGameService, GameService>();
containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
```

Dependencies are then injected automatically.

---

## 3. MainWindow is not created manually

Previously:

```csharp
var window = new MainWindow();
window.Show();
```

With Prism, this is handled by the container.

`MainWindow` is created as the **Shell**, meaning:

👉 the app is built as an architecture, not a set of manual calls

---

## 4. Logic moves out of the View

I keep `MainWindow.xaml.cs` minimal — only UI-specific logic.

If something:

- launches the game
- checks configs
- updates data
- works with files

→ it does NOT belong in the View

---

# 🧱 My project structure philosophy

In short:

## `Services`

> Everything that performs actual work

## `ViewModels`

> Everything that manages UI state and commands

## `Views`

> Everything that is rendered

## `Models`

> Data structures

## `Modules`

> Isolated functional areas

---

# 🚫 Why I abandoned “everything in MainWindow”

Because it breaks very quickly:

- code becomes unreadable
- reuse becomes impossible
- adding features becomes painful
- debugging gets harder
- the project becomes rigid

Prism allows me to build the application **as a system**, not as a single giant event handler.

---

# ✅ Conclusion

Switching to **Prism + Unity** is not about “using patterns for the sake of it”.

It’s about keeping the project maintainable as it grows.

I use this architecture to:

- separate responsibilities
- keep `MainWindow` clean
- scale functionality
- plug in modules
- control dependencies
- build a real application, not a temporary UI script
