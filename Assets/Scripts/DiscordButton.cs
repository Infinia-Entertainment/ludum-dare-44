using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordButton : MonoBehaviour
{
    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }
}
