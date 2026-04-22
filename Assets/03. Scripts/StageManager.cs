using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

    //스폰 장소 
    private Transform[] EnemySpawnPoints;

    //Enemy 프리팹을 위한 레퍼런스
    public GameObject Enemy;

    //게임 끝
    private bool gameEnd;

    // 스테이지 Enemy들을 위한 레퍼런스
    private GameObject[] Enemys;

    // 베이스 스타트를 위한 변수
    public BaseCtrl baseStart;

    void Awake()
    {

        //스폰 위치 얻기
        EnemySpawnPoints = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        // 몬스터 스폰 코루틴 호출
        StartCoroutine(this.CreateEnemy());

    }

    // Use this for initialization
    IEnumerator Start ()
    {
        yield return new WaitForSeconds(5.0f);

        //베이스 지킴이 시작
        baseStart.StartBase();


    }
	
    // 몬스터 생성 코루틴 함수
    IEnumerator CreateEnemy()
    {
        //게임중 일정 시간마다 계속 호출됨 
        while (!gameEnd)
        {
            //리스폰 타임 5초
            yield return new WaitForSeconds(5.0f);

            // 스테이지 총 몬스터 객수 제한을 위하여 찾자~
            Enemys = GameObject.FindGameObjectsWithTag("Enemy");

            // 스테이지 총 몬스터 객수 제한
            if (Enemys.Length < 20)
            {
                //루트 생성위치는 필요하지 않다.그래서 1 번째 인덱스부터 돌리자
                for (int i = 1; i< EnemySpawnPoints.Length; i++)
                {
                    Instantiate(Enemy, EnemySpawnPoints[i].localPosition, EnemySpawnPoints[i].localRotation);
                }
            }
        }
    }
}
