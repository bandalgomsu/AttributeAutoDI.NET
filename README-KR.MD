# AttributeAutoDI

> 🌟 Attribute 기반 의존성 자동 등록 라이브러리

---
## Nuget
> 

---
## 💡 구현 동기 

ASP.NET Core의 기본 DI 컨테이너는 매우 간단하고 강력하지만,  
서비스 등록을 위해 매번 `builder.Services.AddXXX<>()`를 반복하는 것은 다음과 같은 불편을 초래했습니다:

- **서비스 구현체가 많아질수록 등록 코드가 길어지고 관리가 어려움**
- **다중 구현체(예: 인터페이스 A를 구현한 클래스가 여러 개) 상황에서 명확한 주입 지정이 번거로움**
- **클래스 선언과 등록 코드가 서로 분리되어 있어, 의도가 코드에 드러나지 않음**

> 이러한 문제점에 대한 해결책으로 Attribute를 떠올렸습니다.

그래서 `AttributeAutoDI`는 다음 철학을 바탕으로 만들어졌습니다:

- ✅ **서비스는 자기 자신이 어떻게 사용될지 선언할 수 있어야 한다**
- ✅ **구현체가 여러 개여도 Primary/Naming으로 명확하게 구분될 수 있어야 한다**
- ✅ **불필요한 등록 코드 없이도 명확한 DI 구성을 제공해야 한다**

> 이 라이브러리는 생산성과 명시성을 모두 추구합니다. 
> 안전하면서도 덜 귀찮은 개발환경을 구축하고.
> "내가 등록했나?" 보다 "등록이 잘 되어 있겠지"라고 믿을 수 있게.

> 최종적으로는 Program.cs , Startup.cs 의 책임을 줄여서 최대한 간소화 하는것이 목표입니다.
---

## 🧾 요구사항

- .NET 6.0 이상
- Microsoft.Extensions.DependencyInjection 사용 기반 프레임워크 (ASP.NET Core, Console 등)

---

## 🔥 주요 기능

- ✅ `[Singleton]`, `[Scoped]`, `[Transient]` 클래스 의존성 자동 등록
- ⭐ `[Primary]` 지정 시 다중 구현체 중 기본값 자동 지정
- 🏷️ `[Named]` 지정 후 파라미터에 `[Named("...")]`로 명시적 주입
- 🛠️ `[Options()]` 지정 시 IOptions 자동 바인딩 ! 
- 👾 `[PreConfiguration]` , `[PostConfiguration]` 클래스 하위의 , `[Execute]` 메서드를 통해서 설정 메서드 실행 
---

## 🚀 사용 방법

### 1. Extension 등록
```jsx
builder.Services.AddAttributeDependencyInjection(builder.Configuration);
```
or
```jsx
builder.Services.AddAttributeDependencyInjection(builder.Configuration,typeof(Program).Assembly);
```

AddAttributeDependencyInjection() Extension은 내부적으로 다음과 같이 구현되어 있습니다
```jsx
public static class AttributeInjectionExtension
{
    public static void AddAttributeDependencyInjection(
    this IServiceCollection services,
    IConfiguration configuration,
    Assembly? assembly = null
)
{
    assembly ??= Assembly.GetEntryAssembly();

    services.UsePreConfiguration(assembly!);

    services.UseOptionsBindingInjection(configuration, assembly!);
    services.UseAttributeInjection(assembly!);
    services.UsePrimaryInjection();
    services.UseNameParameterInjection(assembly!);
    services.Replace(ServiceDescriptor.Transient<IControllerActivator, NamedControllerActivator>());

    services.UsePostConfiguration(assembly!);
}
```

### 실행 흐름 
1. PreConfiguration 하위 Execute 메서드 실행 
2. Options 등록 
3. [Singleton] , [Transient] , [Scoped] 클래스 의존성 등록 
4. [Primary] 의존성 등록 
5. [Named]가 붙은 파라미터 클래스 및 Controller Activator 등록 
6. PostConfiguration 하위 Execute 메서드 실행 


