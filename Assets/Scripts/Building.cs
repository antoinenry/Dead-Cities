using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Building : MonoBehaviour
{
	public GameObject tranparentObject;
	public GameObject wallObject;
	public GameObject roomFloorObject;
	public GameObject gridColliderObject;
	public int x0; // x position within the city block
	public int y0; // y position within the city block
	public int w; // size along x
	public int h; // size along y
	public int height;
	public int id;
	public Door door;

	MeshRenderer externRenderer;
	Collider externCollider;
	Color color;
	GameObject interior;
	GameObject transparent;
	bool showingInterior = false;

	List<BuildingTile> tiles;
	TileGrid grid;
	int heroCount = -1;

	public void Build()
	{
		BuildExterior();
	}

	void BuildExterior()
	{
		externRenderer = GetComponentInChildren<MeshRenderer>();
		externCollider = GetComponentInChildren<Collider> ();

		float rgb = Random.Range (.3f, .8f);
		color = new Color (rgb, rgb, rgb, 1f);
		externRenderer.material.color = color * new Color(.8f, .8f, .8f);

		externRenderer.transform.localScale = new Vector3(w, height, h);
		externRenderer.transform.localPosition = new Vector3(w/2f, (float)height/2f, h/2f);
	}

	void BuildTransparent()
	{
		transparent = Instantiate(tranparentObject, new Vector3 (0f,0f,0f), Quaternion.AngleAxis(0f, new Vector3(1.0f,0f,0f))) as GameObject;
		transparent.name = "Transparent Exterior";
		transparent.transform.SetParent(this.transform);
		transparent.transform.localScale = new Vector3(w, height, h);
		transparent.transform.localPosition = new Vector3(w/2f, (float)height/2f, h/2f);
	}

	void BuildInterior()
	{
		interior = new GameObject();
		interior.name = "Interior";
		interior.transform.SetParent(this.transform);
		interior.transform.localPosition = Vector3.zero;

		SetGrid ();
		BuildFloor();
		BuildRandomWalls();
	}

	void SetGrid()
	{
		tiles = new List<BuildingTile> ();
		grid = new TileGrid(w, h);

		GameObject instance = Instantiate(gridColliderObject, new Vector3 (0f,0f,0f), Quaternion.identity) as GameObject;
		instance.transform.SetParent(this.transform);
		instance.transform.localPosition = new Vector3 ((float)w/2f, 0.1f, (float)h/2f);
		instance.transform.localScale = new Vector3 ((float)w/10f, 1f, (float)h/10f);
	}

	void BuildRandomWalls()
	{
		int x = Random.Range(0, w+1);
		if(x != 0 && x != w) BuildWall(x, 0, x, h);
		int y = Random.Range(0, h+1);
		if(x != 0 && y != 0 && y != h) BuildWall(0, y, x, y);
		y = Random.Range(0, h+1);
		if(x != w && y != 0 && y != h) BuildWall(x, y, w, y);
	}

	void BuildWall(int x1, int y1, int x2, int y2)
	{
		GameObject instance = Instantiate(wallObject, new Vector3 (0f,0f,0f), Quaternion.identity) as GameObject;
		instance.transform.SetParent(interior.transform);

		float x = ((float)x1 + (float)x2)/2f;
		float y = ((float)y1 + (float)y2)/2f;
		instance.transform.localPosition = new Vector3 (x,.5f,y);

		float xscale = Mathf.Max(Mathf.Abs(x1-x2), .05f);
		float yscale = Mathf.Max(Mathf.Abs(y1-y2), .05f);
		instance.transform.localScale = new Vector3(xscale, 1f, yscale);

		if (y1 == y2)
		{
			for (int ix = Mathf.Min(x1, x2); ix < Mathf.Max(x1, x2); ix++)
			{
				tiles [grid.id (ix, y1 - 1)].ySupWall = true;
				tiles [grid.id (ix, y1)].yInfWall = true;
			}
		}
		else
		{
			for (int iy = Mathf.Min(y1, y2); iy < Mathf.Max(y1, y2); iy++)
			{
				tiles [grid.id (x1 - 1, iy)].xSupWall = true;
				tiles [grid.id (x1, iy)].xInfWall = true;
			}
		}
	}

	void BuildFloor()
	{
		for(int ix = 0; ix < w; ix++)
		{
			for(int iy = 0; iy < h; iy++)
			{
				int tileID = BuildTile (ix, iy);
				grid.set (ix, iy, tileType.building, tileID);
			}
		}
	}

	int BuildTile(int x, int y)
	{
		GameObject instance = Instantiate(roomFloorObject, new Vector3 (0f, 0f, 0f), Quaternion.AngleAxis(90f, new Vector3(1.0f,0f,0f))) as GameObject;
		instance.transform.SetParent(interior.transform);
		instance.transform.localPosition = new Vector3 (x+.5f,0f,y+.5f);

		tiles.Add(instance.GetComponent<BuildingTile>());
		return tiles.Count - 1;
	}

	// Interface
	public bool ShowingInteriorView()
	{
		return showingInterior;
	}

	public int GetTileId (int2 pos)
	{
		if (grid.contains (pos.x - x0, pos.y - y0) == false)
			return -1;
		return grid.id (pos.x - x0, pos.y - y0);
	}

	public int GetTileId (int x, int y)
	{
		return GetTileId(new int2(x, y));
	}

	public void Hover(bool on)
	{
		if (on)
			externRenderer.material.color = color;
		else
			externRenderer.material.color = color * new Color(.8f, .8f, .8f, 0f);
	}

	public void HoverTile (int tileID, bool on)
	{
		tiles [tileID].Hover (on);
	}

	void ToggleInteriorView(bool show)
	{
		interior.SetActive (show);
		externRenderer.shadowCastingMode = show ? UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly : UnityEngine.Rendering.ShadowCastingMode.On;
		externCollider.enabled = !show;
		showingInterior = show;
	}

	public void IncrementHeroCount()
	{
		if (heroCount == -1)
		{
			BuildInterior ();
			BuildTransparent ();
			heroCount = 0;
		}

		if (heroCount == 0)
			ToggleInteriorView (true);
		heroCount++;
	}

	public void DecrementHeroCount()
	{
		heroCount--;
		if (heroCount == 0)
			ToggleInteriorView (false);
	}
		
}








