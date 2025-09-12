# blazor
Blazor web application

![Alt](https://repobeats.axiom.co/api/embed/3fb2a30ba5d2cf6e59b80eb973864b50f2c35ada.svg "Repobeats analytics image")

## Getting start 

- make project 
    - `dotnet new blazorserver -o <name> --no-https`
- publish project 
    - `dotnet publish --configuration Release`
- run program
    - `dotnet bin/Release/net7.0/publish/BlazorAppLee.dll`
  
## docker 

- build blazorapp 

```
docker build -t blazorapp .
```

- docker run 

```
docker run -it -p 5001:5001 blazorapp
```

```
docker run -d -p 5001:5001 blazorapp
```

## docker compose

- install docker-compose

```sh
sudo apt install docker-compose -y 
```

- build 

```sh
docker-compose build
```

- run

```sh
docker-compose up
```

- build & run 

```sh
docker-compose up --build
```

## setting 

>## Tutorial/Reference
>>### Blazor tutorial
>>> https://dotnet.microsoft.com/ko-kr/learn/aspnet/blazor-tutorial/run
>>### using Nginx with ubuntu 
>>>https://learn.microsoft.com/ko-kr/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-ubuntu
>>### blazor with NGINX Server
https://2kiju.tistory.com/59
>>### reference 
https://learn.microsoft.com/ko-kr/training/paths/aspnet-core-minimal-api/
https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0
https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0
https://www.youtube.com/watch?v=bXK-F-uL7Qo
### docker reference ( compose up )
https://docs.docker.com/reference/cli/docker/compose/up/

## Tutorial/Reference
### Blazor tutorial
https://dotnet.microsoft.com/ko-kr/learn/aspnet/blazor-tutorial/run

### using Nginx with ubuntu
https://learn.microsoft.com/ko-kr/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-ubuntu

### blazor with NGINX Server
https://2kiju.tistory.com/59

### reference
https://learn.microsoft.com/ko-kr/training/paths/aspnet-core-minimal-api/ https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0 https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0 https://www.youtube.com/watch?v=bXK-F-uL7Qo

## Daily note

> 2024-11-11 https://learn.microsoft.com/ko-kr/aspnet/core/razor-pages/?view=aspnetcore-8.0&tabs=visual-studio
>> https://learn.microsoft.com/ko-kr/aspnet/core/razor-pages/?view=aspnetcore-8.0&tabs=visual-studio


## Blazor Study 

https://blazor-university.com/overview/what-is-blazor/
https://github.com/IEvangelist/learning-blazor
https://forum.dotnetdev.kr/t/blazor/5253/13
https://learn.microsoft.com/ko-kr/training/modules/build-blazor-todo-list/2-data-binding

## ETC Freecodecamp

https://www.freecodecamp.org/learn/project-euler/project-euler-problems-1-to-100/problem-1-multiples-of-3-or-5

# WPF MVVM JSON Editor (Material Design + AvalonEdit + Real-time Highlight)

이 프로젝트는 .NET 8 WPF에서 MVVM 패턴과 Material Design in XAML Toolkit, AvalonEdit을 활용해 JSON 편집기(메뉴, 파일 열기/저장/다른이름으로 저장, Ctrl+S 자동 정렬, **실시간 텍스트 강조 표시**)를 구현한 완전한 솔루션입니다.

## 구성 요소
- **패턴**: MVVM (Model, ViewModel, View)
- **UI 프레임워크**: WPF (.NET 8)
- **스타일**: Material Design in XAML Toolkit
- **에디터**: AvalonEdit (문법 강조, 줄 번호, 텍스트 마커)
- **기능**: 파일 메뉴(새로 만들기, 열기, 저장, 다른 이름으로 저장), JSON 자동 정렬(Format), Ctrl+S 시 정렬+저장, **실시간 키워드 강조**

## 폴더 구조
```
wpf/
├── Models/
│   └── DocumentModel.cs
├── ViewModels/
│   └── MainViewModel.cs
├── Services/
│   ├── TextHighlightService.cs (✅ 실시간 업데이트)
│   └── TextMarkerService.cs (✅ 렌더링 최적화)
├── Infrastructure/
│   ├── ObservableObject.cs
│   └── RelayCommand.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── App.xaml
├── App.xaml.cs
└── wpf.csproj
```

## 시작하기 (개발 환경)
1) .NET SDK 8 설치
2) IDE: Visual Studio 2022 이상 또는 VS Code + C# Dev Kit
3) NuGet 복원: 처음 빌드 시 자동

