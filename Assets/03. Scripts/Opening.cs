using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Opening : MonoBehaviour
{
    // 애셋번들 참조 변수
    private AssetBundle assetBundle;

    // AssetBundle을 내려받을 주소
    private string url = "";

    // AssetBundle의 버전
    private uint version = 3;

    // 다운로드 완료 여부
    private bool isDownloadDone = false;

    // 다운로드 진행률
    private float downloadProgress = 0f;

    // 오류 메시지
    private string errorMessage = "";

    void Awake()
    {
        // 스크린 절전 모드 안 가게
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // ftp 서버 or 웹 서버 주소 + 애셋번들 파일 위치
        // url = "http://210.122.7.164/dinoScenes/ScLevel_1.0.6.unity3d";
        url = "file:///C:/UnitySt/St5_1/AssetBundles/scene/study.study";
    }

    IEnumerator Start()
    {
        // 지정된 url 주소로 접근하여 해당 파일을 내려받음
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, version, 0))
        {
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                downloadProgress = operation.progress;
                yield return null;
            }

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                errorMessage = request.error;
                Debug.LogError(errorMessage);
            }
            else
            {
                // 내려받은 AssetBundle을 메모리에 로드
                assetBundle = DownloadHandlerAssetBundle.GetContent(request);
                isDownloadDone = true;
            }
        }
    }

    /*
     * GetAssetBundle(url, version, crc) 함수는
     * url 주소   : 내려받을 AssetBundle의 경로와 이름 지정
     * version    : 동일한 version의 AssetBundle 파일이 캐시에 있을 경우
     *              url 경로 대신 캐시된 파일을 사용한다.
     *              새 배포 버전이 올라가면 version 값을 변경해서
     *              다시 받도록 할 수 있다.
     * crc        : 무결성 검사용 값. 모르면 0 사용 가능
     */

    void OnGUI()
    {
        // 오류가 있으면 오류 메시지 출력
        if (!string.IsNullOrEmpty(errorMessage))
        {
            GUI.Label(new Rect(20, 20, 500, 30), "Error : " + errorMessage);
            return;
        }

        // 내려받는 진행상태 표시
        if (!isDownloadDone)
        {
            GUI.Label(
                new Rect(20, 20, 250, 30),
                "Downloading... " + (downloadProgress * 100.0f).ToString("F0") + "%"
            );
        }

        // AssetBundle을 모두 내려받으면 GUI 버튼 생성
        if (isDownloadDone && GUI.Button(new Rect(20, 50, 100, 30), "Start Game"))
        {
            LoadScenes();
        }
    }

    // 먼저 로드한 scLevel Scene에 scMain Scene을 병합해서 로드
    void LoadScenes()
    {
        SceneManager.LoadScene("scLevel");
        SceneManager.LoadScene("scMain", LoadSceneMode.Additive);
    }

    // 주의 사항
    // 씬 분리/병합 처리 시 라이트맵은 다시 굽는 것이 안전하고,
    // 플랫폼도 서로 맞춰서 AssetBundle을 빌드해야 한다.
}

#region 과거 버전
//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class Opening : MonoBehaviour
//{

//    //애셋번들 참조 변수 
//    AssetBundle assetBundle;

//    //AssetBundle을 내려받을 주소 
//    string url = "";

//    //AssetBundle의 버전 
//    int version = 1;

//    //Web 접속하기 위한 변수 
//    [System.Obsolete]
//    WWW www;

//    /* WWW 클래스는 http://, https://, ftp://, file:///
//     * 프로토콜(Protocol). 지원
//     */

//    void Awake()
//    {
//        //스크린 절전 모드 안가게 
//        Screen.sleepTimeout = SleepTimeout.NeverSleep;
//        // ftp 서버 or 웹 서버 주소 + 애셋번들 파일 위치
//        //  url = "http://210.122.7.164/dinoScenes/ScLevel_1.0.6.unity3d";
//        url = "file:///C:\\Bundles\\study.study";

//    }

//    [System.Obsolete]
//    IEnumerator Start()
//    {
//        //지정된 url주소로 접근하여 해당파일을 내려받음 
//        www = WWW.LoadFromCacheOrDownload(url, version);
//        yield return www;

//        //오류가 있으면 메세지 출력 
//        if (!string.IsNullOrEmpty(www.error))
//        {
//            Debug.Log(www.error.ToString());
//        }
//        else
//        {
//            //내려받은 AssetBundle을 메모리에 로드 
//            assetBundle = www.assetBundle;
//        }
//    }

//    /*  LoadFromCacheOrDownload(url, version) 함수는
//     *  url 주소 : 내려받을 AssetBundle의 경로와 이름 지정
//     *  version  : Integer 타입으로 만약 동일한 version의
//     *  AssetBundle 파일이 Catch메모리에 있을 경우 url 경로를
//     *  무시 하고 Catch메모리에 있는 것을 로드한다. 또한 만약 새로
//     *  배포하는 앱의 version을 1에서 2로 변경하면 Catch메모리에 
//     *  있는 AssetBundle 파일을 무시하고 다시 내려받는다.
//     */

//    [System.Obsolete]
//    void OnGUI()
//    {
//        //AssetBundle을 모두 내려받으면 GUI버튼을 생성 
//        if (www.isDone && GUI.Button(new Rect(20, 50, 100, 30), "Start Game"))
//        {
//            LoadScenes();
//        }

//        //내려받는 진행상태 표시
//        GUI.Label(new Rect(20, 20, 200, 30)
//                   , "Downloading..." + (www.progress * 100.0f).ToString() + "%");
//    }

//    /*
//        void OnGUI()
//        {
//            if( GUI.Button( new Rect(50, 50, 200, 50), "Start" ) )
//            {
//                LoadScenes();
//            }
//        }
//    */

//    // 먼저 로드한 scLevel_01 Scene에 scLogic Scene을 병합해서 로드
//    void LoadScenes()
//    {
//        SceneManager.LoadScene("scLevel");
//        SceneManager.LoadScene("scMain", LoadSceneMode.Additive);

//    }

//    // 주의 사항~ 씬 분리/병합처리시 라이트맵은 다시 굽자~ 또한 서로 플랫폼을 맞춰주자~
//}
#endregion