#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;
using Zenject.Internal;

namespace Zenject
{
    public class SubContainerCreatorByNewPrefabWithParams : ISubContainerCreator
    {
        readonly DiContainer _container;
        readonly IPrefabProvider _prefabProvider;
        readonly Type _installerType;
        readonly GameObjectCreationParameters _gameObjectBindInfo;

        public SubContainerCreatorByNewPrefabWithParams(
            Type installerType, DiContainer container, IPrefabProvider prefabProvider,
            GameObjectCreationParameters gameObjectBindInfo)
        {
            _gameObjectBindInfo = gameObjectBindInfo;
            _prefabProvider = prefabProvider;
            _container = container;
            _installerType = installerType;
        }

        protected DiContainer Container
        {
            get { return _container; }
        }

        DiContainer CreateTempContainer(List<TypeValuePair> args)
        {
            var tempSubContainer = Container.CreateSubContainer();

            var installerInjectables = TypeAnalyzer.GetInfo(_installerType);

            foreach (var argPair in args)
            {
                // We need to intelligently match on the exact parameters here to avoid the issue
                // brought up in github issue #217
                var match = installerInjectables.AllInjectables
                    .Where(x => argPair.Type.DerivesFromOrEqual(x.MemberType))
                    .OrderBy(x => ZenUtilInternal.GetInheritanceDelta(argPair.Type, x.MemberType)).FirstOrDefault();

                Assert.IsNotNull(match,
                    "Could not find match for argument type '{0}' when injecting into sub container installer '{1}'",
                    argPair.Type, _installerType);

                tempSubContainer.Bind(match.MemberType)
                    .FromInstance(argPair.Value, true).WhenInjectedInto(_installerType);
            }

            return tempSubContainer;
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args)
        {
            Assert.That(!args.IsEmpty());

            var prefab = _prefabProvider.GetPrefab();
            var gameObject = CreateTempContainer(args).InstantiatePrefab(prefab, _gameObjectBindInfo);

            var context = gameObject.GetComponent<GameObjectContext>();

            Assert.IsNotNull(context,
                "Expected prefab with name '{0}' to container a component of type 'GameObjectContext'", prefab.name);

            // Note: We don't need to call ValidateValidatables here because GameObjectContext does this for us

            return context.Container;
        }
    }
}

#endif


