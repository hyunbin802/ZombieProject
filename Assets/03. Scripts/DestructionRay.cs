#define CBT_MODE
//#define RELEASE_MODE

using UnityEngine;

//이 부분은 단순 공부용/////////////////////////////////////////////////////////////////
/*
     -전 처리기

    유니티에선 #define 지시자가 없다 따라서 두 가지 방법으로 추가해야한다.
    1) PlayerSettings => OtherSettings => Scripting Define Symbols
    2) 매번 생성을 반복하면 힘드니깐 파일로 보관하는 경우 및에 참조...
    3) 각 스크립트 최 상위에 #define 지시어로 선언(해당 컴포넌트만 참조)

    #define AAA 를 전역으로 사용하고 싶다면 메모장에 -define:AAA 를 적고
    #define AAA 와 #define BBB 를 모두 사용하고 싶다면 -define:AAA;BBB 와 같은 형식으로 
    사용.(유니티 에디터에서도 사용법은 동일) 
    모두 작성했으면 유니티 프로젝트에 Assets 폴더에 

    (과거 버전)
    Api Compatibility Level이  .net 2.0 일 경우엔 gmcs.rps 
    .net 2.0 subset 일 경우엔 smcs.rps 로 파일저장
    (22 이전 버전)
    mcs.rps로 통일
    (22 버전)
    csc.rsp로 통일

    주의) csc.rsp 파일을 수정시엔 해당 define 을 사용하는 스크립트를 재 컴파일 해줘야 됨..

    // 사용 방법

    #if UNITY_EDITOR
            유니티 에디터 상태에서만 동작하는 스크립트 로직
    #endif

    #if UNITY_IOS
            빌드 타켓이 아이폰일 때 동작하는 스크립트 로직 
    #endif

    #if UNITY_ANDROID
            빌드 타켓이 안드로이드일 때 동작하는 스크립트 로직 
    #endif

    #지시어로 다음과 같이 선택적 스크립트 실행
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
            // DeviceGeneration는 안드로이드는 지원안하고 안드로이드는 필요도 없다.
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                switch (Device.generation)
                {
                    case DeviceGeneration.iPodTouch5Gen:
                        //처리
                        break;
                    case DeviceGeneration.iPadMini1Gen:
                        //처리
                        break;
                    case DeviceGeneration.iPodTouch3Gen:
                        //처리
                        break;
                }
            }
            //여기까지 공부용이니 차후 삭제////////////////////////////////////////////////////////////////////
    #endif
*/

public class DestructionRay : MonoBehaviour
{
    // 폭발 이펙트
    public GameObject fireEffect;
    //Ray 정보 저장 구조체 
    Ray ray;
    // Ray에 맞은 오브젝트 정보를 저장 할 구조체
    RaycastHit hitInfo;

    // OnDrawGizmosSelected() 로 만든 기즈모는 이 컴포넌트를 가지고 있는 
    // 게임오브젝트를 클릭 시 표현되며 기즈모 클릭이 불가능.
    // 유용한 부분은 폭발 스크립트 작성시 폭발 반경을 보여주는 영역을 그릴 수 있다. 

