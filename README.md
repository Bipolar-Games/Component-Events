# Component Events
[![Unity 2021.1+](https://img.shields.io/badge/unity-2021.1%2B-blue.svg)](https://unity3d.com/get-unity/download)
![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)

## Installation 
There are no custom steps to adding the package to your project. Just choose one of following installation methods:

1) Download ZIP of the repository and extract it to your project Assets folder.

2) Add package through Unity Package Manager by choosing "Add package from git URL..." option and then typing "https://github.com/Bipolar-Games/Component-Events.git".

3) Add the Component Events repository as submodule to your project git repository.

4) Use any other method you prefer.

## Rationale
C# events are generally preferable to UnityEvents, especially in terms of performance, refactor safety, and debugging convenience.

### UnityEvents advantages
However, despite their shortcomings, UnityEvents provide one extremely valuable feature — serialization and Inspector visibility. Game developers (particularly non-programming designers) can attach functions to events directly in the Editor without modifying code. This significantly speeds up prototyping and encourages loose coupling between components.

### UnityEvents problems
That convenience, however, comes with a serious limitation.

UnityEvent bindings are serialized by method name and argument signature. Any change to the method name or its parameters breaks the reference. In many cases, no error is reported until the event is invoked — which may happen much later during runtime. This makes debugging even more difficult.

Because of this, UnityEvents are best suited for simple, low-risk scenarios. In systems where reliability and refactor safety are critical, native C# events remain the preferred solution.

### Alternative approach
For people who want to join reliability with convenience, using C# events while also exposing UnityEvents might be a solution. However this results in duplicated wiring logic:
the same event must be declared in code, joined with UnityEvent, manually forwarded, and maintained. Over time, this repetition increases boilerplate, reduces clarity, and introduces additional maintenance overhead.

### Solution
This module was created to eliminate that duplication. It provides a component that automatically creates Inspector-visible UnityEvents for every C# event of specified compnents. This preserves the reliability and refactor safety of native C# events, while restoring the convenience of Unity’s Inspector-based workflow.

## Usage

### Initial situation
At the beginning a Component with at least one public event is needed.
For example:
```csharp
public class Health : MonoBehaviour
{
    public event System.Action<int> OnHealthChanged;

    public void TakeDamage(int amount)
    {
        OnHealthChanged?.Invoke(amount);
    }
}
```

### Adding ComponentEvents
Attach the `ComponentEvents` component to the same GameObject (or any other GameObject) and reference the target component.
The component will automatically detect and expose all supported C# events in the Inspector.

<img width="386" height="230" alt="image" src="https://github.com/user-attachments/assets/991e0d5c-a298-4a86-83ca-614cd51234fd" />


### Binding Listeners in the Inspector
Each detected C# event has a serialized UnityEvent representation, which gets called each time the original C# event is invoked.

You can now attach:
- Methods from other object
- UI callbacks
- Audio triggers
- VFX triggers

No additional wrapper code is required.














