using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    public Transform[] repeatObjects;
    public float[] xOffsets;
    List<GameObject> disposals = new List<GameObject>();
    public float disposalOffset = 100f;
    int ID = 0;
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < repeatObjects.Length; i++)
        {
            if (repeatObjects[i] != null)
            {
                float camRightXPos = Camera.main.transform.position.x + (Camera.main.orthographicSize * Camera.main.aspect);
                // -14 + 5 = -9

                float camLeftXPos = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);

                float objXPos = repeatObjects[i].position.x;
                //11
                //Debug.Log(repeatObjects[i].position.x);

                if (camRightXPos > objXPos)
                {
                    Vector3 newPos = new Vector3(camRightXPos + xOffsets[i], repeatObjects[i].position.y, repeatObjects[i].position.z);

                    Transform inst = CreateAtPos(repeatObjects[i].gameObject, newPos, repeatObjects[i].parent);
                    disposals.Add(inst.gameObject);
                    repeatObjects[i] = inst;
                }
            }

        }
        for (int i = 0; i < disposals.Count; i++)
        {
            float camLeftXPos = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect) - disposalOffset;

            float objXPos = disposals[i].transform.position.x;
            if (camLeftXPos > objXPos)
            {
                Destroy(disposals[i], 0.1f);
                disposals.RemoveAt(disposals.IndexOf(disposals[i]));
            }
        }   
    }

    Transform CreateAtPos(GameObject obj, Vector3 pos, Transform parent)
    {
        GameObject instance = Instantiate(obj, pos, Quaternion.identity, parent);
        ID++;
        instance.name = ID.ToString();
        return instance.transform;
    }
}