using UnityEngine;


public class individ_uppg3 : MonoBehaviour
{

    private float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        time += Time.deltaTime;
        transform.localScale = Vector3.one * Mathfx.Berp(0f, 1f, time);
    }

    private void OnEnable()
    {
        time = 0;
    }
}
