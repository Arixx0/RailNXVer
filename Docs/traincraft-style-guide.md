# Traincraft Style Guide

## Dependencies and Version Table

트레인크래프트에서는 *Unity 2022.3.11f1 LTS*, *C# 9.0*을 기준으로 작업합니다.

UPM과 AssetStore를 통해서 추가된 패키지는 아래와 같습니다:

| Package Name | Version | Package Name | Version |
| --- | --- | --- | --- |
| ProBuilder | `5.1.1` | Custom NUnit | `1.0.6` |
| Polybrush | `1.1.5` | Editor Coroutines | `1.0.0` |
| FBX Exporter | `4.2.1` | Input System | `1.7.0` |
| Terrain Tools | `5.0.1` | Recorder | `4.0.1` |
| Burst | `1.8.9` | Sequences | `2.0.1` |
| Cinemachine | `2.9.5` | Test Framework | `1.1.33` |
| Code Coverage | `1.2.4` | TextMeshPro | `3.0.6` |
| Core RP Library | `14.0.9` | Timeline | `1.7.6` |
| Universal RP | `14.0.9` | Alembic | `2.3.2` |
| Autodesk FBX SDK for Unity | `4.2.1` | Mathematics | `1.2.6` |
| Memory Profiler | `1.0.0` |

> 프로젝트 생성 시 자동으로 추가되는 패키지는 리스트에서 제외되었습니다.
>
> 제외된 패키지: `Searcher`, `Settings Manager`, `Shader Graph`, `Unity UI`, `Version Control`, `Visual Effect Graph`, `Visual Scripting`

## Basic Rule

- 복사+붙여넣기 식의 코드는 금지합니다.
- *Copilot*, *AI Assistant*등의 coding AI를 이용하는것은 상관없으나, 어디까지나 **보조 도구**로서의 범위까지만 허용합니다.
  - *어떤 기능에 대해 통상적으로 어떻게 구현되는지*, *파일 내의 코드 개선점*, *네이밍 적합성* 등의 **평가 도구**로서 사용할것을 권장합니다.

## Naming Rules

`class`, `interface`, `struct`, `delegate`, `method`는 **파스칼 케이스**를 이용해 네이밍합니다.
- 단, `interface`는 접두사 `I`를 붙입니다.
- `delegate` 선언시에는 접미사 `Callback` 또는 `Delegate`를 붙입니다.

```csharp
public class UnitCostDataTuple { /*... */ }

public struct Coordinate { /* ... */ }

public interface ISliderValueChangeCallbackReceiver { /* ... */ }

// use post-fix "Delegate", so that can declare a delegate variable `OnTaskComplete`
public delegate void OnTaskCompletedDelegate();

public void ValidateProperties();
```

`const`, `static`이 추가된 필드와 **속성**은 한정자와 관계없이 **파스칼 케이스**를 이용합니다.

```csharp
private const float SkinWidth = 0.015f;

public const int SegmentCount = 5;

public static MonoBehaviour Get { get; set; }

public readonly static GUIContent PropertyLabel = new("Property"); 
```

멤버 변수들은 기본적으로 **카멜 케이스**를 사용합니다.
- `private` 한정자가 적용된 경우에는 접두사 `m_`을 사용합니다.
- **직렬화된 멤버 변수**는 **카멜 케이스**를 사용합니다.

```csharp
public bool enabled;

[UnityEngine.SerializeField] private float size;

private UnityEngine.Camera m_CurrentCamera;
```

## Naming Styles

모든것들의 이름들은 그것의 목적이나 용도, 타입 등을 이해할 수 있도록 작성합니다.
- 이름이 길어지더라도 통상적인것 외의 줄임말은 지양합니다.
- 철자에 오류가 없도록 `Jebrains Respeller`, `VSCode SpellChecker` 등의 익스텐션 사용을 적극 권장합니다.

```csharp
// Do
public float holdThreshold;

public bool enableDragToUnlock;

public float GetTimeScaleOfTag(string tag) { /* ... */ }

// Don't
public Button SubmitBtn;

public float destoryDelay = 1.0f;

public void ScrapCancle() { /* ... */ } // CancelScrap()

public void HiLightBtn() { /* ... */ } // HighlightButton()

protected virtual void OnAttackEventExcuteAttack() { /* ... */ } // OnAttackEventExecuted() 
```

`class`, `interface`, `delegate`, `fields`들은 모두 기본적으로 목적에 맞게 이름을 작성합니다.
- 이름이 명사로 이해가 될 수 있도록 작성합니다.
- 참조되는 객체 또는 할당된 값들이 어떤 역할을 하는지 나타낼 수 있도록 작성합니다.

```csharp
// Do
public Vector3 colliderSize;

private Vector3 m_ScreenSpaceColliderSize;

public interface IContextMenuDataProvider { /* ... */ }

public event OnTaskUpdateCallbackDelegate OnTaskUpdated;
public delegate void OnTaskUpdateCallbackDelegate();

// Acceptable
Vector3 moveDir = transform.forward;

public class AISensingComponent : MonoBehaviour { /* ... */ }

// Don't
public class EnemyBaseMovingComponent { /* ... */ } // EnemyMovementComponent
public EnemyBaseMovingComponent MovingComponent; // MovementComponent

float flTime = 0.0f; // m_ElapsedTime;

public class EnemyAirDirectMovingComponent { /* ... */ } // FloatingEnemyMovementComponent

private float m_LimitLowAltitude;
```

`method`와 `function`는 기본적으로 **동사**의 형태가 되도록 작성합니다.

```csharp
// Do
public void DestroyWithDelay(float delay) { /* ... */ }

public void CancelScrap() { /* ... */ }

public void RotateTowards(Transform targetTransform) { /* ... */ }

// Don't
public void ScrapCancle() { /* ... */ }

public override Quaternion MobRotation(Transform Mobtransform, Transform currentTarget, float deltaTime) { /* ... */ }

public override Vector3 MovingPosition(Transform Mobtransform, Transform currentTarget, float deltaTime) { /* ... */ }
```

## Coding Styles

객체들은 모듈화가 가능한 형태로 구현할 것을 권장합니다.
- 아래의 코드 샘플은 `Train` 클래스가 추후에 모듈화 될것을 기준으로 `partial` 키워드를 이용해 기능별로 스크립트를 분리한 형태입니다.

```csharp
// Train.cs
public partial class Train { /* ... */ }

// TrainCompartmentManager.cs
public partial class Train { /* ... */ }

// TrainMovement.cs
public partial class Train { /* ... */ }
```

## Comments

> 대부분의 스크립트 내에는 작성된 코드에 대한 부가설명들이 작성되어있습니다.
>
> 클래스 내에 주석으로 작성하기에는 공간이 부족하거나, 자세한 설명이 필요한 사항들은 스크립트 파일 상단에 주석이 작성되어있으니 스크립트 수정 전에 필수적으로 확인해보는 것이 좋습니다.