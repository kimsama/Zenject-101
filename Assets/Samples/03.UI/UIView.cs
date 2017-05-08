using System.Collections;
using System.Collections.Generic;
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
