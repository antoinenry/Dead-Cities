using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
	public bool open;
	int2 outsideTilePos;
	int2 insideTilePos;

	Transform pivot;

	public void Set(int2 outpos, int2 inpos)
	{
		outsideTilePos = outpos;
		insideTilePos = inpos;
	}

	void Start ()
	{
		pivot = null;
		Transform[] t = GetComponentsInChildren<Transform> ();
		int t_length = t.GetLength (0);
		for (int i = 0; i < t_length; i++) {
			if (t [i].name == "Pivot")
				pivot = t [i];
		}
		Debug.Assert (pivot != null);
	}

	void Update () 
	{
		Quaternion openQuaternion = Quaternion.AngleAxis(90f, new Vector3(0f, 1f, 0f));
		Quaternion closedQuaternion = Quaternion.identity;

		if (open == true && pivot.localRotation != openQuaternion)
		{
			if (Mathf.Abs (Quaternion.Angle (pivot.localRotation, openQuaternion)) > 10f)
				pivot.localRotation = Quaternion.RotateTowards (pivot.localRotation, openQuaternion, 10f);
			else
				pivot.localRotation = openQuaternion;
		}
		else if (open == false && pivot.localRotation != closedQuaternion)
		{
			if (Mathf.Abs (Quaternion.Angle (pivot.localRotation, closedQuaternion)) > 10f)
				pivot.localRotation = Quaternion.RotateTowards (pivot.localRotation, closedQuaternion, 10f);
			else
				pivot.localRotation = closedQuaternion;
		}
	}

	public int2 OutsideTile()
	{
		return outsideTilePos;
	}

	public int2 InsideTile()
	{
		return insideTilePos;
	}
}
