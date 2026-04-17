using UnityEngine;
using System.Collections;

public class FTME04_AnimationSpriteSheet : MonoBehaviour {

	public int uvX = 4;  
	public int uvY = 2; 
	public float fps = 24.0f;
	float index;
	int uIndex;
	int vIndex;
	Vector2 size; 
	Vector2 offset; 

	void Update () {
		index = Time.time * fps;
		index = index % (uvX * uvY);

		size = new Vector2 (1.0f / uvX, 1.0f / uvY);
		uIndex = (int)index % uvX;
		vIndex = (int)index / uvX;
		offset = new Vector2 (uIndex * size.x, 1.0f - size.y - vIndex * size.y);

		GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", offset);
		GetComponent<Renderer>().material.SetTextureScale ("_MainTex", size);	
	}
}
