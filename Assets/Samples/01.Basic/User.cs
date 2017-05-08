using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class User : MonoBehaviour
{
    [Inject]
    SharedData sharedData;

    void Awake ()
    {
        Debug.Log(sharedData.name);
    }
}
