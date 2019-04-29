using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganController : MonoBehaviour
{
    public BiometricsManager bioManager;
    //Button events
    public void HeartPressed()
    {
        bioManager.heartPressed = true;
        Debug.Log("Heart clicked, animating...");
    }

    public void LungsPressed()
    {
        bioManager.lungsPressed = true;
        Debug.Log("Lungs clicked, animating...");
    }
}
