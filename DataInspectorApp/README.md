# Data Inspector App

Data Inspector는 다양한 데이터 항목을 필터링하고 그리드 형태로 조회하며, 각 항목의 상세 JSON 데이터를 트리 형태로 시각화하여 검토할 수 있는 Blazor 기반 멀티플랫폼 애플리케이션입니다. .NET MAUI를 통해 Windows 데스크톱 앱으로 실행 가능하며, Blazor WebAssembly를 통해 웹 브라우저에서도 동일한 기능을 제공합니다. UI는 MudBlazor 컴포넌트 라이브러리를 사용하여 구현되었습니다.

## 주요 기능

*   **데이터 필터링:** 태그(Multi-select), 상태(ComboBox), 이름/값 검색(TextBox)을 사용한 다중 조건 필터링.
*   **데이터 그리드 뷰:**
    *   항목 번호, 이름, 값 요약, 상태 정보를 표 형태로 표시.
    *   상태(Pass/Fail/None)에 따른 시각적 구분 (색상 칩).
    *   검색어 하이라이트.
    *   페이징 지원.
*   **상세 JSON 트리 뷰:**
    *   그리드에서 특정 항목의 상태를 클릭하면, 해당 항목의 상세 JSON 데이터를 트리 형태로 보여주는 별도 페이지로 이동.
    *   트리 노드 확장/축소 기능.
    *   데이터 타입에 따른 값 시각화.
*   **멀티플랫폼 지원:**
    *   Windows 데스크톱 (.NET MAUI)
    *   웹 브라우저 (Blazor WebAssembly)
*   **테마:** 라이트 모드 / 다크 모드 전환 가능.

## 프로젝트 구조

*   **`DataInspectorApp.sln`**: 솔루션 파일 (생성되었다고 가정).
*   **`DataInspector.SharedComponents/`**: 핵심 로직, UI 컴포넌트, 페이지, 데이터 모델을 포함하는 Razor 클래스 라이브러리 (RCL). MudBlazor가 주로 사용됩니다.
*   **`DataInspector.Maui/`**: .NET MAUI 애플리케이션 프로젝트. `DataInspector.SharedComponents`를 참조하여 Blazor Hybrid 방식으로 UI를 호스팅합니다. Windows 플랫폼을 주 타겟으로 합니다.
*   **`DataInspector.Web/`**: Blazor WebAssembly 애플리케이션 프로젝트. `DataInspector.SharedComponents`를 참조하여 웹 UI를 제공합니다.

## 시작하기

### 요구 사항

*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   MAUI 개발을 위한 워크로드 설치 (Visual Studio Installer 또는 `dotnet workload install maui` CLI 사용)
*   MudBlazor는 프로젝트에 NuGet 패키지로 포함되어 있습니다.

### 설정 및 실행

1.  **저장소 복제 (Clone Repository):**
    ```bash
    # git clone <repository-url>
    # cd DataInspectorApp
    ```

2.  **솔루션 파일 생성 (아직 없다면):**
    만약 `.sln` 파일이 없다면, `DataInspectorApp` 폴더에서 다음 명령을 실행하여 생성하고 프로젝트들을 추가합니다:
    ```bash
    dotnet new sln -n DataInspectorApp
    dotnet sln DataInspectorApp.sln add DataInspector.SharedComponents/DataInspector.SharedComponents.csproj
    dotnet sln DataInspectorApp.sln add DataInspector.Maui/DataInspector.Maui.csproj
    dotnet sln DataInspectorApp.sln add DataInspector.Web/DataInspector.Web.csproj
    ```

3.  **솔루션 빌드:**
    Visual Studio 또는 Rider와 같은 IDE에서 `DataInspectorApp.sln` 파일을 열고 빌드합니다.
    또는 CLI를 사용하여 빌드할 수 있습니다 (솔루션 루트 폴더에서):
    ```bash
    dotnet build
    ```

