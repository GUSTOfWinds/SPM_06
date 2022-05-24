using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Guard : StateBase
{
    private float moveSpeed = 1.5f;
    private float counter = 0;
    private Vector3 movingDirection;
    private bool canChangeDirec;
   
    private Animator animator;
    private GameObject gameObject;
    
    private Collider[] sphereColliders;
    private GameObject chasingObject;
    private int detectScopeRadius;
    private Vector3 spwnPos;
    private int maxChasingRange;
    private float waitFrame;
    [Header("Calculation")]
    private int traces = 6;
    private float visionAngle = 45.0f;
    private Vector3 tempPos;

    public State_Guard(Animator animator, GameObject gameObject, Vector3 spawnpos) : base(animator, gameObject, spawnpos)
    {
        this.animator = animator;
            this.gameObject = gameObject;
        this.spwnPos = spawnpos;
    }

    public override void OnEnter()
    {
        Debug.Log("start Guarding");
        detectScopeRadius = 5;
        maxChasingRange = 25;
        waitFrame = 0;
        movingDirection = RandomVector(-detectScopeRadius, detectScopeRadius);
    }

    public override void OnExit()
    {
        Debug.Log("stop Guarding");
        spwnPos.y = gameObject.transform.position.y;
    }

    public override void OnUppdate()
    {

        Movement();
  

      
    }


    private void Movement()
    {
        tempPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+1f, gameObject.transform.position.z);
        if (Vector3.Distance(gameObject.transform.position, spwnPos) > 20f)
        {
            if(canChangeDirec)
            {
                movingDirection = RandomVector(movingDirection);
                counter = 0;
            }
            else
            {
                movingDirection = Vector3.zero;
            }
            
        }
        counter += Time.fixedDeltaTime;
        if(counter > 2f)
        {
            canChangeDirec = true;
        }
        
        if (movingDirection != Vector3.zero)
        {
            Vector3 nevVector = CalculateMovement();
            movingDirection = Vector3.Lerp(movingDirection, nevVector.normalized, moveSpeed * Time.deltaTime);
            waitFrame++;
            if (waitFrame % 60 == 0)
            {
                ChangeFacingDirection(movingDirection);

            }
        }

        gameObject.transform.position += movingDirection * moveSpeed * Time.fixedDeltaTime;
        //RaycastHit hitInfo;
        //if (Physics.BoxCast(gameObject.transform.position, gameObject.GetComponent<Collider>().bounds.size, movingDirection, out hitInfo, Quaternion.identity, 0.2f, (1 << 8)))
        //{
        //    Debug.Log("hit");
        //    Debug.Log(hitInfo.normal);
        //    Debug.Log(Vector3.Dot(movingDirection, hitInfo.normal));
        //    if (Vector3.Dot(movingDirection, hitInfo.normal) == -1)
        //    {
        //        movingDirection = -movingDirection;
        //    }

        //}
        CheckForPlayer();
    }


    private void CheckForPlayer()
    {
        sphereColliders = Physics.OverlapSphere(gameObject.transform.position, detectScopeRadius);
        foreach (var coll in sphereColliders)
        {
            if (coll.tag == "Player") //find Player and start chasing
            {
                if (chasingObject == null && coll.gameObject.GetComponent<GlobalPlayerInfo>().GetHealth() > 0) //if we are not chasing someone, only aim one target
                {
                    Debug.Log("get you");
                    chasingObject = coll.gameObject;
                    //TO DO Change to chase state
                }

            }
        }
    }

    private Vector3 RandomVector(float min, float max) //version1 lock enemy's y-axel
    {
        Vector3 movingDirection =
            new Vector3(UnityEngine.Random.Range(min, max), 0.0f, UnityEngine.Random.Range(min, max));
        ChangeFacingDirection(movingDirection);
        return movingDirection;
    }
    private Vector3 RandomVector(Vector3 current)
    {
        float angle = UnityEngine.Random.Range(130f, 230f);
        Vector3 temp = Quaternion.Euler(0, angle, 0) * current;
        ChangeFacingDirection(temp);
        return temp;
    }


    private void ChangeFacingDirection(Vector3 movingDirection)
    {
        Quaternion newRotation = Quaternion.LookRotation(movingDirection);
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, newRotation, 1);
    }
    private Vector3 CalculateMovement()
    {

        Vector3 movementVector = Vector3.zero;


        float stepAngle = (visionAngle * 2.0f) / (traces - 1);
        RaycastHit hitInfo1;

        // Create movement vector based on lidar sight.
        for (int i = 0; i < traces; i++)
        {

            float angle1 = (90.0f + visionAngle - (i * stepAngle)) * Mathf.Deg2Rad;
            Vector3 direction1 = gameObject.transform.TransformDirection(new Vector3(Mathf.Cos(angle1), 0.0f, Mathf.Sin(angle1)));


            if (Physics.Raycast(tempPos, direction1, out hitInfo1, 500f))
            {
                if (hitInfo1.collider.tag != "Player")
                {
                    movementVector += direction1 * (hitInfo1.distance - 3f);
                }

                Debug.DrawLine(tempPos, tempPos + direction1 * hitInfo1.distance);

                Vector3 Perp = Vector3.Cross(direction1, Vector3.up);
                Debug.DrawLine(hitInfo1.point + Perp, hitInfo1.point - Perp, Color.red);

            }
            else
            {
                movementVector += direction1 * 500f;

                Debug.DrawLine(tempPos, tempPos + direction1 * maxChasingRange);

            }

        }

        return movementVector;
    }
}
