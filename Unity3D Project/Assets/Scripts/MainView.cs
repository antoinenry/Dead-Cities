using UnityEngine;
using System.Collections;

public class MainView : MonoBehaviour
{
	public float stalkSpeed;
	public float rotateSpeed;
	public float leanSpeed;
	public float raiseSpeed;


	public float distance;
	public float altitude;
	public int angle = 4;

	float[] angles = new float[8] {0f, Mathf.PI/4f, Mathf.PI/2f, 3*Mathf.PI/4f, Mathf.PI, 5*Mathf.PI/4f, 3*Mathf.PI/2f, 7*Mathf.PI/4f};
	bool rightKeyUp = true;
	bool leftKeyUp = true;
	bool rotating = false;
	bool stalking = false;

	Vector3 lookAt;
	Transform target;
	Vector3 offset;

	void Start ()
	{
		transform.position = new Vector3(5f, altitude, 5f);
		Vector3 offset = new Vector3(distance * Mathf.Cos(angles[angle]), altitude, distance * Mathf.Sin(angles[angle]));
		lookAt = transform.position - offset;
		transform.LookAt(lookAt);
	}

	void Update ()
	{
		CameraControls();
		if(stalking)
			lookAt = Vector3.Lerp(lookAt, target.position, stalkSpeed * Time.deltaTime);
		Stalk ();
		if(rotating)
			Rotate();
	}

	void Stalk()
	{
		Vector3 offset2be = new Vector3(distance * Mathf.Cos(angles[angle]), altitude, distance * Mathf.Sin(angles[angle]));
		offset = Vector3.Lerp(offset, offset2be, stalkSpeed * Time.deltaTime);
		transform.position = lookAt + offset;
	}

	void CameraControls()
	{
		if(Input.GetAxisRaw("Horizontal") == 1.0f)
		{
			if(rightKeyUp)
				StartRotating(false);
			rightKeyUp = false;
		}
		else rightKeyUp = true;

		if(Input.GetAxisRaw("Horizontal") == -1.0f)
		{
			if(leftKeyUp)
				StartRotating(true);
			leftKeyUp = false;
		}
		else leftKeyUp = true;

		if(Input.GetAxisRaw("Vertical") == 1.0f)
			Lean(true);

		if(Input.GetAxisRaw("Vertical") == -1.0f)
			Lean(false);

		if (Input.GetAxisRaw ("Mouse ScrollWheel") > 0f)
			Raise (false);

		if (Input.GetAxisRaw ("Mouse ScrollWheel") < 0f)
			Raise (true);
	}

	void StartRotating(bool clockwise)
	{
		rotating = true;
		if(clockwise)
			angle = (angle+7)%8;
		else
			angle = (angle+1)%8;
	}

	void Rotate()
	{
		Vector3 offset2be = new Vector3(distance * Mathf.Cos(angles[angle]), altitude, distance * Mathf.Sin(angles[angle]));
		offset = Vector3.Lerp(offset, offset2be, rotateSpeed * Time.deltaTime);
		transform.LookAt(transform.position - offset);
		transform.position = lookAt + offset;

		if(Vector3.Distance(offset, offset2be) <= .01f)
			rotating = false;
	}

	void Lean(bool forward)
	{
		if(forward && distance > .5f)
			distance -= leanSpeed * Time.deltaTime;
		if(!forward && distance < 10.0f)
			distance += leanSpeed * Time.deltaTime;

		transform.LookAt(transform.position - offset);
	}

	void Raise(bool up)
	{
		if(up && altitude < 20f)
			altitude += raiseSpeed * Time.deltaTime;
		else if(altitude > 6f)
			altitude -= raiseSpeed * Time.deltaTime;
	}

	public void ToggleStalking(Transform toStalk)
	{
		if(target == toStalk)
			stalking = !stalking;
		else
		{
			target = toStalk;
			stalking = true;
		}
	}

	public int2 GetLookAt()
	{
		return new int2 ((int)lookAt.x, (int)lookAt.z);
	}
}