## NuGet 패키지
- **MaterialDesignThemes** (5.x) - Material Design UI
- **MaterialDesignColors** (5.x) - Material Design 색상
- **AvalonEdit** (6.x) - 고급 텍스트 에디터

## 절차 지향 개발 가이드 (체크리스트)

### A. 템플릿 초기화 ✅ 완료
- [x] wpf/wpf.csproj에 MaterialDesignThemes, MaterialDesignColors, AvalonEdit 패키지 참조 추가
- [x] App.xaml에 Material Design ResourceDictionary 병합 구성
- [x] MVVM 기본 클래스(ObservableObject, RelayCommand) 추가
- [x] Model(DocumentModel) 추가
- [x] ViewModel(MainViewModel) 추가 및 명령 구현(New, Open, Save, Save As, FormatJson)
- [x] MainWindow.xaml UI: AvalonEdit 에디터, 메뉴(파일/편집/설정), 하이라이트 컨트롤
- [x] 키보드 단축키: Ctrl+S(정렬+저장), Ctrl+O(열기), Ctrl+N(새로 만들기), Ctrl+Shift+F(포맷)

### B. 에디터 강화 ✅ 완료
- [x] AvalonEdit 통합(문법 강조/줄번호/괄호 매칭)
- [x] TextMarkerService 구현(텍스트 강조 표시)
- [x] TextHighlightService 구현(검색어 하이라이트)
- [x] 실시간 키워드 강조 연결(ViewModel.HighlightTerm ↔ UI)
- [x] JSON 문법 하이라이트 적용
- [x] **실시간 렌더링 업데이트 최적화**

### C. 파일 기능 확장 ✅ 완료
- [x] 파일 열기(Open) 기능 추가
- [x] 파일 저장/다른 이름으로 저장 구현
- [x] 문서 제목 변경 추적 및 창 Title 바인딩

### D. 실시간 하이라이트 강화 ✅ 완료
- [x] **즉시 렌더링 업데이트**: TextView.Redraw() 호출로 실시간 시각 업데이트
- [x] **디바운싱**: 300ms 지연으로 입력 성능 최적화
- [x] **텍스트 변경 시 재하이라이트**: 에디터 내용 변경 시 자동 하이라이트 유지
- [x] **PropertyChanged 즉시 반영**: ViewModel 변경 시 즉시 UI 업데이트
- [x] **메모리 관리**: 윈도우 종료 시 리소스 정리

### E. 정규표현식 기반 Real-time Filtering 🚀 계획 단계
> **목표**: 정규표현식을 입력하면 매칭되는 텍스트만 실시간으로 하이라이트 표시하는 고급 필터링 기능

#### E-1. UI 컴포넌트 확장 📋 계획
- [ ] **Regex 입력 모드 토글**: CheckBox 또는 ToggleButton으로 정규표현식 모드 활성화
- [ ] **Regex 패턴 입력 상자**: 기존 HighlightBox를 확장하여 정규표현식 지원
- [ ] **Regex 유효성 표시**: 패턴 오류 시 빨간색 테두리/아이콘으로 시각적 피드백
- [ ] **매칭 개수 표시**: "Found: 5 matches" 형태로 매칭 결과 개수 실시간 표시
- [ ] **Regex 옵션 패널**: IgnoreCase, Multiline, Singleline 등 정규표현식 옵션 토글

