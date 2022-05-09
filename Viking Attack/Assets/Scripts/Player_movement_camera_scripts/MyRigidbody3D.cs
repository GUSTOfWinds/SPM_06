using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyRigidbody3D : NetworkBehaviour
{
    [SerializeField] private float gravity = 3f;
    [SerializeField] private float staticFrictionCoefficient = 0.3f;
    [SerializeField] private float kineticFrictionCoefficient = 0.2f;
    [SerializeField] private float airResistance = 0.0001f;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] private CapsuleCollider capsuleCollider;
    private float colliderMargin = 0.01f;
    private float groundCheckDistance = 0.01f;
    
    
    private Vector3 point1;
    private Vector3 point2;
    private float capsuleHeight;
    private float capsuleRadius;
    private int UpdateVelocityTimes;
    public Vector3 velocity;

    //syncPosition �r till f�r att synkronisera alla spelarpositioner gentemot servern
    [SyncVar] private Vector3 syncPosition;
    //syncRotation ser till synkronisera alla rotationer, quaternion ist�llet f�r gimbal f�r att kunna rotera p� x-axeln men inte y-axeln
    [SyncVar] private Quaternion syncRotation;

    void Awake()
    {
        
        //Set collisionMask to hit everything except self
        collisionMask = ~(collisionMask);
        //capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleHeight = capsuleCollider.height;//* transform.localScale.y;
        capsuleRadius = capsuleCollider.radius;//* transform.localScale.x;
        NetworkServer.SpawnObjects();
    }

    void FixedUpdate()
    {

        // //H�r ser vi om det �r lokal spelare eller inte, om det inte �r det s� uppdaterar vi vyn f�r den andra och avbryter.
        if (!isLocalPlayer)
        {
            base.transform.position = syncPosition;
            base.transform.rotation = syncRotation;
            return;
        }

        //Add gravity     
        velocity += Vector3.down * gravity;

        //Add air resistance
        velocity *= Mathf.Pow(airResistance, Time.fixedDeltaTime);

        //Updates capsule (Collider hitbox) circle component position.
        point1 = gameObject.transform.position + capsuleCollider.center + Vector3.up * (capsuleHeight / 2 - capsuleRadius);
        point2 = gameObject.transform.position + capsuleCollider.center + Vector3.down * (capsuleHeight / 2 - capsuleRadius);

        PhysicsObjectFrictionFunction();
        UpdateVelocity();
        UpdateVelocityTimes = 0;

        //Add velocity variable to object position
        transform.position += velocity * Time.fixedDeltaTime;

        //Foljande 2 rader skickar ett kommando till servern och da andrar antingen positionen eller rotationen.
        CmdSetSynchedPosition(this.transform.position);
        CmdSetSynchedRotation(this.transform.rotation);
    }

    //Kommandlinjer f�r att be servern om uppdateringar p� rotation och position
    [Command]
    void CmdSetSynchedPosition(Vector3 position) => syncPosition = position;
    [Command]
    void CmdSetSynchedRotation(Quaternion rotation) => syncRotation = rotation;

    //Check if object is on ground (on another collider) returns a bool
    public bool GroundedBool()
    {
        RaycastHit hit = new RaycastHit();
        return Physics.CapsuleCast(point1, point2, capsuleRadius, Vector3.down, out hit, (groundCheckDistance + colliderMargin), collisionMask);
    }

    //Check if object is on ground (on another collider) returns a RaycastHit veribal
    public RaycastHit Grounded()
    {
        RaycastHit hit = new RaycastHit();
        Physics.CapsuleCast(point1, point2, capsuleRadius, Vector3.down, out hit, (groundCheckDistance + colliderMargin), collisionMask);
        return hit;
    }
    private void UpdateVelocity()
    {
        UpdateVelocityTimes++;
        if(UpdateVelocityTimes >= 100)
            return;
        if(velocity.magnitude  < 0.0001f)
        {
            velocity = Vector3.zero;
            return;
        }
        RaycastHit hit1;
        Vector3 normalForce = Vector3.zero;
        if(Physics.CapsuleCast(point1, point2, capsuleRadius, velocity.normalized, out hit1 ,Mathf.Infinity,collisionMask))
        {
            float distanceToColliderNeg = colliderMargin / Vector3.Dot(velocity.normalized, hit1.normal);
            float allowedMovementDistance = hit1.distance + distanceToColliderNeg;
            if (allowedMovementDistance > velocity.magnitude * Time.fixedDeltaTime) 
            {
                return;
            }
            if (allowedMovementDistance > 0.0f) 
            {
                velocity += velocity.normalized * allowedMovementDistance;
            }
            normalForce += GetComponent<GeneralHelpFunctions3D>().CalculateNormalForce(velocity,hit1.normal);
            velocity += normalForce;
            UpdateVelocity();
        }
        Collider[] hitList = Physics.OverlapCapsule(point1, point2, capsuleRadius, collisionMask);
        if(hitList.Length > 0)
        {
            Vector3 direction;
            float distance = Mathf.Infinity;
            Collider colliderToStartWith = null;
            foreach(Collider hit2 in hitList)
            {
                Vector3 tempDirection;
                float tempDistance;
                Physics.ComputePenetration(capsuleCollider,capsuleCollider.transform.position,capsuleCollider.transform.rotation,hit2,hit2.transform.position,hit2.transform.rotation,out tempDirection,out tempDistance);
                if(tempDistance < distance)
                {
                    distance = tempDistance;
                    colliderToStartWith = hit2;
                }
            }
            Physics.ComputePenetration(capsuleCollider,capsuleCollider.transform.position,capsuleCollider.transform.rotation,colliderToStartWith,colliderToStartWith.transform.position,colliderToStartWith.transform.rotation,out direction,out distance);

            Vector3 separationVector = direction * distance;
            transform.position += separationVector + direction.normalized * colliderMargin * Time.fixedDeltaTime * 10;

            normalForce += GetComponent<GeneralHelpFunctions3D>().CalculateNormalForce(velocity, direction.normalized);
            velocity += normalForce;
            UpdateVelocity();
        }
        FrictionFunction(normalForce);
    }
    //Gives friction to velovity with given normalforce
    private void FrictionFunction(Vector3 normalForce)
    {
        if (velocity.magnitude < normalForce.magnitude * staticFrictionCoefficient)
            velocity = Vector3.zero;
        else
            velocity -= velocity.normalized * normalForce.magnitude * kineticFrictionCoefficient;
    }
    //Gives friction to velovity with collided PhysicsObject (object with MyRigidbody3D)
    private void PhysicsObjectFrictionFunction()
    {
        if (GroundedBool() && Grounded().transform.gameObject.GetComponent<MyRigidbody3D>() != null)
        {
            Vector3 normalForce = GetComponent<GeneralHelpFunctions3D>().CalculateNormalForce(velocity, Grounded().normal);
            if (velocity.magnitude - Grounded().transform.gameObject.GetComponent<MyRigidbody3D>().velocity.magnitude < normalForce.magnitude * staticFrictionCoefficient)
                velocity = Grounded().transform.gameObject.GetComponent<MyRigidbody3D>().velocity;
            else
                velocity -= velocity.normalized * normalForce.magnitude * kineticFrictionCoefficient;
        }
    }
}