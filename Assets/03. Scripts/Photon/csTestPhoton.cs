using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csTestPhoton : MonoBehaviour
{
    //앱의 버전 정보 (번들 버전과 일치시키는게 좋음)
    public string version="Ver 0.1.0";
    // unity 콘솔에 로그찍어줌 Full은 전부,익숙해지면 Informational으로 변경
    public PhotonLogLevel LogLevel =PhotonLogLevel.Full;
    //Player의 생성 위치 저장 레퍼런스
    public Transform playerPos;

    // App 인증 및 로비연결
    void Start()
    {
        /*
            포톤 클라우드
         
            포톤 클라우드는 로비/룸의 개념이 있다. 포톤 서버에 접속에 성공하면 로비에 자동 입장 된다.
            로비에 입장한 다음 룸을 생성하면 다수의 유저들이 해당 룸에 입장하여 네트워크 게임을 진행하는 방식
            이며, 처음 룸을 생성한 유저가 Master Client가 된다.=> 우리가 알고있는 방장에 해당하는 역할(몬스터 스폰)과 권한을 갖는다.(부여된다..)
            유니티 빌트인 네트워크 API를 사용하여 만든 네트워크 온라인 게임은 실행중 Server Peer(방장)의 접속이 끊기면 
            게임 자체가 성립이 안된다. 그러나 포톤 클라우드의 경우 Master Client가 룸에서 나가면 다음으로 입장한 
            다른 네트워크 유저가 Master Client를 인계받는다. 따라서 유니티 빌트인 네트워크 API를 이용한 네트워크 게임 처럼
            게임이 도중에 없어져서 중단되지 않는다.
         */
        // 룸 -> 로비로 씬 전환시 반복해서 다시 포톤 클라우드 서버로 접속하는 로직이 다시 실행되는것을 막음
        if (!PhotonNetwork.connected)
        {
            //버전 정보를 전달하며 포톤 클라우드에 접속 ( 과정: 지역 서버 접속 -> 사용자 인증 -> 로비 입장 )
            //포톤서버 접속 시도 => 인증 => 포톤서버 접속 => 포톤 마스터서버 접속 시도 => 인증 => 포톤 마스터서버 접속 및 Lobby 조인
            //인자로 전달하는 version은 동일 버전의 게임이 설치된 유저만 같은 로비에 접속할 수 있다(1.0 버전의 게임이 설치된
            //유저와 2.0 버전의 게임이 설치된 유저는 네트워크상에서 만날 수 없다 . 따라서 서로 다른 버전의 게임으로 인한 
            //error 및 오동작을 일으실 수 있는것을 원천적으로 막는다.) 
            //EX) 1.0 버전 총 Damage 100  2.0 버전 총 Damage 200)
            PhotonNetwork.ConnectUsingSettings(version);

            //개발하는 동안 PUN 으로 개발하는 것이 처음이면 최대한 Unity 콘솔에 로그를 많이 찍어 어떤 
            //사항이 발생하는지 파악하는 것을 권장. 
            //예상하는 대로 동작하는 것에 대하여 확신이 서면 로그 레벨을 Informational 으로 변경 하자.
            PhotonNetwork.logLevel = LogLevel;


            //현재 클라이언트 유저의 이름을 포톤에 설정
            //PhotonView 컴포넌트의 요소 Owner의 값이 된다.
            PhotonNetwork.playerName = "GUEST " + Random.Range(1, 9999);
        }

        // 현재 접속 지역을 기준으로 핑 타임이 가장 빠른 클라우드 서버에 자동으로 접속 (RTT (Round Trip Time) )
        // PhotonNetwork.ConnectToBestCloudServer( version );

        // 특정 클라우드 서버에 직접 접속 하는 함수로, 인자는 포톤 클라우드 서버 IP 주소, Port 번호, AppID, 버전
        // PhotonNetwork.ConnectToMaster( "string serverAddress", 3306, "asdafasdda01091207", version  );
    }

       void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby !!!");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("No Rooms !!!");
        bool isSucces = PhotonNetwork.CreateRoom("MyRoom");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Enter Room");
    }
}

