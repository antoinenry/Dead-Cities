using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;


public enum tileType {unspecified, empty, street, building};
public class GridTile
{
	public tileType type;
	public int id;
	public GridTile()
	{
		type = tileType.empty;
		id = -1;
	}
}

public class TileGrid
{
	public int2 width;
	private List<GridTile> g;
	
	public TileGrid (int w)
	{
		width = new int2(w, w);
		g = new List<GridTile>();
		for(int i=0; i<w*w; i++)
			g.Add(new GridTile());
	}

	public TileGrid (int w, int h)
	{
		width = new int2(w, h);
		g = new List<GridTile>();
		for(int i=0; i<w*h; i++)
			g.Add(new GridTile());
	}

	public tileType type(int x, int y)
	{
		if (this.contains (x, y) == false)
			return tileType.unspecified;
		return g[x+y*width.x].type;
	}

	public int id(int x, int y)
	{
		return g[x+y*width.x].id;
	}

	public void set(int x, int y, tileType t = tileType.unspecified, int id = -1000)
	{
		if(t != tileType.unspecified)
			g[x+y*width.x].type = t;
		if(id != -1000)
			g[x+y*width.x].id = id;

	}

	public bool contains(int x, int y)
	{
		return x>=0 && x<width.x && y>=0 && y<width.y;
	}
}

public class CityBlock : MonoBehaviour
{
	public GameObject streetObject;
	public GameObject buildingObject;
	public GameObject doorObject;

	public int width;
	public int[] mainConnections = new int[4] {-1, -1, -1, -1};
	public int[] secondaryConnections = new int[4] {-1, -1, -1, -1};
	public bool visited = false;

	TileGrid grid;
	List<Tile> streets;
	List<Building> buildings;
	bool playingRaiseAnimation;

	void Update()
	{
		if (playingRaiseAnimation == true)
		{
			float height = transform.localScale.y;
			height +=  Time.deltaTime * (1f - height);
			transform.localScale = new Vector3(1f, height, 1f);

			if (Mathf.Abs (1f - height) <= .01f)
			{
				transform.localScale = new Vector3(1f, 1f, 1f);
				playingRaiseAnimation = false;
			}
		}
	}

	public void BuildBlock(int w)
	{
		width = w;
		SetGrid ();
		SetMainStreets();
		SetMediumStreets();
		BuildStreets();
		SetBuildings();
		SetDoors ();

		transform.localScale = new Vector3 (1f, 0f, 1f);
		playingRaiseAnimation = true;
	}

	void SetGrid()
	{
		grid = new TileGrid(width);

		Collider gridCollider = this.gameObject.GetComponentInChildren<Collider> ();
		gridCollider.gameObject.transform.SetParent(this.transform);
		gridCollider.gameObject.transform.localPosition = new Vector3 ((float)width/2f, -0.001f, (float)width/2f);
		gridCollider.gameObject.transform.localScale = new Vector3 ((float)width/10f, 1f, (float)width/10f);
		gridCollider.GetComponent <MeshRenderer> ().material.color = Color.black;
	}

	// Main streets
	void SetMainStreets()
	{
		int x, y;
		int w = width/2;
		TileGrid g = new TileGrid(w);

		for(int i = 0; i < 4; i++)
		{
			if(mainConnections[i] == -1)
				mainConnections[i] = Random.Range(w/4, 3*w/4);
		}

		x = mainConnections[0];
		for(y=0; y <= mainConnections[1]; y++)
			g.set(x,y,tileType.street);

		x = mainConnections[2];
		for(y=mainConnections[1]; y < w; y++)
			g.set(x,y,tileType.street);

		y = mainConnections[1];
		for(x=0; x < Math.Max(mainConnections[0], mainConnections[2]); x++)
			g.set(x,y,tileType.street);

		y = mainConnections[3];
		for(x=w-1; x >=0; x--)
		{
			if(g.type(x,y) == tileType.street)
				break;
			g.set(x,y,tileType.street);
		}

		for(x = 0; x<width; x++)
			for(y = 0; y<width; y++)
				grid.set (x,y,g.type(x/2,y/2));
	}

