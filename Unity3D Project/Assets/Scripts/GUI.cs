using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

enum selectable {none, hero, streetTile, buildingTile, building};

public class GUI : MonoBehaviour
{
	GameManager GM;
	selectable hoveredType = selectable.none;
	int hoveredID = -1;
	int selectedID = -1;
	int2 gridMousePosition;
	CityBlock hoveredBlock;
	Building hoveredBuilding;

	bool leftClicUp = true;

	void Start ()
	{
		GameObject ob = GameObject.FindGameObjectWithTag("GameManager");
		GM = ob.GetComponent<GameManager>();
		gridMousePosition = new int2();
	}

	void Update ()
	{
		HoverAction();
		ClickAction();

		if (Input.GetButton ("Cancel") == true)
			Application.Quit ();
	}

	void HoverAction()
	{
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit mouseHit;
		int id = -1;
		selectable type = selectable.none;
		CityBlock block = null;
		Building building = null;
		Human human = null;

		if(EventSystem.current.IsPointerOverGameObject () == false 
			&& Physics.Raycast (camRay, out mouseHit, 100f, LayerMask.GetMask ("Clickable")))
		{
			gridMousePosition.x = Mathf.FloorToInt (mouseHit.point.x);
			gridMousePosition.y = Mathf.FloorToInt (mouseHit.point.z);

			human = mouseHit.collider.GetComponent<Human>();
			if (human != null)
			{
				id = human.id;
				type = selectable.hero;
			}
			else
			{
				block = mouseHit.collider.transform.parent.GetComponent<CityBlock> ();
				building = mouseHit.collider.transform.parent.GetComponent<Building> ();

				if (block != null)
				{
					type = selectable.streetTile;
					id = block.GetTileId (gridMousePosition % block.width);
				}
				else if (building != null)
				{
					block = building.GetComponentInParent<CityBlock> ();
					if (building.ShowingInteriorView () == true)
					{
						type = selectable.buildingTile;
						id = building.GetTileId (gridMousePosition % block.width);
					}
					else
					{
						type = selectable.building;
						id = building.id;
					}
				}
			}
		}

		if(type != hoveredType || id != hoveredID || block != hoveredBlock || building != hoveredBuilding)
		{
			SwitchHovered(false);
			hoveredType = type;
			hoveredID = id;
			hoveredBlock = block;
			hoveredBuilding = building;
			SwitchHovered(true);
		}
	}

	void SwitchHovered(bool on)
	{
		switch(hoveredType)
		{
		case selectable.building: hoveredBlock.HoverBuilding(hoveredID, on);
			break;
		case selectable.hero: 
			break;
		case selectable.streetTile:
			hoveredBlock.HoverStreetTile (hoveredID, on);
			break;
		case selectable.buildingTile:
			hoveredBuilding.HoverTile (hoveredID, on);
			break;
		}
	}

	void ClickAction()
	{
		if(Input.GetButton("Fire1") == true && leftClicUp)
		{
			leftClicUp = false;
			switch(hoveredType)
			{
			case selectable.streetTile:
				if(Input.GetButton("Jump") == true)
				{
					Human h = GM.AddNewHero(gridMousePosition.x, gridMousePosition.y);
					TeamPanel tp = GetComponentInChildren<TeamPanel>();
					tp.AddMember(h);
				}
				else
					GM.FindPath(selectedID, gridMousePosition);
				break;
			case selectable.buildingTile:
				GM.FindPath(selectedID, hoveredBlock.GetBuilding(hoveredID));
				break;
			case selectable.building:
				GM.FindPath(selectedID, hoveredBlock.GetBuilding(hoveredID));
				break;
			case selectable.hero:
				SelectHero(hoveredID);
				break;
			}
		}
		if(Input.GetButton("Fire1") == false)
			leftClicUp = true;
	}

	public void SelectHero(int id)
	{
		if(selectedID == id)
			GM.ToggleStalking(id);
		else
			selectedID = id;
	}
}
