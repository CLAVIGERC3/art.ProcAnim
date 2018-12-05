using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMouse : MonoBehaviour {	
	public float forceMod;
	private float lerping;
	// Update is called once per frame
	void Start(){
		lerping = 0.01f;
	}
	void Update () {
		if(Input.GetMouseButton(0)){
			this.GetComponent<Rigidbody2D>().AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition)-transform.position)*Mathf.Lerp(0,forceMod,lerping));
			lerping += 0.01f;
		}else lerping =0.01f;
	}
}
