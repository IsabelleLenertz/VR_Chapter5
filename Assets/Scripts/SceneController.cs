using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public int timeBeforeStart = 10;

    public enum GameState { Loaded, GetReady, InGame, Paused, GameOver };
    public static GameState currentState;
    private GameState targetState;

    public static int sprayedScore;
    public static int stings;

    [Space(20)]
    public float waspSpawnMinInterval = 2f;
    public float waspSpawnMaxInterval = 0.2f;

    [Space(20)]
    public Transform waspPrefab;

    [Space (20)]
    public Text sprayedText;
    public Text stingCountText;

    [Space(20)]
    public float minSpawnHeight = 2f;
    public float maxSpawnHeight = 8f;
    public float spawnCircleInnerRadius = 20f;
    public float spawnCircleOuterRadius = 30f;
    public int maxStingsToEndGame = 10;

    [Space(20)]
    public Transform spawnPoint;
    public Transform waspTarget;

    [Space(20)]
    public GameObject getReadyMessage;
    public Text gameOverMessage;
    public Text countdownText;

    private float currentSpawnInterval;

    private Transform tempTR;
    private bool underAttack;
    private int countdownTime;

    void OnEnable()
    {
        // do anything we need to do that's specific to VR
        SetUpVR();

        // reset everything in an init function
        Init();
	}
	
    void SetUpVR()
    {
        // recenter the view
        UnityEngine.XR.InputTracking.Recenter(); 
    }

    // this function is public so that it can be added as an event to the restart button
    public void BackToMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }

    void Init()
    {
        // set game state 
        currentState = GameState.Loaded; 
        targetState = GameState.GetReady;

        // reset score and stings
        sprayedScore = 0;
        stings = 0;

        // show the get ready message
        ShowGetReadyMessage();

        // hide game over message
        HideGameOverMessage();

        // do a countdown before the game starts
        countdownTime = timeBeforeStart;
        InvokeRepeating("Countdown", 0, 1);

        // invoke a call to start the game in <timeBeforeStart> seconds (enough time to allow the user to read the get ready message)
        Invoke("StartGame", timeBeforeStart);
    }

    void Countdown()
    {
        // set the text of the countdown text ..
        countdownText.text = "The bug invasion begins in .. " + countdownTime.ToString();
        
        // decrease the countdown number for the next update
        countdownTime--;
    }

    void StartGame()
    {
        // stop the repeating invoke call doing the countdown
        CancelInvoke("Countdown");

        // hide the get ready message and start the gameplay
        HideGetReadyMessage();

        // set the current spawn interval to the max (start slow!)
        currentSpawnInterval = waspSpawnMaxInterval;

        // call to begin the invasion!
        StartAttackWave();

        // set up a repeating invoke to increase the spawn speed every second
        InvokeRepeating("IncreaseSpawnSpeed", 1, 1);

        // change the game state ready to play!
        targetState = GameState.InGame;
    }

    void EndGame()
    {
        targetState = GameState.GameOver;
    }

    void UpdateGameState()
    {
        switch (targetState)
        {
            // if we are to transition to the 'InGame' state, we may need to do some things as we exit from the current game state..

            case GameState.InGame:
                // if we are starting the game (transitioning from the GetReady state) we need to do things to start the game!
                if (currentState==GameState.GetReady)
                {
                    StartGame();
                }
                
                // if we are unpausing the game, we need to set time scale back to 1
                if (currentState == GameState.Paused)
                {
                    Time.timeScale = 1;
                }
                break;

            // if the target game state is to pause, we ha ve to set the time scale to 0 to stop everything in place
            case GameState.Paused:
                if (currentState == GameState.InGame)
                {
                    Time.timeScale = 0;
                }
                break;

            case GameState.GameOver:
                // show the game over message
                ShowGameOverMessage();

                // stop the recurring invoked call to spawn wasps
                CancelInvoke("SpawnWasp");
                break;
        }

        // now that we've done anything we needed to do before switching state, we can go ahead and switch now..
        currentState = targetState;
    }

    void LateUpdate()
    {
        // update the UI text to show score andn how many have gotten into the house so far
        sprayedText.text = sprayedScore.ToString();
        stingCountText.text = stings.ToString();

        // check to see if too many invaders
        if(stings>= maxStingsToEndGame)
        {
            EndGame();
        }

        // if the target state is different to the current game state, let's update states
        if(currentState != targetState)
            UpdateGameState();
    }

    void IncreaseSpawnSpeed()
    {
        // as long as the spawn time is greater than the minimum, here we reduce spawn time by 0.1 every second
        if(currentSpawnInterval>waspSpawnMinInterval)
            currentSpawnInterval -= 0.01f;
    }

	void StartAttackWave()
    {
        // drop out if the attack is already underway!
        if (underAttack)
            return;

        // set up a call to spawn the first wasp
        Invoke("SpawnWasp", currentSpawnInterval);
    }

    void SpawnWasp()
    {
        // this is a really cool trick I got from Unity that picks a random point within a circle - it's great for spawning random things like this!
        Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * Random.Range(spawnCircleInnerRadius, spawnCircleOuterRadius);

        // Find a random height between the camera's height and the maximum.
        float randomHeight = Random.Range(minSpawnHeight, maxSpawnHeight);

        // The the random point on the circle is on the XZ plane and the random height is the Y axis.
        Vector3 thePosition = spawnPoint.position + new Vector3(randomCirclePoint.x, randomHeight, randomCirclePoint.y);

        // now we instantiate the wasp at the value held in thePosition 
        tempTR = (Transform) Instantiate(waspPrefab, thePosition, Quaternion.identity);
        
        // and point it at the house / waspTarget so that it will start flying in the right direction
        tempTR.LookAt(waspTarget, Vector3.up);

        // now set up an invoke to call this function and spawn the next wasp
        Invoke("SpawnWasp", Random.Range(currentSpawnInterval, currentSpawnInterval+0.5f));
    }

    void ShowGetReadyMessage()
    {
        // show the get ready message and make sure that the game over message is hidden
        getReadyMessage.SetActive(true);
    }

    void HideGetReadyMessage()
    {
        // show the get ready message and make sure that the game over message is hidden
        getReadyMessage.SetActive(false);
    }

    void ShowGameOverMessage()
    {
        // set the text of the game over message to show the final score
        gameOverMessage.text = "GAME OVER : YOU SPRAYED "+sprayedScore.ToString()+" BUGS";

        // show the get ready message and make sure that the game over message is hidden
        gameOverMessage.gameObject.SetActive(true);
    }

    void HideGameOverMessage()
    {
        // show the get ready message and make sure that the game over message is hidden
        gameOverMessage.gameObject.SetActive(false);
    }
}
