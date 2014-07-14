using UnityEngine;
using System.Collections;

public static class MathsFunctions
{
	public static bool CheckPointIsCloseToPoint(Vector3 pointA, Vector3 pointB)
	{
		if(pointA.x - 0.1f <= pointB.x && pointA.x + 0.1f >= pointB.x)
		{
			if(pointA.y - 0.1f <= pointB.y && pointA.y + 0.1f >= pointB.y)
			{
				return true;
			}
		}
		
		return false;
	}

	public static float RotationOfLine(Vector3 pointA, Vector3 pointB)
	{
		float xDif = pointA.x - pointB.x;
		float yDif = pointA.y - pointB.y;
		float angle = Mathf.Atan (yDif / xDif);
		
		angle = angle * Mathf.Rad2Deg;
		
		if(xDif == 0 && yDif > 0) //If x equals zero and y is positive the angle is 90 degrees (vertical up)
		{
			angle = 90f;
		}
		
		if(xDif == 0 && yDif < 0) //If x equals zero and y is negative the angle is 270 degrees (vertical down)
		{
			angle = 270f;
		}
		
		if(yDif == 0 && xDif < 0) //If y equals zero and x is negative the angle is 180 degrees (horizontal back)
		{
			angle = 180;
		}
		
		if(yDif == 0 && xDif > 0) //If y equals zero and x is positive then angle is 360 degrees (horizontal forward
		{
			angle = 360;
		}
		
		if(xDif < 0f && yDif > 0f) //If x is negative and y is positive the angle is in the top left quadrant
		{
			angle = 180 + angle;
		}
		
		if(xDif < 0 && yDif < 0f) //If x is negative and y is negative the angle is in the bottom left quadrant
		{
			angle = 180 + angle;
		}
		
		if(xDif > 0f && yDif < 0f) //If x is positive and y is negative the angle is in the bottom right quadrant
		{
			angle = 360 + angle;
		}
		
		return angle;
	}

	public static Vector3 ABCLineEquation(Vector3 pointA, Vector3 pointB)
	{
		float A = pointB.y - pointA.y;
		float B = pointA.x - pointB.x;
		float C = (A * pointA.x) + (B * pointA.y);
		
		return new Vector3 (A, B, C);
	}

	public static Vector2 IntersectionOfTwoLines (Vector3 lineA, Vector3 lineB)
	{
		float determinant = (lineA.x * lineB.y) - (lineB.x * lineA.y);
		
		if(determinant == 0f)
		{
			return Vector2.zero;
		}
		
		float x = (lineB.y * lineA.z - lineA.y * lineB.z) / determinant;
		float y = (lineA.x * lineB.z - lineB.x * lineA.z) / determinant;
		
		Vector2 intersection = new Vector3(x, y);
		
		return intersection;
	}

	public static Vector3 PerpendicularLineEquation(Vector3 systemA, Vector3 systemB)
	{
		Vector3 midpoint = (systemA + systemB) / 2;
		float gradient = (systemB.y - systemA.y) / (systemB.x - systemA.x);
		
		if(systemB.x - systemA.x == 0)
		{
			return new Vector3(0, 1, midpoint.y);
		}
		
		if(systemB.y - systemA.y == 0f)
		{
			return new Vector3(1, 0, midpoint.x);
		}
		
		else
		{
			float perpGradient = -1/gradient;
			float yIntersect = midpoint.y - (perpGradient * midpoint.x);
			Vector3 perpSecondPoint = new Vector3(0, yIntersect, midpoint.z);
			return ABCLineEquation(midpoint, perpSecondPoint);
		}
	}

	public static bool PointLiesOnLine(Vector3 pointAVec3, Vector3 pointBVec3, Vector3 intersectionVec3)
	{	
		if(intersectionVec3.x -0.01f <= Mathf.Max(pointAVec3.x, pointBVec3.x) && intersectionVec3.x + 0.01f >= Mathf.Min (pointAVec3.x, pointBVec3.x))
		{
			if(intersectionVec3.y - 0.01f <= Mathf.Max(pointAVec3.y, pointBVec3.y) && intersectionVec3.y + 0.01f >= Mathf.Min (pointAVec3.y, pointBVec3.y))
			{
				return true;
			}
		}
		
		return false;
	}
}
