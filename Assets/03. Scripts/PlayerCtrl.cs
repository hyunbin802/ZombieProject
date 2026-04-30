using System.Collections;
using UnityEngine;
// 유니티 네비게이션 활용을 위한 선언
using UnityEngine.AI;

// (추가)

// 유니티 UI 사용을 위한 선언
using UnityEngine.UI;

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
    RaycastHit hitInfo2; // (추가)

    // public 멤버 인스펙터에 노출을 막는 어트리뷰트
    // 인스펙터에 노출은 막고 외부 노출은 원하는 경우 사용
    [HideInInspector]
    //죽었는지 상태변수 
    public bool isDie;

    // (추가)

    //적과의 거리를 위한 변수
    public float dist1;
    public float dist2;

    //Enemy를 찾기 위한 배열 
    private GameObject[] Enemys;
    private Transform EnemyTarget;

    //자신의 Transform 참조 변수  
    private Transform myTr;

    // 회전의 중심축
    public Transform targetTr;

    // 플레이어 발사 변수
    private bool isEnemyDetected;
    // 적을 봐라보는 회전 속도
    private float enemyLookTime;
    //적을 봐라보는 회전각
    private Quaternion enemyLookRotation;

    //총탄의 발사 시작 좌표 연결 변수 
    public Transform firePos;
    //총알 발사 주기
    private float bulletSpeed;
    //AudioSource 컴포넌트 저장할 레퍼런스 
    private AudioSource source = null;
    //MuzzleFlash GameObject를 연결 할 레퍼런스 
    public GameObject muzzleFlash;
    //총탄의 발사 사운드 
    public AudioClip fireSfx;

    //Ray 센서를 위한 변수
    bool LockOn;

    //레이저 발사를 위한 컴포넌트
    public LineRenderer rayLine;

    //레이저 도트 타겟을 위한 변수
    public Transform rayDot;

    //케릭터 센서 Idle 방향
    private bool turnRight;
    //케릭터 센서 각도
    private float turnValue;

    //플레이어 데미지
    public int power;
    // 이동중에 총 안쏜다(난이도)
    private bool fireAction;

    //애니메이터 연결
    Animator myAnim;

    //플레이어 라이프
    public int life;
    //플레이어 라이프 바
    public Image lifeBar;

    //데미지 효과 프리팹
    public GameObject damageEffect;
    //데미지 프로젝터 연결
    public Projector damageProjector;

    //드럼통 파괴
    [HideInInspector]
    public bool barrelFire;
    private Transform barrelPos;

    // 리지드바디 (연출)
    Rigidbody rbody;

    void Awake()
    {
        //레퍼런스 연결 

        //NavMeshAgent 컴포넌트를 해당 레퍼런스에 연결
        myTraceAgent = GetComponent<NavMeshAgent>();
        //nvAgent.isStopped = true; //네비게이션 멈춤 
        //nvAgent.velocity = Vector3.zero; //네비게이션 멈춤

        // 추가

        //자기 자신의 Transform 연결 (추가)
        myTr = GetComponent<Transform>();
        //AudioSource 컴포넌트를 해당 변수에 할당
        source = GetComponent<AudioSource>();
        // 애니메이터 연결 
        myAnim = GetComponentInChildren<Animator>();
        // 리지드 바디 연결
        rbody = GetComponent<Rigidbody>();
    }

    // 추가
    // Use this for initialization
    IEnumerator Start()
    {
        //처음에 MuzzleFlash 를 비활성화  
        muzzleFlash.SetActive(false);

        yield return new WaitForSeconds(5.0f);

        // 일정 간격으로 주변의 가장 가까운 Enemy를 찾는 코루틴 
        StartCoroutine(this.TargtSetting());

        // 가장 가까운 적을 찾아 발사...
        StartCoroutine(this.ShotSetting());
    }

    // Update is called once per frame
    void Update()
    {
        ////(추가)///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Debug.Log(myTraceAgent.velocity.z);
        //Debug.Log(GetComponent<Rigidbody>().velocity.z);

        //ray 정보 업데이트
        ray.origin = firePos.position;
        ray.direction = firePos.TransformDirection(Vector3.forward);

        //Scene 뷰에만 시각적으로 표현함
        //Debug.DrawRay(ray.origin, ray.direction * 13.0f, Color.green);

        //위에서 미리 생성한 ray를 인자로 전달, out(메서드 안에서 메서드 밖으로 데이타를 전달 할때 사용)hit, ray 거리
        if (Physics.Raycast(ray, out hitInfo1, 13.0f))
        {
            // hitInfo.point 는 월드좌표이다 따라서 로컬 좌표로 변환
            Vector3 posValue = firePos.InverseTransformPoint(hitInfo1.point);
            //타겟 거리체크 레이저 생성
            rayLine.SetPosition(0, posValue);
            //타겟에 레이저 Dot 생성 
            rayDot.localPosition = posValue;

            if (isEnemyDetected && (hitInfo1.collider.tag == "Enemy" || hitInfo1.collider.tag == "Barrel"))
            {
                //발사를 위한 변수 true
                LockOn = true;
            }
        }
        else
        {
            //난 달릴땐 레이저 빔 안보이게... 개인의 취향
            if (Mathf.Abs(myTraceAgent.velocity.z) > 0)
            {
                //Debug.Log("2coll");
                //기본 거리체크 레이저 생성
                rayLine.SetPosition(0, new Vector3(0, 0, 0));
                //타겟에 레이저 Dot 초기화 
                rayDot.localPosition = Vector3.zero;
            }
            else
            {
                //Debug.Log("2coll");
                //기본 거리체크 레이저 생성
                rayLine.SetPosition(0, new Vector3(0, 0, 13.0f));
                //타겟에 레이저 Dot 초기화 
                rayDot.localPosition = Vector3.zero;
            }
        }

        if (!isEnemyDetected)
        {
            if (fireAction)
            {
                //2.5초 마다 Idle 방향 전환 (코루틴이 더 효율)
                if (Time.time > turnValue)
                {
                    turnRight = !turnRight;
                    turnValue = Time.time + 2.5f;
                }
                if (turnRight)
                {
                    myTr.Rotate(Vector3.up * Time.deltaTime * 55.0f);
                }
                if (!turnRight)
                {
                    myTr.Rotate(Vector3.up * -Time.deltaTime * 55.0f);
                }
            }

            /*
                공부

                Debug.Log(myTr.localRotation.y); radian
                Debug.Log(myTr.localRotation.y*Mathf.Rad2Deg);// degree
                Debug.Log(myTr.rotation.eulerAngles.y);// degree
                Debug.Log(myTr.localEulerAngles.y );// degree

                degree (디그리) : 원 한바퀴 => 360도, 반원 => 180도, 직각 => 90도 (우리가 익숙한 각도)
                radian (라디안) : 1라디안은 원주 호의 길이가 반지름과 같은 길이가 될 때의 각도 (수학에서 편리)

                PI radians == 180
                1 radian   == 180/PI
                            == 57.2958...

                디그리->라디안 : PI/180를 곱한다.
                라디안->디그리 : 180/PI를 곱한다.

                ========================================================
                벡터 A와 B의 내적은 A·B 로 표시하며, 공식은 다음과 같다.

                A·B = |A| |B| cosθ

                A 내적 B 는 (벡터 A의 길이) * (벡터 B의 길이) * cosθ 이다.

                A 내적 B는 Vector3.Dot(A, B)
                벡터 A의 길이는 Vector3.Magnitude(A)
                벡터 B의 길이는 Vector3.Magnitude(B)
                cosθ 는 Mathf.Cos(θ) 
                로 구할 수 있다.

                내적 공식을 유니티 메서드를 이용하여 적어보면 다음과 같다.
                A·B = |A| |B| cosθ
                Vector3.Dot(A, B) = Vector3.Magnitude(A) * Vector3.Magnitude(B) * Mathf.Cos(θ);

                우변의 Cos만 남기고 반대편으로 넘기면 다음과 같이 된다.

                Mathf.Cos(θ) = Vector3.Dot(A, B) / Vector3.Magnitude(A) / Vector3.Magnitude(B);

                Cos를 제거하기 위해 Cos의 역 메서드인 Mathf.ACos() 을 사용하면 다음과 같이 된다.

                θ = Mathf.ACos( Vector3.Dot(A, B) / Vector3.Magnitude(A) / Vector3.Magnitude(B) );

                θ는 radian 값이므로 degree로 변환하기 위해 소스에서는 Mathf.Rad2Deg 를 사용.

                ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                두 벡터의 각도 구하기

                상대 벡터 : A, 기준 벡터 : B

                float Dot = Vector3.Dot(A,B);

                float Angle = Mathf.Acos(Dot);

                Acos을  했을 때 나오는 값은 Radian 값이므로 Mathf.Rad2Deg 를 곱해주면 degree 값을 얻을 수 있다.

                내적으로 각도를 구하기 때문에 나오는 각도는 0~180도 이다.
                ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
               */

            //transform.RotateAroundLocal(Vector3.up, Time.deltaTime * 55.0f);

            //발사를 위한 변수 false
            LockOn = false;
        }
        else
        {
            //적을 봐라봄  
            if (isEnemyDetected)
            {
                if (Time.time > enemyLookTime)
                {
                    Vector3 targetDir = EnemyTarget.position - myTr.position;
                    //외적????
                    //Vector3 tempDir = Vector3.Cross(myTr.forward, targetDir.normalized);
                    float dotValue = Vector3.Dot(myTr.forward, targetDir.normalized);

                    // 범위 지정안하면 NaN(Not a Number) 에러...
                    if (dotValue > 1.0f)
                    {
                        dotValue = 1.0f;
                    }
                    else if (dotValue < -1.0f)
                    {
                        dotValue = -1.0f;
                    }

                    //이거 해줘야 내적....(라디안 리턴)
                    float value = Mathf.Acos(dotValue);

                    //Debug.Log(value * Mathf.Rad2Deg);

                    // 디그리로 변경해서 각도 계산
                    if (value * Mathf.Rad2Deg > 35.0f)
                    {
                        //enemyLookRotation = Quaternion.LookRotation(-(EnemyTarget.forward)); // - 해줘야 바라봄  
                        enemyLookRotation = Quaternion.LookRotation(EnemyTarget.position - myTr.position); // - 해줘야 바라봄  
                        myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 11.0f);
                        enemyLookTime = Time.time + 0.01f;
                    }
                    else
                    {
                        //타겟 봐라봄
                        myTr.LookAt(EnemyTarget);
                    }
                }
            }
        }

        //만약 발사가 true 이면....
        if (isEnemyDetected && LockOn)
        {
            if (Time.time > bulletSpeed)
            {
                //일정 주기로 발사
                ShotStart();
                bulletSpeed = Time.time + 0.3f;
            }
        }

        //애니메이션 설정 
        myAnim.SetFloat("Speed", Mathf.Abs(myTraceAgent.velocity.z));
        if (isEnemyDetected && LockOn)
        {
            myAnim.SetBool("Shot", true);
        }
        else
        {
            myAnim.SetBool("Shot", false);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Main Camera 에서 마우스 커서(Vector3 타입이지만 Z값 무시한 값 (0~1280,0~800,0) )의 위치로 캐스팅되는 Ray를 생성함
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Scene 뷰에만 시각적으로 표현함
        Debug.DrawRay(ray.origin, ray.direction * 30.0f, Color.blue);

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
                //드럼통 타격 취소 (추가)
                barrelFire = false;
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
                //드럼통 타격 취소 (추가)
                barrelFire = false;
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
                //드럼통 타격 취소 (추가)
                barrelFire = false;
            }
        }
