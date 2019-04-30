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
    }

    public void LungsPressed()
    {
        bioManager.lungsPressed = true;
    }
}
