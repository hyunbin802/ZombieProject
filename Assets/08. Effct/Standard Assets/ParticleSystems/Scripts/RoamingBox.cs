using UnityEngine;

public class RoamingBox : MonoBehaviour 
{
    void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Enemy" )
        {
            coll.gameObject.GetComponent<EnemyCtrl>().RoamingCheckStart();
        }
    }
}