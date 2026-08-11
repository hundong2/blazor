# 04. IIS 배포

IIS가 외부 80/443과 인증서를 담당한다. Portal은 IIS의 ASP.NET Core Module을 통해 in-process로 실행하며 Caddy는 설치하지 않는다.

## IIS 준비

1. IIS Web Server, Management Console, WebSocket Protocol, Application Initialization을 활성화한다.
2. IIS 설치 후 .NET 10 Hosting Bundle을 설치한다. Hosting Bundle을 먼저 설치했다면 repair한다.
3. `MyWebPortal` 전용 Application Pool을 만들고 `.NET CLR Version = No Managed Code`로 설정한다.
4. `Start Mode = AlwaysRunning`, `Idle Time-out = 0`, Site의 `Preload Enabled = True`로 설정한다.
5. App Pool identity에 배포 폴더 읽기 권한과 `C:\ProgramData\MyWeb\data` 수정 권한만 부여한다.

## publish

```powershell
.\scripts\doctor.ps1 -ConfigPath C:\ProgramData\MyWeb\config\appsettings.Production.json
.\scripts\publish.ps1 -Version 0.1.0 -OutputRoot C:\ProgramData\MyWeb\releases
```

IIS Site의 physical path를 publish 폴더로 설정한다. 최초에는 HTTP host binding만 만들고 HTTPS는 다음 단계의 win-acme가 추가하게 한다.

IIS in-process에서는 `Server:Port`가 공개 포트가 되지 않는다. 이 값은 로컬 개발 및 향후 out-of-process/Windows Service 모드에 사용된다. 외부에서는 IIS binding의 80/443만 보인다.

## 완료 확인

- [ ] IIS Site가 HTTP에서 Portal을 표시한다.
- [ ] App Pool 재시작 후 Portal이 자동으로 시작한다.
- [ ] WebSocket Protocol이 활성화되어 있다.
- [ ] IIS 사용자에게 config/secrets 디렉터리의 불필요한 권한이 없다.
