using UnityEngine;
// 유니티 네비게이션 활용을 위한 선언
using UnityEngine.AI;

public class PlayerCtrl : MonoBehaviour
{
    //NavMeshAgent 컴포넌트 할당 레퍼런스 
    private NavMeshAgent myTraceAgent;

    //케릭이 이동할 목적지 좌표
    Vector3 movePoint = Vector3.zero;

    //Ray 정보 저장 구조체 
    Ray ray;

    // Ray에 맞은 오브젝트 정보를 저장 할 구조체
    RaycastHit hitInfo1;

    // public 멤버 인스펙터에 노출을 막는 어트리뷰트
    // 인스펙터에 노출은 막고 외부 노출은 원하는 경우 사용
    [HideInInspector]
    //죽었는지 상태변수 
    public bool isDie;

    void Awake()
    {
        //NavMeshAgent 컴포넌트를 해당 레퍼런스에 연결
        myTraceAgent = GetComponent<NavMeshAgent>();
        //nvAgent.isStopped = true; //네비게이션 멈춤 
        //nvAgent.velocity = Vector3.zero; //네비게이션 멈춤        
    }

    // Update is called once per frame
    void Update()
    {
        //Main Camera 에서 마우스 커서(Vector3 타입이지만 Z값 무시한 값 (0~1280,0~800,0) )의 위치로 캐스팅되는 Ray를 생성함
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Scene 뷰에만 시각적으로 표현함
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue);

#if UNITY_EDITOR
        //마우스 왼쪽 버튼을 클릭시 Ray를 캐스팅  
        if (Input.GetMouseButtonDown(0) && !isDie)
        {
            //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리, 레이어 마스크 값(레이어가 Barrel 일때만 충돌)
            // Mathf.Infinity 이 값은 무한한 값이라고 생각하면 된다. 따라서 거리가 무한~~~
            if (Physics.Raycast(ray, out hitInfo1, Mathf.Infinity, 1 << LayerMask.NameToLayer("Barrel")))
            {
                //ray에 맞은 위치를 이동할 목표지점으로 설정
                movePoint = hitInfo1.point;

                //NavMeshAgent 컴포넌트의 목적지 설정
                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 7.7f;
            }
            //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리, 레이어 마스크 값(레이어가 Ground 일때만 충돌)
            // Mathf.Infinity 이 값은 무한한 값이라고 생각하면 된다. 따라서 거리가 무한~~~
            else if (Physics.Raycast(ray, out hitInfo1, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
            {
                //ray에 맞은 위치를 이동할 목표지점으로 설정
                movePoint = hitInfo1.point;

                //NavMeshAgent 컴포넌트의 목적지 설정
                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 0.0f;
            }
        }
#endif

#if UNITY_STANDALONE_WIN
        //마우스 왼쪽 버튼을 클릭시 Ray를 캐스팅  
        if (Input.GetMouseButtonDown(0) && !isDie)
        {
            //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리, 레이어 마스크 값(레이어가 Barrel 일때만 충돌)
            // Mathf.Infinity 이 값은 무한한 값이라고 생각하면 된다. 따라서 거리가 무한~~~
            if (Physics.Raycast(ray, out hitInfo1, Mathf.Infinity, 1 << LayerMask.NameToLayer("Barrel")))
            {
                //ray에 맞은 위치를 이동할 목표지점으로 설정
                movePoint = hitInfo1.point;

                //NavMeshAgent 컴포넌트의 목적지 설정
                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 7.7f;
            }
            //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리, 레이어 마스크 값(레이어가 Ground 일때만 충돌)
            // Mathf.Infinity 이 값은 무한한 값이라고 생각하면 된다. 따라서 거리가 무한~~~
            else if (Physics.Raycast(ray, out hitInfo1, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
            {
                //ray에 맞은 위치를 이동할 목표지점으로 설정
                movePoint = hitInfo1.point;

                //NavMeshAgent 컴포넌트의 목적지 설정
                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 0.0f;
            }
        }
#endif

#if UNITY_ANDROID
        //스크린에 터치가 이루어진 상태에서 첫 번째 손가락 터치가 시작됐는지 비교
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !isDie)
        {
            //Main Camera에서 손가락 터치 위치로 캐스팅되는 Ray를 생성 함
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);

            //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리, 레이어 마스크 값(레이어가 Barrel 일때만 충돌)
            // Mathf.Infinity 이 값은 무한한 값이라고 생각하면 된다. 따라서 거리가 무한~~~
            if (Physics.Raycast(ray, out hitInfo1, Mathf.Infinity, 1 << LayerMask.NameToLayer("Barrel")))
            {
                //ray에 맞은 위치를 이동할 목표지점으로 설정
                movePoint = hitInfo1.point;

                //NavMeshAgent 컴포넌트의 목적지 설정
                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 7.7f;
            }
            //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리, 레이어 마스크 값(레이어가 Ground 일때만 충돌)
            // Mathf.Infinity 이 값은 무한한 값이라고 생각하면 된다. 따라서 거리가 무한~~~
            else if (Physics.Raycast(ray, out hitInfo1, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
            {
                //ray에 맞은 위치를 이동할 목표지점으로 설정
                movePoint = hitInfo1.point;

                //NavMeshAgent 컴포넌트의 목적지 설정
                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 0.0f;
            }
        }
#endif       
    }
}

// https://docs.unity3d.com/kr/2022.3/ScriptReference/AI.NavMeshAgent.html
// https://docs.unity3d.com/kr/2022.3/ScriptReference/AI.NavMeshObstacle.html
// https://docs.unity3d.com/kr/2022.3/ScriptReference/AI.OffMeshLink.html
