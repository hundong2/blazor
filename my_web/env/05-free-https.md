# 05. Let’s Encrypt 무료 HTTPS

IIS에서는 Let’s Encrypt와 win-acme를 사용한다.

## 일반 인증서: 권장 시작점

`portal.example.com` 하나로 시작하면 HTTP-01 검증이 가장 단순하다.

1. DNS A/AAAA record를 공인 IP로 설정한다.
2. 공유기에서 80/443만 IIS PC로 전달한다.
3. `%ProgramFiles%\win-acme`에 win-acme를 설치하고 관리자 권한으로 `wacs.exe`를 실행한다.
4. IIS site를 선택해 staging에서 먼저 시험한다.
5. production 인증서를 발급하고 IIS HTTPS binding 생성을 선택한다.
6. win-acme renewal Scheduled Task와 Event Viewer/log를 확인한다.

## wildcard 인증서

`*.apps.example.com`은 DNS-01만 사용할 수 있다. DNS 제공업체 API가 자동화를 지원할 때만 사용한다. DNS token은 최소 권한으로 만들고 Git/config/.env에 쓰지 않는다.

## 완료 확인

- [ ] 외부 모바일 네트워크에서 인증서 경고 없이 접속된다.
- [ ] HTTP가 HTTPS로 redirect된다.
- [ ] win-acme 강제 renewal을 staging에서 시험했다.
- [ ] 17831~17836은 외부 port scan에서 닫혀 있다.
