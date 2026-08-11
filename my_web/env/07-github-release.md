# 07. GitHub CI와 Release 배포

## 개발 흐름

1. `codex/<feature>` 또는 기능 branch에서 작업한다.
2. Pull Request에서 .NET/C++ build와 test를 통과시킨다.
3. main merge 후 version tag를 만든다.
4. GitHub Actions가 release ZIP, checksum, SBOM을 생성한다.
5. Windows 서버에서 `update.ps1 -Version vX.Y.Z`로 가져온다.

운영 config, DB, 인증서, DNS token은 GitHub Actions artifact에 포함하지 않는다. GitHub-hosted runner가 가정의 IIS 서버에 직접 접속하도록 만들지 않는다.

## 최초 개발 단계 명령

```powershell
git checkout -b codex/my-web-mvp
git add my_web
git diff --cached --check
```

commit/push/PR은 변경 범위를 직접 검토한 뒤 수행한다.

## workflow 설치

GitHub는 repository root의 `.github/workflows`만 인식한다. 프로젝트 파일은 `my_web` 안에 유지하고 다음 스크립트로 template을 설치한다.

```powershell
# 변경 대상 미리보기
.\scripts\install-github-workflows.ps1

# repository root에 실제 설치
.\scripts\install-github-workflows.ps1 -Apply
```

설치 후 root `.github/workflows/my-web-ci.yml`과 `my-web-release.yml`을 검토해 함께 commit한다. 현재 repository처럼 My Web이 하위 폴더인 구성을 기준으로 path filter와 working directory가 지정되어 있다.

## 완료 확인

- [ ] main branch protection을 설정했다.
- [ ] CI는 운영 secret 없이 실행된다.
- [ ] release artifact의 checksum을 검증한다.
- [ ] 이전 release로 rollback하는 절차를 시험했다.
