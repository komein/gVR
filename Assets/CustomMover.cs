using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMover : AbstractMover
{


	protected override void Update ()
    {
        base.Update();
    }

    void FixedUpdate()
    {
        if (mode == ControlMode.move)
            rb.MovePosition(rb.transform.position + cam.transform.forward * currentSpeed * Time.deltaTime);
        else
            rb.MovePosition(rb.transform.position + (Vector3.forward * currentSpeed + 
                new Vector3(-cam.transform.right.y,0,0) * strafeSpeed) * Time.deltaTime);
    }
}
