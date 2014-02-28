﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
	public Texture selkies, nereides, humans;
	private List<string> raceList = new List<string>();
	private List<MainMenuScrollView> objectList = new List<MainMenuScrollView> ();
	private List<string> chosenRaces = new List<string>();

	void Awake () 
	{
		string[] strArr = new string[3] {"Player 1", "Player 2", "Player 3"};

		raceList.Add ("None");
		raceList.Add ("Humans");
		raceList.Add ("Selkies");
		raceList.Add ("Nereides");
	
		for(int i = 0; i < 3; ++i)
		{
			MainMenuScrollView scrollObject = new MainMenuScrollView();

			scrollObject.raceChooseList = GameObject.Find (strArr[i]).GetComponent<UIPopupList>();
			scrollObject.raceIcon = scrollObject.raceChooseList.transform.FindChild("Race Icon").GetComponent<UISprite>();
			scrollObject.currentRace = scrollObject.raceChooseList.transform.FindChild("Selected Race Label").GetComponent<UILabel>();
			scrollObject.currentRace.text = raceList[0];

			objectList.Add (scrollObject);
		}

		ChangeOptions ();
	}

	public void FindCurrentObject()
	{
		int selectedScroll = -1;

		chosenRaces.Clear ();

		for(int i = 0; i < 3; ++i)
		{
			if(objectList[i].currentRace.text != null && objectList[i].currentRace.text != "None")
			{
				chosenRaces.Add (objectList[i].currentRace.text);
			}
			if(UIPopupList.current.gameObject == objectList[i].raceChooseList.gameObject)
			{
				selectedScroll = i;
			}
		}
		
		ChangeOptions();
		ShowSymbol(selectedScroll);
	}

	void ChangeOptions()
	{
		for(int i = 0; i < 3; ++i)
		{
			objectList[i].raceChooseList.items.Clear ();

			for(int j = 0; j < raceList.Count; ++j)
			{
				if(chosenRaces.Contains (raceList[j]) == false)
				{
					objectList[i].raceChooseList.items.Add(raceList[j]);
				}
			}
		}
	}

	void ShowSymbol(int i)
	{
		if(objectList[i].currentRace.text != null)
		{
			switch(objectList[i].currentRace.text)
			{
			case "Humans":
				//TODO
				break;
			case "Selkies":
				objectList[i].raceIcon.spriteName = "Selkies Race Symbol";
				break;
			case "Nereides":
				objectList[i].raceIcon.spriteName = "Nereides Racial Symbol (Flat)";
				break;
			default:
				break;
			}
		}
	}

	public void SetGameInfo()
	{
		UILabel size = GameObject.Find ("Size Label").GetComponent<UILabel> ();

		if(size.text != "-" && objectList[0].currentRace.text != "None" && objectList[1].currentRace.text != "None")
		{
			PlayerPrefs.DeleteAll ();

			switch(size.text)
			{
			case "Very Small (30 Systems)":
				PlayerPrefs.SetInt ("Map Size", 30);
				break;
			case "Small (60 Systems)":
				PlayerPrefs.SetInt ("Map Size", 60);
				break;
			case "Medium (90 Systems)":
				PlayerPrefs.SetInt ("Map Size", 90);
				break;
			case "Large (120 Systems)":
				PlayerPrefs.SetInt ("Map Size", 120);
				break;
			case "Very Large (150 Systems)":
				PlayerPrefs.SetInt ("Map Size", 150);
				break;
			case "Massive (180 Systems)":
				PlayerPrefs.SetInt ("Map Size", 180);
				break;
			default:
				break;
			}

			PlayerPrefs.SetString ("Player Race", objectList [0].currentRace.text);
			PlayerPrefs.SetString ("AI One", objectList [1].currentRace.text);
			PlayerPrefs.SetString ("AI Two", objectList [2].currentRace.text);

			Application.LoadLevel("Crucible");
		}
	}
}

public class MainMenuScrollView
{
	public UIPopupList raceChooseList;
	public UISprite raceIcon;
	public UILabel currentRace;
}