#endif       
    }

    ////(추가)///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /*
        position : 월드좌표(절대좌표)
        localPosition : 로컬좌표(상대좌표)

        만약 부모객체가 (1, 1, 0) 에 있다고 가정하고
        자식객체가 localPosition 이 (1, 1, 0) 이라면
        자식객체의 position 은 (2, 2, 0) 이 됩니다.
        position은 실제 절대 좌표를 말하고 localPosition은
        해당 부모객체기준의 좌표를 말합니다.
        만약 부모-자식 관계를 해제하게되면
        자식객체의 position, localPosition 모두 (2, 2, 0) 이 됩니다.
    */

    // 자신과 가장 가까운 적을 찾음
    IEnumerator TargtSetting()
    {
         while (!isDie)
         {
             yield return new WaitForSeconds(0.2f);

             // 자신과 가장 가까운 플레이어 찾음
             Enemys = GameObject.FindGameObjectsWithTag("Enemy");
             if (Enemys == null || Enemys.Length == 0)
             {
                 if (!barrelFire)
                 {
                     EnemyTarget = null;
                 }
                 continue;
             }

             Transform EnemyTargets = Enemys[0].transform;
             float dist = (EnemyTargets.position - myTr.position).sqrMagnitude;
             foreach (GameObject _Enemy in Enemys)
             {
                 if (_Enemy == null)
                 {
                     continue;
                 }

                  if ((_Enemy.transform.position - myTr.position).sqrMagnitude < dist)
                  {
                      EnemyTargets = _Enemy.transform;
                     dist = (EnemyTargets.position - myTr.position).sqrMagnitude;
                 }
             }
             //barrelFire 상태에선 발사 대상을 barrelPos 로 설정
             if(barrelFire)
             {
                 EnemyTarget = barrelPos;
             }
             else
             {
                 EnemyTarget = EnemyTargets;
             }
         }
    }

    // 가장 가까운 적을 찾아 발사...
    IEnumerator ShotSetting()
    {
         while (!isDie)
         {
             yield return new WaitForSeconds(0.2f);

             if (EnemyTarget == null)
             {
                 isEnemyDetected = false;
                 fireAction = false;
                 continue;
             }

              // 여기선 불 필요
              // dist1 = (EnemyTarget.position - myTr.position).sqrMagnitude;
             dist2 = Vector3.Distance(myTr.position, EnemyTarget.position);

             //네비게이션이 중지가 아니면 
             if(myTraceAgent.velocity.z != 0.0f)
             {
                 fireAction = false;
             }
             else
             {
                fireAction = true;
             }

             // 체크후 발사 지정... 코루틴으로 처리가 더 효율
             if(fireAction)
             {
                 if (dist2 < 13.0f)
                 {
                    isEnemyDetected = true;
                 }
                 else
                 {
                    isEnemyDetected = false;

                 }
             }
             else
             {
                isEnemyDetected = false;
             }
         }
    }

    //플레이어 발사
    private void ShotStart()
    {
        if (!isDie)
            //잠시 기다리는 로직처리를 위해 코루틴 함수로 호출
            StartCoroutine(this.FireStart());
    }

    IEnumerator FireStart()
    {       
        //총탄 사운드 발생 
        source.PlayOneShot(fireSfx, fireSfx.length + 0.2f);

        //MuzzleFlash 스케일을 불규칙하게 하자 
        float scale = Random.Range(1.0f, 1.3f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //MuzzleFlash를 Z축으로 불규칙하게 회전시키자 
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        //활성화 시킴
        muzzleFlash.SetActive(true);

        // Ray를 발사해 충돌된 게임오브젝트가 있을 때 true 리턴
        if (Physics.Raycast(firePos.position, firePos.forward, out hitInfo2, 30.0f))
        {
            //Debug.Log("shot");

            //Ray에 충돌된 게임오브젝트의 Tag 값으로 Enemy 인지 아닌지 체킹 
            if (hitInfo2.collider.tag == "Enemy")
            {
                //SendMessage로 전달할 모든 인자를 배열에 저장
                object[] _params = new object[2];
                //Enemy가 Ray에 맞은 정확한 월드 좌표 값
                _params[0] = hitInfo2.point;
                //Enemy에게 가할 플레이어 데미지
                _params[1] = power;

                //Enemy의 OnCollision 함수 호출 (_params 배열이름 전달)
                hitInfo2.collider.gameObject.SendMessage("OnCollision", _params
                                                    , SendMessageOptions.DontRequireReceiver);
            }
            //Ray에 충돌된 게임오브젝트의 Tag 값으로 Barrel 인지 아닌지 체킹 
            else if (hitInfo2.collider.tag == "Barrel")
            {
                //Debug.Log(123);
                //Barrel이 Ray에 충돌했을때의 입사각을 알기위하여 맞은 지점과 발사원점을 전달하여
                //정확한 위치에 타격을 줘서 확실한 물리효과 연출 
                object[] _params = new object[2];
                _params[0] = hitInfo2.point;
                _params[1] = firePos.position;
                //BarrelCtrl의 OnCollision 함수 호출 (_params 배열이름 전달)
                hitInfo2.collider.gameObject.SendMessage("OnCollision", _params
                                                    , SendMessageOptions.DontRequireReceiver);
            }
        }

        //랜덤 시간 동안 Delay한 다음 MeshRenderer를 비활성화
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        //비활성
        muzzleFlash.SetActive(false);
    }
    /*
        // 총탄 발사 코루틴 함수
        IEnumerator FireStart()
        {
            //Debug.Log("Fire");
            //Bullet 프리팹을 동적 생성
            //Instantiate(bullet, firePos.position, firePos.rotation);

            //총탄 사운드 발생 
            source.PlayOneShot(fireSfx, fireSfx.length - 0.2f);

            //MuzzleFlash 스케일을 불규칙하게 하자 
            float scale = Random.Range(1.0f, 2.5f);
            muzzleFlash.transform.localScale = Vector3.one * scale;

            //MuzzleFlash를 Z축으로 불규칙하게 회전시키자 
            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
            muzzleFlash.transform.localRotation = rot;

            //활성화 시킴
            muzzleFlash.SetActive(true);

            //랜덤 시간 동안 Delay한 다음 MeshRenderer를 비활성화
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

            //비활성
            muzzleFlash.SetActive(false);
        }
    */

    // 충돌시 파티클 생성 후 삭제
    void OnCollisionEnter(Collision coll)
    {   
        if (coll.gameObject.tag == "EnemyWeapon")
        {
            //총알이 맞은 위치를 위한 ContactPoint 구조체 선언 및 할당
            ContactPoint contact = coll.contacts[0];

            //데미지 효과 함수를 호출
            CreateDamage(contact.point);

            int damage = coll.gameObject.GetComponent<EnemyWeapon>().power;
            //적 무기에 당했을때 데미지를 받는데
            life -= damage;
            //라이프바를 증가 시켜서 위험 상태 표시
            lifeBar.fillAmount += damage / 100.0f;
            //프로젝터로 피 효과
            damageProjector.farClipPlane -= damage / 20.0f;

            // 생명력이 바닥이면 죽이자
            if (life <= 0)
            {
               StartCoroutine( PlayerDie() );
            }
        }
    }

    //데미지 이펙트 생성
    void CreateDamage(Vector3 pos)
    {
        //데미지 효과를 위한 코루틴 함수 호출 (생성과 소멸은 항상 코루틴으로...)
        StartCoroutine(this.CreateDamageEffect(pos));
    }

    IEnumerator CreateDamageEffect(Vector3 pos)
    {
        Instantiate(damageEffect, pos, Quaternion.identity);
        yield return null;
    }

    //플레이어 죽음
    IEnumerator PlayerDie()
    {
        //죽이고
        isDie = true;
        //총구 비활성
        firePos.gameObject.SetActive(false);
        //적이 추적 못하게 테그 바꾸고
        gameObject.tag = "Untagged";
        //네비게이션 비 활성화
        myTraceAgent.enabled = false;
        // 일단 공중으로 안 날라가게 잡고
        rbody.isKinematic = true;

        //모든 리지드 바디를 얼리고 (레그돌 리지드 바디만 활용)
        Rigidbody[] rigidbody = GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody _rbody in rigidbody)
        {
            _rbody.constraints = RigidbodyConstraints.None;
            //_rbody.constraints = RigidbodyConstraints.FreezeAll;
            //_rbody.constraints = RigidbodyConstraints.FreezePositionX;
            //_rbody.constraints = RigidbodyConstraints.FreezePositionY;
            _rbody.constraints = RigidbodyConstraints.FreezePositionZ;
        }

        //애니메이션 스톱으로 Ragdoll 효과 
        myAnim.enabled = false;

        yield return null;
    }

    // 드럼통 어택 로직
    public void BarrelFire(Transform barrelTr)
    {
        barrelPos = barrelTr;
        barrelFire = true;
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}

