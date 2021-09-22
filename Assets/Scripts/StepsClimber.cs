using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsClimber : MonoBehaviour
{
    [Header("Player Steps Climb")]
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 1f;
    [SerializeField] float stepSmooth = 0.8f;

    new Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        stepRayUpper.transform.position = new Vector3(
            stepRayUpper.transform.position.x,
            stepHeight,
            stepRayUpper.transform.position.z
        );
    }

    private void FixedUpdate()
    {
        StepClimb(Time.fixedDeltaTime);
    }

    private void StepClimb(float delta)
    {
        float outerDistance = 0.1f, innerDistance = 0.2f;

        Debug.DrawRay(stepRayLower.transform.position,
            this.transform.TransformDirection(Vector3.forward),
            Color.green,
            outerDistance);

        // creates Ray at stepRayLower in Forward
        if (Physics.Raycast(
            stepRayLower.transform.position,
            this.transform.TransformDirection(Vector3.forward),
            out RaycastHit hitLower,
            outerDistance
            ))
        {
            Debug.LogWarning("Hit in front " + hitLower);
            // Not hitting stepRayUpper, so its not tall enough for us to climb
            if (
                !Physics.Raycast(
                stepRayUpper.transform.position,
                this.transform.TransformDirection(Vector3.forward),
                out RaycastHit hitUpper,
                innerDistance
                )
            )
            {
                Debug.LogWarning("Hit in front, can climb!!!");
                // we can climb, the more heigher the stepSmooth more the character will jump up
                rigidbody.position -= new Vector3(0, -stepSmooth * delta, 0);
            }
        }

        // 45 degree right
        if (Physics.Raycast(
            stepRayLower.transform.position,
            this.transform.TransformDirection(1.5f, 0, 1),
            out RaycastHit _,
            outerDistance
            ))
        {
            // Not hitting stepRayUpper, so its not tall enough for us to climb
            if (
                !Physics.Raycast(
                stepRayUpper.transform.position,
                this.transform.TransformDirection(1.5f, 0, 1),
                out RaycastHit _,
                innerDistance
                )
            )
            {
                // we can climb, the more heigher the stepSmooth more the character will jump up
                rigidbody.position -= new Vector3(0, -stepSmooth * delta, 0);
            }
        }

        // 45 degree left
        if (Physics.Raycast(
            stepRayLower.transform.position,
            this.transform.TransformDirection(-1.5f, 0, 1),
            out RaycastHit _,
            outerDistance
            ))
        {
            // Not hitting stepRayUpper, so its not tall enough for us to climb
            if (
                !Physics.Raycast(
                stepRayUpper.transform.position,
                this.transform.TransformDirection(-1.5f, 0, 1),
                out RaycastHit _,
                innerDistance
                )
            )
            {
                // we can climb, the more heigher the stepSmooth more the character will jump up
                rigidbody.position -= new Vector3(0, -stepSmooth * delta, 0);
            }
        }
    }
}
