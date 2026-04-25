# Step 3: .NET MAUI 크로스 플랫폼 앱 개발

.NET MAUI(Multi-platform App UI)는 C#과 XAML을 사용하여 안드로이드, iOS, macOS, Windows 앱을 하나의 코드베이스로 개발할 수 있는 프레임워크입니다.

## 1. XAML과 데이터 바인딩 (Data Binding)

MAUI에서 UI는 주로 XAML로 작성되며, C# 코드(ViewModel)와 데이터를 연결하기 위해 데이터 바인딩을 사용합니다.

```xml
<!-- XAML 예시 -->
<VerticalStackLayout>
    <Label Text="{Binding GreetingMessage}" FontSize="24" HorizontalOptions="Center" />
    <Button Text="인사하기" Command="{Binding SayHelloCommand}" />
</VerticalStackLayout>
```

### 💡 실무 팁: Compiled Bindings
바인딩 오류를 컴파일 타임에 잡고 성능을 향상시키기 위해 `x:DataType`을 반드시 지정하세요.

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:viewmodel="clr-namespace:MyApp.ViewModels"
             x:Class="MyApp.Views.MainPage"
             x:DataType="viewmodel:MainViewModel">
    <!-- 이제 Binding 속성 작성 시 자동 완성이 지원됩니다 -->
</ContentPage>
```

---

## 2. CommunityToolkit.Mvvm을 활용한 MVVM 패턴

과거에는 MVVM 패턴을 구현하기 위해 보일러플레이트 코드를 많이 작성해야 했지만, 이제는 `CommunityToolkit.Mvvm` (Source Generators 기능)을 사용하여 매우 간결하게 작성할 수 있습니다.

### ❌ 기존 방식 (INotifyPropertyChanged 수동 구현)
```csharp
private string _name;
public string Name
{
    get => _name;
    set
    {
        if (_name != value)
        {
            _name = value;
            OnPropertyChanged(nameof(Name)); // 속성 변경 알림
        }
    }
}
```

### ✅ Modern MAUI 방식 (Source Generator)
`ObservableObject`를 상속받고 `[ObservableProperty]` 속성을 사용하면, 컴파일러가 위 코드를 자동으로 생성해 줍니다. 버튼 클릭 이벤트는 `[RelayCommand]`로 간결하게 만듭니다.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = "User"; // ⚠️ 소문자로 필드 선언 (생성되는 프로퍼티는 대문자 Name)

    [RelayCommand]
    private void SayHello()
    {
        Name = $"안녕하세요, {Name}님!"; // 자동 생성된 프로퍼티 접근
    }
}
```

---

## 3. App Shell을 이용한 네비게이션

MAUI에서는 `AppShell`을 사용하여 탭바, 플라이아웃 메뉴, 그리고 페이지 간 이동(Routing)을 매우 쉽게 관리할 수 있습니다.

```xml
<!-- AppShell.xaml -->
<Shell ...>
    <TabBar>
        <Tab Title="홈" Icon="home.png">
            <ShellContent ContentTemplate="{DataTemplate views:HomePage}" />
        </Tab>
        <Tab Title="설정" Icon="settings.png">
            <ShellContent ContentTemplate="{DataTemplate views:SettingsPage}" />
        </Tab>
    </TabBar>
</Shell>
```

**C# 코드에서 이동하기:**
```csharp
// "Details"라는 라우트로 이동하며, 쿼리 파라미터 전달
await Shell.Current.GoToAsync($"Details?id={userId}");
```

---

## 4. 플랫폼 고유 API (Platform Specifics)

모든 코드를 공유하더라도, 특정 플랫폼(예: Android의 센서 권한, iOS의 특정 UI)에만 접근해야 할 때가 있습니다.

* **조건부 컴파일(Conditional Compilation):** `#if ANDROID`, `#if IOS` 등을 사용합니다.
* **부분 클래스(Partial Classes):** `Platforms` 폴더 아래에 각 OS별 구현체를 분리합니다.

```csharp
public partial class DeviceInfoService
{
    public partial string GetDeviceModel();
}

// Platforms/Android/DeviceInfoService.cs 파일에서
public partial class DeviceInfoService
{
    public partial string GetDeviceModel() => Android.OS.Build.Model;
}
```

---

## 🎯 실습 과제

1. `CommunityToolkit.Mvvm`을 사용하여 텍스트 박스에 입력된 텍스트를 대문자로 변환해주는 `StringConverterViewModel`을 만들어 보세요.
2. XAML에서 `x:DataType`을 적용하고 텍스트 박스와 라벨, 버튼을 ViewModel과 바인딩해 보세요.
