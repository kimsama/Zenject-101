# UI

여기에서는 Zenject를 사용하여 uGUI 버튼 객체의 참조와 관련한 종속성을 처리하는 방법에 대해서 다룬다.


## Context 생성

SceneContext 컴포넌트를 가지는 GameObject를 생성한다.

## Installer 생성

*Installer란?...*

``` csharp
using System;
using Zenject;

public class InstallerSample : MonoInstaller<InstallerSample>
{
    public override void InstallBindings()
    {
        Container.Bind<ZenjectSample>().AsSingle();
        Container.Bind<IInitializable>().To<ZenjectSample>().AsSingle();
        Container.Bind<IDisposable>().To<ZenjectSample>().AsSingle();
    }
}
```

## Context에 Installer 연결

SceneContext의 Installer에 InstallerSample을 연결한다.

## ZenjectBinding을 이용한 UI 객체 등록

``` csharp
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIView : MonoBehaviour
{
    [SerializeField]
    Button button;

    [SerializeField]
    Text text;

    public IObservable<string> OnClickObservable()
    {
        return button.OnClickAsObservable().Select(_ => text.text);
    }
}
```

## 의존성 주입

``` csharp
using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

public class ZenjectSample : IInitializable, IDisposable
{
    [Inject]
    List<UIView> buttons;

    List<IDisposable> subscriptions = new List<IDisposable>();

    void IInitializable.Initialize()
    {
        UnityEngine.Debug.Log("Initialize");

        buttons.ForEach(button =>
        {
            subscriptions.Add(button.OnClickObservable().Subscribe(text => OnClick(text)));
        });
    }

    void OnClick(string buttonText)
    {
        UnityEngine.Debug.Log(buttonText);
    }

    public void Dispose()
    {
        subscriptions.ForEach(subscription => subscription.Dispose());
        subscriptions.Clear();

        UnityEngine.Debug.Log("Dispose");
    }
}
```
