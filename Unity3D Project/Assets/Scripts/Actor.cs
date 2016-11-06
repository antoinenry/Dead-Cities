using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum action_type {nothing, enter_building, exit_building};

public class Action
{
	public action_type type;
	public GameObject target;
	public float completion;
	public float completionTime;
	public int2 location;

	public Action(action_type actionType, GameObject o = null)
	{
		float[] t = new float[2] { 0f, .8f };

		type = actionType;
		target = o;
		completionTime = t [(int)type];
		completion = 0f;
		location = new int2 ();
	}

	public void Update()
	{
		switch (type)
		{
		case action_type.enter_building:
			location = target.GetComponent<Door> ().OutsideTile ();
			break;
		}
	}
}

public class Actor : MonoBehaviour
{
	GameManager GM;

	public float speed = 2f;
	public int2 gPos;
	int2 gNext;
	public bool isOutside = true;
	public Queue<int2> path;
	public Queue<Action> toDoList;
	public int id;
	public Color color;

	void Start ()
	{
		GameObject ob = GameObject.FindGameObjectWithTag("GameManager");
		GM = ob.GetComponent<GameManager>();

		MeshRenderer mr = GetComponent<MeshRenderer> ();
		mr.material.color = color;
		toDoList = new Queue<Action> ();
	}

	void Update ()
	{
		gPos.x = Mathf.FloorToInt(transform.position.x);
		gPos.y = Mathf.FloorToInt(transform.position.z);

		Action nextAction = null;
		if (toDoList.Count > 0)
		{
			nextAction = toDoList.Peek ();
			nextAction.Update ();
		}

		if(gPos == gNext && path.Count > 0)
		{
			if (nextAction != null && nextAction.location == gNext)
			{
				PerformAction ();
			}
			else
				gNext = path.Dequeue();
		}
		transform.position = Vector3.MoveTowards(transform.position, new Vector3(gNext.x+.5f, .2f, gNext.y+.5f), speed*Time.deltaTime);
	}

	void PerformAction()
	{
		Action action = toDoList.Peek ();
		action.completion += Time.deltaTime;
		if (action.completion >= action.completionTime)
		{
			GM.ActionEffect (action, this);
			toDoList.Dequeue ();
		}
	}

	public int GetID()
	{
		return id;
	}

	public void SetPos(int x, int y)
	{
		transform.position = new Vector3((float)x+.5f, 0.1f, (float)y+.5f);
		gPos = new int2(x, y);
		path = new Queue<int2>();
		gNext = new int2(x, y);
	}
}
