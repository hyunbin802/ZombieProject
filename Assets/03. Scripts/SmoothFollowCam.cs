using UnityEngine;

public class SmoothFollowCam : MonoBehaviour 
{
    // 따라갈 대상의 Transform
    public Transform target;
    // 대상과의 거리
    public float distance = 10.0f;
    // 대상과의 높이 차이
    public float height = 5.0f;
    // 높이를 부드럽게 따라가는 속도
    public float heightDamping = 2.0f;
    // 회전을 부드럽게 따라가는 속도
    public float rotationDamping = 3.0f;

    // 모든 Update가 끝난 뒤 카메라를 따라가게 한다.
    void LateUpdate()
    {
        // 대상이 없으면 종료
        if (!target)
            return;

        // 목표 회전값과 높이값 계산
        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;

        // 현재 카메라 상태
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // 현재 회전과 높이를 목표값으로 부드럽게 보간
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // 보간된 Y축 회전값으로 회전 Quaternion 생성
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // 대상 위치를 기준으로 뒤쪽 distance만큼 떨어진 위치 계산
        Vector3 tempDis = target.position;
        tempDis -= currentRotation * Vector3.forward * distance;

        // 높이값 적용 후 카메라 위치 반영
        tempDis.y = currentHeight;
        transform.position = tempDis;

        // 항상 대상을 바라보게 설정
        transform.LookAt(target);
    }
}
