 using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Secker : MonoBehaviour
{
    private Rigidbody rb;
    public float MoveSpeed = 7;


    //target: what to control the position of to move the leg
    //position: the position the leg should be at
    //rayposition: the start position of the ray

    //left arm
    public GameObject LeftArmTarget;
    public Transform LeftArmRayPosition;

    //right arm
    public GameObject RightArmTarget;
    public Transform RightArmRayPosition;

    //left Leg
    public GameObject LeftLegTarget;
    public Transform LeftLegRayPosition;

    //right Leg
    public GameObject RightLegTarget;
    public Transform RightLegRayPosition;

    //variables
    public float distanceToStep = 2f;
    public float ScanDepth = 6f;
    public float LegMoveSpeed = 10;
    public float StepHeight = 0.2f;

    private int ActiveLeg = -1;

    private class LegData
    {
        public LegData(GameObject target, Transform raystart)
        {
            Target = target;
            RayPosition = raystart;
            PositionWanted = Target.transform.position;
            PositionPrevious = PositionWanted;
            Lerp = 0;
            onGround = true;
            Distance = 0;
        }
        public GameObject Target;
        public Transform RayPosition;
        public Vector3 PositionWanted;
        public Vector3 PositionPrevious;
        public float Lerp;
        public bool onGround;
        public float Distance;


    }

    List<LegData> Legs = new List<LegData>();

    private void Start() {
        rb = GetComponent<Rigidbody>();
        //note:
        //global targets is needed so the limbs stay while the body moves
        
        Legs.Add(new LegData(LeftArmTarget, LeftArmRayPosition));
        Legs.Add(new LegData(RightArmTarget, RightArmRayPosition));
        Legs.Add(new LegData(LeftLegTarget, LeftLegRayPosition));
        Legs.Add(new LegData(RightLegTarget, RightLegRayPosition));

        //by offsetting the starting points, each point shifts at different times by default
        Legs[0].Target.transform.position += new Vector3(0.0f, 0.0f,0.0f);//front left moves first
        Legs[3].Target.transform.position += new Vector3(0.25f * distanceToStep,0.0f,0.25f * distanceToStep);//then back right
        Legs[2].Target.transform.position += new Vector3(0.5f * distanceToStep, 0.0f,0.5f * distanceToStep);//then front left
        Legs[1].Target.transform.position += new Vector3(0.75f * distanceToStep,0.0f,0.75f * distanceToStep);//finally back right

        
    }

    private void Update() {

        //temp testing controls
        //aka remote controller hehe
        Vector3 movement = Vector3.zero;
        movement.x += Input.GetAxis("Horizontal") * MoveSpeed;
        movement.z += Input.GetAxis("Vertical") * MoveSpeed;

        rb.velocity = new Vector3(movement.x,rb.velocity.y, movement.z);
        
        RaycastHit hitInfo;

        //align body with the ground
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, ScanDepth))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation (transform.up, hitInfo.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation,slopeRotation * transform.rotation,10*Time.deltaTime);
        }
        //     transform.LookAt(transform.forward + transform.position, hitInfo.normal);
        for (int i = 0; i < Legs.Count; i++)
        {
            if (Physics.Raycast(Legs[i].RayPosition.position, -transform.up, out hitInfo, ScanDepth))
            {
                //check if the distance between current Leg position and the position to move to is
                //greater than the step distance, and if it is, update it
                Legs[i].Distance = (Legs[i].Target.transform.position - hitInfo.point).sqrMagnitude;
                Legs[i].PositionWanted = hitInfo.point;
                if (Legs[i].Distance > distanceToStep && Legs[i].Lerp >= 1)
                {
                    Legs[i].Lerp = 0;
                    //Debug.Log("Updated wanted leg position to " + Legs[i].PositionWanted);
                }
            }
            else
            {
                //if leg doesn't see anything below it, make the move to spot half way in the air
                Legs[i].Distance = (Legs[i].Target.transform.position - (Legs[i].RayPosition.position - new Vector3(0,0.5f,0))).sqrMagnitude;
                Legs[i].PositionWanted = Legs[i].RayPosition.position - new Vector3(0,0.5f,0);
                if (Legs[i].Distance > distanceToStep && Legs[i].Lerp >= 1)
                {
                    Legs[i].Lerp = 0;
                }
            }
        }

        //now that each leg knows where it wants to be, and how much it wants to be there, 
        //find the leg that needs to move the most and move it.
        if (ActiveLeg == -1)
        {
            ActiveLeg = 0;
            //we need to select a leg to move.
            int index = 0;
            float maxDist = 0;
            foreach(LegData leg in Legs)
            {
                if (leg.Distance > maxDist)
                {
                    maxDist = leg.Distance;
                    ActiveLeg = index;
                }
                index++;
            }
        }

        //update the leg that was selected, and only that leg
        //LegData ActiveLeg = Legs[Legs[ActiveLeg]];
        Debug.Log("current lerp val: " + Legs[ActiveLeg].Lerp);
        if (Legs[ActiveLeg].Lerp < 1)
        {
            Vector3 footPos = Vector3.Lerp(Legs[ActiveLeg].PositionPrevious, Legs[ActiveLeg].PositionWanted, Legs[ActiveLeg].Lerp);
            footPos.y += Mathf.Sin(Legs[ActiveLeg].Lerp * Mathf.PI) * StepHeight;
            Legs[ActiveLeg].Target.transform.position = footPos;
            Legs[ActiveLeg].Lerp += Time.deltaTime * LegMoveSpeed;
        }else
        {
            Legs[ActiveLeg].PositionPrevious = Legs[ActiveLeg].PositionWanted;
            //Legs[ActiveLeg].Lerp = 0;
            ActiveLeg = -1;
        }

        
    }
}
