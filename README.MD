#[README-KR](https://github.com/bandalgomsu/AttributeAutoDI.NET/blob/main/README-KR.MD)

# AttributeAutoDI

> 🌟 Attribute-based automatic dependency injection library for .NET

---
## Nuget
>

---
## 💡 Motivation
The default DI container in ASP.NET Core is simple and powerful,
but having to call builder.Services.AddXXX<>() repeatedly introduces a few pain points:

❌ The more services you implement, the longer and harder your Program.cs becomes to manage

❌ Handling multiple implementations of the same interface requires verbose, error-prone registration

❌ Service intent is not self-explanatory – registration and class definition are separated

👉 What if... classes could declare how they want to be injected?
AttributeAutoDI was born from this idea.

It is built on the following principles:
✅ Services should declare their own lifecycle and purpose

✅ Multiple implementations should be clearly handled via [Primary] or [Named]

✅ We should reduce boilerplate code in Program.cs without sacrificing readability

> Our goal is to simplify Program.cs and Startup.cs as much as possible
so you can trust the system to "just work."

---

## 🧾  Requirements

- .NET 6.0 or later
- Compatible with Microsoft.Extensions.DependencyInjection (ASP.NET Core, Console apps, etc.)

---

## 🔥 Features

✅ Automatic registration via [Singleton], [Scoped], and [Transient]

⭐ [Primary] support for selecting default implementation

🏷️ [Named] support for explicit injection via constructor parameters

🛠️ [Options("Section")] for automatic configuration binding

👾 Executes configuration methods marked with [Execute] inside classes decorated with [PreConfiguration] or [PostConfiguration].

---

## 🚀 Usage

### 1. Register the extension
```jsx
builder.Services.AddAttributeDependencyInjection(builder.Configuration);
```
or
```jsx
builder.Services.AddAttributeDependencyInjection(builder.Configuration,typeof(Program).Assembly);
```
Internally, it expands to:

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

### Execution Flow
1. Execute methods under [PreConfiguration] classes

2. Register [Options] bindings

3. Register class dependencies marked with [Singleton], [Transient], or [Scoped]

4. Register [Primary] dependencies

5. Register constructor parameter classes and controller activators with [Named]

6. Execute methods under [PostConfiguration] classes

### ⚠️ Important: Register in correct order!
✔️ This works as expected:
```jsx
builder.Service.AddSingleton...
builder.Service.AddAttributeDependencyInjection 
```
❌ This may not work correctly:
```jsx
builder.Service.AddAttributeDependencyInjection
builder.Service.AddSingleton...
```
If you use both manual and attribute-based registration, always call AddAttributeDependencyInjection() after manual registrations.

### 2. Attribute-based Auto registration - [Singleton], [Transient], [Scoped]

```jsx
[Singleton]
public class MyService : IMyService
{
    public string Get() => "hello";
}
```
This is equivalent to:
```jsx
builder.Services.AddSingleton<IMyService, MyService>();
```

### 3. Primary service selection - [Primary]
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
When injecting IMyService, the system will prefer MyPrimaryService.

### 4. Named injection - [Named]

```jsx
[Singleton]
public class MyService : IMyService
{
    public string Get() => "hello";
}
```
This will register myService (default camel-case name).
Or set custom name:

```jsx
[Singleton("myNamedService")]
public class MyService : IMyService
{
    public string Get() => "hello";
}
```
Then inject with:

```jsx
[Singleton]
public class TestFacade([Named("myNamedService")]IMyservice myservice)
{
...
}
// myservice == MyService
```
Named injection works in registered Class

### 5. Options binding via attribute - [Options]
```jsx
[Options("Sample:Sample")]
public class SampleOption
{
    public string Sample { get; set; } = "";
}
```
This is equivalent to:

```jsx
builder.Services.Configure<SampleOption>(configuration.GetSection("Sample:Sample"));
```
Then you can inject it with:

```jsx
public class SampleController(
    IOptions<SampleOption> sampleOption) ...
```

### 6. Configuration Execution - [PreConfiguration], [PostConfiguration], [Execute]
You can run setup methods before or after AddAttributeDependencyInjection using [Execute] methods.

The execution order is:

PreConfiguration → AddAttributeDependencyInjection → PostConfiguration


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
✅ Only these four method signatures are supported.

ℹ️ Return types are ignored — any return type is allowed but not used.

---

## 🧪 Examples
Check out the AttributeAutoDI.Sample project for practical examples.

---

## 🙌 Contributing
Found a bug? Have an improvement idea?
Feel free to open an issue or submit a pull request.

---

## 📄 License
MIT License
Copyright (c) 2025