	// Medium streets
	void SetMediumStreets()
	{
		int x = 0, y = 0;
		int o = 0;
		List<int> offset = new List<int>();
		int minSpace = 3;

		for(int k=0; k<4; k++)
		{
			offset.Clear();

			if(secondaryConnections[k] == -1)
			{
				for(o = minSpace; o < width-minSpace; o++)
				{
					bool skip = false;
					for(int i=0; i<minSpace; i++)
					{
						switch(k)
						{
						case 0: if(grid.type(o-i,0) == tileType.street || grid.type(o+i,0) == tileType.street)
								skip = true;
							break;
						case 1: if(grid.type(o-i,width-1) == tileType.street || grid.type(o+i,width-1) == tileType.street)
								skip = true;
							break;
						case 2: if(grid.type(0,o-i) == tileType.street || grid.type(0,o+i) == tileType.street)
								skip = true;
							break;
						case 3: if(grid.type(width-1,o-i) == tileType.street || grid.type(width-1,o+i) == tileType.street)
								skip = true;
							break;
						}
					}
					if(!skip)
						offset.Add(o);
				}
				if(offset.Count > 0)
					secondaryConnections[k] = offset[Random.Range(0, offset.Count)];
			}

			if(secondaryConnections[k] != -1)
			{
				for(int t=0; t<width; t++)
				{
					switch(k)
					{
					case 0: x = secondaryConnections[k]; y = t;
						break;
					case 1: x = t; y = secondaryConnections[k];
						break;
					case 2: x = secondaryConnections[k]; y = width-1-t;
						break;
					case 3: x = width-1-t; y = secondaryConnections[k];
						break;
					}
					if(grid.type(x,y) == tileType.street)
						break;
					grid.set(x,y,tileType.street);
				}
			}
		}
	}

	void BuildStreets()
	{
		streets = new List<Tile>();
		for(int ix = 0; ix < width; ix++)
		{
			for(int iy = 0; iy < width; iy++)
			{
				if(grid.type(ix,iy) == tileType.street)
				{
					int id = BuildStreetTile(ix, iy);
					grid.set(ix, iy, tileType.unspecified, id);
				}
			}
		}
	}

	int BuildStreetTile(int x, int y)
	{
		GameObject instance = Instantiate(streetObject, new Vector3 (0f, 0f, 0f), Quaternion.AngleAxis(90f, new Vector3(1.0f,0f,0f))) as GameObject;
		instance.transform.SetParent(this.transform);
		instance.transform.localPosition = new Vector3 (x+.5f,0f,y+.5f);
		instance.name = "Street Tile " + streets.Count;

		streets.Add(instance.GetComponent<Tile>());

		return streets.Count - 1;
	}

	// Buildings
	void SetBuildings()
	{
		buildings = new List<Building>();
		for(int x=0; x<width; x++)
		{
			for(int y=0; y<width; y++)
			{
				if(grid.type (x,y) == tileType.empty)
				{
					RandomBuildingAt(x,y);
				}
			}
		}
	}

	void RandomBuildingAt(int x0, int y0)
	{
		int h = 1, w = 1;
		bool xstop = false, ystop = false;

		while(!xstop && !ystop)
		{
			for(int ix = x0; ix<x0+w; ix++)
			{
				if(grid.contains(ix, y0+h) == false || grid.type (ix, y0+h) != tileType.empty )
					ystop = true;
			}
			if(!ystop) h++;

			for(int iy = y0; iy<y0+h; iy++)
			{
				if(grid.contains(x0+w, iy) == false || grid.type (x0+w, iy) != tileType.empty)
					xstop = true;
			}
			if(!xstop) w++;
		}

		if(w>4 && Random.Range(0,w)!=0) w/=2;
		if(h>4 && Random.Range(0,h)!=0) h/=2;
		int buildingHeight = Random.Range(1, 6);

		int id = BuildBuilding(x0, y0, w, h, buildingHeight);

		for(int x=x0; x<x0+w; x++)
		{
			for(int y=y0; y<y0+h; y++)
			{
				grid.set(x,y,tileType.building, id);
			}
		}
	}

