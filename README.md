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

# WPF MVVM JSON Editor (Material Design)

이 프로젝트는 .NET 8 WPF에서 MVVM 패턴과 Material Design in XAML Toolkit을 활용해 JSON 편집기(메뉴, 저장/다른이름으로 저장, Ctrl+S 자동 정렬, 강조 표시)를 구현하기 위한 템플릿과 체크리스트를 제공합니다.

구성 요소
- 패턴: MVVM (Model, ViewModel, View)
- UI 프레임워크: WPF (.NET 8)
- 스타일: Material Design in XAML Toolkit
- 기능: 파일 메뉴(새로 만들기, 저장, 다른 이름으로 저장), JSON 자동 정렬(Format), Ctrl+S 시 정렬+저장, 키워드 강조(확장 가능)

폴더 구조
- wpf/
  - Models/: DocumentModel
  - ViewModels/: MainViewModel
  - Views/: MainWindow.xaml (필요 시 이동)
  - Infrastructure/: ObservableObject, RelayCommand
  - Behaviors/: TextBoxBehaviors (강조 기능 샘플)

시작하기 (개발 환경)
1) .NET SDK 8 설치
2) IDE: Visual Studio 2022 이상 또는 VS Code + C# Dev Kit
3) NuGet 복원: 처음 빌드 시 자동

NuGet 패키지
- MaterialDesignThemes (5.x)
- MaterialDesignColors (2.x)

절차 지향 개발 가이드 (체크리스트)
A. 템플릿 초기화
- [x] wpf/wpf.csproj에 MaterialDesignThemes, MaterialDesignColors 패키지 참조 추가
- [x] App.xaml에 Material Design ResourceDictionary 병합 구성
- [x] MVVM 기본 클래스(ObservableObject, RelayCommand) 추가
- [x] Model(DocumentModel) 추가
- [x] ViewModel(MainViewModel) 추가 및 명령 구현(New, Save, Save As, FormatJson)
- [x] MainWindow.xaml UI 초안: 메뉴(파일/편집/설정), 에디터(TextBox), 상태/강조 입력
- [x] Ctrl+S: JSON 정렬 후 저장 동작 바인딩

B. UI/스타일 강화
- [ ] MainWindow to Material Design 레이아웃/컨트롤 최적화 (ColorZone, Typography, Buttons)
- [ ] Theme 변경(Primary/Accent) 옵션 추가 (Settings 메뉴에 바인딩)
- [ ] 다크/라이트 토글 추가

C. 파일 기능 확장
- [ ] 파일 열기(Open) 추가, 최근 파일 리스트 유지(앱 설정/JumpList)
- [ ] 저장 전 유효성 검사 및 에러 표시(Validation)
- [ ] 문서 제목 변경 추적 및 창 Title 바인딩 개선

D. 에디터 강화
- [ ] AvalonEdit 또는 ICSharpCode.TextEditor 도입(문법 강조/줄번호/괄호 매칭)
- [ ] JSON 스키마 검증(Newtonsoft.Json.Schema 또는 System.Text.Json.Schema)
- [ ] 검색/바꾸기 UI 및 키워드 강조 연결(Behaviors -> 실제 에디터 하이라이트 API로 교체)
- [ ] 자동 서식 지정 옵션 (입력 시 포매팅, 들여쓰기, 탭/스페이스)

E. 테스트/품질
- [ ] ViewModel 단위 테스트(파일 저장 mock, JSON 포맷 로직)
- [ ] UI 테스트(WinAppDriver 또는 Playwright for .NET Desktop Experimental)

실행 방법
1) 솔루션 빌드 후 wpf 프로젝트 시작
2) 앱 실행 시 상단 메뉴 이용
   - 파일 > 새로 만들기: {} 초기화
   - 파일 > 저장: 파일이 없으면 경로 선택, 있으면 덮어쓰기
   - 파일 > 다른 이름으로 저장: 새 경로 선택
   - 편집 > Format JSON: 현재 내용 포매팅
   - Ctrl+S: 포매팅 후 저장

데이터 바인딩
- TextBox.Text <-> MainViewModel.Content (TwoWay, PropertyChanged)
- 제목 표시: MainViewModel.Title
- 명령: NewCommand, SaveCommand, SaveAsCommand, FormatJsonCommand

강조(하이라이트) 설계
- 현재 샘플: RichTextBox용 Attached Property(TextBoxBehaviors.HighlightText)
- 추천: AvalonEdit 채택 시 TextMarkerService로 특정 텍스트 범위에 강조 표시
- 연결 절차(예정)
  1) AvalonEdit 설치: NuGet ICSharpCode.AvalonEdit
  2) XAML에 xmlns:avalonedit 추가, TextEditor 컨트롤 배치
  3) ViewModel에 HighlightTerm 속성 추가, 바인딩
  4) Behavior/Service에서 TextEditor.Document에서 검색 후 마커 추가

향후 확장 설계
- 다중 문서 탭(MDI): ObservableCollection<DocumentModel> + SelectedDocument
- 명령 단축키: RoutedUICommand로 이동 및 InputBindings XAML화
- 설정 저장: User-scoped Settings 또는 JSON config

코딩 규칙
- Code-behind 로직 최소화(입력 바인딩/윈도우 이벤트 라우팅 정도만 허용)
- 파일 I/O, 포맷, 상태 관리는 ViewModel에 배치
- 비동기 파일 작업 시 async/await 적용 및 UI Lock 방지

개발 흐름 예시
1) UI: AvalonEdit 교체 및 하이라이트 서비스 구현
2) ViewModel: HighlightTerm 추가, INotifyPropertyChanged로 변경 알림
3) 메뉴/단축키: Open/Undo/Redo/Find 등 추가
4) Settings: Theme/Editor 설정 저장 및 적용

문제 해결 TIP
- JSON 포맷 오류 발생 시 메시지박스 알림(향후 Snackbar로 UX 개선)
- Material 리소스 로딩 실패 시 패키지 버전/ResourceDictionary 경로 확인
- Ctrl+S 중 포맷/저장 순서 제어는 ICommand 조합 또는 CompositeCommand로 리팩토링

참고 라이브러리
- Material Design in XAML Toolkit
- ICSharpCode.AvalonEdit (추천)

라이선스/크레딧
- 본 템플릿은 오픈소스 패키지를 사용합니다. 각 패키지의 라이선스 정책을 준수하세요.
