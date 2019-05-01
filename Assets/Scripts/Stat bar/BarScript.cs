using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarScript : MonoBehaviour {

	private float fillAmount;
	[SerializeField]
	private float lerpSpeed;

	[SerializeField]
	private Image content;
	 
	private TMP_Text valueText;

	[SerializeField]
	private Color fullColor;

	[SerializeField]
	private Color lowColor;

	[SerializeField]
	private bool lerpColors;

	public float MaxValue { get; set;}

	public float Value {
		set
		{
			//string[] tmp = valueText.text.Split(':');
			//valueText.text = tmp [0] + ": " + value;
			fillAmount = Map (value, 0, MaxValue, 0, 1);
		}
	}

	// Use this for initialization
	void Start () {
		if (lerpColors) 
		{
			content.color = fullColor;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		HandleBar ();
	}

	private void HandleBar () {
		if (fillAmount != content.fillAmount)
		{
			content.fillAmount = Mathf.Lerp(content.fillAmount,fillAmount,Time.fixedDeltaTime * lerpSpeed);
		}
		if (lerpColors) 
		{
			content.color = Color.Lerp (lowColor, fullColor, fillAmount);
		}


	}

	private float Map(float value,float inMin,float inMax,float outMin,float outMax) {
		return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		//calculates the float for fill amount based on the value of heath etc thats being iptuted
	}


}
