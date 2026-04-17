#define CBT_MODE
//#define RELEASE_MODE

using UnityEngine;

// 아래 내용은 전처리기 학습용 메모
/*
    - 전처리기

    유니티에서는 전역 define 심볼을 다음 방법으로 추가할 수 있다.
    1) PlayerSettings => OtherSettings => Scripting Define Symbols
    2) 응답 파일(.rsp / .csc.rsp)에 define 심볼을 기록
    3) 스크립트 최상단에 #define 지시어로 선언(해당 파일에만 적용)

    예)
    -define:AAA
    -define:AAA;BBB

    (과거 버전)
    Api Compatibility Level이 .NET 2.0이면 gmcs.rsp
    .NET 2.0 subset이면 smcs.rsp
    (22 이전 버전)
    mcs.rsp로 통일
    (22 버전)
    csc.rsp로 통일

    주의) csc.rsp 파일을 수정하면 해당 define을 사용하는 스크립트를 다시 컴파일해야 한다.

    사용 예시

    #if UNITY_EDITOR
            유니티 에디터에서만 동작
    #endif

    #if UNITY_IOS
            iOS 빌드에서만 동작
    #endif

    #if UNITY_ANDROID
            Android 빌드에서만 동작
    #endif

    선택적 스크립트 실행 예시
    #if CBT_MODE
        int HP = 10;
    #elif RELEASE_MODE
        int HP = 100;
    #endif

    #if AAA
            Debug.Log("AAA");
    #endif

    #if BBB
            Debug.Log("BBB");
    #endif

    #if UNITY_5_3_2
            Debug.Log("CCC");
    #endif

    #if UNITY_2017_3_1
            //Debug.Log("DDD");
    #endif

    #if UNITY_EDITOR
            //Debug.Log("EDITOR");
    #endif

    #if UNITY_IPHONE
            Debug.Log("UNITY_IPHONE");
    #endif

    #if UNITY_ANDROID
            //Debug.Log("UNITY_ANDROID");
    #endif

    #if UNITY_IPHONE
            // DeviceGeneration은 안드로이드에서 지원하지 않는다.
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                switch (Device.generation)
                {
                    case DeviceGeneration.iPodTouch5Gen:
                        // 처리
                        break;
                    case DeviceGeneration.iPadMini1Gen:
                        // 처리
                        break;
                    case DeviceGeneration.iPodTouch3Gen:
                        // 처리
                        break;
                }
            }
            // 여기까지는 학습용 예제
    #endif
*/

public class DestructionRay : MonoBehaviour
{
    // 폭발 이펙트 프리팹
    public GameObject fireEffect;
    // Ray 정보
    Ray ray;
    // Ray 충돌 정보
    RaycastHit hitInfo;

    // 이 오브젝트를 선택했을 때만 기즈모를 표시한다.
    // 폭발 반경 같은 범위를 시각적으로 확인할 때 유용하다.

    // 씬 뷰에서 카메라 기준 특정 위치에 구체를 그린다.
    void OnDrawGizmosSelected()
    {
        // 스크린 좌표를 월드 좌표로 변환한다.
        Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(100, 100, Camera.main.nearClipPlane));
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(p, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 위치를 기준으로 카메라에서 Ray를 생성한다.
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if CBT_MODE // #if WSJ,MODE_1,MODE_2 로 바꿔보기
        // Scene 뷰에서만 시각적으로 보이는 디버그 Ray
        Debug.DrawRay(ray.origin, ray.direction * 150.0f, Color.green);
#elif RELEASE_MODE
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red);
#endif


#if UNITY_EDITOR
        // 마우스 왼쪽 버튼 클릭 시 Raycast
        if (Input.GetMouseButtonDown(0))
        {
            // 충돌한 오브젝트를 검사한다.
            if (Physics.Raycast(ray, out hitInfo, 150.0f))
            {
                // 지정한 태그의 오브젝트만 파괴한다.
                if (hitInfo.collider.tag == "DESTROYOBJECT")
                {
                    // 폭발 이펙트 생성
                    Instantiate(fireEffect, hitInfo.point, Quaternion.identity );
                    // 잠시 후 오브젝트 제거
                    Destroy(hitInfo.collider.gameObject, 3.5f);
                }
            }
        }
#endif

#if UNITY_ANDROID
        // 첫 번째 터치가 시작되었는지 확인
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // 터치 위치를 기준으로 Ray 생성
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);

            // 충돌한 오브젝트를 검사한다.
            if (Physics.Raycast(ray, out hitInfo, 150.0f))
            {
                // 지정한 태그의 오브젝트만 파괴한다.
                if (hitInfo.collider.tag == "DESTROYOBJECT")
                {
                    // 폭발 이펙트 생성
                    Instantiate(fireEffect, hitInfo.point, Quaternion.identity );
                    // 잠시 후 오브젝트 제거
                    Destroy(hitInfo.collider.gameObject, 3.5f);
                }
            }
        }
#endif

#if UNITY_IPHONE
        // 첫 번째 터치가 시작되었는지 확인
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // 터치 위치를 기준으로 Ray 생성
            ray = Camera.main.ScreenPointToRay( Input.touches[0].position );

            // 충돌한 오브젝트를 검사한다.
            if (Physics.Raycast(ray, out hitInfo, 150.0f))
            {
                // 지정한 태그의 오브젝트만 파괴한다.
                if (hitInfo.collider.tag == "DESTROYOBJECT")
                {
                    // 폭발 이펙트 생성
                    Instantiate(fireEffect, hitInfo.point, Quaternion.identity );
                    // 잠시 후 오브젝트 제거
                    Destroy(hitInfo.collider.gameObject, 3.5f);
                }

            }
        }
#endif     
    }
}

/*
    Touch 구조체는 화면을 누른 손가락의 상태와 정보를 담는다.

    position: 터치 좌표(픽셀 단위)
    fingerId: 손가락의 고유 인덱스
    deltaPosition: 이전 위치에서 얼마나 이동했는지
    deltaTime: 마지막 위치 변경 후 지난 시간
    tapCount: 탭 횟수
    phase: 터치 상태

    TouchPhase는 터치 상태를 나타내는 열거형이다.
    Began: 터치 시작
    Moved: 터치 후 이동 중
    Stationary: 터치 후 움직이지 않는 상태
    Canceled: 터치 취소
    Ended: 터치 종료

    Input.touches는 여러 손가락 터치를 처리하기 위해 Touch 배열을 반환한다.
    Input.touches[0]은 첫 번째 터치 정보다.
*/
