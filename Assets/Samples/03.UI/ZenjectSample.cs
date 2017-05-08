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
