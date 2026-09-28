# CLAUDE.md

이 저장소의 공용 에이전트 규칙은 [`AGENTS.md`](./AGENTS.md)에 있다. 작업을 시작하기 전에
반드시 `AGENTS.md` 전체를 읽고 따른다 (프로젝트 개요, 레이어 아키텍처, asmdef 의존 규칙,
테스트 규칙, 작업 규칙, 보안 규칙).

Claude Code 전용 참고사항:

- 이 폴더는 사용자의 홈 디렉터리이며 다른 도구들의 설정 폴더(`.ssh`, `.agents` 등)와
  같은 위치에 있다. Unity 프로젝트(`Assets/`, `Packages/`, `ProjectSettings/`)와
  `AGENTS.md`/`CLAUDE.md`/`.gitignore`/`.gitattributes` 외의 파일·폴더는 이 프로젝트와
  무관하므로 건드리지 않는다.
- git 저장소는 이미 `git init` + Git LFS(로컬)로 초기화되어 있고, `.gitignore`가 위 Unity
  프로젝트 관련 경로만 추적하도록 나머지를 전부 무시한다. 새 최상위 폴더/파일을 추가로
  추적해야 할 경우 `.gitignore`의 화이트리스트 규칙을 함께 갱신한다.
