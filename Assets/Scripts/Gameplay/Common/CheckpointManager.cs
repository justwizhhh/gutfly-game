using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    // ----------------------
    //
    // This object keeps track of which checkpoint gate is currentlly active, and tells the LevelController to respawn the player there
    // Based on a tutorial by Jonny Aiello (https://jonnyaiello.wordpress.com/2020/06/10/making-level-checkpoints-in-unity/)
    //
    // ----------------------

    [Header("Current Checkpoint Information")]
    public static CheckpointManager active;
    public string SceneID { get { return currentSceneID; } }

    public Vector2 CurrentPosition;
    public PlayerController.PlayerModes CurrentPlayerMode;
    public float CurrentAngle;
    public float CurrentTime;
    public int CurrentScore;

    // Private variables
    private string currentSceneID;
    private bool isPlayerReset;

    private void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        currentSceneID = scene.name;

        if (CheckpointManager.active != null)
        {
            // If the CheckpointManager was assigned to a different level, delete it
            if (CheckpointManager.active.SceneID != scene.name)
            {
                Destroy(CheckpointManager.active.gameObject);
                CheckpointManager.active = null;
            }
            // If the CheckpointManager already exists for this level, delete this current instance
            else
            {
                Destroy(gameObject);
            }
        }

        // Otherwise, make this the currently-active CheckpointManager
        if (CheckpointManager.active == null)
        {
            CheckpointManager.active = this;
            DontDestroyOnLoad(this);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetLevel();
    }

    public void ResetLevel()
    {
        // Reposition the player if a checkpoint has already been processed
        if (CurrentPosition != Vector2.zero)
        {
            LevelController.Instance.ResetPlayer(
                CurrentPosition,
                CurrentPlayerMode);
            LevelController.Instance.ResetLevel(
                CurrentAngle, 
                CurrentTime, 
                CurrentScore);
        }
    }

    public void RestartLevel()
    {
        LevelController.Instance.StartLevel(CurrentPlayerMode);
    }

    public void SetNewCheckpoint(CheckpointGate checkpoint, PlayerController player)
    {
        CurrentPosition = checkpoint.transform.position;
        CurrentPlayerMode = player.currentPlayerMode;

        CurrentAngle = LevelController.Instance.UpAngle;
        CurrentTime = LevelController.Instance.CurrentTime;
        CurrentScore = LevelController.Instance.CurrentScore;
    }
}
