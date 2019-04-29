using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMove : MonoBehaviour
{

    public float maxSpeed = 6f;
    public float startSpeed = 0.1f;
    public float speedIncreaseFactor = 0.1f;
    public float yieldTime = 1f;
    float currentSpeed;
    public float delay = 4;
    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = startSpeed;
        StartCoroutine(SpeedIncrease());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * currentSpeed *Time.fixedDeltaTime);
    }
    IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(delay);
        while (currentSpeed < maxSpeed)
        {
            if (currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
            currentSpeed += speedIncreaseFactor;
            yield return new WaitForSeconds(yieldTime);
        }
    }
}
