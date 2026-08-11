# 03. 로컬 개발 실행

## 복원과 빌드

```powershell
Set-Location D:\workspace\blazor\my_web
dotnet restore .\MyWeb.slnx
dotnet build .\MyWeb.slnx --no-restore
```

## 최초 소유자 생성

공개 회원가입 기능은 없다. 최초 한 번만 interactive bootstrap 명령을 실행한다.

```powershell
.\scripts\bootstrap-admin.ps1 -Email 'owner@example.com'
```

암호는 화면에 표시하거나 PowerShell history에 남기지 않는다. 최소 14자와 대/소문자, 숫자, 특수문자가 필요하다. 계정이 하나라도 존재하면 bootstrap은 거부된다.

## Portal 실행

```powershell
.\scripts\run-local.ps1
```

브라우저에서 `http://127.0.0.1:17831`을 연다. 로컬 개발만 HTTP를 허용하며 운영 cookie는 반드시 IIS HTTPS에서만 사용한다.

로그인 후 `서비스 관리 → 서비스 추가`에서 다음 예시를 등록한다.

```text
서비스 이름: Open WebUI
URL 식별자: open-webui
내부 URL: http://127.0.0.1:17832/
분류: AI
상태 확인 경로: /health
```

저장하면 `/apps/open-webui/` route와 좌측 메뉴가 서버 재시작 없이 생성된다.

다른 설정 파일로 실행하려면 다음과 같이 지정한다.

```powershell
.\scripts\run-local.ps1 -ConfigPath D:\MyWeb\appsettings.Local.json
```

## 완료 확인

- [ ] `/health/live`가 200을 반환한다.
- [ ] `/`는 로그인하지 않은 상태에서 로그인 페이지로 이동한다.
- [ ] bootstrap 소유자로 로그인한다.
- [ ] Authenticator 앱에 TOTP 키를 등록하고 첫 코드를 검증한다.
- [ ] 다시 로그인할 때 비밀번호 다음에 6자리 코드가 요구된다.
- [ ] 존재하지 않는 Open WebUI backend가 외부에 직접 노출되지 않는다.