#### E-2. ViewModel 확장 📋 계획
```csharp
// 추가할 속성들
public bool IsRegexMode { get; set; }           // 정규표현식 모드 활성화
public string RegexPattern { get; set; }        // 정규표현식 패턴
public RegexOptions RegexOptions { get; set; }  // 정규표현식 옵션
public int MatchCount { get; set; }              // 매칭된 항목 개수
public string RegexErrorMessage { get; set; }   // 정규표현식 오류 메시지
public bool IsValidRegex { get; set; }          // 정규표현식 유효성

// 추가할 명령들
public ICommand ToggleRegexModeCommand { get; }     // 정규표현식 모드 토글
public ICommand ClearRegexCommand { get; }          // 정규표현식 초기화
public ICommand ShowRegexHelpCommand { get; }       // 정규표현식 도움말
```

#### E-3. TextHighlightService 고도화 📋 계획
```csharp
// 확장할 메서드들
public void HighlightWithRegex(string pattern, RegexOptions options, TextDocument document)
public void ValidateRegexPattern(string pattern, out bool isValid, out string errorMessage)
public int GetMatchCount() // 현재 매칭된 항목 개수 반환
public void SetHighlightColors(Color primary, Color secondary) // 다중 하이라이트 색상

// 추가할 기능들
- 정규표현식 컴파일 캐싱 (성능 최적화)
- 매칭 그룹별 다른 색상 하이라이트
- 매칭 결과 네비게이션 (Next/Previous)
- 복합 패턴 지원 (여러 정규표현식 동시 적용)
```

#### E-4. UI 레이아웃 개선 📋 계획
```xaml
<!-- 추가할 UI 컴포넌트들 -->
<ToggleButton x:Name="RegexModeToggle" Content=".*" ToolTip="정규표현식 모드" />
<TextBox x:Name="RegexPatternBox" materialDesign:HintAssist.Hint="정규표현식 패턴 입력" />
<TextBlock x:Name="MatchCountDisplay" Text="{Binding MatchCount, StringFormat='매칭: {0}개'}" />
<Button x:Name="RegexHelpButton" Content="?" ToolTip="정규표현식 도움말" />

<!-- 정규표현식 옵션 패널 -->
<Expander Header="정규표현식 옵션">
    <StackPanel>
        <CheckBox Content="대소문자 무시 (IgnoreCase)" IsChecked="{Binding IgnoreCase}" />
        <CheckBox Content="여러 줄 모드 (Multiline)" IsChecked="{Binding Multiline}" />
        <CheckBox Content="한 줄 모드 (Singleline)" IsChecked="{Binding Singleline}" />
    </StackPanel>
</Expander>
```

#### E-5. 정규표현식 도움말 시스템 📋 계획
- [ ] **팝업 도움말 창**: 자주 사용되는 정규표현식 패턴 가이드
- [ ] **실시간 패턴 검증**: 입력과 동시에 패턴 유효성 검사
- [ ] **예제 패턴 제공**: 이메일, URL, 전화번호 등 공통 패턴 템플릿
- [ ] **매칭 미리보기**: 패턴 입력 시 매칭될 텍스트 미리보기

#### E-6. 성능 최적화 📋 계획
- [ ] **정규표현식 컴파일 캐싱**: 동일 패턴 재사용 시 성능 향상
- [ ] **점진적 매칭**: 큰 문서에서 청크 단위로 매칭 처리
- [ ] **백그라운드 처리**: 복잡한 정규표현식의 경우 비동기 처리
- [ ] **매칭 결과 캐싱**: 텍스트 변경이 없으면 이전 결과 재사용