/*
    포톤 클라우드 콜백 함수

    1 서버 접속

    Method (PhotonNetwork)
    PhotonNetwork.ConnectUsingSettings(string version);

    Event :서버접속후 발생하는 이벤트
    void OnConnectedToPhoton(){ ~이벤트처리 로직~ }=> Event들을 이렇게 사용

    - OnConnectedToPhoton : 포톤에 접속되었을 때
    - OnLeftRoom : 방에서 나갔을 때
    - OnMasterClientSwitched : 마스터클라이언트가 바뀌었을 때
    - OnPhotonCreateRoomFailed : 방만들기 실패
    - OnPhotonJoinRoomFailed : 방에 들어가기 실패
    - OnCreatedRoom : 방이 만들어 졌을 때
    - OnJoinedLobby : 로비에 접속했을 때
    - OnLeftLobby : 로비에서 나갔을 때
    - OnDisconnectedFromPhoton : 포톤 접속 종료
    - OnConnectionFail : 연결실패 void OnConnectionFail(DisconnectCause cause){ ... }
    - OnFailedToConnectToPhoton : 포톤에 연결 실패 시  void OnFailedToConnectToPhoton(DisconnectCause cause){... }
    - OnReceivedRoomList : 방목록 수신시
    - OnReceivedRoomListUpdate : 방목록 업데이트 수신시
    - OnJoinedRoom : 방에 들어갔을 때
    - OnPhotonPlayerConnected : 다른 플레이어가 방에 접속했을 때 void OnPhotonPlayerConnected(PhotonPlayer newPlayer){ ... }
    - OnPhotonPlayerDisconnected : 다른 플레이어가 방에서 접속 종료시  void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer){ ... }
    - OnPhotonRandomJoinFailed : 렌덤하게 방으로 입장하는게 실패했을 때
    - OnConnectedToMaster : 마스터로 접속했을 때   "Master_Join"
    - OnPhotonSerializeView : 네트워크싱크시  void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){ ...}
    - OnPhotonInstantiate : 네트워크 오브젝트 생성시 void OnPhotonInstantiate(PhotonMessageInfo info){ ... } "NetworkObj"


    2 로비관련 :로비 접속에 성공하면, 이제부터는 방목록을 얻어 온다던가... 아니면 방을 만든다던가... 이것도 아니라면 아무 방에 들어가기 위한 노력

    Method
    RoomInfo [] PhotonNetwork.GetRoomList ( ) //방목록을 가져옴
    void PhotonNetwork.CreateRoom ( string roomName ) //방을 이름으로만 만듬
    void PhotonNetwork.CreateRoom ( string roomName, bool isVisible, bool isOpen, int maxPlayers ) //방을 만들되 이름, 보임여부, 오픈여부, 최대 플레이어 수를 지정함.
    void PhotonNetwork.JoinRandomRoom ( ) //아무방이나 들어감.
    void PhotonNetwork.JoinRoom ( string roomName ) //지정한 방으로 들어감.
    [RPC]도 호출이 가능하니 로비에서 채팅 가능


    3 방에서

    Method
    //resources폴더에 있는 prefab의 이름으로 게임오브젝트를 생성.
    GameObject PhotonNetwork.Instantiate ( string prefabName, Vector3 position, Quaternion rotation, int group)
    //씬에 종속된 게임오브젝트를 생성.(마스터만이 네트워크 게임오브젝트를 생성가능함)
    GameObject PhotonNetwork.InstantiateSceneObject ( string prefabName, Vector3 position, Quaternion rotation, int group, object[] data )

    위 2개의 Method 차이는 플레이어에 종속된 GameObject를 만들것인가, 플레이어의 접속이 끝나도 계속 살아남을 Object를 만들것인가...

    Event :방에서 PhotonView로 설정한 객체는 서로 데이터를 Sync할 수 있다. 이때 발생하는 이벤트는 딱 1개라고 생각해도 됨
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){ ...}


    네트워크상에서의 동기화는 다음과 같이 수행됨.: 게임오브젝트에 반드시 PhotonView(Component)가 추가 되야한다.

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }

    위 코드에서 사용가능한 데이터형
    - Bool
    - Int
    - string
    - char
    - short
    - float
    - PhotonPlayer
    - Vector3
    - Vector2
    - Quaternion
    - PhotonViewID
*/

