using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour {
	public float cellResolution;
	public int height;
	public int width;
	bool isInitialized = false;
	public GameObject debugRenderPrefab;
	public Cell[,] grid;

	public class Cell{
		public float cost = float.MaxValue;
		public float heur = float.MinValue;
		public int x;
		public int y;

		public Cell cameFrom;
		public CellType cType;
	}
	public enum CellType {
		AIR,
		GRABABLE,
		IMPASSIBLE
	}
	public LinkedList<Cell> AStar(Vector2 goal, Vector2 navigator, int grabWeight){
		Vector2 start = WorldToGrid(navigator);
		//print("A* first node pos given: "+start);
		int s_x = (int) start.x;
		int s_y = (int) start.y;
		int g_x = (int) goal.x;
		int g_y = (int) goal.y;
		for(int i = 0; i < width; i++)for(int j = height -1; j >= 0; j--){
			grid[i,j].cost = float.MaxValue;
			grid[i,j].heur = float.MaxValue;
			grid[i,j].cameFrom = null;
		}  
		grid[s_x,s_y].cost = 0;
		grid[s_x,s_y].heur = heuristic(s_x,s_y,g_x,g_y);
		grid[s_x,s_y].cameFrom = null;
		LinkedList<Cell> openSet = new LinkedList<Cell>();
		openSet.AddFirst(grid[s_x,s_y]);
		LinkedList<Cell> closedSet = new LinkedList<Cell>();
		

		while(openSet.Count > 0){
			Cell current = openSet.First.Value;
			foreach(Cell c in openSet){
				if(c.heur < current.heur){
					current = c;
				}
			}
			if(current == grid[g_x,g_y]){
				return reconstructPath(current);
			}
			openSet.Remove(current);
			closedSet.AddFirst(current);
			int xup=current.x;
			int yup=current.y+1;
			int xright=current.x+1;
			int yright=current.y;
			int xleft=current.x-1;
			int yleft=current.y;
			int xdown=current.x;
			int ydown=current.y-1;

			if(yup < height && !openSet.Contains(grid[xup,yup]) && !closedSet.Contains(grid[xup,yup])){
				Cell here = grid[xup,yup];
				float newCost;
				switch(here.cType){
					case CellType.AIR:
						newCost = 1;
					break;
					case CellType.GRABABLE:
						newCost = 1*grabWeight;
					break;
					case CellType.IMPASSIBLE:
						newCost = -1;
					break;
					default:
						newCost = -1;
					break;
				}
				if(newCost != -1){
					float posCost = current.cost + newCost;
					if(posCost < here.cost){
						here.cost = posCost;
						here.cameFrom = current;
						here.heur = here.cost + heuristic(here.x,here.y,g_x,g_y);
					} 
					openSet.AddFirst(here);
				}
			}
			if(xright< width && !openSet.Contains(grid[xright,yright]) && !closedSet.Contains(grid[xright,yright])){
				Cell here = grid[xright,yright];
				float newCost;
				switch(here.cType){
					case CellType.AIR:
						newCost = 1;
					break;
					case CellType.GRABABLE:
						newCost = 1*grabWeight;
					break;
					case CellType.IMPASSIBLE:
						newCost = -1;
					break;
					default:
						newCost = -1;
					break;
				}
				if(newCost != -1){
					float posCost = current.cost + newCost;
					if(posCost < here.cost){
						here.cost = posCost;
						here.cameFrom = current;
						here.heur = here.cost + heuristic(here.x,here.y,g_x,g_y);
					} 
					openSet.AddFirst(here);
				}
			}
			if(xleft > 0 && !openSet.Contains(grid[xleft,yleft]) && !closedSet.Contains(grid[xleft,yleft])){
				Cell here = grid[xleft,yleft];
				float newCost;
				switch(here.cType){
					case CellType.AIR:
						newCost = 1;
					break;
					case CellType.GRABABLE:
						newCost = 1*grabWeight;
					break;
					case CellType.IMPASSIBLE:
						newCost = -1;
					break;
					default:
						newCost = -1;
					break;
				}
				if(newCost != -1){
					float posCost = current.cost + newCost;
					if(posCost < here.cost){
						here.cost = posCost;
						here.cameFrom = current;
						here.heur = here.cost + heuristic(here.x,here.y,g_x,g_y);
					} 
					openSet.AddFirst(here);
				}
			}
			if(xdown > 0 && !openSet.Contains(grid[xdown,ydown]) && !closedSet.Contains(grid[xdown,ydown])){
				Cell here = grid[xdown,ydown];
				float newCost;
				switch(here.cType){
					case CellType.AIR:
						newCost = 1;
					break;
					case CellType.GRABABLE:
						newCost = 1*grabWeight;
					break;
					case CellType.IMPASSIBLE:
						newCost = -1;
					break;
					default:
						newCost = -1;
					break;
				}
				if(newCost != -1){
					float posCost = current.cost + newCost;
					if(posCost < here.cost){
						here.cost = posCost;
						here.cameFrom = current;
						here.heur = here.cost + heuristic(here.x,here.y,g_x,g_y);
					} 
					openSet.AddFirst(here);
				}
			}
		}
		print("No path found");
		return null;
	}
	float heuristic(int startx, int starty, int goalx, int goaly){
		return Mathf.Sqrt(Mathf.Pow(goalx-startx,2.0f)+Mathf.Pow(goaly-starty,2.0f));
	}
	LinkedList<Cell> reconstructPath(Cell curr){
		LinkedList<Cell> toReturn = new LinkedList<Cell>();
		Cell here = curr;
		while(here!=null){
			toReturn.AddFirst(here);
			here = here.cameFrom;
		}
		//print("A* path first node pos actual: ("+toReturn.First.Value.x+","+toReturn.First.Value.y+")");
		//print("A* path length: "+toReturn.Count);
		return toReturn;
	}

	void Start(){
		grid = new Cell[width,height];
		for(int i = 0; i < width; i++)for(int j = height -1; j >= 0; j--){
			grid[i,j] = new Cell();
			grid[i,j].cType = CellType.AIR;
			grid[i,j].x = i;
			grid[i,j].y = j;
		}  
		InitializeGrid();
	}
	void Update(){
		// if(Input.GetKeyDown(KeyCode.R)) {
		// 	//debugPath = AStar(WorldToGrid(GameObject.Find("testGoal").transform.position),GameObject.Find("BodyChunk0").transform.position,2);
		// 	//if(debugPath != null) renderPath = true;
		// }
	}

	void InitializeGrid(){
		for(int i = 0; i < width; i++){
			for(int j = 0; j < height; j++){
				RaycastHit2D hit = Physics2D.BoxCast(GridToWorld(i,j),Vector2.one*0.25f,0,Vector2.zero);
				if(hit && hit.collider.gameObject.layer == 8){
					if(j-1 > 0) grid[i,j-1].cType = CellType.GRABABLE;
					if(j+1 < height) grid[i,j+1].cType = CellType.GRABABLE;
					if(i-1 > 0) grid[i-1,j].cType = CellType.GRABABLE;
					if(i+1 < width) grid[i+1,j].cType = CellType.GRABABLE;
				}
			}
		}
		isInitialized = true;
		for(int i = 0; i < width; i++){
			for(int j = 0; j < height; j++){
				RaycastHit2D hit = Physics2D.BoxCast(GridToWorld(i,j),Vector2.one*0.25f,0,Vector2.zero);
				if(hit && hit.collider.gameObject.layer == 8){
					grid[i,j].cType = CellType.IMPASSIBLE;
				}else{
					if(grid[i,j].cType != CellType.GRABABLE) grid[i,j].cType = CellType.AIR;
				}
			}
		}
	}
	void RenderGrid(){
		if(isInitialized){
			for(int x = 0; x < width; x++){
				for(int y = 0; y < height; y++){
					Vector2 pos = GridToWorld(x,y);
					switch(grid[x,y].cType){
						case CellType.AIR:
							GameObject a = Instantiate(debugRenderPrefab,pos,Quaternion.identity);
							a.GetComponent<SpriteRenderer>().color = Color.blue;
							break;
						case CellType.GRABABLE:
							GameObject g = Instantiate(debugRenderPrefab,pos,Quaternion.identity);
							g.GetComponent<SpriteRenderer>().color = Color.green;
							break;
					}
				}
			}
		}
	}
	public Vector2 WorldToGrid(Vector2 worldPoint){
		return new Vector2(Mathf.FloorToInt((worldPoint.x-transform.position.x)/cellResolution),Mathf.FloorToInt((worldPoint.y-transform.position.y)/cellResolution));
	}
	public Vector2 GridToWorld(int x, int y){
		return new Vector2((x*cellResolution+transform.position.x)+cellResolution/2,(y*cellResolution+transform.position.y)+cellResolution/2);
	}
	void OnDrawGizmos(){
		Gizmos.color = Color.grey;
		for(int i = 0; i <= width;i ++){
			Vector2 start = GridToWorld(i,0);
			start.x-=cellResolution/2;
			start.y-=cellResolution/2;
			Vector2 end = GridToWorld(i,height);
			end.x-=cellResolution/2;
			end.y-=cellResolution/2;
			Gizmos.DrawLine(start,end);
		}
		for(int j = 0; j <= height; j ++){
			Vector2 start = GridToWorld(0,j);
			start.x-=cellResolution/2;
			start.y-=cellResolution/2;
			Vector2 end = GridToWorld(width,j);
			end.x-=cellResolution/2;
			end.y-=cellResolution/2;
			Gizmos.DrawLine(start,end);
		}
		// if(isInitialized && renderPath){		
		// 	foreach(Cell c in debugPath){
		// 		Gizmos.color = Color.red;
		// 		Gizmos.DrawCube(GridToWorld(c.x,c.y),Vector3.one*0.25f);
		// 	}
		// }
	}
}
