# 02. 내부 포트와 config

## 포트 정책

외부 공개 포트와 내부 서비스 포트를 분리한다.

| 구분 | 기본값 | 바인딩 | 외부 공개 |
|---|---:|---|---|
| IIS HTTP/ACME | 80 | 공인 interface | 인증서 검증과 HTTPS redirect만 |
| IIS HTTPS | 443 | 공인 interface | 예, 유일한 서비스 진입점 |
| My Web Portal | 17831 | `127.0.0.1` | 아니요 |
| Open WebUI | 17832 | `127.0.0.1` | 아니요 |
| Guacamole(예정) | 17833 | `127.0.0.1` 또는 VM 사설 IP | 아니요 |
| C++ Native Host(예정) | 17834 | `127.0.0.1` | 아니요 |
| Device Agent Gateway(예정) | 17835 | 사설망 | 아니요 |
| Ollama | 17836 | `127.0.0.1` | 아니요 |

Portal은 `1024..49151` 범위와 loopback 주소만 허용하며 흔히 충돌하는 `3000`, `5000`, `5001`, `8000`, `8080`, `11434`를 거부한다. 내부 서비스 포트도 시작 전에 점유 여부를 확인한다.

```powershell
Get-NetTCPConnection -State Listen |
  Where-Object LocalPort -In 17831,17832,17833,17834,17835,17836
```

## config 만들기

```powershell
New-Item -ItemType Directory -Force C:\ProgramData\MyWeb\config
Copy-Item .\config\appsettings.Production.example.json `
  C:\ProgramData\MyWeb\config\appsettings.Production.json
notepad C:\ProgramData\MyWeb\config\appsettings.Production.json
```

포트 하나만 임시 변경할 때는 환경변수를 사용할 수 있다.

```powershell
$env:MYWEB_Server__Port = '27831'
```

Portal 자체 포트와 도메인은 config/env에서 관리한다. Open WebUI 같은 등록 서비스의 내부 URL과 포트는 로그인 후 `서비스 관리` 화면에서 변경하며 SQLite App Registry에 저장된다. 저장 즉시 메뉴와 YARP route가 갱신된다.

우선순위는 `명령행 > MYWEB_ 환경변수 > MYWEB_CONFIG_FILE > ProgramData 운영 config > appsettings.json`이다.

## 완료 확인

- [ ] 선택한 내부 포트가 현재 사용 중이 아니다.
- [ ] Portal bind address가 `127.0.0.1` 또는 `::1`이다.
- [ ] 공유기/Windows 방화벽에 17831~17836 inbound 규칙이 없다.
- [ ] 운영 config를 Git에 추가하지 않았다.
