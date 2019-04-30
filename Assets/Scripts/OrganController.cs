using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganController : MonoBehaviour
{
   public PlayerController player;
    public BiometricsManager bioManager;

    //Button events
    public void HeartPressed()
    {
        if (player.hasEscaped)
        {
            bioManager.heartPressed = true;
        }
    }

    public void LungsPressed()
    {
        if (player.hasEscaped)
        {
            bioManager.lungsPressed = true;
        }

    }
}
