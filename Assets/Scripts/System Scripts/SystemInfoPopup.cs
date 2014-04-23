﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SystemInfoPopup : MasterScript 
{
	public string[] systemSize = new string[6]{"Inner Ring 1", "Inner Ring 2", "Inner Ring 3", "Inner Ring 4", "Inner Ring 5", "Inner Ring 6"};
	public GameObject overlayObject;
	public GameObject overlayContainer;
	private List<OverlayObject> overlayObjectList = new List<OverlayObject> ();
	public Camera mainCamera, uiCamera;
	private bool allfade;

	public void LoadOverlays () 
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			OverlayObject tempObj = new OverlayObject();

			tempObj.container = NGUITools.AddChild(overlayContainer, overlayObject);
			tempObj.power = tempObj.container.transform.Find ("Power").GetComponent<UILabel>();
			tempObj.knowledge = tempObj.container.transform.Find ("Knowledge").GetComponent<UILabel>();
			tempObj.name = tempObj.container.transform.Find ("Name").GetComponent<UILabel>();
			tempObj.planets = tempObj.container.transform.Find ("Planets Colonised").GetComponent<UISprite>();
			tempObj.overlayOuter = tempObj.container.transform.Find ("Overlay Outer").transform;

			tempObj.name.text = systemListConstructor.systemList[i].systemName.ToUpper();
			tempObj.planets.spriteName = systemSize[systemListConstructor.systemList[i].systemSize - 1];
			tempObj.rotSpeed = UnityEngine.Random.Range (2f, 6f);

			int rnd = UnityEngine.Random.Range(0,2);

			if(rnd == 0)
			{
				tempObj.rotDir = Vector3.forward;
			}
			if(rnd == 1)
			{
				tempObj.rotDir = Vector3.back;
			}

			TweenAlpha.Begin(tempObj.container, 0f, 0f);

			overlayObjectList.Add(tempObj);
		}
	}

	private void UpdatePosition(int i)
	{
		Vector3 position = mainCamera.WorldToViewportPoint (systemListConstructor.systemList[i].systemObject.transform.position);
		
		position = uiCamera.ViewportToWorldPoint (position);
		
		Vector3 newPosition = new Vector3(position.x, position.y, -37.0f);
		
		overlayObjectList[i].container.transform.position = newPosition;

		float scale = ((0.025f / systemListConstructor.systemScale) * mainCamera.transform.position.z) + 1.5f; //Orig 1.5f 

		overlayObjectList [i].container.transform.localScale = new Vector3 (scale / 1.5f, scale / 1.5f, 0f);
	}

	private void UpdateVariables(int i)
	{
		systemSIMData = systemListConstructor.systemList [i].systemObject.GetComponent<SystemSIMData> ();

		overlayObjectList [i].power.text = Math.Round(systemSIMData.totalSystemPower, 1).ToString ();
		overlayObjectList [i].knowledge.text = Math.Round(systemSIMData.totalSystemKnowledge, 1).ToString();

		float colonisedPlanets = 0;
		
		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true)
			{
				++colonisedPlanets;
			}
		}

		overlayObjectList[i].planets.fillAmount = (1f/systemListConstructor.systemList[i].systemSize) * colonisedPlanets;
	}

	void Update () 
	{
		if(mainCamera.transform.position.z > -65f)
		{
			for(int i = 0; i < overlayObjectList.Count; ++i)
			{
				if(systemListConstructor.systemList[i].systemOwnedBy == playerTurnScript.playerRace)
				{
					if(overlayObjectList[i].fade == false)
					{
						TweenAlpha.Begin(overlayObjectList[i].container, 0.25f, 1f);
						overlayObjectList[i].fade = true;
					}

					overlayObjectList[i].overlayOuter.Rotate(overlayObjectList[i].rotDir * Time.deltaTime * overlayObjectList[i].rotSpeed, Space.World);

					UpdatePosition(i);
					UpdateVariables(i);
				}
			}

			allfade = true;
		}

		else if(mainCamera.transform.position.z <= -65f && allfade == true)
		{
			for(int i = 0; i < overlayObjectList.Count; ++i)
			{
				if(overlayObjectList[i].fade == true)
				{
					TweenAlpha.Begin(overlayObjectList[i].container, 0.25f, 0f);
					overlayObjectList[i].fade = false;

					UpdatePosition(i);
					UpdateVariables(i);
				}
			}

			allfade = false;
		}
	}
}

public class OverlayObject
{
	public GameObject container;
	public UILabel name, power, knowledge;
	public UISprite planets;
	public Transform overlayOuter;
	public bool fade;
	public float rotSpeed;
	public Vector3 rotDir;
}
