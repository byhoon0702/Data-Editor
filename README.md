# Data Table Editor

- 개요

  - 데이터 편집을 Unity Editor 상에서 할 수 있도록 만든 툴
  - 엑셀과 비슷한 GUI를 가짐으로 기획에서 좀 더 쉽게 접근 하도록 하는 것이 목적
  - 대부분의 데이터 class 를 사용가능
  - 중첩 class는 버튼을 눌러 별도의 창을 띄우도록 작업
  - Json, CSV 로 저장 및 불러오기 가능
  - 필요에 따라 Class 이름을 기입 할 경우 기본적인 구조를 가지는 Script 파일을 생성, 단 실제 내용은 직접 채워 넣어야 함
  - Reorderable List, Newtonsoft Json 등 외부 패키지 필요

- 사용법

  <img width="1308" height="658" alt="image-20250823213628984" src="https://github.com/user-attachments/assets/d444c790-46c7-44dc-8bce-4ce1783bf906" />

  - TID 규칙 : 규칙성을 가지는 ID를 직접 부여 할 경우 팀원들이 다같이 볼 수 있는 사이트나 다른 문서를 연결하여 쉽게 접근 하도록 만든 버튼
  - 데이터 리스트 : 해당 에디터로 표현 가능 한 모든 데이터 리스트를 보여줌
    - Load Json List : Json 형태의 파일 모두 로드
    - Load CSV List : Csv 형태의 파일 모두 로드

- 데이터 편집 <img width="1307" height="463" alt="image-20250823214013568" src="https://github.com/user-attachments/assets/1582a202-73d2-4434-8cc8-29212fa5b98f" />


  - 데이터 테이블 이름 

    - 빈칸일 경우 Json 또는 Csv 를 로드 하여 데이터 편집

    - 존재 하지 않는 이름을 입력한 경우
      <img width="971" height="194" alt="image-20250823214325808" src="https://github.com/user-attachments/assets/47c99604-959f-4532-933c-f5123e2e238d" />

      위와 같이 Create  CSharp File 버튼이 생기며 이를 통해 해당 이름의 데이터 시트 클래스를 생성

    - 이름이 존재할 경우

      <img width="1568" height="889" alt="image-20250823215120936" src="https://github.com/user-attachments/assets/093de3af-d91d-41b1-b4ab-b862e39dccf3" />

      위와 같이 테이블을 로드 하여 수정할 수 있다.

    - 중첩 클래스 수정시 보조 창이 뜬다

      <img width="1831" height="611" alt="image-20250823215246169" src="https://github.com/user-attachments/assets/918cd316-2bc7-4489-9cc6-9bf618668e42" />


    





