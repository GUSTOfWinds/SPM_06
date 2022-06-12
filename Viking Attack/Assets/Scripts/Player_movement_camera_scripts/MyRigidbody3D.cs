using UnityEngine;
using Mirror;

public class MyRigidbody3D : NetworkBehaviour
{
    /*
        @Author Love Strignert - lost9373
    */
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

    //syncPosition syncs the player position to the server
    [SyncVar] private Vector3 syncPosition;
    //syncRotation syncs the player rotation to the server
    [SyncVar] private Quaternion syncRotation;
    
    /*
     * @author Martin kings
     */
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
    

    void Awake()
    {
        collisionMask = ~(collisionMask);
        capsuleHeight = capsuleCollider.height;
        capsuleRadius = capsuleCollider.radius;
        NetworkServer.SpawnObjects();
    }

    void FixedUpdate()
    {
        //Here we se if it's a local player or not, if it isn't we update the view for the other and cancel.
        if (!isLocalPlayer)
        {
            base.transform.position = syncPosition;
            base.transform.rotation = syncRotation;
            return;
        }

        //Add gravity     
        velocity += Vector3.down * gravity;

        //Add air resistance
        velocity.x *= Mathf.Pow(airResistance, Time.deltaTime);
        velocity.z *= Mathf.Pow(airResistance, Time.deltaTime);


        //Updates capsule (Collider hitbox) circle component position.
        point1 = gameObject.transform.position + capsuleCollider.center + Vector3.up * (capsuleHeight / 2 - capsuleRadius);
        point2 = gameObject.transform.position + capsuleCollider.center + Vector3.down * (capsuleHeight / 2 - capsuleRadius);

        PhysicsObjectFrictionFunction();
        UpdateVelocity();
        UpdateVelocityTimes = 0;

        // Plays walking and running sounds
        PlayMovementSounds();

        //Add velocity variable to object position
        transform.position += velocity * Time.deltaTime;

        //Send command to server to change position and rotation
        CmdSetSynchedPosition(this.transform.position);
        CmdSetSynchedRotation(this.transform.rotation);

    }

    /*
     * @author Martin kings
     */
    
    private void PlayMovementSounds()
    {
        
        // Stops playing the audio if standing still
        if (velocity.magnitude < 1)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.loop = false;
            return;
        }

        // Plays the walking sound if walking and plays the running sound if running
        if(velocity.magnitude > 1 && velocity.magnitude < 9)
        {
            audioSource.clip = audioClips[0];
        }
        else
        {
            audioSource.clip = audioClips[1];
        }

        if (audioSource.isPlaying == false)
        {
            audioSource.Play();
        }

        audioSource.loop = true;
    }
    

    /**
    * @author Victor Wikner
    * Commandlines to ask server for updates on rotation and position
    */
    [Command(requiresAuthority = false)] 
    public void CmdSetSynchedPosition(Vector3 position) => syncPosition = position;
    [Command(requiresAuthority = false)]
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
    //Checks for collisions and updates the velocity accordingly
    private void UpdateVelocity()
    {
        //If run the function over 100 times exit (For it not to crash if it can't find a way to exit a collider)
        UpdateVelocityTimes++;
        if(UpdateVelocityTimes >= 100)
            return;
        //If the velocity is to low to notice set velocity to 0 and exit function
        if(velocity.magnitude  < 0.0001f)
        {
            velocity = Vector3.zero;
            return;
        }
        //Checks if there is a collision with the help of a capsulecast
        RaycastHit hit1;
        Vector3 normalForce = Vector3.zero;
        if(Physics.CapsuleCast(point1, point2, capsuleRadius, velocity.normalized, out hit1 ,Mathf.Infinity,collisionMask))
        {
            //Gets the distance to the hit collider
            float distanceToColliderNeg = colliderMargin / Vector3.Dot(velocity.normalized, hit1.normal);
            float allowedMovementDistance = hit1.distance + distanceToColliderNeg;
            //Checks if the object with current velocity can move and not hit the collider, if true exit function
            if (allowedMovementDistance > velocity.magnitude * Time.fixedDeltaTime) 
            {
                return;
            }
            //If the object is going to hit a collider but there is space to move, move only the alowed distance 
            if (allowedMovementDistance > 0.0f) 
            {
                velocity += velocity.normalized * allowedMovementDistance;
            }
            //Applys normalforce with the collided object
            normalForce += GetComponent<GeneralHelpFunctions3D>().CalculateNormalForce(velocity,hit1.normal);
            velocity += normalForce;
            UpdateVelocity();
        }
        //If the capsulecast failed and the object is inside a collider this part trys to move the object outside of the overlaping coliider 
        Collider[] hitList = Physics.OverlapCapsule(point1, point2, capsuleRadius, collisionMask);
        if(hitList.Length > 0)
        {
            Vector3 direction;
            float distance = Mathf.Infinity;
            Collider colliderToStartWith = null;
            //Iterates through all hit colliders and finds the collider with the shortes distance to move to get out of 
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

            //Applys normalforce with the collided object
            normalForce += GetComponent<GeneralHelpFunctions3D>().CalculateNormalForce(velocity, direction.normalized);
            velocity += normalForce;
            UpdateVelocity();
        }
        //Applys friction from the accumulated normalforce from all colliders hit
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