/*
 * = 네트워크 기초 =
 *                 
 * 1) 네트워크 게임??
 * 물리적/공간적 떨어져 있는 다른 유저와 통신망(LAN, 인터넷 망)을 통해서 서로 데이타를 
 * 주고받고 게임하는 것.
 * 
 * 2)네트워크 게임의 물리적 구조 (네트워크 구조를 그림으로 그릴줄 알아야함...기본!!!)
 * 
 * ■ P2P(Peer to Peer) 방식 => 서버/클라이언트 모델에 대응되는 모델
 * 
 *  => 유저끼리 별도의 서버 없이 네트워크 연결하여 데이타를 송수신하는 구조를 말함
 *     접속자가 적은 게임에 적용되며, 네트워크 상 유저(사용자)가 직접 접속해서 게임을 함.
 * 
 *    ○  응용 중심에 따른 분류(사용 용도에 따라 구분)
 *
 *       정보 공유형 응용
 *
 *          - 파일이나 데이터 등을 공유하거 메시지의 교환을 통하여 정보를 공유 (메신저)
 *
 *          - mp3 음악 파일을 공유하는 넵스터, 그누텔라, 인스턴트 메시지를 교환하는 ICQ 등 (파일 공유 프로그램)
 *
 *       자원 공유형 응용
 *
 *          - 하나의 커다란 처리를 세분하여 분산 클라이언트가 처리한 후 최종적으로 중심이 되는
 *
 *             서버에게 처리 결과를 전송하여 결합하는 시스템
 *
 *          - 각 컴퓨터의 처리능력(CPU)을 하나의 컴퓨팅 자원으로 취급하여 여러 컴퓨터가
 *
 *             필요에 따라 공유
 *             
 * ■ 서버/클라이언트 모델(방식) => 우리가 할거...(거의 대부분의 온라인 게임 방식)
 * 
 *  => 게임 서버를 구축하고 여러 유저(클라이언트)가 접속해서 상호간의 데이터를 
 *     게임 서버를 이용해 송/수신하는 방식으로 게임 서버의 기능적 역할은 접속한 클라이언트 사이의 데이타를 Relay하고
 *     게임 데이터를 DB 서버에 저장한다. 
 *     
 *     (참고) 유니티 빌트인 네트워크(Built in Network))로 이러한 서버/클라이언트 네트워크
 *     게임을 깊이 있고 복잡한 네트워크 지식없이 손쉽게 개발할 수 있다. 하지만 우린 포톤을 이용한 네트워크
 *     게임을 개발할것이다...그러나 포톤 하나만 알면 유니티 빌트인 네트워크는 껌이다...이유는
 *     포톤 개발사는 Built in Network를 맞춰서 (98% 이상(내 생각..) ) 용어, 사용법, 
 *     주요 기능(Network View, State Synchronization, RPC: Remote Procedure Call) 등을 제공한다
 *     (즉 포톤 클라우드는 유니티 빌트인 네트워크 API의 부족한 기능을 보완하고 네트워크 게임에 필요한 필수적인 기능을 확장했기 때문에 API 사용법이 거의 동일)
 *     따라서 포톤만 완벽히 알면 Built in Network 뿐만 아니라 유니티를 지원하는 검증된 여러 네트워크 게임 엔진(서드파티)을 
 *     살만 약간 붙혀서 손쉽게 사용 가능~!
 *     
 *     또한, 온라인 네트워크 게임 개발을 위해선 물리적 서버(돈) + 네트워크 게임 서버(기술력)가 구축되야 한다.
 *     네트워크 게임을 직접 구현하는것은 숙련된 네트워크 개발 경험 및 스킬을 갖추어야 한다.
 *     그리고 수많은 알파/베타 테스트등을 거쳐 네트워크 속도/안정성/최적화등의 작업이 이루어져야 한다.
 *     소규모/인디개발자에겐 현실적으로 어렵다...따라서 검증된 네트워크 게임 엔진을 사용하자.
 *     우린 여기서 서드파티(유니티를 베이스로...) 네트워크 게임 엔진인 포톤을 활용!~
 *     
 *     유니티를 지원하는 서드파티 네트워크 게임 엔진(서버) (가격/성능/서버의 운영체제 등을 고려하여 네트워크 게임 서버 선택!!!)
 *     Photon           http://www.exitgames.com (유니티에서 가장 오래 사랑받고 검증된 엔진)
 *     프라우드넷       http://www.nettention.com
 *     ElectroServer    http://www.electrotank.com
 *     SmartFox         http://www.smartfoxserver.com
 *     
 *     (추가) 유니티에선 Unity 5.3이상 버전부터 대규모 네트워크 게임 개발이 가능한 UNET을 제공한다.
 *     Built in Network API를 대체하는 API로서 MMO등의 대규모 네트워크 게임 개발을 위한 필요한 기능을
 *     쉽고 편리하게 구현해 놓은 네트워크 게임 엔진이다. 아직 안정화 단계지만 우리기 포톤만 열심히 한다면
 *     문제없이 사용가능하다(구글링등..)
 *     
 * 3) 네트워크 통신 프로토콜
 * => 프로토콜이란 네트워크 상에서 데이터를 통신하기 위한 규약(약속)으로 게임에선
 *    TCP/IP(정확성 위주), UDP(속도 위주) 프로토콜 사용
 * 
 * ■ Packet(패킷) 
 * => 프로토콜에 따라 정해진 송/수신하는 데이터의 단위
 *     
 * ■ TCP/IP 프로토콜 
 * => 정합성을 위해 탄생한 통신 규약으로 데이터의 유실이 없다. 즉 100개의 데이터를 보내면
 *    상대편에서 100개를 다 받을 수 있다.
 *    Packet에 보내는 Data 순서를 정확하게 지켜 전송하며 만약에 송신중 Packet이 유실됐을 경우
 *    수신측은 전송된 패킷의 유실된 부분을 알고 재전송을 송신측에 요구한다. 따라서 송신측은
 *    다시 패킷을 재전송한다. 그러므로 속도가 UDP 프로토콜에 보다 느리다.(전송/대기/응답/재전송 등의 이유로...)
 *    ( 카드 게임처럼 속도감 보단 정합성의 비중이 큰 게임에서는 주로 TCP/IP 프로토콜 사용 )
 *    
 * ■ UDP 프로토콜
 * => TCP/IP와 다르게 수신측이 받을 준비가 돼 있는지 확인하는 절차 없이 무조건 패킷을 보내는 프로토콜로서
 *    패킷의 순서를 보증하지 않으며 네트워크 상황에 따라 전송도중 중간에 Packet이 유실 되어도 재전송 절차가 없다
 *    그러므로 정합성은 무시되나 매우 빠른 전송 속도를 보장. 따라서 송신 중간에 패킷이 하나씩 유실돼도 크게 상관없는
 *    환경에 주로 사용된다.( FPS,액션,대결 게임처럼 정합성 보단 액션의 비중이 크고 속도감 있는 게임에서는 주로 UDP 프로토콜 사용)
 *    
 *    
 * 
 */

