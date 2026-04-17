using UnityEngine;
using System.Collections;

public class FTME04_AnimationScrollTexture : MonoBehaviour {

	public float speedX = 0.0f;
	public float speedY = 0.0f;
	float offsetX;
	float offsetY;

	void Start () {
	
	}

	void FixedUpdate () {
		offsetX = Time.time * (-speedX);
		offsetY = Time.time * (-speedY);
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2 (offsetX,offsetY);	
	}
}
