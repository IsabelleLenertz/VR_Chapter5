using UnityEngine;
using System.Collections;

public class WaspBrain : MonoBehaviour
{
    // explode particle effect
    public Transform explodeEffect;

    public AudioSource ouchSound;
    public AudioSource sprayedSound;

    // we will pick a random speed between min and max values here
    public float minMoveSpeed = 0.5f;
    public float maxMoveSpeed = 1f;

    // the moveSpeed var is used for the movement
    private float moveSpeed = 0.5f;

    private bool isSprayed;
    private bool inTheStingArea;
    private bool isFlyingAway;

    private Transform myTransform;

    void Start()
    {
        Init();
    }

    void Init()
    {
        // choose a random move speed
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

        // grab a reference to this transform so that we can move it etc.
        myTransform = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        // move the wasp
        if (!isFlyingAway)
        {
            myTransform.Translate(0, 0, moveSpeed * Time.deltaTime);
        } else
        {
            myTransform.Translate((Vector3.up * 10 ) * Time.deltaTime);
        }

        // fly away if the game is over!
        if (!isFlyingAway)
        {
            if (SceneController.currentState != SceneController.GameState.InGame)
            {
                isFlyingAway = true;
                
                // destroy this wasp in 10 seconds (a bit longer just because it's the end of the game)
                Invoke("DestroyMe", Random.Range(10,12));
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // check the the object this wasp has collided with is on the spray layer
        // note that we also don't allow wasps to be sprayed inside the house, too
        if (!isFlyingAway && !inTheStingArea && !isSprayed && collision.gameObject.layer == LayerMask.NameToLayer("Spray") && SceneController.currentState == SceneController.GameState.InGame)
        {
            // set a flag to make sure we don't repeat these calls on multiple collisions
            isSprayed = true;

            // increase score
            SceneController.sprayedScore++;

            // play sprayed sound
            sprayedSound.Play();

            // fly home!
            isFlyingAway = true;

            // destroy this wasp in 4 seconds
            Invoke("DestroyMe", 4);
        } else if(!isFlyingAway)
        {
            // fly home!
            isFlyingAway = true;

            // destroy this wasp in 4 seconds
            Invoke("DestroyMe", 4);
        }
    }

    void OnTriggerEnter()
    {
        if (!inTheStingArea)
        {
            // use a flag to make sure we don't repeat this code on multiple trigger entries
            inTheStingArea = true;
            
            // increase the house invader count on SceneController
            SceneController.stings++;

            // play ouch sound
            ouchSound.Play();

            // fly home!
            isFlyingAway = true;

            // destroy this wasp in 4 seconds
            Invoke("DestroyMe", 4);
        }
    }

    void DestroyMe()
    {
        // destroy this gameObject
        Destroy(this.gameObject);
    }
}
