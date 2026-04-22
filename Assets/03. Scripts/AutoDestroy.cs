using UnityEngine;

public class AutoDestroy : MonoBehaviour {

    public ParticleSystem tmpPtclObj;
	
	// Update is called once per frame
	void Update () {
	// 플레이가 끝났을때, Destroy 
	if (tmpPtclObj.isStopped)
		Destroy(gameObject);		
	}
}
