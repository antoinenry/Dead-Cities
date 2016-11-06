using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{ 
	public GameObject blockObject;
	public GameObject heroObject;

	public int citySize;
	public int blockSize;
	public int deadEndRatio;
	
	MainView view;
	Dictionary<int2, CityBlock> cityBlocks;
	List<Human> heroes;

	void Start ()
	{
		GameObject ob = GameObject.FindGameObjectWithTag("MainCamera");
		view = ob.GetComponent<MainView>();
		cityBlocks = new Dictionary<int2, CityBlock>(citySize);
		heroes = new List<Human>();

		AddNewBlock(0, 0);
		AddNewBlock(-1, 0);
		AddNewBlock(-1, -1);
		AddNewBlock(0, -1);
		AddNewBlock(1, 0);
		AddNewBlock(0, 1);
		AddNewBlock(1, 1);
		AddNewBlock(-1, 1);
		AddNewBlock(1, -1);
	}

	void Update()
	{
		for(int k = 0; k < heroes.Count; k++)
		{
			int2 heroPos = heroes[k].gPos;
			heroPos = heroPos.DivideAndFloor (blockSize);

			CityBlock cb;
			if (cityBlocks.TryGetValue (heroPos, out cb) == true)
			{
				if (cb.visited == false)
				{
					cb.visited = true;
					int2[] neigh = new int2[8] {new int2(1,0), new int2(0,1), new int2(-1,0), new int2(0,-1), new int2(1,1), new int2(1,-1), new int2(-1,-1), new int2(1,-1)};
					for (int i = 0; i < 8; i++)
					{
						if (cityBlocks.TryGetValue (heroPos + neigh [i], out cb) == false)
							AddNewBlock (heroPos + neigh [i]);
					}
				}
			}
		}
	}

	void AddNewBlock(int x, int y)
	{
		int2 at = new int2(x, y);
		GameObject instance = Instantiate(blockObject, new Vector3 (x * blockSize, 0f, y * blockSize), Quaternion.identity) as GameObject;
		instance.name = "City Block (" + x + ", " + y + ")";
		CityBlock cb = instance.GetComponent<CityBlock>();
		cb.transform.SetParent (this.transform);

		int2[] neigh = new int2[4] { new int2(0,-1), new int2(-1,0), new int2(0,1), new int2(1,0) };
		CityBlock othercb;
		for(int i = 0;  i < 4; i++)
		{
			if(cityBlocks.TryGetValue(at + neigh[i], out othercb) == true)
			{
				cb.mainConnections[i] = othercb.mainConnections[(i+2)%4];
				if(Random.Range(0, 100) > deadEndRatio)
					cb.secondaryConnections[i] = othercb.secondaryConnections[(i+2)%4];
			}
		}

		cb.BuildBlock(blockSize);
		cityBlocks.Add(at, cb);
	}

	void AddNewBlock(int2 pos)
	{
		AddNewBlock (pos.x, pos.y);
	}

	public Human AddNewHero(int x, int y)
	{
		GameObject instance = Instantiate(heroObject, new Vector3 (x+.5f,.1f,y+.5f), Quaternion.identity) as GameObject;
		Human h = instance.GetComponent<Human>();
		h.SetPos(x, y);
		h.id = heroes.Count;
		Color[] colors = new Color[6] {Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow};
		h.color = colors[Random.Range(0,6)];
		string[] names = new string[6] { "Marcelle", "Marcel", "Marceline", "Marcelin", "Marcelio", "Marcelette" };
		h.name = names[Random.Range(0,6)];
		heroes.Add(h);

		return h;
	}

	public void ToggleStalking(int ID)
	{
		view.ToggleStalking(heroes[ID].transform);
	}

	public bool FindPath(int heroID, int2 destination)
	{
		if(heroID > heroes.Count) return false;

		CityBlock endBlock;

		if(cityBlocks.TryGetValue(destination.DivideAndFloor(blockSize), out endBlock) == false)
			return false;

		CityBlock startBlock;
		int2 start = heroes[heroID].gPos;
		if(cityBlocks.TryGetValue(start.DivideAndFloor(blockSize), out startBlock) == false)
			Debug.Assert(false);

		if(endBlock == startBlock)
			return startBlock.FindPath(start%blockSize, destination%blockSize, heroes[heroID].path);

		Queue<int2> path = new Queue<int2>();
		CityBlock currentBlock = startBlock;
		int2 connection1 = new int2();
		int2 connection2 = new int2();
		heroes[heroID].path.Clear();
		start = start % blockSize;

		while(currentBlock != endBlock)
		{
			if(endBlock.GridPos().x > currentBlock.GridPos().x)
			{
				connection1.Set(blockSize-1, currentBlock.mainConnections[3] * 2);
				if(start.y > connection1.y) connection1.y += 1;
				connection2.Set(blockSize-1, currentBlock.secondaryConnections[3]);

				if(FindBestPathAndAdd(currentBlock, start, connection1, connection2, heroID, out connection1) == false)
					return false;

				start.Set(0, connection1.y);
				if (cityBlocks.TryGetValue (currentBlock.GridPos () + new int2 (1, 0), out currentBlock) == false)
					return false;
			}
			if(endBlock.GridPos().x < currentBlock.GridPos().x)
			{
				connection1.Set(0, currentBlock.mainConnections[1] * 2);
				if(start.y > connection1.y) connection1.y += 1;
				connection2.Set(0, currentBlock.secondaryConnections[1]);
				
				if(FindBestPathAndAdd(currentBlock, start, connection1, connection2, heroID, out connection1) == false)
					return false;
				
				start.Set(blockSize-1, connection1.y);
				if(cityBlocks.TryGetValue(currentBlock.GridPos() + new int2(-1, 0), out currentBlock) == false)
					return false;
			}
			if(endBlock.GridPos().y > currentBlock.GridPos().y)
			{
				connection1.Set(currentBlock.mainConnections[2] * 2, blockSize-1);
				if(start.x > connection1.x) connection1.x += 1;
				connection2.Set(currentBlock.secondaryConnections[2], blockSize-1);
				
				if(FindBestPathAndAdd(currentBlock, start, connection1, connection2, heroID, out connection1) == false)
					return false;
				
				start.Set(connection1.x, 0);
				if(cityBlocks.TryGetValue(currentBlock.GridPos() + new int2(0, 1), out currentBlock) == false)
					return false;
			}
			if(endBlock.GridPos().y < currentBlock.GridPos().y)
			{
				connection1.Set(currentBlock.mainConnections[0] * 2, 0);
				if(start.x > connection1.x) connection1.x += 1;
				connection2.Set(currentBlock.secondaryConnections[0], 0);
				
				if(FindBestPathAndAdd(currentBlock, start, connection1, connection2, heroID, out connection1) == false)
					return false;
				
				start.Set(connection1.x, blockSize-1);
				if(cityBlocks.TryGetValue(currentBlock.GridPos() + new int2(0, -1), out currentBlock) == false)
					return false;
			}
		}

		if (endBlock.FindPath (start, destination % blockSize, path) == false)
			return false;
		
		while(path.Count > 0)
			heroes[heroID].path.Enqueue(path.Dequeue());

		return true;
	}

	bool FindBestPathAndAdd(CityBlock block, int2 start, int2 end1, int2 end2, int heroID, out int2 bestEnd)
	{
		Queue<int2> path1 = new Queue<int2>();
		Queue<int2> path2 = new Queue<int2>();
		int distance1 = new int();
		int distance2 = new int();
		bool found1 = false;
		bool found2 = false;
		bestEnd = null;

		found1 = block.FindPath(start, end1, path1, out distance1);
		found2 = block.FindPath(start, end2, path2, out distance2);

		if (found1 == false)
			return false;

		if(found2 == false || distance1 <= distance2)
		{
			bestEnd = end1;
			while(path1.Count > 0)
				heroes[heroID].path.Enqueue(path1.Dequeue());
		}
		else
		{
			bestEnd = end2;
			while(path2.Count > 0)
				heroes[heroID].path.Enqueue(path2.Dequeue());
		}
		return true;
	}

	public bool FindPath(int heroID, Building destBuilding)
	{
		CityBlock destBlock = destBuilding.GetComponentInParent <CityBlock> ();
		int2 destination = destBlock.GridPos() * blockSize + destBuilding.door.OutsideTile();

		if (FindPath (heroID, destination) == false)
			return false;

		heroes [heroID].toDoList.Enqueue (new Action(action_type.enter_building, destBuilding.door.gameObject));
		heroes [heroID].path.Enqueue (destBlock.GridPos () * blockSize + destBuilding.door.InsideTile());
		destBuilding.IncrementHeroCount ();

		return true;
	}

	public void ActionEffect(Action action, Actor actor)
	{
		switch (action.type)
		{
		case action_type.enter_building:
			action.target.GetComponent<Door> ().open = true;
			actor.isOutside = false;
			break;
		}
	}
}







