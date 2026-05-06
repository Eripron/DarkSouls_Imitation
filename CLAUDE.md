# DrakSouls 프로젝트 규칙

## PLAN.md 참조 규칙

아래 유형의 질문을 받으면 반드시 `PLAN.md`를 읽고 현재 구현 단계와 우선순위를 확인한 후 답한다.

- 무엇을 만들어야 하는지 / 다음에 뭘 해야 하는지 묻는 경우
- 특정 기능 구현을 요청하는 경우 (해당 기능이 어느 Phase에 속하는지 확인)
- 설계 / 구조에 대해 묻는 경우
- 이미 완료된 기능인지 아닌지 판단이 필요한 경우

아래 유형은 PLAN.md를 읽지 않아도 된다.

- 특정 코드의 버그 수정처럼 맥락이 명확한 경우
- 문법 / 언어 / Unity API에 대한 일반적인 질문
- PLAN.md와 무관한 단순 질문

## 프로젝트 개요

- **장르:** 다크소울 스타일 액션 RPG
- **엔진:** Unity (InputSystem 사용)
- **네임스페이스:** `Game`
- **스크립트 경로:** `Assets/_Project/Scripts/`

## 코드 컨벤션

- 멤버 변수: `_camelCase` (접두사 `_`)
- float 변수: `_f` 접두사 (예: `_fMoveSpeed`)
- Vector 변수: `_v` 접두사 (예: `_vMoveDir`)
- Transform 변수: `_trans` 접두사 (예: `_transTarget`)
- InputAction 변수: `_input` 접두사 (예: `_inputMove`)
- 네임스페이스: `Game`
- 주석은 한국어로 작성
