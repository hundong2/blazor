# 01. 필수 환경 확인

## 지원 기준

- Windows 11 Pro/Enterprise 또는 지원 중인 Windows Server
- .NET 10 Hosting Bundle: IIS 운영 서버
- .NET 10 SDK: 개발 PC
- IIS 10과 WebSocket Protocol 기능
- Git
- 선택: Docker Desktop + WSL2 Linux containers(Windows 11)
- 선택: win-acme(Let’s Encrypt 자동 갱신)

## 확인 명령

```powershell
dotnet --info
git --version
Get-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Get-WindowsOptionalFeature -Online -FeatureName IIS-WebSockets
```

Windows Server에서 Linux container를 운영한다면 Docker Desktop을 전제로 하지 말고 Hyper-V의 Ubuntu VM에서 Docker Compose를 운영한다.

## 완료 확인

- [ ] Windows edition/version을 기록했다.
- [ ] .NET 10 SDK 또는 Hosting Bundle을 설치했다.
- [ ] IIS와 WebSocket 기능을 활성화했다.
- [ ] `dotnet --info`가 오류 없이 실행된다.
- [ ] Docker가 필요한 경우 `docker version`과 `docker compose version`을 확인했다.
