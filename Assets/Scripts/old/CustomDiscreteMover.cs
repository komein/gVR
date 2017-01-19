using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomDiscreteMover : AbstractMover
{
    enum DiscreteState { ready, switching, idle };

    private DiscreteState state = DiscreteState.ready;

    public float moveAngleToSwitch = 20f;
    public float rotateAngleToSwitch = 0.2f;

    public float distanceToSwitch = 1f;

    public float idleDelay = 1f;

    public int lanes = 3;
    public int currentLane = 1;

    float currentDelay = 0f;

    Vector3 whereToMove = Vector3.zero;
    Vector3 startSwitchingPosition;

    protected override void Update ()
    {
        base.Update();

    }

    void FixedUpdate()
    {

        switch (state)
        {
            case DiscreteState.idle:
                if (currentDelay < idleDelay)
                {
                    currentDelay += Time.deltaTime;
                }
                else
                {
                    currentDelay = 0;
                    state = DiscreteState.ready;
                }
                break;

            case DiscreteState.ready:
                if (mode == ControlMode.move)
                {
                    if (Vector3.Angle(Vector3.forward, cam.transform.forward) > moveAngleToSwitch)
                    {
                        if (cam.transform.forward.x < 0)
                        {
                            if (currentLane < lanes - 1)
                            {
                                currentLane++;
                                startSwitchingPosition = cam.transform.position;
                                state = DiscreteState.switching;
                                whereToMove = Vector3.left;
                            }
                        }
                        else
                        {
                            if (currentLane > 0)
                            {
                                currentLane--;
                                startSwitchingPosition = cam.transform.position;
                                state = DiscreteState.switching;
                                whereToMove = Vector3.right;
                            }
                        }
                    }
                }
                else
                {
                    //if (Vector3.Angle(Vector3.right, cam.transform.right) > rotateAngleToSwitch)
                    {
                        if (cam.transform.right.y < -rotateAngleToSwitch)
                        {
                            if (currentLane < lanes - 1)
                            {
                                currentLane++;
                                startSwitchingPosition = cam.transform.position;
                                state = DiscreteState.switching;
                                whereToMove = Vector3.right;
                            }
                        }
                        else if (cam.transform.right.y > rotateAngleToSwitch)
                        {
                            if (currentLane > 0)
                            {
                                currentLane--;
                                startSwitchingPosition = cam.transform.position;
                                state = DiscreteState.switching;
                                whereToMove = Vector3.left;
                            }
                        }
                    }
                }
                break;

            case DiscreteState.switching:
                if (Mathf.Abs(startSwitchingPosition.x - cam.transform.position.x) > distanceToSwitch)
                {
                    whereToMove = Vector3.zero;
                    state = DiscreteState.idle;
                }
                break;
        }

        rb.MovePosition(rb.transform.position + (Vector3.forward * currentSpeed 
            + whereToMove * strafeSpeed ) * Time.deltaTime);

    }
}