    // 씬뷰에서 좌 하단으로 부터 100픽셀 떨어진... 
    // 선택된 카메라의 가까운면의 위치에 
    // 노란색 구체를 그린다.
    void OnDrawGizmosSelected()
    {
        //position을 화면 공간(스크린)에서 월드 공간으로 반환해준다.
        //화면 공간은 픽셀로 정의 되며, 화면의 좌하단은(0,0)이고 
        //우상단은(pixelWidth, pixelHeight)이다.
        //z position은 선택 카메라로부터의 거리를 월드 단위로 환산한 값 이다.

        Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(100, 100, Camera.main.nearClipPlane));
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(p, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        //Main Camera 에서 마우스 커서(Vector3 타입이지만 Z값 무시한 값 (0~1280,0~800,0) )의 위치로 캐스팅되는 Ray를 생성함
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if CBT_MODE // #if WSJ,MODE_1,MODE_2 로 바꿔보기
        //Scene 뷰에만 시각적으로 표현함
        Debug.DrawRay(ray.origin, ray.direction * 150.0f, Color.green);
#elif RELEASE_MODE
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red);
#endif


#if UNITY_EDITOR
        //마우스 왼쪽 버튼을 클릭시 Ray를 캐스팅
        if (Input.GetMouseButtonDown(0))
        {
            //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리
            if (Physics.Raycast(ray, out hitInfo, 150.0f))
            {
                //ray에 hit된 객체의 태그를 비교함 
                if (hitInfo.collider.tag == "DESTROYOBJECT")
                {
                    //파티클 생성 
                    //.............
                    Instantiate(fireEffect, hitInfo.point, Quaternion.identity );
                    //오브젝트 제거
                    Destroy(hitInfo.collider.gameObject, 3.5f);
                }
                //드럼통에 Hit 되면 MovePlayerCtrlRay에 BarrelFire 함수호출
                else if (hitInfo.collider.tag == "Barrel")
                {
                    GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>().BarrelFire(hitInfo.collider.gameObject.transform);
                }
            }
        }
#endif

#if UNITY_ANDROID
        //스크린에 터치가 이루어진 상태에서 첫 번째 손가락 터치가 시작됐는지 비교
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //Main Camera에서 손가락을 터치한  벡터 위치로 캐스팅하는 Ray를 생성 함
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);

            //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리
            if (Physics.Raycast(ray, out hitInfo, 150.0f))
            {
                //ray에 hit된 객체의 태그를 비교함 
                if (hitInfo.collider.tag == "DESTROYOBJECT")
                {
                    //파티클 생성 
                    //.............
                    Instantiate(fireEffect, hitInfo.point, Quaternion.identity );
                    //오브젝트 제거
                    Destroy(hitInfo.collider.gameObject, 3.5f);
                }
                //드럼통에 Hit 되면 MovePlayerCtrlRay에 BarrelFire 함수호출
                else if(hitInfo.collider.tag == "Barrel")
                {
                    GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>().BarrelFire(hitInfo.collider.gameObject.transform);
                }
            }
        }
#endif

#if UNITY_IPHONE
        //스크린에 터치가 이루어진 상태에서 첫 번째 손가락 터치가 시작됐는지 비교
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //Main Camera에서 손가락을 터치한  벡터 위치로 캐스팅하는 Ray를 생성 함
            ray = Camera.main.ScreenPointToRay( Input.touches[0].position );

            //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리
            if (Physics.Raycast(ray, out hitInfo, 150.0f))
            {
                //ray에 hit된 객체의 태그를 비교함 
                if (hitInfo.collider.tag == "DESTROYOBJECT")
                {
                    //파티클 생성 
                    //.............
                    Instantiate(fireEffect, hitInfo.point, Quaternion.identity );
                    //오브젝트 제거
                    Destroy(hitInfo.collider.gameObject, 3.5f);
                }
                //드럼통에 Hit 되면 MovePlayerCtrlRay에 BarrelFire 함수호출
                else if(hitInfo.collider.tag == "Barrel")
                {
                    GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>().BarrelFire(hitInfo.collider.gameObject.transform);
                }
            }
        }
#endif     
    }
}

/*
    Touch 구조체는 스크린상에 터치한 손가락의 모든 상태와 정보를 담는다.

    position 터치된 좌표 (픽셀 단위)
    fingerId   손가락의 고유한 인덱스(터치별로 고유한 값이 순서대로 설정된다.)
    deltaPosition 마지막의 위치로부터 총 움직인 위치 변위 값 
    deltaTime  마지막 위치 변경 이후부터 총 경과된 시간
    tapCount  탭의 수 (더블클릭 등...)
    phase   터치의 유형(터치의 시작, 터치의 종료, 터치의 취소, 터치의 오래 누르기)

    = TouchPhase 는 터치의 유형을 정의한 열거형 타입 =
    Began  스크린에 터치를 시작
    Moved  스크린에 터치를 한 후 이동하는 상태
    Stationary  터치를 한 후에 이동하지 않고 계속 터치중인 상태 
    Canceled 터치가 취소됐을 경우 
    Ended 터치를 종료했을 경우 

    Input.touches는 한 번에 여러 개의 손가락 터치를 처리하기 위해서 Touch 구조체 배열을 반환한다. 
    (Input.touches[0] 는 첫 번째 터치 정보)
*/