#### E-7. 에러 처리 및 사용자 경험 📋 계획
- [ ] **정규표현식 오류 처리**: 잘못된 패턴 입력 시 친화적 오류 메시지
- [ ] **성능 경고**: 과도하게 복잡한 패턴 시 성능 경고 표시
- [ ] **패턴 히스토리**: 최근 사용한 정규표현식 패턴 저장/불러오기
- [ ] **단축키 지원**: Regex 모드 토글을 위한 키보드 단축키 (예: Ctrl+R)

### F. UI/스타일 강화 ✅ 부분 완료
- [x] AvalonEdit + Material Design 레이아웃 통합
- [x] ColorZone을 활용한 메뉴 영역 및 컨트롤 영역 구성
- [x] **실시간 하이라이트 상태 표시**: 현재 검색어 실시간 표시
- [ ] **정규표현식 UI 패널**: Regex 모드, 옵션, 도움말 통합 UI
- [ ] Theme 변경(Primary/Accent) 옵션 추가 (Settings 메뉴에 바인딩)
- [ ] 다크/라이트 토글 추가

### G. 테스트/품질 🔄 진행중
- [ ] ViewModel 단위 테스트(파일 저장 mock, JSON 포맷 로직)
- [ ] **정규표현식 테스트**: 다양한 패턴에 대한 매칭 정확성 검증
- [ ] **성능 테스트**: 대용량 JSON 파일에서의 정규표현식 성능 측정
- [ ] UI 테스트(WinAppDriver 또는 Playwright for .NET Desktop Experimental)

## 정규표현식 기반 필터링 개발 순서 📝

### 1단계: 기본 구조 확장 (예상 소요: 2-3시간)
1. MainViewModel에 정규표현식 관련 속성 추가
2. UI에 Regex 모드 토글 버튼 추가
3. 기본적인 정규표현식 모드 활성화/비활성화 구현

### 2단계: 정규표현식 엔진 통합 (예상 소요: 3-4시간)
1. TextHighlightService에 정규표현식 지원 추가
2. 정규표현식 유효성 검증 로직 구현
3. 매칭 개수 계산 및 표시 기능 구현

### 3단계: 고급 옵션 및 UI 개선 (예상 소요: 4-5시간)
1. 정규표현식 옵션 패널 구현 (IgnoreCase, Multiline 등)
2. 매칭 결과 네비게이션 기능 (Next/Previous)
3. 정규표현식 도움말 시스템 구현

### 4단계: 성능 최적화 및 오류 처리 (예상 소요: 2-3시간)
1. 정규표현식 컴파일 캐싱 구현
2. 복잡한 패턴에 대한 성능 경고 시스템
3. 사용자 친화적 오류 메시지 시스템

### 5단계: 테스트 및 문서화 (예상 소요: 2-3시간)
1. 단위 테스트 작성 (정규표현식 매칭 정확성)
2. 성능 테스트 및 벤치마킹
3. 사용법 가이드 및 예제 작성

## 정규표현식 예제 패턴 라이브러리 📚

개발 완료 후 제공될 기본 패턴들:
```javascript
// JSON 관련 패턴
"[A-Za-z_][A-Za-z0-9_]*"     // JSON 키 이름
"\d+"                         // 숫자 값
"\".*?\""                     // 문자열 값
"true|false|null"             // JSON 리터럴

// 일반적인 패턴
"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}"  // 이메일
"https?://[^\s]+"                                   // URL
"\b\d{3}-\d{3}-\d{4}\b"                            // 전화번호
"\b\d{4}-\d{2}-\d{2}\b"                            // 날짜 (YYYY-MM-DD)
```

