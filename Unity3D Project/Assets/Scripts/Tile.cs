using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
	public float color;
	MeshRenderer mr;

	void Start ()
	{
		mr = GetComponent<MeshRenderer>();
		mr.material.color = new Color(color * .5f, color * .5f, color * .5f);
	}

	public void Hover(bool on)
	{
		if(mr == null) return;
		if(on)
			mr.material.color = new Color(color, color, color);
		else
			mr.material.color = new Color(color * .5f, color * .5f, color * .5f);
	}

	void Update ()
	{
	}
}