// https://docs.unity3d.com/kr/2022.3/ScriptReference/AI.NavMeshAgent.html
// https://docs.unity3d.com/kr/2022.3/ScriptReference/AI.NavMeshObstacle.html
// https://docs.unity3d.com/kr/2022.3/ScriptReference/AI.OffMeshLink.html

/*
    GameObject.SendMessage(string methodName, SendMessageOptions option); 
    이 함수는 특정 게임오브젝트의 함수를 호출하는 명령으로, 여러 게임오브젝트의 함수를 호출하는 로직에 
    사용되면 효율적이다. 
    두 번째 인자를 SendMessageOptions.DontRequireReceiver로 설정하면 해당 함수가 없더라도 함수가 없다는 메시지를 
    리턴받지 않겠다라는 옵션이다. 그러므로 빠른 실행을 위해서 반드시 이 옵션을 사용.
    또한 호출 함수의 접근제어자가 private 여도 호출가능하다.(내부접근)
*/

/* 
    SendMessage,SendMessageUpwards,BroadcastMessage
                                                                        
    SendMessage :원하는 GameObject에 붙어있는 한개 이상의 Script에 구현되어있는 메소드를 
    호출하는 방법. 이 방법을 사용하면 해당 GameObject에 실행하고자 하는 메소드를 
    가진 스크립트가 컴포넌트로 존재하는지 아닌지를 크게 신경쓰지 않고도 호출하는 것이 가능하다
    SendMessage 함수는 현재 스크립트가 실행중인 GameObject에 붙어있는 
    모든 MonoBehaviour 스크립트의 원하는 함수를 호출해 줍니다.

    원형)
    public void SendMessage(string methodName, object value = null,
    SendMessageOptions options = SendMessageOptions.RequireReceiver);

    ex1)
    SendMessage는 GameObject에 추가되어있는 모든 Script 컴포넌트들에 전달이 되게 된다.
    그중에 같은 이름을 가지고 있는 메소드가 있다면 그것이 실행되게 됨. 
    해당 이름의 메소드가 호출하는 스크립트 내부에 선언되어 있다면 그것이 최우선 순위로 실행이 되고 
    이후에는 컴포넌트에 등록되어있는 순서대로 메시지가 전달된다.

    public class ExampleClass1 : MonoBehaviour {
 
	    void Start () {
		    gameObject.SendMessage("ApplyDamage", 5.0f);
	    }
 
	    void ApplyDamage(float damage) {
		    Debug.Log ("ExampleClass1 Damage: " + damage);
	    }
    }
 
    public class ExampleClass2 : MonoBehaviour {
 
	    void Start () {
		
	    }
 
	    void ApplyDamage(float damage) {
		    Debug.Log ("ExampleClass2 Damage: " + damage);
	    }
    }
 
    //=> 
    ExampleClass1 Damage: 5
    ExampleClass2 Damage: 5

    ex2)
    SendMessage를 호출할때는 파라미터를 가지고 있더라도 수신하는 메소드는 파라미터를 받지 않음으로써 
    넘어올 파라미터를 무시할 수 있다. 다음의 코드는 문제 없이 ApplyDamage() 메소드가 호출된다.
    SendMessage는 .Net 리플렉션을 통하여 구현된다. 때문에 처음 찾게 되는 같은 이름을 가진 메소드를 
    실행하게 되며 만약 메소드 오버로딩이 되어있는 상태라면 정상적으로 동작하지 않게 된다.
    아래 코드는 Damage: Ignored 를 출력. 추가로 SetActive(false)와 같은 방법으로 비활성화 
    되어있는 GameObject는 메시지를 수신하지 않는다.

    void Start () {
	    gameObject.SendMessage("ApplyDamage", 5.0f);
    }
 
    void ApplyDamage() {
	    Debug.Log ("Damage: Ignored");
    }
 
    void ApplyDamage(float damage) {
	    Debug.Log ("Damage: " + damage);
    }

    //=> 
    Damage: Ignored

    SendMessageUpwards : 기본적으로 SendMessage 와 동일하게 동작하지만 자신(GameObject)를 포함하여 
    부모 GameObject까지 메시지를 전달한다
    기타 비활성화 되어있는 GameObject는 이벤트를 받을 수 없다거나 오버로딩에 정상적으로 대응하지 못하는 
    특성은 SendMessage와 동일.

    원형)
    public void SendMessageUpwards(string methodName, object value = null,
    SendMessageOptions options = SendMessageOptions.RequireReceiver);

    ex1) 두개의 GameObject를 부모 자식 형태로 배치를 했을경우...(RyanParent/RyanChild)

    public class RyanParent : MonoBehaviour {
 
	    void Start () {
	
	    }
 
	    void ApplyDamage(float damage) {
		    Debug.Log ("RyanParent Damage: " + damage);
	    }
    }
 
    public class RyanChild : MonoBehaviour {

	    void Start () {
		    gameObject.SendMessageUpwards ("ApplyDamage", 5.0f);
	    }
 
	    void ApplyDamage(float damage) {
		    Debug.Log ("RyanChild Damage: " + damage);
	    }
    }

    //=> 
    RyanChild Damage: 5
    RyanParent Damage: 5

    이 코드의 실행 결과는 RyanChild로 시작하여 부모인 RyanParent까지 전달이 된다.

    BroadcastMessage :SendMessageUpwards와 반대로 동작하는 메소드. 
    BroadcastMessage를 통해 메소드를 호출하게 되면 자기 자신의 GameObject를 포함하여 
    그의 모든 자식 객체들에게 메시지가 전달된다.

    (원형)
    public void BroadcastMessage(string methodName, object parameter = null,
    SendMessageOptions options = SendMessageOptions.RequireReceiver);

    ex) 위와 반대로 부모 GameObject에서 BroadcastMessage를 실행

    public class RyanParent : MonoBehaviour {
 
	    void Start () {
		    gameObject.BroadcastMessage ("ApplyDamage", 5.0f);
	    }
 
	    void ApplyDamage(float damage) {
		    Debug.Log ("RyanParent Damage: " + damage);
	    }
    }
 
    public class RyanChild : MonoBehaviour {
 
	    void Start () {
		
	    }
 
	    void ApplyDamage(float damage) {
		    Debug.Log ("RyanChild Damage: " + damage);
	    }
    }

    //=> 
    RyanParent Damage: 5
    RyanChild Damage: 5

    부모의ApplyDamage가 호출되고 그 다음에 자식의 ApplyDamage가 호출된다.

    SendMessageOptions :위에서 설명한 모든 메소드들의 3번째 인자로 SendMessageOptions 가 있습니다. 
    enum 타입이며 다음과 같은 두가지 타입을 선택할 수 있습니다.

    • RequireReceiver : SendMessage에 대응할 수 있는 수신자가 반드시 있어야 한다.
    • DontRequireReceiver : SendMessage에 대응할 수 있는 수신자가 없어도 괜찮음.

    3번째 파라미터를 지정하지 않아도 기본 값은 RequireReceiver이며 이는 한번 메시지 호출이 발생하면 
    누군가는 그것을 받아서 처리를 해주어야 한다는것을 의미. 만약에 대응되는 메소드가 존재하지 않는다면
    오류가 발생.
    하지만 DontRequireReceiver 는 아무도 처리하지 않아도 문제가 되지 않는다. 
    Optional한 처리를 하는 경우 쓸만한 옵션이다.
 */