## 실행 방법
1) **빌드**: `dotnet build wpf/wpf.csproj` 또는 Visual Studio에서 빌드
2) **실행**: wpf 프로젝트 시작
3) **기능 사용**:
   - **파일 > 새로 만들기**: `{}`로 초기화 (Ctrl+N)
   - **파일 > 열기**: JSON 파일 선택 후 로드 (Ctrl+O)
   - **파일 > 저장**: 파일이 없으면 경로 선택, 있으면 덮어쓰기 (Ctrl+S)
   - **파일 > 다른 이름으로 저장**: 새 경로 선택 (Ctrl+Shift+S)
   - **편집 > Format JSON**: 현재 내용 포매팅 (Ctrl+Shift+F)
   - **실시간 하이라이트**: 하단 입력 상자에 검색어 입력 → **즉시 강조 표시**
   - **🆕 정규표현식 필터링**: Regex 모드 활성화 후 정규표현식 패턴 입력 → 매칭 텍스트만 하이라이트
   - **Ctrl+S**: JSON 포맷팅 후 저장

## 데이터 바인딩
- **AvalonEdit.Text** ↔ **MainViewModel.Content** (양방향, PropertyChanged)
- **제목 표시**: MainViewModel.Title → Window.Title 영역
- **명령**: NewCommand, OpenCommand, SaveCommand, SaveAsCommand, FormatJsonCommand
- **실시간 하이라이트**: TextBox 입력 → MainViewModel.HighlightTerm → 즉시 시각 업데이트
- **🆕 정규표현식**: RegexPattern → TextHighlightService → 패턴 매칭 하이라이트

## 강조(하이라이트) 아키텍처 ✅ 실시간 최적화 완료

### 🚀 실시간 성능 개선사항
- **TextMarkerService 개선**:
  - `SetTextView()`: TextView 참조로 즉시 렌더링 제어
  - `RequestRedraw()`: Dispatcher를 통한 즉시 시각 업데이트
  - 마커 추가/제거 시 자동 렌더링 트리거
  
- **TextHighlightService 개선**:
  - **디바운싱**: 300ms 타이머로 과도한 검색 방지
  - `HighlightTextImmediate()`: PropertyChanged 시 즉시 하이라이트
  - 컴파일된 정규표현식으로 성능 향상
  - 리소스 해제 메서드 (`Dispose()`)

### 🔄 실시간 업데이트 플로우
1. **사용자 입력** → HighlightBox.TextChanged
2. **ViewModel 업데이트** → MainViewModel.HighlightTerm
3. **PropertyChanged 트리거** → OnViewModelPropertyChanged
4. **즉시 하이라이트** → HighlightTextImmediate()
5. **렌더링 업데이트** → TextView.Redraw()
6. **화면 반영** → 실시간 시각 피드백

## 키보드 단축키
- **Ctrl+N**: 새 문서
- **Ctrl+O**: 파일 열기
- **Ctrl+S**: JSON 포맷 후 저장
- **Ctrl+Shift+S**: 다른 이름으로 저장
- **Ctrl+Shift+F**: JSON 포맷
- **🆕 Ctrl+R**: 정규표현식 모드 토글 (계획)

## 정규표현식 필터링 특징 🎯 (개발 예정)
- **패턴 매칭**: 정규표현식으로 복잡한 텍스트 패턴 검색
- **실시간 검증**: 패턴 입력과 동시에 유효성 검사
- **매칭 통계**: 매칭된 항목 개수 실시간 표시
- **옵션 지원**: IgnoreCase, Multiline, Singleline 등
- **성능 최적화**: 패턴 컴파일 캐싱 및 디바운싱
- **오류 처리**: 친화적인 정규표현식 오류 메시지

## 향후 확장 설계
- **다중 문서 탭**: ObservableCollection<DocumentModel> + SelectedDocument
- **테마 설정**: Primary/Accent 색상 변경, 다크/라이트 모드
- **🆕 고급 정규표현식**: 다중 패턴, 그룹별 색상, 매칭 네비게이션
- **검색/바꾸기**: Find/Replace 다이얼로그 (정규표현식 지원)
- **패턴 라이브러리**: 자주 사용하는 정규표현식 템플릿 저장/관리
- **최근 파일**: JumpList 또는 설정 기반 리스트
- **설정 저장**: User-scoped Settings 또는 JSON config

