using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberAI : MonoBehaviour {
	public Rigidbody2D mainBod;
	public Rigidbody2D myBod;
	public WorldGrid grid;
	public GameObject searchNode;
	public float forceFactor;
	public float maxDistDiff;
	public float randCheckRange;
	public float grabDistThresh;
	public Vector2 actualNode = Vector2.negativeInfinity;
	float tempRangeBoost = 0;
	bool attached;

	void Start(){
		StartCoroutine("CheckStuck");
	}

	void PickRandomNode(){
		Vector2 randTest = mainBod.position;
		bool pointFound = false;
		int iterations = 0;
		while(!pointFound){
			float randX = searchNode.transform.position.x + Random.Range(-randCheckRange-tempRangeBoost,randCheckRange+tempRangeBoost);
			float randY = searchNode.transform.position.y + Random.Range(-randCheckRange,randCheckRange);
			randTest = grid.WorldToGrid(new Vector2(randX,randY));
			if(randTest.x >= grid.width || randTest.x < 0 || randTest.y >= grid.height || randTest.y < 0) {
				break;
			}
			else if(grid.grid[(int)randTest.x,(int)randTest.y].cType == WorldGrid.CellType.GRABABLE){
				Collider2D hit = Physics2D.OverlapCircle(grid.GridToWorld((int)randTest.x,(int)randTest.y),1.0f);
				if(hit!=null){
					//print("Hit!");
					actualNode = hit.transform.position;
					pointFound = true;
				}//else {print("Null hit");}
			}
			iterations++;
			tempRangeBoost = iterations/10.0f;
		}
	}
	Vector2 stuckPos;
	IEnumerator CheckStuck(){
		stuckPos = transform.position;
		while(true){
			if(Vector2.Distance(transform.position,stuckPos) <= 0.5f){
				GetComponent<Collider2D>().isTrigger = true;
				yield return new WaitForSeconds(1.0f);
				GetComponent<Collider2D>().isTrigger = false;
			}
			stuckPos = transform.position;
			yield return new WaitForSeconds(3.0f);
		}
	}
	void Attach(){
		mainBod.gameObject.SendMessage("AttachedTentacle");
		myBod.constraints = RigidbodyConstraints2D.FreezePosition;
		attached = true;
	}
	void Detach(){
		mainBod.gameObject.SendMessage("DetachedTentacle");
		myBod.constraints = RigidbodyConstraints2D.None;
		attached = false;
	}
	void MoveToNode(){
		Vector2 dir = (actualNode-myBod.position).normalized;
		if(Vector2.Distance(myBod.position,actualNode) < grabDistThresh) Attach();
		else myBod.AddForce(dir*forceFactor);
	}
	void Update(){
		if(actualNode.Equals(Vector2.negativeInfinity)) PickRandomNode();
		else if(Vector2.Distance(actualNode,searchNode.transform.position) > (maxDistDiff+tempRangeBoost)){
			if(attached) Detach();
			actualNode = Vector2.negativeInfinity;
		}else if(!attached){
			MoveToNode();
		}
	}


}
