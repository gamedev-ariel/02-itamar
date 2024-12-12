using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This component spawns the given laser-prefab with shooting cooldown and limited ammunition.
 * It prevents continuous shooting, tracks remaining ammo, and handles game over when out of ammo.
 */
public class LaserShooter : ClickSpawner
{
    [SerializeField]
    [Tooltip("How many points to add to the shooter, if the laser hits its target")]
    private int pointsToAdd = 1;

    [SerializeField]
    [Tooltip("Cooldown time between shots in seconds")]
    private float shootCooldown = 0.5f;

    [SerializeField]
    [Tooltip("Maximum number of shots before reloading")]
    private int maxAmmo = 5;

    [SerializeField]
    [Tooltip("Key to press for shooting")]
    private KeyCode keyToPress = KeyCode.Space;

    [SerializeField]
    [Tooltip("Scene to load when game is over (out of ammo)")]
    private string gameOverSceneName = "GameOverScene";

    // A reference to the field that holds the score that has to be updated when the laser hits its target.
    private NumberField scoreField;

    // Track current ammo and last shot time
    private int currentAmmo;
    private float lastShotTime;
    private bool isGameOver = false;

    private void Start()
    {
        scoreField = GetComponentInChildren<NumberField>();
        if (!scoreField)
            Debug.LogError($"No child of {gameObject.name} has a NumberField component!");

        // Initialize ammo at the start of the level
        ResetAmmo();
    }

    private void Update()
    {
        // Check if game is already over
        if (isGameOver)
        {
            return;
        }

        // Only allow shooting if conditions are met
        if (CanShoot() && Input.GetKeyDown(keyToPress))
        {
            Shoot();

            // Check if this was the last shot
            if (currentAmmo <= 0)
            {
                HandleGameOver();
            }
        }
    }

    private bool CanShoot()
    {
        // Check if enough time has passed since last shot and ammo is available
        bool isCooldownReady = Time.time >= lastShotTime + shootCooldown;
        bool hasAmmoRemaining = currentAmmo > 0;
        return isCooldownReady && hasAmmoRemaining;
    }

    private void Shoot()
    {
        // Spawn laser
        GameObject newObject = spawnObject();

        // Modify the text field of the new object
        ScoreAdder newObjectScoreAdder = newObject.GetComponent<ScoreAdder>();
        if (newObjectScoreAdder)
            newObjectScoreAdder.SetScoreField(scoreField).SetPointsToAdd(pointsToAdd);

        // Reduce ammo and update last shot time
        currentAmmo--;
        lastShotTime = Time.time;
    }

    private void HandleGameOver()
    {
        // Prevent multiple game over calls
        if (isGameOver) return;

        isGameOver = true;

        // Optional: Add any additional game over logic here
        Debug.Log("Game Over: Out of Ammo!");

        // Load the game over scene
        SceneManager.LoadScene(gameOverSceneName);
    }

    // Method to reset ammo (call this at the start of each level)
    public void ResetAmmo()
    {
        currentAmmo = maxAmmo;
        lastShotTime = 0f;
        isGameOver = false;
    }
}