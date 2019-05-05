using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Stat 
{
	[SerializeField]
	private BarScript bar;
	[SerializeField]
	private float maxVal;
    [SerializeField]
    private float minVal = 0;
    [SerializeField]
    private float currentVal;

    public float MinVal
    {
        get
        {
            return minVal;
        }
        set
        {
            this.minVal = value;
            bar.MaxValue = maxVal;
        }
    }

    public float MaxVal {
		get 
		{
			return maxVal;
		}
		set 
		{
			this.maxVal = value;
			bar.MaxValue = maxVal;
		}
	}

	public float CurrentVal {
		get 
		{
			return currentVal;
		}
		set 
		{
			this.currentVal = Mathf.Clamp (value, 0, MaxVal);
			bar.Value = currentVal;
		}
	}

	public void Initialize() 
	{
        this.minVal = minVal;
		this.MaxVal = maxVal;
	}
   
}
