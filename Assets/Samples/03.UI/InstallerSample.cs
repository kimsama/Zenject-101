using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
