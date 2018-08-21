using UnityEngine;
using System.Collections;

public class SprayController : MonoBehaviour {

    // explode particle effect
    public Transform explodeEffect;

    public float moveSpeed;
    public Vector3 moveVector = new Vector3(0, 0, 1);

    private Transform myTransform;
    private bool isSprayed;

	void Start ()
    {
        myTransform = GetComponent<Transform>();
    }
	
	void Update ()
    {
        myTransform.Translate(moveSpeed * moveVector * Time.deltaTime);
	}

    void OnCollisionEnter(Collision collision)
    {
        if (!isSprayed)
        {
            // set a flag to make sure we don't repeat these calls on multiple collisions
            isSprayed = true;
            
            // show particle effect
            Instantiate(explodeEffect, myTransform.position, Quaternion.identity);

            // destroy this gameObject
            Destroy(this.gameObject);
        }
    }

}
