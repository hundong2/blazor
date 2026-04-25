using System;

namespace Study.ModernCSharp;

// 1. Record 적용: 데이터 전달을 위한 불변 객체
public record UserInfo(string Name, string Email, bool IsActive);

// 2. Primary Constructor 적용 (C# 12): 코드 상단에 주입받을 의존성 선언
// (여기서는 ILogger 대신 콘솔 출력을 사용한다고 가정)
public class UserService(string serviceName)
{
    // 3. Pattern Matching 적용: 간결한 상태 검사 로직
    public string CheckUserStatus(object data) => data switch
    {
        UserInfo { IsActive: true } user => $"[{serviceName}] {user.Name}님은 현재 활동 중입니다.",
        UserInfo { IsActive: false } user => $"[{serviceName}] {user.Name}님의 계정은 비활성화 상태입니다.",
        null => "입력된 데이터가 없습니다.",
        _ => "알 수 없는 데이터 형식입니다."
    };

    public void RunExample()
    {
        var activeUser = new UserInfo("Alice", "alice@example.com", true);
        var inactiveUser = activeUser with { Name = "Bob", IsActive = false }; // with 키워드로 일부 속성만 변경하여 새 객체 생성

        Console.WriteLine(CheckUserStatus(activeUser));
        Console.WriteLine(CheckUserStatus(inactiveUser));
        Console.WriteLine(CheckUserStatus(null!));
    }
}
