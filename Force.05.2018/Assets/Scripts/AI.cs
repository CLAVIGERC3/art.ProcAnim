using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {
	public WorldGrid grid;
	Rigidbody2D mainBod;
	public float distThresh;
	public float forceFactor;
	float forceAdjusted;

	public int tentaclesTouching;
	public LinkedList<Vector2> lerpedPath;
	public Vector2 currentNode;
	public int lerpedPathCount;

	Vector2 randomNode;
	bool haveRandomNode;
	bool atRandomNode;
	bool started= false;



	void Start(){
		lerpedPath = new LinkedList<Vector2>();
		mainBod = this.GetComponent<Rigidbody2D>();
		haveRandomNode = false;
		atRandomNode = false;
		started = true;
		tentaclesTouching = 0;
		forceAdjusted = forceFactor;
		StartCoroutine("CheckStuck");
	}

	void AttachedTentacle(){
		tentaclesTouching++;
	}
	void DetachedTentacle(){
		tentaclesTouching--;
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
	void PickRandomNode(){
		//print("Picking node");
		atRandomNode = false;
		while(!haveRandomNode){
			int upperX = grid.width;
			int upperY = grid.height;

			int randX= Random.Range(0,upperX);
			int randY= Random.Range(0,upperY);

			if(grid.grid[randX,randY].cType == WorldGrid.CellType.AIR){
				LinkedList<WorldGrid.Cell> path;
				randomNode = new Vector2(randX,randY);
				haveRandomNode = true;
				//print("AI current pos: ("+grid.WorldToGrid(transform.position)+")");
				path = grid.AStar(randomNode,transform.position,4);
				//print("AI path first node pos: ("+path.First.Value.x+","+path.First.Value.y+")");
				//print("AI current pos: ("+grid.WorldToGrid(transform.position)+")");
				LerpPath(path);
			}
		}
	}

	void LerpPath(LinkedList<WorldGrid.Cell> path){
		if(lerpedPath!=null) lerpedPath.Clear();
		LinkedListNode<WorldGrid.Cell> actualFirstNode;
		Vector2 lerpedFirstNode;
		LinkedListNode<WorldGrid.Cell> secondNode;
		LinkedListNode<WorldGrid.Cell> thirdNode;
		actualFirstNode= path.First;
		lerpedFirstNode = grid.GridToWorld(actualFirstNode.Value.x,actualFirstNode.Value.y);
		lerpedPath.AddLast(lerpedFirstNode);
		for(int i = 0; i < path.Count-1; i++){
			if(i < path.Count - 2){
				secondNode = actualFirstNode.Next; 
				thirdNode = secondNode.Next;
			}else if(i < path.Count - 1){
				secondNode = actualFirstNode.Next;
				thirdNode = secondNode;
			}else{
				secondNode = actualFirstNode;
				thirdNode = secondNode;
			}
			lerpedFirstNode = Vector2.Lerp(grid.GridToWorld(actualFirstNode.Value.x,actualFirstNode.Value.y),Vector2.Lerp(grid.GridToWorld(secondNode.Value.x,secondNode.Value.y),grid.GridToWorld(thirdNode.Value.x,thirdNode.Value.y),0.5f),0.5f);
			lerpedPath.AddLast(lerpedFirstNode);
			actualFirstNode = actualFirstNode.Next;
		}
		//print(lerpedPath.Count+" nodes added to lerpedPath");
		currentNode = lerpedPath.First.Value;
	}

	void MoveToRandomNode(){
		if(lerpedPath.Count > 0){
			if(Vector2.Distance(transform.position,currentNode)<= distThresh){
				lerpedPath.RemoveFirst();
				if(lerpedPath.Count > 0)currentNode = lerpedPath.First.Value;
			}else{
				Vector2 dir = (currentNode-(Vector2)transform.position).normalized;
				mainBod.AddForce(dir*forceAdjusted);
			}
			return;
		}
		atRandomNode = true;
		haveRandomNode = false;
	}

	void Update(){
		mainBod.drag = 5*(5.0f-tentaclesTouching)/5.0f;
		forceAdjusted =  forceFactor - forceFactor*0.05f*(5.0f-tentaclesTouching)/5.0f;
		lerpedPathCount = lerpedPath.Count;
		if(atRandomNode || !haveRandomNode)PickRandomNode();
		if(haveRandomNode) MoveToRandomNode();
	}

	void OnDrawGizmos(){
		if(started && lerpedPath.Count > 0){
			LinkedListNode<Vector2> curr = lerpedPath.First;
			for(int i = 0; i < lerpedPath.Count-1; i++){
				Gizmos.color = Color.red;
				Gizmos.DrawLine(curr.Value,curr.Next.Value);
				curr = curr.Next;
			}
		}
	}
}
