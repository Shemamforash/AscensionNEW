using UnityEngine;
using System.Collections;

public class ToolTip : MasterScript
{	
	public GameObject tooltip;
	public UIWidget tooltipWidget;
	public UIPanel rootPanel;
	private float timer, xDif, yDif;
	private bool showTooltip = false;
	private string tooltipText = null;
	private TechTreeGUI techTreeGUI;

	void Start()
	{
		NGUITools.SetActive(tooltip, false);
		techTreeGUI = GameObject.Find ("GUIContainer").GetComponent<TechTreeGUI> ();
	}

	private void TooltipPosition()
	{
		yDif = tooltipWidget.height / 2;
		xDif = 0.0f;
		
		if(Input.mousePosition.x + 250 > rootPanel.width)
		{
			xDif = -250f;
		}
		
		Vector3 position = new Vector3(Input.mousePosition.x + xDif, Input.mousePosition.y + yDif, 0.0f); //TODO
		
		position = overlayGUI.mainCamera.ScreenToViewportPoint(position);
		
		position = overlayGUI.uiCamera.ViewportToWorldPoint (position);
		
		tooltip.transform.position = new Vector3(position.x, position.y, tooltip.transform.position.z);
	}

	public void Update()
	{
		if(UICamera.hoveredObject != null)
		{
			if(UICamera.hoveredObject.tag == "Improvement" || UICamera.hoveredObject.tag == "Improvement Slot" || UICamera.hoveredObject.tag == "TechLabel")
			{
				tooltipText = "";

				if(UICamera.hoveredObject.tag == "Improvement" || UICamera.hoveredObject.tag == "Improvement Slot")
				{
					for(int i = 0; i < systemListConstructor.basicImprovementsList.Count; ++i)
					{
						if(UICamera.hoveredObject.gameObject.name == systemListConstructor.basicImprovementsList[i].name)
						{
							tooltipText = systemListConstructor.basicImprovementsList[i].details;
						}
					}
				}

				if(UICamera.hoveredObject.tag == "TechLabel")
				{
					for(int i = 0; i < heroTechTree.heroTechList.Count; ++i)
					{
						if(UICamera.hoveredObject.name == heroTechTree.heroTechList[i].techName)
						{
							for(int j = 0; j < techTreeGUI.techLabels.Count; ++j)
							{
								if(techTreeGUI.techLabels[j].label.gameObject.name == heroTechTree.heroTechList[i].techName)
								{
									if(techTreeGUI.techLabels[j].label.text == techTreeGUI.techLabels[j].label.gameObject.name)
									{
										tooltipText = heroTechTree.heroTechList[i].techDetails;
										Debug.Log (tooltipText);
									}
								}
							}
						}
					}
				}

				if(showTooltip == false)
				{
					timer = Time.time;
					showTooltip = true;
				}

				if(showTooltip == true && timer + 0.5f <= Time.time)
				{
					if(tooltipText != "")
					{
						NGUITools.SetActive(tooltip, true);
						tooltip.GetComponent<UILabel>().text = tooltipText;
					}

					TooltipPosition();
				}
			}

			else
			{
				showTooltip = false;
			}
		}

		else
		{
			if(tooltip.activeInHierarchy == true)
			{
				NGUITools.SetActive(tooltip, false);
				showTooltip = false;
			}
		}
	}
}