### 주의 
```jsx
builder.Service.AddSingleton...
builder.Service.AddAttributeDependencyInjection 
```
- 위의 코드는 의도한대로 작동합니다 하지만
```jsx
builder.Service.AddAttributeDependencyInjection
builder.Service.AddSingleton...
```
이 코드는 의도한대로 동작하지 않을 가능성이 있습니다.
따라서 AttributeAutoDI가 제공하는 등록방법을 사용하거나, 순서를 유의해주세요.

### 2. 클래스 의존성 자동 주입 - [Singleton], [Transient], [Scoped]
```jsx
[Singleton]
public class MyService : IMyService
{
    public string Get() => "hello";
}
```
이것은 다음과 같습니다
```jsx
builder.Services.AddSingleton<IMyService, MyService>();
```

### 3. 다중 구현체 중 기본값 지정 - [Primary]
```jsx

[Singleton]
public class MyService : IMyService
{
    public string Get() => "hello";
}

[Primary]
[Singleton]
public class MyPrimaryService : IMyService
{
    public string Get() => "I am primary";
}
```
이것은 IMyService가 주입될 때 MyPrimaryService가 주입되는것을 보장합니다.
```jsx
[Singleton]
public class TestFacade(IMyservice myservice)
{
    ...
}
// myservice == MyPrimaryService
```

### 4. 명시적 주입 기능 - [Named]
[Named]를 통해서 주입 구현체중 이름과 매핑되는 구현체를 지정할 수 있습니다
```jsx
[Singleton]
public class MyService : IMyService
{
    public string Get() => "hello";
}
```
--> 기본적으로 LowerCamelCase로 이름이 등록됩니다. (MyService -> myService)
```jsx
[Singleton("myNamedService")]
public class MyService : IMyService
{
    public string Get() => "hello";
}
```
--> name 프로퍼티를 통해서 이름을 지정할 수 있습니다
```jsx
[Singleton]
public class TestFacade([Named("myNamedService")]IMyservice myservice)
{
...
}
// myservice == MyService
```
--> 파라미터에 [Named]를 통해서 이름과 매핑되는 구현체를 주입받을 수 있습니다 (NamedParameter 실행 시점에 등록된 클래스만 작동합니다)

### 5. 옵션 바인딩 기능 - [Options]
```jsx
[Options("Sample:Sample")]
public class SampleOption
{
    public string Sample { get; set; } = "";
}
```
이 코드는 다음과 일치합니다.
```jsx
builder.Services.Configure<SampleOption>(configuration.GetSection("Sample:Sample"));
```
따라서 아래와 같이 주입받아서 사용할 수 있습니다.
```jsx
public class SampleController(
    IOptions<SampleOption> sampleOption) ...
```

### 6. Configuration 기능 - [PreConfiguration] , [PostConfiguration] , [Execute]
AddAttributeDependencyInjection 전 후로 [Execute]가 붙은 메서드를 실행할 수 있는 기능입니다.

PreConfiguration -> AddAttributeDependencyInjection -> PostConfiguration 순으로 실행

```jsx
[PreConfiguration] //or [PostConfiguration]
public static class Configuration{
    [Execute]
    public static void TestConfig1(IServiceCollection service){
        ...
    }

    [Execute]
    public static void TestConfig2(this IServiceCollection service){
    ...
    }
    
    [Execute]
    public static void TestConfig3(IServiceCollection service, IConfiguration configuration){
        ...
    }

    [Execute]
    public static void TestConfig4(this IServiceCollection service, IConfiguration configuration){
    ...
    }
}
```

위에 4개의 메서드 시그니처만 작동합니다 (리턴 타입은 상관 X)

---

## 🧪 예제
예제는 AttributeAutoDi.Sample 프로젝트를 통해서 확인해볼 수 있습니다.

---

## 🙌 기여하기
취약점이나 개선점을 발견하면 이슈 등록 또는 PR 부탁드립니다.

---

## 📄 라이선스
MIT License
Copyright (c) 2025


