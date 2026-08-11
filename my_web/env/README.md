# My Web 환경 설정 가이드

이 폴더는 처음 개발 환경을 만들고 IIS 운영 서버까지 배포하는 순서형 가이드다. 번호 순서대로 진행한다.

1. [01-prerequisites.md](01-prerequisites.md) — Windows와 필수 프로그램 확인
2. [02-ports-and-config.md](02-ports-and-config.md) — 내부 포트와 config 결정
3. [03-local-development.md](03-local-development.md) — 로컬 실행과 최초 계정 생성
4. [04-iis-deployment.md](04-iis-deployment.md) — IIS에 ASP.NET Core 배포
5. [05-free-https.md](05-free-https.md) — Let’s Encrypt 무료 인증서
6. [06-docker-services.md](06-docker-services.md) — Open WebUI와 Ollama 연결
7. [07-github-release.md](07-github-release.md) — GitHub Actions/Release 운영

각 문서의 확인표를 완료한 뒤 다음 단계로 이동한다. 실제 비밀번호, DNS API token, private key는 이 폴더나 Git에 저장하지 않는다.
