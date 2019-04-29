using UnityEngine;

//Put this on any parallax layer and assign higher value for more distant objects
[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{ 
    public float parallaxFactor;
    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;
        transform.localPosition = newPos;
    }
}
