using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TeamPanel : MonoBehaviour
{
	public float buttonHeight = 20f;
	public int size;
	public GameObject[] buttons;
	public int maxSize;

	void Start ()
	{
		SetSize (0);
	}

	public void SetSize(int nButtons)
	{
		size = nButtons;
		RectTransform rt = GetComponent<RectTransform>();
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * buttonHeight + 10f);
		for(int i = 0; i < size; i++)
			buttons[i].SetActive(true);
		for(int i = size; i < maxSize; i++)
			buttons[i].SetActive(false);
	}

	public void AddMember(Human newHero)
	{
		SetSize(size+1);
		Image colorLabel = buttons[size-1].GetComponentsInChildren<Image>()[1];
		colorLabel.color = newHero.color;
		Text nameLabel = buttons[size-1].GetComponentInChildren<Text>();
		nameLabel.text = newHero.name;
	}
}
