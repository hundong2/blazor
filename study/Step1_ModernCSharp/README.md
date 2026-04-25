# Step 1: Modern C# 실무 필수 문법

Blazor와 MAUI로 개발할 때 코드의 양을 줄이고, 버그를 줄이며, 가독성을 극대화하기 위해 최신 C# 문법(C# 9.0 ~ 12.0)을 적극 활용해야 합니다.

## 1. Records (불변 데이터 모델링)

`record`는 데이터를 담는 객체(DTO, ViewModel의 상태 등)를 정의할 때 매우 유용합니다. 불변(Immutable) 특성을 가지며, 값 기반의 동등성(Value Equality)을 제공합니다.

### ❌ 기존 방식 (C# 8.0 이전)
```csharp
public class UserDto
{
    public string Name { get; init; }
    public int Age { get; init; }

    // 값 비교를 위해 Equals, GetHashCode, == 연산자 오버로딩 등 많은 코드가 필요함
}
```

### ✅ Modern C# 방식
```csharp
public record UserDto(string Name, int Age);
```

### 💡 실무 적용 포인트 (Blazor/MAUI)
* API 응답을 받는 **DTO(Data Transfer Object)**를 정의할 때 무조건 `record`를 사용하세요.
* Blazor에서 컴포넌트의 상태(State)를 관리할 때, 상태 객체를 `record`로 만들고 `with` 키워드를 사용하여 새로운 상태를 생성하면, 예측 가능하고 디버깅하기 쉬운 단방향 데이터 흐름을 구성할 수 있습니다.
  ```csharp
  var user1 = new UserDto("Alice", 25);
  // user1의 데이터는 유지한 채 나이만 26으로 변경한 새로운 객체 생성
  var user2 = user1 with { Age = 26 };
  ```

---

## 2. Pattern Matching (고급 제어 흐름)

복잡한 `if-else` 체인이나 `switch` 문을 간결하고 선언적으로 작성할 수 있습니다. 특히 타입 검사와 값 검사를 동시에 할 때 강력합니다.

### 💡 실무 적용 포인트
MAUI나 Blazor에서 다양한 상태나 타입을 처리할 때 유용합니다.

```csharp
// API 응답 결과 처리 예시
public string GetResultMessage(object result) => result switch
{
    UserDto { Age: >= 18 } user => $"{user.Name}님은 성인입니다.",
    UserDto { Age: < 18 } user  => $"{user.Name}님은 미성년자입니다.",
    Exception ex => $"에러 발생: {ex.Message}",
    null => "결과가 없습니다.",
    _ => "알 수 없는 응답입니다."
};
```

---

## 3. Primary Constructors (기본 생성자 - C# 12)

의존성 주입(DI)을 위해 생성자를 만들고 필드에 할당하는 보일러플레이트 코드를 획기적으로 줄여줍니다.

### ❌ 기존 방식 (Blazor 컴포넌트 코드비하인드 또는 MAUI 뷰모델)
```csharp
public class UserViewModel
{
    private readonly IUserService _userService;
    private readonly ILogger<UserViewModel> _logger;

    public UserViewModel(IUserService userService, ILogger<UserViewModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }
}
```

### ✅ Modern C# 방식 (C# 12)
클래스 이름 뒤에 바로 매개변수를 선언합니다.

```csharp
public class UserViewModel(IUserService userService, ILogger<UserViewModel> logger)
{
    public async Task LoadUserAsync()
    {
        // 바로 매개변수를 필드처럼 사용 가능
        logger.LogInformation("유저 로딩 중...");
        var user = await userService.GetUserAsync();
    }
}
```

### 💡 실무 적용 포인트
* DI(Dependency Injection)가 필수적인 Blazor의 Service 클래스나 MAUI의 ViewModel을 작성할 때 반드시 도입하세요. 코드 가독성이 매우 높아집니다.

---

## 4. Nullable Reference Types (안전한 참조 타입)

`NullReferenceException`을 컴파일 타임에 방지하기 위한 기능입니다. 프로젝트 설정 파일(`.csproj`)에 `<Nullable>enable</Nullable>`이 켜져 있어야 합니다.

### 💡 실무 적용 포인트
* 모델의 속성이 null을 허용하는지(`string?`) 반드시 값이 있어야 하는지(`string`) 명확히 구분하세요.
* Blazor에서 컴포넌트 렌더링 전 데이터가 로드되지 않은 상태를 처리할 때 유용합니다.

```csharp
public class Product
{
    // C# 11+의 required 키워드와 함께 사용하여 객체 생성 시 반드시 초기화하도록 강제
    public required string Name { get; set; }

    // null일 수 있음을 명시
    public string? Description { get; set; }
}
```

---

## 🎯 실습 과제

1. `UserInfo`라는 이름의 `record`를 생성하세요 (속성: Name, Email, IsActive).
2. Primary Constructor를 사용하여 `ILogger`를 주입받는 `UserService` 클래스를 만드세요.
3. Pattern Matching을 사용하여 `UserInfo`의 `IsActive` 상태에 따라 다른 메시지를 반환하는 메서드를 작성해 보세요.
