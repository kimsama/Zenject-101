using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class User : MonoBehaviour
{
    [Inject]
    ReferenceData referenceData;

    void Awake ()
    {
        Debug.Log(referenceData.name);
    }
}