## 코딩 규칙
- **Code-behind 최소화**: 입력 바인딩/이벤트 라우팅만 허용
- **비즈니스 로직**: ViewModel에 배치 (파일 I/O, 포맷, 상태 관리)
- **비동기 작업**: async/await 적용으로 UI 블로킹 방지
- **리소스 관리**: IDisposable 패턴으로 메모리 누수 방지
- **정규표현식 최적화**: 컴파일 캐싱 및 복잡도 제한

## 문제 해결 TIP
- **JSON 포맷 오류**: MessageBox 알림 (향후 Snackbar로 UX 개선)
- **Material 리소스 로딩 실패**: 패키지 버전/ResourceDictionary 경로 확인
- **AvalonEdit 바인딩**: TextChanged 이벤트로 양방향 바인딩 구현
- **하이라이트 성능**: 디바운싱 및 컴파일된 정규표현식 사용
- **실시간 업데이트**: TextView.Redraw()와 Dispatcher 활용
- **🆕 정규표현식 성능**: 복잡한 패턴 시 백그라운드 처리 및 취소 토큰 활용

## 참고 라이브러리
- **Material Design in XAML Toolkit**: https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit
- **AvalonEdit**: https://github.com/icsharpcode/AvalonEdit
- **.NET Regex**: https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions

## 정규표현식 학습 리소스 📖
- **정규표현식 기초**: https://regexr.com/
- **C# Regex 가이드**: https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference
- **온라인 테스터**: https://regex101.com/
- **패턴 예제**: https://regexlib.com/

## 라이선스/크레딧
본 템플릿은 오픈소스 패키지를 사용합니다. 각 패키지의 라이선스 정책을 준수하세요.

## 완료된 기능 요약 ✅
1. **✅ Open 기능**: 파일 선택 다이얼로그로 JSON 파일 로드
2. **✅ AvalonEdit 통합**: 문법 강조, 줄 번호, 고급 편집 기능
3. **✅ 실시간 하이라이트 서비스**: 즉시 텍스트 강조 표시 및 마커 관리
4. **✅ Material Design UI**: ColorZone, 메뉴, 버튼 스타일 적용
5. **✅ 키보드 단축키**: 모든 주요 기능에 단축키 바인딩
6. **✅ MVVM 완전 구현**: 데이터 바인딩, 명령 패턴, 관심사 분리
7. **✅ 실시간 반응성**: 타이핑과 동시에 하이라이트 적용, 성능 최적화

## 개발 예정 기능 요약 🚀
1. **📋 정규표현식 기반 필터링**: 복잡한 패턴 매칭으로 고급 텍스트 검색
2. **📋 정규표현식 UI**: 모드 토글, 옵션 패널, 도움말 시스템
3. **📋 매칭 네비게이션**: Next/Previous 버튼으로 매칭 결과 탐색
4. **📋 패턴 라이브러리**: 자주 사용하는 정규표현식 템플릿 관리
5. **📋 성능 최적화**: 대용량 문서에서의 정규표현식 처리 최적화

---

## 🔧 다음 개발자를 위한 시작 가이드

### 즉시 시작 가능한 작업들:
1. **정규표현식 UI 추가** (난이도: ⭐⭐☆☆☆)
2. **RegexOptions 바인딩** (난이도: ⭐⭐⭐☆☆) 
3. **매칭 개수 표시** (난이도: ⭐⭐☆☆☆)
4. **정규표현식 도움말** (난이도: ⭐⭐⭐⭐☆)
5. **성능 최적화** (난이도: ⭐⭐⭐⭐⭐)

### 작업 우선순위:
1. E-1, E-2 (UI 및 ViewModel 확장) → 기반 구조
2. E-3 (TextHighlightService 정규표현식 지원) → 핵심 기능  
3. E-4 (UI 레이아웃) → 사용자 경험
4. E-5, E-6 (도움말, 최적화) → 고급 기능
5. E-7, G (오류 처리, 테스트) → 품질 보증
