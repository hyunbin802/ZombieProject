
using System.Collections;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {

    public int power;
    public Collider co;

    // 충돌이 발생하면 잠시 동안 연속 충돌을 막는다.
    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            StartCoroutine(this.ResetColl() );
        }
        
    }

    IEnumerator ResetColl()
    {
        co.enabled = false;
        yield return new WaitForSeconds(1.5f);
        co.enabled = true;
    }
}
