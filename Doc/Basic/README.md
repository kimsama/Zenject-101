# Basic

여기에서는 Zenject 사용을 위한 가장 기본적인 방법에 대해서 알아 본다.

* SceneContext
* ZenjectBinding
* Inject Attribute

Zenject를 사용하기 위해서는 먼저 Zenject의 Container에 대한 이해가 필요하다. Zenject의 가장 기본이 되는 작동은 아래와 같다.

1. Container에 객체의 형(type)을 등록.
2. Container에 등록된 객체의 인스턴스를 생성.

*'Container'* 는 참조에 사용할 객체들을 담고 있는 장소에 해당한다. 다른 클래스에서 필요로 하는 참조가 있는 경우 이 Container를 통해 참조에 대한 의존 관계를 해결하는 것이 바로 DI(Dependency Injection)이다. 그러므로 이 Container에 등록되지 않은 객체는 DI를 이용해서 참조할 수 없다.

``` csharp
public class Foo {}

public class BindClass
{
  public BindClass (Foo foo)
  {
  }
}

public class Installer
{
  void Bind()
  {
    // Foo 클래스 등록.
    Container.Bind<Foo>().AsSingle();
    // BindClass 등록.
    Container.Bind<BindClass>().AsSingle();
    // BindClass의 인스턴스를 생성.
    Container.Resolve<BindClass>();
  }
}
```

마지막 라인의 Resolve 호출부터 살펴보자. 우선 Resolve<BindClass>로 BindClass의 인스턴스를 생성한다. BindClass의 생성자를 보면 Foo 클래스가 필요한다. Container.Bind<Foo>로 Container에 Foo 클래스가 등록되어 있으므로 BindClass의 인스턴스 생성시 Foo 클래스의 인스턴스에 대한 참조가 이루어진다. 이렇게 Container.Bind로 참조 관계를 정의하는 것이 Container 사용의 핵심이다.  

## SceneContext 생성.

GameObject > Zenject > SceneContext 메뉴를 선택하면 Hierarchy 뷰에 SceneContext 컴포넌트를 포함하는 GameObject가 하나 생성된다.

## ReferenceData 클래스 생성.

```csharp
using UnityEngine;

public class ReferenceData : MonoBehaviour
{
}
```

## ZenjectBinding 컴포넌트

ZenjectBinding의 `Components`에 UnityEngine.Component를 상속한 클래스를 추가하여 Container에 등록할 수 있다.

ZenjectBinding 컴포넌트를 추가하고 Components에 *'ReferenceData'* 컴포넌트를 추가한다.


*'참조(Referece)'* 의 관점에서 생각하면 참조가 되는 쪽과 참조하는 쪽으로 구분해서 생각할 수 있다. ZenjectBinding 컴포넌트는 참조가 되는-참조 당하는-쪽의 GameObject 객체에 설정하는 컴포턴트가 된다.



## User 클래스

User 클래스는 참조를 사용하는 쪽에 해당하는 클래스이다. 이 User 클래스에서 참조할 클래스는 *'ReferenceData'* 클래스이다.


``` csharp
using UnityEngine;
using Zenject;

public class User : MonoBehaviour
{
    // Container에 등록된 ReferenceData 인스턴스를 여기에 주입.
    [Inject]
    ReferenceData referenceData;

    void Awake ()
    {
        // ReferenceData 인스턴스가 생성되어 있다.
        Debug.Log(referenceData.name);
    }
}
```

MonoBehaviour를 상속한 클래스의 경우 Inject 속성을 사용하여 자동으로 참조를 설정할 수 있다.

User.Awake 함수가 호출되는 시점에서 이미 ZenjectBinding에 ReferenceData 인스턴스로 Cointainer에 등록되어 있기 때문에 ReferenceData 인스턴스를 참조하기 위한 별도의 처리 과정이 없이도 ReferenceData 인스턴스의 사용이 가능하다.


>``` csharp
>public class User : MonoBehaviour
>{
>    // 이렇게 public으로 선언한 후 ReferenceData가 있는 GameObject를
>    // 마우스로 드래그해서 참조하는 방법을 사용하지 않아도 된다는 이야기다.
>    public ReferenceData referenceData;
>```
