# CLAUDE.md

이 저장소의 공용 에이전트 규칙은 [`AGENTS.md`](./AGENTS.md)에 있다. 작업을 시작하기 전에
반드시 `AGENTS.md` 전체를 읽고 따른다 (프로젝트 개요, 레이어 아키텍처, asmdef 의존 규칙,
테스트 규칙, 작업 규칙, 보안 규칙).

Claude Code 전용 참고사항:

- 이 폴더는 사용자의 홈 디렉터리이며 다른 도구들의 설정 폴더(`.ssh`, `.agents` 등)와
  같은 위치에 있다. Unity 프로젝트(`Assets/`, `Packages/`, `ProjectSettings/`)와
  `AGENTS.md`/`CLAUDE.md`/`.gitignore`/`.gitattributes` 외의 파일·폴더는 이 프로젝트와
  무관하므로 건드리지 않는다.
- git 저장소는 `git init`으로 초기화되어 있고, `.gitignore`가 위 Unity 프로젝트 관련
  경로만 추적하도록 나머지를 전부 무시한다. 새 최상위 폴더/파일을 추가로 추적해야 할
  경우 `.gitignore`의 화이트리스트 규칙을 함께 갱신한다.
- **Git LFS는 이 저장소에서 쓰지 않는다.** 원격(`origin`)이 fork라서 GitHub가 fork에
  새 LFS 오브젝트 업로드를 막는다. `.gitattributes`에 LFS 필터를 다시 추가하지 말 것 —
  폰트/이미지 등 바이너리도 그냥 일반 git blob으로 커밋한다.
- **원격 저장소**: `origin` = `https://github.com/cornaill008/csharp-sourse-2026.git`
  (사용자의 C# 수업 과제 저장소, 이 Unity 프로젝트와 무관한 기존 커밋들이 `master`에 있음).
  **`master`는 절대 건드리지 않는다** — 이 프로젝트는 `unity-image-search` 브랜치에서만
  작업하고 push한다.
- 프로젝트는 의도적으로 홈 디렉터리 바로 아래(하위 폴더 없이)에 둔다 — 한 번
  `Day09_Unity CLI/` 폴더로 옮겼다가 사용자 요청으로 다시 되돌린 이력이 있음(커밋
  `fcd80c2`→`8e49141`). 다시 하위 폴더로 옮기라는 요청이 없는 한 지금 구조를 유지한다.

## 현재 진행 상황 (이어서 작업 시 참고)

- Core/Domain/Data/Presentation/Composition 레이어와 EditMode 테스트(37개, 전부 통과)
  구현 완료. 검색 화면 UI는 `Tools/UI/Generate Search Screen` 메뉴(`Assets/Editor/
  SearchScreenBuilder.cs`)로 재생성 가능.
- 화면은 **Mock DataSource**(`Assets/Scripts/Composition/Mock/`)에 연결되어 실제
  Pixabay API 없이 앱이 동작한다. Mock 시나리오(성공/빈 결과/연결 실패/서버 오류)는
  씬의 `Composition` 오브젝트에 있는 `MockPixabaySearchDataSource` 컴포넌트 Inspector에서
  고른다.
- **다음 단계 후보**: (1) `Composition/SearchScreenInstaller`가 Mock 대신 실제
  `UnityWebRequestSearchDataSource`/`UnityWebRequestThumbnailDataSource`(이미 Data
  레이어에 구현되어 있음)를 쓰도록 교체, (2) 상세 화면 추가, (3) API 키를 커밋되지 않는
  로컬 설정에서 주입하는 배선 마련.
- Unity CLI로 이 프로젝트를 다시 조작할 때: 창 포커스가 없으면 Play 모드 시뮬레이션이
  멈출 수 있어 `Application.runInBackground`가 켜져 있는지 확인할 것(`ProjectSettings.asset`
  의 `runInBackground: 1`). 또한 `capture_game_view` 스크린샷이 간헐적으로 오래된/겹친
  프레임을 보여줄 수 있으니, 중요한 상태 확인은 스크린샷만 믿지 말고 `eval`로 하이러키를
  직접 조회해 대조할 것.
