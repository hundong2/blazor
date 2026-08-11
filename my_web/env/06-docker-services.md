# 06. Docker Compose 서비스

Windows 11에서는 Docker Desktop WSL2 Linux containers를 사용한다. Windows Server에서는 Ubuntu Hyper-V VM의 Docker Compose를 권장한다.

## Open WebUI와 Windows Ollama

Ollama는 Windows에서 loopback `17836`으로 실행한다.

```powershell
$env:OLLAMA_HOST = '127.0.0.1:17836'
ollama serve
```

Docker Compose 설정을 준비한다.

```powershell
Set-Location .\deploy\compose
Copy-Item .env.example .env
notepad .env
docker compose config
docker compose up -d
docker compose ps
```

Open WebUI는 host의 `127.0.0.1:17832`에만 publish한다. container에서 Windows Ollama는 `http://host.docker.internal:17836`으로 접근한다.

운영에서는 Open WebUI image를 immutable version 또는 digest로 고정한다. `main`, `latest` tag는 사용하지 않는다.

## 완료 확인

- [ ] `docker compose config`에 의도하지 않은 `0.0.0.0` publish가 없다.
- [ ] Windows에서 `http://127.0.0.1:17832`가 열린다.
- [ ] 외부에서는 17832와 17836에 직접 접속할 수 없다.
- [ ] Portal 로그인 후 `/apps/open-webui/`로 접근한다.