/*
    포톤 네트워크 게임 엔진

    1) 제품 종류는 크게 Photon Server/ Photon PUN 으로 나뉨 (참고 : Photon Cloude=>Photon PUN으로 이름이 바뀜)

    ■ Photon Server : 물리적인 서버를 직접 운영하는 것으로서 운영하려면 서버의 보안 및 백업/네트워크 트래픽/로드밸런싱/유지보수 등의
    관리를 위한 전문 네트워크 관련 인력이 필요함
    (제품 종류 <Photon Server 계열> : Photon Server)

    ■ Photon PUN    : 우리가 사용할 방식으로 소프트웨어를 임대해 사용하는 방식인 SaaS(Software as a Service)의 개념으로
    서버를 임대하여 사용함으로써 Photon Server의 제한적 사항을 전혀 신경 쓰지 않아도 됨
    또한 Photon PUN(Photon Unity Networking)은 게임 개발 테스트를 위하여 동시 접속 사용자( CCU(Concurrent User) )수 20명까지 무료로 제공
    (제품 종류 <Photon Cloud 계열> : Photon PUN(우리가 사용할 제품), Photon RealTime, Photon Bolt, Photon Thunder, Photon TrueSync,
    Photon Chat, Photon Voice)

    ■ 비교
                            Photon Server                                Photon PUN
    라이선스                추가 서버당 과금 체계                        동시 접속 사용자(CCU)별 과금 체계
    서버 운영 및 관리            O                                           ×
    서버 사이드 게임 로직     커스텀 마이징((EX) DB 서버 연동) 가능               ×
    로드밸런싱(확장성)       직접 관리(병목현상을 줄여주는)                       ×

    cf) 네트워크 병목 현상 : 처리량 이상의 네트워크 트래픽이 몰리거나 특정 서버에 트래픽이 집중되는 경우
    네트워크 병목 현상이 발생함.=>로드밸런싱으로 병목현상을 줄여줌(스케줄링,라운드로빈,해싱)

    로드밸런싱 : 여러대의 서버를 미리 준비해 작업을 분산해두면 병목 현상을 예방할 수 있다. 이렇게 부하를 분산하는
    방식을 로드밸런싱 이라고 한다.(Photon Server)

    클라우드 로드밸런싱 : 클라우드에서 인스턴스(가상 서버)로 들어오는 트래픽을 자동 분산해
    트래픽 병목 현상을 예방하고 네트워크가 효율적으로 동작하도록 하는 것을 말한다.(Photon PUN)

    2) 회원가입 : 포톤 클라우드 서비스를 위해 http://www.exitgames.com 에서 회원가입을 하자~
    접속 => 신규 가입 메뉴 => 회원가입 페이지 => 이메일 주소 입력 => 가입 완료 => 가입 시 입력한
    이메일로 Confirm 메일 수신 및 (비밀번호 설정)링크 확인 후 계정 비밀번호 설정 => 계정 만들기 클릭
    => 메뉴에 사용자 설정(사람모양) 클릭 => 퍼블릭 클라우드(어플리케이션 리스트) 클릭
    => 가입하자마자 바로 사용 할 수 있는 Application Id(게임당 하나씩 부여되는 고유넘버로서
    포톤 서버와 통신하기 위함)가 자동으로 할당 즉 자동으로 Apllication 정보가 생성됨(우리 게임에 연동해야함)
    => (포톤 클라우드를 이용하기 위해) 메뉴에 제품 클릭 => PUN 클릭 => Photon Unity Networking (플러그인) 에셋 스토어 패키지 에서
    PUN FREE 다운 및 PUN 플러그인 설치 (유니티 에셋스토어 에서 다운 가능 : PUN(Photon Unity Networking) )

    3) PUN(Photon Unity Networking) 플러그인 설정
    PUN 플러그인 임포트 완료시 또는 메뉴 => Window => Photon Unity Networking => PUN WIzard 클릭 시 PUN WIzard 창 오픈
    PUN WIzard(여기서 회원가입 가능 => Cloud Dashboard Login)에서 => Setup Project 클릭 => AppId(포톤 사이트에 등록된...)  or Email 입력(=>Cloud Dashboard Login 후 AppId 얻어와서 셋팅)
    후 => Setup Project 클릭
    =>
    PhotonServerSettings 플러그인 세팅을 자동 선택 및 인스펙터의 요소들이 나열 된다.(나중에 직접 수정시..폴더 경로로 찾아가도 되지만..
    PUN Wizard => Locate PhotonServerSettings 클릭으로 찾아가도 됨)

    =>
    인스펙터의 속성 중 Hosting 속성 : 각 옵션을 선택하여 포톤 구동 방식을 선택하는 것으로  Photon Cloude(우리가 사용할 Photon PUN), Selef Hosted(Photon Server : 물리적인 서버)
    정도로 나뉘며 각 항목중 Photon Cloude 옵션 선택시 하단에 Region 속성이 활성화 된다. 이것은 현재 대륙별 서버를ㄴ 운영 중인 지역을 선택 하는것으로 가장 가까운 서버를 선택해야 함
    또는 Hosting 옵션중에 Best Region 선택시 현재 App 접속 지역을 기준으로 각 대륙 서버로 Ping 테스트를 한다(RTT : Round Trip Time) 즉 가장 빠른 속도로 응답이 오는 지역 서버로
    자동 접속된다.(우린 이걸 사용~!) 또한 하단에 Enabled Regions 설정으로 대륙을 선택 할 수 있다. 즉, Ping 테스트를 제외 시킬수 있다.(그냥 Everything 으로 셋팅!)
    Protocol : udp/tcp 중 우리 게임에 유리한 udp 프로토콜 셋팅!!!
    Client Settings => Auto-Join Lobby 체크시 : 마스터 접속후 자동으로 로비로 조인한다.  => PUN 플러그인 설정 완료!!!

    (참고) PUN 플러그인은 Project 뷰 => Photon Unity Networking 안에 설치된다..또한 PhotonServerSettings 플러그인은 => Resources 안에 저장 되어있다. PhotonServerSettings 클릭시
    인스펙터 뷰에 포톤 클라우드 접속 정보 및 RPC 목록을 확인 할 수 있다.
    즉, 인스펙터 뷰에 호스팅 방법(포톤 서버, 포톤 클라우드, 오프라인 모드), AppId(Application Id), Protocol 정보를 확인 및 수정할 수 있다.

    % RPC( 원격 프로시저 호출, Remote Procedure Call, 리모트 프로시저 콜) ???
    별도의 원격 제어를 위한 코딩 없이 다른 주소 공간에서 함수나
    프로시저(프로시저는 특정 작업을 수행하기 위한 프로그램의 일부== (프로그래밍에선) 함수)를 실행할 수 있게하는 프로세스(컴퓨터 내에서 실행중인 프로그램) 간 통신 기술이다.
*/