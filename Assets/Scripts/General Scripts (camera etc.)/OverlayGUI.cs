using UnityEngine;
using System.Collections;
using System;

public class OverlayGUI : MasterScript 
{
	public Camera mainCamera, uiCamera;
	public GameObject mouseOverSystem, systemOverlay;
	public UIWidget cornerAnchor;
	public UILabel knowledgeOverlay, powerOverlay, colonisationOverlay, nameOverlay, defenceOverlay;
	private int system;

	private string UpdateVariables()
	{
		systemSIMData = mouseOverSystem.GetComponent<SystemSIMData>();

		system = RefreshCurrentSystem(mouseOverSystem);

		int colonisedPlanets = 0;
		
		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == true)
			{
				++colonisedPlanets;
			}
		}

		string systemSummaryString = "Colonised: " + colonisedPlanets + "/" + systemListConstructor.systemList[system].systemSize;

		return systemSummaryString;
	}

	private void UpdatePosition()
	{
		Vector3 position = mainCamera.WorldToViewportPoint (mouseOverSystem.transform.position);
		
		position = uiCamera.ViewportToWorldPoint (position);
		
		Vector3 newPosition = new Vector3(position.x, position.y, -37.0f);
		
		systemOverlay.transform.position = newPosition;
	}

	private void Update()
	{
		if(mouseOverSystem == null)
		{
			NGUITools.SetActive(systemOverlay, false);
		}

		if (mouseOverSystem != null && playerTurnScript.playerRace != null && cameraFunctionsScript.selectedSystem != null) 
		{		
			NGUITools.SetActive(systemOverlay, true);

			UpdateVariables();

			colonisationOverlay.text = UpdateVariables();
			
			nameOverlay.text = mouseOverSystem.name;

			knowledgeOverlay.text = (Math.Round(systemSIMData.totalSystemKnowledge,1)).ToString();

			powerOverlay.text = (Math.Round(systemSIMData.totalSystemPower,1)).ToString();

			defenceOverlay.text = systemListConstructor.systemList [system].systemDefence.ToString();

			UpdatePosition();
		}
	}
}
