using System;

namespace Study.BlazorApp;

// 1. 상태 관리를 위한 Service 클래스
// 실무에서는 이 클래스를 Program.cs에서 DI 컨테이너에 AddScoped 또는 AddSingleton으로 등록해야 합니다.
public class CartStateService
{
    // 외부에서 상태를 직접 변경하지 못하도록 private set 사용
    public int ItemCount { get; private set; }

    // 상태가 변경되었음을 구독자(컴포넌트)들에게 알리기 위한 이벤트
    public event Action? OnStateChange;

    public void AddItem()
    {
        ItemCount++;
        NotifyStateChanged();
    }

    public void RemoveItem()
    {
        if (ItemCount > 0)
        {
            ItemCount--;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnStateChange?.Invoke();
}
