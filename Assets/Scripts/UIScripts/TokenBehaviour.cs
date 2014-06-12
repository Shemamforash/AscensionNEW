using UnityEngine;
using System.Collections;

public class TokenBehaviour : MonoBehaviour 
{
	public GameObject tokenPrefab, currentToken;

	public void ButtonClicked()
	{
		if(currentToken == null)
		{
			currentToken = (GameObject)GameObject.Instantiate(tokenPrefab, gameObject.transform.position, Quaternion.identity);
		}
	}
}