4.  **애플리케이션 실행:**

    *   **MAUI (Windows):**
        *   IDE에서 `DataInspector.Maui` 프로젝트를 시작 프로젝트로 설정하고 실행합니다.
        *   또는 CLI 사용 (솔루션 루트 또는 `DataInspector.Maui` 폴더에서):
          ```bash
          # Windows에서 실행 시 특정 프레임워크 타겟을 지정해야 할 수 있습니다.
          # 예: dotnet run --project DataInspector.Maui -f net8.0-windows10.0.19041.0
          # 정확한 프레임워크 모니커는 DataInspector.Maui.csproj 파일의 TargetFrameworks에서 확인하세요.
          dotnet run --project DataInspector.Maui
          ```
          (참고: MAUI CLI 실행은 대상 플랫폼에 따라 명령이 다를 수 있습니다. IDE 사용이 더 간편합니다.)

    *   **Blazor WebAssembly (웹):**
        *   IDE에서 `DataInspector.Web` 프로젝트를 시작 프로젝트로 설정하고 실행합니다. (보통 Kestrel 서버가 함께 실행됩니다.)
        *   또는 CLI 사용 (`DataInspector.Web` 폴더에서):
          ```bash
          dotnet run --project DataInspector.Web
          ```
          이후 브라우저에서 (일반적으로 `https://localhost:xxxx` 또는 `http://localhost:yyyy`) 주소로 접속합니다.

## 주요 컴포넌트 및 페이지 사용법

*   **메인 페이지 (`/`):**
    *   상단의 필터 패널(`FieldPanel`)을 사용하여 데이터를 필터링할 수 있습니다.
        *   **Tags:** 여러 태그를 선택하여 해당 태그를 포함하는 항목을 필터링합니다 (Mock 데이터에서는 `DetailJsonString` 내 "tags" 배열 또는 Name/Value 필드 검색).
        *   **Status:** Pass, Fail, None 상태 중 하나를 선택하여 필터링하거나 "Any Status"로 전체를 봅니다.
        *   **Search:** 입력된 텍스트를 기반으로 항목의 ID, 이름, 값 요약에서 검색합니다.
    *   필터링된 결과는 하단의 그리드(`ItemsGridView`)에 표시됩니다.
    *   그리드의 "Status" 컬럼에 있는 칩을 클릭하면 해당 항목의 상세 정보 페이지로 이동합니다.
    *   AppBar의 아이콘을 통해 라이트/다크 모드를 전환할 수 있습니다.

*   **상세 페이지 (`/detail/{ItemId}`):**
    *   선택된 항목의 기본 정보(ID, 이름, 상태, 값 요약)와 함께 `DetailJsonString`의 전체 내용이 JSON 트리 형태로 표시됩니다.
    *   트리 뷰에서 객체나 배열 노드는 확장/축소하여 내용을 탐색할 수 있습니다.
    *   상단의 툴바에 있는 뒤로 가기 아이콘을 사용하여 메인 페이지로 돌아갈 수 있습니다.

## 향후 개선 방향 (아키텍처)

현재 애플리케이션은 기능 구현에 중점을 두었지만, 향후 확장성과 재사용성을 더욱 높이기 위해 다음과 같은 아키텍처 개선을 고려할 수 있습니다:

1.  **코어 로직 및 인터페이스 분리 (`DataInspector.Core`):**
    *   데이터 모델, 데이터 제공/필터링 인터페이스, JSON 유틸리티 등을 UI와 무관한 별도 라이브러리로 분리합니다.
2.  **UI 컴포넌트 라이브러리 순수성 강화 (`DataInspector.SharedComponents`):**
    *   페이지 로직보다는 순수하고 재사용 가능한 UI 빌딩 블록에 집중합니다.
3.  **기능 모듈화:**
    *   특정 데이터 소스나 검사 유형에 따른 기능을 별도의 "플러그인" 형태 모듈로 개발하여 DI를 통해 주입하고 동적으로 구성할 수 있도록 합니다.
4.  **서비스 추상화:**
    *   데이터 접근, 상태 관리 등을 인터페이스 기반 서비스로 만들어 유연성을 높입니다.

이러한 개선은 애플리케이션이 더 복잡해지고 다양한 데이터 소스나 기능을 지원해야 할 때 유용할 것입니다.
