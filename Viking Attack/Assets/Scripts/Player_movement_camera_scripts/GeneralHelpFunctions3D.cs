using UnityEngine;

public class GeneralHelpFunctions3D : MonoBehaviour
{
    public Vector3 CalculateNormalForce(Vector3 velocity, Vector3 normal)
    {
        /*
            @Author Love Strignert - lost9373
        */
        Vector3 projection;
        //If objects are inside each other return Vector3(0,0,0) 
        if(Vector3.Dot(velocity, normal) > 0)
            projection = Vector3.zero;
        else
            projection = Vector3.Dot(velocity, normal) * normal;
        return -projection;
    }

}