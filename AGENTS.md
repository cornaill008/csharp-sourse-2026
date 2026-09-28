# AGENTS.md

이 문서는 이 저장소에서 작업하는 모든 에이전트(Claude Code, 기타 CLI 코딩 에이전트 등)가
공통으로 따라야 하는 규칙입니다. 도구별 전용 지침은 각 도구의 전용 파일(`CLAUDE.md` 등)에서
이 문서를 참조합니다.

## 1. 프로젝트 개요

- **앱**: 키워드로 이미지를 검색하고 결과를 그리드로 보여주는 모바일 앱
- **화면**: 세로(Portrait) 고정, 모바일 (Android / iOS)
- **성격**: 게임 요소 없음, UI 중심 앱 (uGUI 기반)
- **Unity 버전**: 6000.3.10f1
- **렌더 파이프라인**: Built-in Render Pipeline (Core 템플릿, URP 미사용)
- **타겟 플랫폼**: Android(빌드 모듈 설치됨), iOS(Bundle ID만 설정, 로컬에 빌드 모듈 미설치 — Unity Hub에서 iOS Build Support 추가 설치 필요)

## 2. 아키텍처 규칙

레이어 구조와 의존 방향:

```
Presentation → (Service) → Domain ← Data
                              ^
                            Core (모든 레이어가 참조 가능한 공통 레이어)
```

- **Domain** (Model, Repository 인터페이스)
  - `UnityEngine`에 의존하지 않는다 (엔진 API 사용 금지).
  - Data 레이어의 DTO에 의존하지 않는다. Domain은 순수 C# Model과 Repository 인터페이스만 가진다.
- **Data**
  - 외부 API/네트워크 응답을 표현하는 DTO는 Data 레이어 밖으로 나가지 않는다.
  - DTO → Domain Model 변환은 반드시 Mapper를 통해서만 한다.
  - Domain의 Repository 인터페이스를 구현한다.
- **Service** (선택적 레이어)
  - Domain의 Repository/UseCase를 조합해 Presentation에 제공한다.
  - Data를 직접 참조하지 않는다 (Domain 인터페이스로만 접근).
- **Presentation**
  - MonoBehaviour, View, ViewModel/Controller 등 UI 관련 코드.
  - Service(또는 Domain)를 통해서만 데이터에 접근하고, Data의 DTO를 직접 다루지 않는다.
- **Core**
  - 모든 레이어가 공통으로 쓰는 유틸리티, `Result<T>` 타입 등을 포함한다.
  - 다른 레이어(Domain/Data/Service/Presentation)에 의존하지 않는다.

**실패 처리**: 예상 가능한 실패(네트워크 오류, 파싱 실패, not found 등)는 예외 대신
`Result` 패턴으로 반환한다. 단, 취소(`OperationCanceledException`/`CancellationToken` 취소)는
예외로 정상 전파한다 — Result로 감싸지 않는다.

**Assembly Definition (asmdef)**: 레이어마다 asmdef를 분리해 의존 방향을 컴파일 타임에 강제한다.

- `Core.asmdef` — 다른 레이어 참조 없음
- `Domain.asmdef` — `Core`만 참조, **No Engine References 체크** (UnityEngine API 컴파일 차단)
- `Data.asmdef` — `Domain`, `Core` 참조
- `Service.asmdef` — `Domain`, `Core` 참조 (`Data` 직접 참조 금지)
- `Presentation.*.asmdef` — `Service`(또는 `Domain`), `Core` 참조 (`Data` 직접 참조 금지)

새 스크립트를 추가할 때 위 참조 규칙을 위반하면(예: Domain에서 UnityEngine 사용, Presentation에서
Data의 DTO 직접 참조) 컴파일 에러로 드러나야 정상이다 — 규칙 위반을 우회하기 위해 asmdef 참조를
느슨하게 풀지 않는다.

## 3. 테스트 규칙

- 새 기능을 구현하기 전에 테스트 시나리오를 먼저 합의하고 작성한다 (테스트 우선).
- Unit Test는 실제 네트워크를 사용하지 않는다. Repository/HTTP 클라이언트는 Mock으로 대체한다.
- 테스트를 통과시키기 위해 테스트 코드를 고치지 않는다. 테스트가 잘못됐다고 판단되면
  먼저 근거(무엇이 왜 틀렸는지)를 보고하고, 합의된 후에만 테스트를 수정한다.

## 4. 작업 규칙

- Unity 에디터 관련 작업(씬 구성, 프리팹 생성/수정, 에셋 임포트 설정 등)은 `unity-cli` 스킬을
  통해 수행한다. `.unity`/`.prefab`/`.asset` 등 YAML 직렬화 파일을 텍스트 에디터로 직접
  손으로 편집하지 않는다 (GUID/직렬화 참조가 깨질 수 있음).
- 작업 완료 보고에는 다음을 포함한다:
  - 컴파일 결과 (성공/실패, 실패 시 에러 요약)
  - 테스트 결과 요약 (실행한 테스트 수, 통과/실패)
- 커밋은 사용자가 명시적으로 요청했을 때만 수행한다. 임의로 커밋하지 않는다.

## 5. 보안

- API 키, 토큰 등 비밀 값은 절대 커밋하지 않는다.
- 비밀 값은 소스 코드나 `.asset`/씬 파일에 하드코딩하지 않고, 커밋되지 않는 로컬 설정
  (예: `.gitignore`에 등록된 로컬 전용 파일, 환경변수, CI/CD 시크릿)으로 주입한다.
- 새로운 비밀 값 파일을 추가할 경우 반드시 `.gitignore`에 패턴을 먼저 등록한다.