	int BuildBuilding(int x0, int y0, int w, int h, int height)
	{
		GameObject instance = Instantiate(buildingObject, new Vector3(0f, 0f, 0f), Quaternion.AngleAxis(1f, new Vector3(1.0f,0f,0f))) as GameObject;
		instance.transform.SetParent(this.transform);
		instance.transform.localPosition = new Vector3(x0, 0f, y0);
		instance.name = "Building " + buildings.Count;

		Building b = instance.GetComponent<Building>();
		b.id = buildings.Count;
		b.x0 = x0;
		b.y0 = y0;
		b.h = h;
		b.w = w;
		b.height = height;
		b.Build();
		buildings.Add(instance.GetComponent<Building>());

		return b.id;
	}

	void SetDoors()
	{
		List<int2> buildingTour = new List<int2> ();

		for (int i = 0; i < buildings.Count; i++)
		{
			buildingTour.Clear ();

			int x0 = buildings [i].x0;
			int y0 = buildings [i].y0;

			for (int ix = 0; ix < buildings [i].w; ix++)
			{
				if (grid.type (x0 + ix, y0 - 1) == tileType.street)
					buildingTour.Add (new int2 (x0 + ix, y0 - 1));
				if (grid.type (x0 + ix, y0 + buildings [i].h) == tileType.street)
					buildingTour.Add (new int2 (x0 + ix, y0 + buildings [i].h));
			}
			for (int iy = 0; iy < buildings [i].h; iy++)
			{
				if (grid.type (x0 - 1, y0 + iy) == tileType.street)
					buildingTour.Add (new int2 (x0 - 1, y0 + iy));
				if (grid.type (x0 + buildings [i].w, y0 + iy) == tileType.street)
					buildingTour.Add (new int2 (x0 + buildings [i].w, y0 + iy));
			}
			if (buildingTour.Count != 0)
			{
				int2 doorPos = buildingTour [Random.Range (0, buildingTour.Count)];
				int side = new int ();
				if (doorPos.y == y0 - 1)
					side = 0;
				if (doorPos.y == y0 + buildings [i].h)
					side = 1;
				if (doorPos.x == x0 - 1)
					side = 2;
				if (doorPos.x == x0 + buildings [i].w)
					side = 3;

				buildings[i].door = BuildDoor (doorPos.x, doorPos.y, side);
			}
		}
	}

