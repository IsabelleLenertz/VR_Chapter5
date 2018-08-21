using UnityEngine;
using System.Collections;

public class SprayShooter : MonoBehaviour {

    public Transform nozzleExitPoint;
    public Transform projectilePrefab;

	void Start () {
	
	}
	
	public void Fire () {
        // we only want to fire when the game is in its 'ingame' state
        if (SceneController.currentState == SceneController.GameState.InGame)
        {
            Instantiate(projectilePrefab, nozzleExitPoint.position, nozzleExitPoint.rotation);
        }
	}
}
