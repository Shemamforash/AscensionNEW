using UnityEngine;
using System.Collections;

public class StarColourChange : MasterScript 
{
	public Color32 colourA;
	public Color32 colourB;
	private Color32 zeroColour = new Color (0, 0, 0);
	public bool changeColour;
	public Light thisLight;
	private float timer;
	
	void Awake()
	{
		thisLight = gameObject.GetComponent<Light> ();
	}

	void Update () 
	{
		if(changeColour == true)
		{
			if(timer == 0.0f)
			{
				timer = Time.time;
			}

			thisLight.color = Color.Lerp(colourA, colourB, 1f);

			thisLight.color = colourB;

			if(thisLight.color == colourB || timer + 1.0f < Time.time)
			{
				thisLight.color = colourB;
				changeColour = false;
				timer = 0.0f;
			}
		}
	}
}
