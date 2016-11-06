using UnityEngine;

public class int2
{
	public int x;
	public int y;
	public int2(int xval=0, int yval=0)
	{
		x = xval;
		y = yval;
	}
	public void Set(int xval, int yval)
	{
		x = xval;
		y = yval;
	}
	public override bool Equals(System.Object obj)
	{
		if (obj == null)
			return false;
		
		int2 p = obj as int2;
		if ((System.Object)p == null)
			return false;
		
		return (x == p.x) && (y == p.y);
	}
	
	public bool Equals(int2 p)
	{
		if ((object)p == null)
			return false;
		
		return (x == p.x) && (y == p.y);
	}
	
	public override int GetHashCode()
	{
		return x ^ y;
	}
	public static bool operator ==(int2 a, int2 b)
	{
		return a.x == b.x && a.y == b.y;
	}
	public static bool operator !=(int2 a, int2 b)
	{
		return !(a == b);
	}
	public static int2 operator +(int2 a, int2 b)
	{
		return new int2(a.x + b.x, a.y + b.y);
	}
	public static int2 operator -(int2 a, int2 b)
	{
		return new int2(a.x - b.x, a.y - b.y);
	}
	public static int2 operator *(int2 a, int f)
	{
		return new int2(a.x * f, a.y * f);
	}
	public static int2 operator /(int2 a, int f)
	{
		return new int2(a.x / f, a.y / f);
	}
	public static int2 operator %(int2 a, int f)
	{
		int2 result = new int2(a.x % f, a.y % f);

		if (result.x < 0)
			result.x += f;
		if (result.y < 0)
			result.y += f;
		
		return result;
	}
	public int2 DivideAndFloor(int f)
	{
		float xf = (float)this.x;
		float yf = (float)this.y;
		float ff = (float)f;
		int2 result = new int2 ();

		result.x = Mathf.FloorToInt (xf / ff);
		result.y = Mathf.FloorToInt (yf / ff);

		return result;
	}
	public int Area()
	{
		return x * y;
	}
}






