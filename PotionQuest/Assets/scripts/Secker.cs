using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Secker : MonoBehaviour
{
    //public CharacterController cc;
    private PlayerController player;
    private LightManager lightManager;
    public Transform pathingTarget;
    
    private Vector3 targetPosition;
    private bool chasingPlayer = false;
    private bool inLight = false;
    //stays up to date on what direction the light is, if we are lit
    private Vector3 directionToLight = Vector3.up;
    public bool canKill = true;

    private Vector3 roamPosition;
    public float roamDistance = 50.0f;
    public float roamHeight = 50.0f;

    public NavMeshAgent agent;
    private Rigidbody rb;
    public float MoveSpeed = 10;

    public AudioSource footstepSoundSource;
    public AudioClip footstepSound;


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

    private Vector3 movement = Vector3.zero;

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
        lightManager = GameObject.FindGameObjectWithTag("LightManager").GetComponent<LightManager>();
        //previousPosition = pathingTarget.transform.position;
        targetPosition = transform.position;
        roamPosition = transform.parent.position;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        agent.SetDestination(targetPosition);

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
        //respawn if under water
        if (transform.position.y <= 48)
        {
            transform.position = roamPosition;
            rb.position = roamPosition;
        }
        //see if we are in light
        inLight = GetLightStatus();

        agent.baseOffset = 2;

        //Secker AI and movement
        //first, 
        agent.speed = MoveSpeed;
        if (inLight) // we see if we are in light first, and if we are we curl up
        {
            //move body to the ground to curl up
            agent.baseOffset = 0;
            //make it so we don't kill the player
            canKill = false;
            //slow down the secker
            agent.speed = MoveSpeed / 2;
            //try to move away from light
            if (NavMesh.SamplePosition(transform.position - (directionToLight * 5), out NavMeshHit hit, 25, NavMesh.AllAreas))
            {
                targetPosition = hit.position;
                agent.SetDestination(targetPosition);
                //Debug.Log("Roaming to location " + targetPosition);
            }

        }else if (player.inLight) //lets see if the player is in light.
        {

            //if we are close to the current target position, then get a new roam position
            if (agent.pathStatus == NavMeshPathStatus.PathInvalid || chasingPlayer || (targetPosition - transform.position).sqrMagnitude < 5.0f)
            {
                chasingPlayer = false;
                //it is in light, so lets just roam around our region
                Vector3 randomPosition = UnityEngine.Random.onUnitSphere * roamDistance;
                randomPosition.y = (randomPosition.y / roamDistance) * roamHeight;
                randomPosition.y += roamHeight / 2.0f;
                randomPosition += roamPosition;
                if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 25, NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                    agent.SetDestination(targetPosition);
                    //Debug.Log("Roaming to location " + targetPosition);
                }
            }
        }else
        {
            //chase the player
            agent.speed = 20;
            
            if (agent.pathStatus == NavMeshPathStatus.PathInvalid || !chasingPlayer || (targetPosition - player.transform.position).sqrMagnitude > 2.0f)
            {
                if (NavMesh.SamplePosition(player.transform.position, out NavMeshHit hit, 25, NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                    agent.SetDestination(targetPosition);
                    //Debug.Log("chasing player at location " + targetPosition);
                }    
            }
            chasingPlayer = true;
            
        }
        
        RaycastHit hitInfo;

        //align body with the ground
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, ScanDepth, 9))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation (transform.up, hitInfo.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation,slopeRotation * transform.rotation,10*Time.deltaTime);
        }
        //     transform.LookAt(transform.forward + transform.position, hitInfo.normal);
        for (int i = 0; i < Legs.Count; i++)
        {
            if (Physics.Raycast(Legs[i].RayPosition.position, -transform.up, out hitInfo, ScanDepth, 9))
            {
                //check if the distance between current Leg position and the position to move to is
                //greater than the step distance, and if it is, update it
                Legs[i].Distance = (Legs[i].Target.transform.position - hitInfo.point).sqrMagnitude;
                //Legs[i].PositionWanted = hitInfo.point;
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
                Legs[i].Distance = (Legs[i].Target.transform.position - (Legs[i].RayPosition.position - new Vector3(0,2,0))).sqrMagnitude;

                Legs[i].PositionWanted = Legs[i].RayPosition.position - new Vector3(0,2,0);
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
        if (Legs[ActiveLeg].Lerp < 1)
        {
            Vector3 footPos = Vector3.Lerp(Legs[ActiveLeg].PositionPrevious, Legs[ActiveLeg].PositionWanted, Legs[ActiveLeg].Lerp);
            footPos.y += Mathf.Sin(Legs[ActiveLeg].Lerp * Mathf.PI) * StepHeight;
            Legs[ActiveLeg].Target.transform.position = footPos;
            Legs[ActiveLeg].Lerp += Time.deltaTime * LegMoveSpeed;
            //update the previous position if we finished moving the leg
            if (Legs[ActiveLeg].Lerp > 1) Legs[ActiveLeg].PositionPrevious = Legs[ActiveLeg].Target.transform.position;
        }else
        {
            //basically a one time call after lerping
            if (Legs[ActiveLeg].Lerp < 100)
            {
                footstepSoundSource.Play();
                Legs[ActiveLeg].Lerp = 200;
                //Legs[ActiveLeg].PositionPrevious = Legs[ActiveLeg].PositionWanted;
            }
            //Legs[ActiveLeg].Lerp = 0;
            ActiveLeg = -1;
        }

        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            if (canKill)
            {
                player.Respawn = true;
            }
        }
    }

    
    public bool GetLightStatus()
    {
        bool lit = false;
        foreach (SafeLight light in lightManager.lights)
        {
            if (light.type == LightType.PointLight)
            {
                //point light
                //get the range of the point light, and exlude it if we are past that
                Vector3 ToLight = light.gameObject.transform.position - transform.position;

                if (ToLight.sqrMagnitude <= light.light.range * light.light.range)
                {
                    //the distance to the light is lesser than the lights range, so lets scan for anything blocking the light
                    if (!Physics.Raycast(transform.position, ToLight.normalized, ToLight.magnitude, 8, QueryTriggerInteraction.Ignore))
                    {
                        //something is blocking the light.
                        directionToLight = ToLight;
                        lit = true;
                        break;
                    }
                }
            }
            else
            {
                //ignore directional light for secker. can't avoid the damn sun
            }
        }
        return lit;
    }
}
