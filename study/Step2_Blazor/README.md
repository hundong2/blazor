# Step 2: Blazor 실전 가이드

Blazor는 C#으로 인터랙티브한 웹 UI를 구축할 수 있게 해주는 프레임워크입니다. 실무에서 Blazor를 다룰 때 가장 중요한 것은 **컴포넌트 설계**, **상태 관리**, 그리고 **외부(JavaScript)와의 통신**입니다.

## 1. 컴포넌트 생명주기와 설계 (Component Lifecycle)

Blazor 컴포넌트는 생성부터 소멸까지 특정 단계를 거칩니다. 실무에서는 API를 호출하거나 초기 설정을 할 때 적절한 생명주기 메서드를 선택하는 것이 성능과 직결됩니다.

* `OnInitializedAsync()`: 컴포넌트가 처음 로드될 때 한 번 실행됩니다. 주로 **초기 데이터 로딩(API 호출)**에 사용됩니다.
* `OnParametersSetAsync()`: 부모 컴포넌트로부터 전달받은 파라미터(`[Parameter]`)가 변경될 때마다 실행됩니다.
* `OnAfterRenderAsync(bool firstRender)`: 컴포넌트 렌더링이 완료된 후 실행됩니다. `firstRender`가 `true`일 때 **JS Interop 호출**(ex: 차트 라이브러리 초기화, 포커스 설정)을 수행하는 것이 안전합니다.

### 💡 실무 팁: Smart vs Dumb 컴포넌트 분리
* **Smart (Container) Component:** API 통신, 상태 관리 등 비즈니스 로직을 담당하고 UI는 최소화합니다. (`Page` 컴포넌트들이 주로 이 역할)
* **Dumb (Presentational) Component:** 상태를 가지지 않고 부모로부터 데이터를 `[Parameter]`로 받아 UI만 렌더링합니다. 재사용성이 높습니다.

---

## 2. 상태 관리 (State Management)

앱이 커질수록 여러 컴포넌트 간에 상태(ex: 로그인된 사용자 정보, 장바구니 등)를 공유해야 합니다.

### 간단한 상태 공유: Cascading Parameters
부모에서 하위의 모든 자식 컴포넌트로 데이터를 내려줄 때 유용합니다. (ex: 테마 정보, 로그인 상태)

```razor
<!-- App.razor (최상단) -->
<CascadingValue Value="currentUser">
    <Router AppAssembly="@typeof(App).Assembly"> ... </Router>
</CascadingValue>
```

### 복잡한 상태 관리: State Container Service (DI 활용)
서비스 클래스를 만들고 Dependency Injection(DI)을 통해 여러 컴포넌트에서 주입받아 사용합니다. 상태가 변경되면 이벤트를 발생시켜 컴포넌트들이 리렌더링되도록 합니다.

```csharp
// 상태 관리 서비스 예시
public class CartState
{
    public int ItemCount { get; private set; }
    public event Action? OnChange;

    public void AddItem()
    {
        ItemCount++;
        OnChange?.Invoke(); // 상태 변경 알림
    }
}
```

---

## 3. JavaScript Interop (JS 통신)

Blazor가 C#으로 동작하지만, 기존의 강력한 JS 라이브러리(차트, 맵, 브라우저 API 등)를 사용해야 할 때가 많습니다.

* `IJSRuntime`: C#에서 JS 함수를 호출할 때 사용합니다.
* `[JSInvokable]`: JS에서 C# 메서드를 호출할 수 있게 해줍니다.

### 💡 실무 팁
`IJSRuntime`은 컴포넌트가 완전히 렌더링된 이후(`OnAfterRenderAsync`)에 호출해야 오류가 발생하지 않습니다. 특히 Blazor Server 모델에서는 사전 렌더링(Prerendering) 단계에서는 브라우저 환경이 없으므로 JS 호출 시 에러가 납니다.

```razor
@inject IJSRuntime JSRuntime

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // 첫 렌더링 시 브라우저의 alert 호출
            await JSRuntime.InvokeVoidAsync("alert", "Blazor에 오신 것을 환영합니다!");
        }
    }
}
```

---

## 4. Forms 및 Validation (입력 검증)

실무에서 데이터 입력 폼은 필수입니다. Blazor의 `EditForm` 컴포넌트와 C#의 DataAnnotations 속성을 결합하면 강력하고 일관된 폼 검증 로직을 작성할 수 있습니다.

```razor
<EditForm Model="userModel" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <label>이름:</label>
    <InputText @bind-Value="userModel.Name" />
    <ValidationMessage For="@(() => userModel.Name)" />

    <button type="submit">제출</button>
</EditForm>
```

---

## 🎯 실습 과제

1. `CartState` 서비스 클래스를 작성해 보세요. (`ItemCount` 프로퍼티와 값을 증가시키는 메서드 포함)
2. `Counter.razor` 같은 컴포넌트에서 `CartState`를 주입(`@inject`)받고, 버튼 클릭 시 카운트를 증가시켜 보세요.
3. C# 코드를 분리하는 실무 패턴인 **Code-Behind (컴포넌트명.razor.cs)** 방식을 적용하여 위의 컴포넌트를 리팩토링해 보세요.
