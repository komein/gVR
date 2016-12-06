using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPrototype1 : MonoBehaviour {

    protected Rigidbody rb;
    public Camera cam;

    public GvrReticle camReticle;
    public float speed = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (null == rb)
            rb = GetComponentInChildren<Rigidbody>();
    }
    // Update is called once per frame
 
	void Update () {
		if (camReticle.Gazed)
        {
            RaycastHit hitInfo;
            Physics.Raycast(cam.transform.position, cam.transform.forward - cam.transform.position, out hitInfo);

            Debug.DrawLine(cam.transform.position, hitInfo.point, Color.red, 1f);
            //rb.MovePosition(Vector3.Lerp(rb.transform.position, camReticle.transform.position - rb.transform.position, speed * Time.deltaTime));
        }
	}
}
