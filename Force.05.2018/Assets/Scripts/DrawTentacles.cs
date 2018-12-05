using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTentacles : MonoBehaviour {
	Rigidbody2D mainBod;
	public Transform[] NodeList;
	LineRenderer rend;

	public int tempsPerNode;

	void Start(){
		NodeList = new Transform[7];
		PopulateList(gameObject,0);
		rend = GetComponent<LineRenderer>();
		rend.positionCount = tempsPerNode*NodeList.Length+1;
		mainBod = GameObject.Find("BodyChunk0").GetComponent<Rigidbody2D>();
		RenderTentacle();
	}
	void Update(){
		RenderTentacle();
	}
	void RenderTentacle(){
		Vector2 firstNode;
		Vector2 secondNode;
		Vector2 thirdNode;
		firstNode = NodeList[0].position;
		rend.SetPosition(0,(Vector2)mainBod.position);
		for(int i = 0; i < NodeList.Length; i++){
			if(i < NodeList.Length - 2){
				secondNode = NodeList[i+1].position;
				thirdNode = NodeList[i+2].position;
			}else if(i < NodeList.Length - 1){
				secondNode = NodeList[i+1].position;
				thirdNode = secondNode;
			}else{
				secondNode = firstNode;
				thirdNode = secondNode;
			}
			for(int j = 0; j < tempsPerNode; j++){
				Vector2 newPoint = Vector2.Lerp(firstNode,Vector2.Lerp(secondNode,thirdNode,j/(float)tempsPerNode),j/(float)tempsPerNode);
				rend.SetPosition(tempsPerNode*i + j+1,newPoint);
				firstNode = newPoint;
			}
		}
		
	}
	void PopulateList(GameObject curr,int count){
		HingeJoint2D hinge = curr.GetComponent<HingeJoint2D>();
		NodeList[count]= curr.transform;
		if(hinge == null) return;
		PopulateList(hinge.connectedBody.gameObject,count+1);
	}

}