	Door BuildDoor(int x, int y, int side)
	{
		Vector3 yAxis = new Vector3 (0f, 1f, 0f);
		GameObject instance = Instantiate(doorObject, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;
		instance.transform.SetParent(this.transform);

		Door d = instance.GetComponent<Door> ();

		switch (side)
		{
		case 0:
			instance.transform.localPosition = new Vector3 (x + .5f, 0f, y + 1f);
			d.Set(new int2 (x, y), new int2 (x, y + 1));
			break;
		case 1:
			instance.transform.localPosition = new Vector3 (x + .5f, 0f, y);
			instance.transform.localRotation = Quaternion.AngleAxis (180f, yAxis);
			d.Set(new int2 (x, y), new int2 (x, y - 1));
			break;
		case 2:
			instance.transform.localPosition = new Vector3 (x + 1f, 0f, y + .5f);
			instance.transform.localRotation = Quaternion.AngleAxis (90f, yAxis);
			d.Set(new int2 (x, y), new int2 (x + 1, y));
			break;
		case 3:
			instance.transform.localPosition = new Vector3 (x, 0f, y + .5f);
			instance.transform.localRotation = Quaternion.AngleAxis (-90f, yAxis);
			d.Set(new int2 (x, y), new int2 (x - 1, y));
			break;
		}

		return d;
	}

	// Interface
	public void HoverStreetTile(int id, bool on)
	{
		if(id < 0 || id >= streets.Count)
			return;
		streets[id].Hover(on);
	}

	public int GetTileId(int2 pos)
	{
		if (this.ContainsLocalCoordinates (pos) && grid.type(pos.x, pos.y) == tileType.street)
			return grid.id (pos.x, pos.y);
		else
			return -1;
	}

	public tileType GetTileType(int2 pos)
	{
		if (this.ContainsLocalCoordinates (pos))
			return grid.type (pos.x, pos.y);
		else
			return tileType.empty;
	}

	public void HoverBuilding(int id, bool on)
	{
		if(id < 0 || id >= buildings.Count)
			return;
		buildings[id].Hover(on);
	}

	public bool ContainsLocalCoordinates(int2 pos)
	{
		return grid.contains(pos.x, pos.y);
	}

	public bool ContainsGlobalCoordinates(int2 pos)
	{
		return grid.contains (pos.x - Mathf.FloorToInt(transform.position.x), pos.y - Mathf.FloorToInt(transform.position.z));
	}

	public bool FindPath(int2 start, int2 end, Queue<int2> path, out int distance, bool clearPrevious = true)
	{
		Debug.Assert (this.ContainsLocalCoordinates (start) == true);
		Debug.Assert (this.ContainsLocalCoordinates (end) == true);
		
		if(start == end)
		{
			distance = 0;
			return true;
		}

		int dMax = width*width;
		List<int> distanceMap = new List<int>();
		for(int iy = 0; iy < width; iy++)
		{
			for(int ix = 0; ix < width; ix++)
			{
				if(grid.type(ix,iy) == tileType.street)
					distanceMap.Add(int.MaxValue);
				else
					distanceMap.Add(-1);
			}
		}

		bool pathfound = false;
		Queue<int2> toVisit = new Queue<int2>();
		int x = end.x;
		int y = end.y;
		int2 current = new int2();
		distanceMap[x + y*width] = 0;
		toVisit.Enqueue(end);
		
		int d = 0;
		distance = -1;
		int2[] neigh = new int2[4] {new int2(1,0), new int2(0,1), new int2(-1,0), new int2(0,-1)};
		while(pathfound == false)
		{
			int countToVisit = toVisit.Count;
			if (++d >= dMax)
				return false;
			
			for(int i=0; i<countToVisit; i++)
			{
				current = toVisit.Dequeue();

				for(int j=0; j<4; j++)
				{
					x = current.x + neigh[j].x;
					y = current.y + neigh[j].y;
					if(grid.contains(x,y) && distanceMap[x + y*width] > d)
					{
						distanceMap[x + y*width] = d;
						toVisit.Enqueue(new int2(x,y));
						if(x == start.x && y == start.y)
							pathfound = true;
					}
				}
			}
		}

		if(clearPrevious == true) 
			path.Clear();
		
		distance = d;
		current = start;
		int2 blockOffset = new int2(Mathf.FloorToInt(this.transform.position.x), Mathf.FloorToInt(this.transform.position.z));
		path.Enqueue(current + blockOffset);
		while(current != end)
		{
			d--;
			int ran = Random.Range(0,4);
			for(int i=0; i<4; i++)
			{
				int j = (i+ran)%4;
				x = current.x + neigh[j].x;
				y = current.y + neigh[j].y;
				if(grid.contains(x,y) && distanceMap[x+y*width] == d)
					break;
			}
			current = new int2(x, y);
			path.Enqueue(current + blockOffset);
		}

		return true;
	}

	public bool FindPath(int2 start, int2 end, Queue<int2> path)
	{
		int d = new int();
		return this.FindPath(start, end, path, out d);
	}

	public int2 GridPos()
	{
		return new int2(Mathf.FloorToInt(this.transform.position.x), Mathf.FloorToInt(this.transform.position.z)) / width;
	}

	public Building GetBuilding(int id)
	{
		if (id >= buildings.Count)
			return null;

		return buildings [id];
	}
}












