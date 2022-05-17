using UnityEngine;

public class State_Guard : StateBase
{
    private float moveSpeed = 15f;
    private Vector3 movingDirection = new Vector3(1, 0, 1);
    private Vector3 oldDirection; 
    private Animator animator;
    private GameObject gameObject;
    private float counter = 2f;
    private Collider[] sphereColliders;
    private GameObject chasingObject;
    private int detectScopeRadius;
    public State_Guard(Animator animator, GameObject gameObject) : base(animator, gameObject)
    {
        this.animator = animator;
            this.gameObject = gameObject;
    }

    public override void OnEnter()
    {
        Debug.Log("start Guarding");
        detectScopeRadius = 5;
    }

    public override void OnExit()
    {
        Debug.Log("stop Guarding");
    }

    public override void OnUppdate()
    {

        Movement();
  

      
    }


    private void Movement()
    {

        if (Vector3.Distance(gameObject.transform.position, Vector3.zero) > 20f)
        {

            float angle = Random.Range(170f,190f);
            Vector3 moving = Quaternion.Euler(0, angle, 0) * movingDirection;
            movingDirection = moving;

        }

        gameObject.transform.position += movingDirection * moveSpeed * Time.fixedDeltaTime;
        //RaycastHit hitInfo;
        //if (Physics.BoxCast(gameObject.transform.position, gameObject.GetComponent<Collider>().bounds.size,movingDirection, out hitInfo, Quaternion.identity,0.2f, (1 << 8)))
        //{
        //    Debug.Log("hit");
        //    Debug.Log(hitInfo.normal);
        //    Debug.Log(Vector3.Dot(movingDirection, hitInfo.normal));
        //    if(Vector3.Dot(movingDirection, hitInfo.normal) == -1)
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
                }

            }
        }
    }

}
