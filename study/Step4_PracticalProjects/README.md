# Step 4: 실전 프로젝트 & Blazor Hybrid

이 단계에서는 이전 단계에서 배운 Modern C#, Blazor, MAUI 기술을 결합하여 실무에 즉시 투입 가능한 하이브리드 아키텍처와 클린 아키텍처 개념을 학습합니다.

## 1. Blazor Hybrid (MAUI + Blazor)

**Blazor Hybrid**는 MAUI의 네이티브 껍데기(WebView) 안에 Blazor(웹 기술)를 호스팅하는 방식입니다.

### 왜 실무에서 Blazor Hybrid를 사용할까?
* **코드 재사용성 극대화:** 이미 만들어둔 Blazor 웹사이트의 UI 컴포넌트(Razor 파일)를 모바일 및 데스크톱 앱에서 100% 그대로 재사용할 수 있습니다.
* **네이티브 기능 접근:** 웹뷰 안에서 돌지만, WebAssembly와 달리 로컬 장치에서 C# 코드가 실행되므로 MAUI가 제공하는 네이티브 API(카메라, GPS, 파일 시스템 등)에 즉시 접근할 수 있습니다.
* **학습 곡선 단축:** 프론트엔드 개발자(웹)가 XAML을 깊게 배우지 않아도 모바일 앱 화면을 구성할 수 있습니다.

### 핵심 구성요소: `BlazorWebView`
MAUI의 XAML 화면에 웹뷰를 배치하고, 어느 Blazor 컴포넌트를 루트로 띄울지 지정합니다.

```xml
<!-- MAUI의 MainPage.xaml -->
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:MyBlazorHybridApp"
             x:Class="MyBlazorHybridApp.MainPage">

    <!-- BlazorWebView를 통해 웹 컴포넌트를 호스팅 -->
    <BlazorWebView HostPage="wwwroot/index.html">
        <BlazorWebView.RootComponents>
            <RootComponent Selector="#app" ComponentType="{x:Type local:Components.Routes}" />
        </BlazorWebView.RootComponents>
    </BlazorWebView>
</ContentPage>
```

---

## 2. 코드 공유 전략 (Shared Library)

웹앱(Blazor Server/WebAssembly)과 모바일앱(MAUI)을 동시에 서비스해야 한다면 프로젝트 구조를 잘 잡아야 합니다.

### 권장 프로젝트 구조 (클린 아키텍처 기반)

1. **`MyApp.Shared` (Class Library):**
   * 가장 핵심이 되는 도메인 로직, DTO(`record`), Interface를 모아둡니다.
   * **의존성:** 없음 (.NET Core 라이브러리만)

2. **`MyApp.UIComponents` (Razor Class Library - RCL):**
   * 웹과 모바일에서 공통으로 사용할 화면(Razor Components), CSS, JS 파일들을 모아둡니다.
   * **의존성:** `MyApp.Shared`

3. **`MyApp.Web` (Blazor Web App):**
   * 실제 웹 브라우저로 서비스될 호스트 프로젝트입니다.
   * **의존성:** `MyApp.Shared`, `MyApp.UIComponents`

4. **`MyApp.Mobile` (MAUI Blazor App):**
   * 앱스토어에 배포될 모바일 호스트 프로젝트입니다.
   * **의존성:** `MyApp.Shared`, `MyApp.UIComponents`

---

## 3. 실무 아키텍처 적용: 제어 역전 (IoC)과 DI

공유 UI 라이브러리(`MyApp.UIComponents`)에서 기기 고유 기능(예: 알림, 로컬 저장소)을 써야 할 때는 어떻게 할까요?

**Interface 기반 DI (Dependency Injection)를 활용합니다.**

1. `MyApp.Shared`에 인터페이스 정의:
   ```csharp
   public interface IAppNotificationService
   {
       Task ShowAlertAsync(string title, string message);
   }
   ```
2. 웹과 모바일 프로젝트에서 각각 구현:
   ```csharp
   // Web 호스트의 구현체 (JS alert 호출 등)
   public class WebNotificationService : IAppNotificationService { ... }

   // MAUI 호스트의 구현체 (MAUI 네이티브 알림 호출)
   public class NativeNotificationService : IAppNotificationService
   {
       public Task ShowAlertAsync(string title, string message) =>
           App.Current.MainPage.DisplayAlert(title, message, "OK");
   }
   ```
3. 각 호스트의 `Program.cs` / `MauiProgram.cs`에서 DI 등록:
   ```csharp
   // MAUI
   builder.Services.AddSingleton<IAppNotificationService, NativeNotificationService>();
   ```
4. `MyApp.UIComponents`의 Razor 화면에서는 인터페이스만 주입받아 사용:
   ```razor
   @inject IAppNotificationService NotificationService
   <button @onclick="() => NotificationService.ShowAlertAsync('알림', '성공!')">클릭</button>
   ```

---

## 🎯 최종 과제 제안 (토이 프로젝트)

**"크로스 플랫폼 To-Do 리스트 앱 구축하기"**

1. `Shared` 라이브러리에 `TodoItem` Record와 `ITodoService` 인터페이스 생성.
2. `UIComponents` 라이브러리에 목록 추가/수정/삭제 기능을 가진 Razor 컴포넌트 개발 (Step 1, 2 기술 활용).
3. `Web` 프로젝트에서는 Entity Framework 등을 연결해 DB에 저장하는 `WebTodoService` 구현.
4. `Mobile` 프로젝트에서는 SQLite를 활용해 기기 내부에 저장하는 `NativeTodoService` 구현 및 연동.
