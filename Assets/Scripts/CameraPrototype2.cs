using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPrototype2 : MonoBehaviour
{
    public CharacterPrototype2 ch;
    public float distance = 3f;
    public float height = 3f;

    public bool strafeCamera = false;
    public float strafeSpeed = 2f;
    
    void Update()
    {
        if (ch == null)
            return;

        float xPos;
        if (!strafeCamera)
        {
            xPos = gameObject.transform.position.x;
        }
        else
        {
            xPos = Mathf.Lerp(gameObject.transform.position.x, ch.transform.position.x, Time.deltaTime * strafeSpeed);
        }

        Vector3 newPos = new Vector3(xPos, ch.transform.position.y + height, ch.transform.position.z - distance);
        gameObject.transform.position = newPos;
    }
}
