using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmbientStarRandomiser : MasterScript 
{
	public int totalStars;
	public float maxGenerateDistance;
	public GameObject ambientStar;
	private List<GameObject> ambientStarList = new List<GameObject> ();
	private StarColourChange starColourChange;
	private Color32 humanColour = new Color(0, 255, 0, 1);
	private Color32 selkiesColour = new Color(255, 0, 0, 1);
	private Color32 nereidesColour = new Color(0, 106, 255, 1);
	private Color32 defaultColour = new Color(255, 105, 0, 1);

	public void GenerateStars () 
	{
		int ambientStarsPerSystem = totalStars / systemListConstructor.systemList.Count;

		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			for(int j = 0; j < ambientStarsPerSystem; ++j)
			{
				float systemX = systemListConstructor.systemList[i].systemObject.transform.position.x;
				float systemY = systemListConstructor.systemList[i].systemObject.transform.position.y;

				float xDis = Random.Range(systemX - maxGenerateDistance, systemX + maxGenerateDistance);
				float yDis = Random.Range(systemY - maxGenerateDistance, systemY + maxGenerateDistance);

				Vector3 location = new Vector3(xDis, yDis, 0.0f);

				GameObject star = Instantiate(ambientStar, location, Quaternion.identity) as GameObject;

				ambientStarList.Add (star);

				star.transform.parent = GameObject.Find ("Ambient Star Container").transform;
			}
		}
	}

	public void AmbientColourChange(int system)
	{
		for(int i = 0; i < ambientStarList.Count; ++i)
		{
			float distance = Vector3.Distance(systemListConstructor.systemList[system].systemObject.transform.position, ambientStarList[i].transform.position);

			if(distance < maxGenerateDistance / 2)
			{
				starColourChange = ambientStarList[i].GetComponent<StarColourChange>();

				starColourChange.colourA = starColourChange.thisLight.color;

				switch(systemListConstructor.systemList[system].systemOwnedBy)
				{
				case "Humans":
					starColourChange.colourB = humanColour;
					break;
				case "Selkies":
					starColourChange.colourB = selkiesColour;
					break;
				case "Nereides":
					starColourChange.colourB = nereidesColour;
					break;
				default:
					starColourChange.colourB = defaultColour;
					break;
				}

				starColourChange.changeColour = true;
			}
		}
	}
}
