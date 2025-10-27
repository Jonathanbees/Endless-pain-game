using System.Collections;
using UnityEngine;

/// <summary>
/// Detects lethal crashes and triggers a game over screen.
/// Conditions: a heavy impact followed by a strong speed drop or near stop.
/// </summary>
public class DeathController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Rigidbody carRigidbody;
    [SerializeField] CarHandler carHandler;
    [SerializeField] CarCollisionHandler collisionHandler;
    [Tooltip("UI root to show on game over (enable on death)")]
    [SerializeField] GameObject gameOverScreen;

    [Header("Death Conditions")]
    [Tooltip("If speed drops by this fraction after impact, consider death (0.6 = 60% drop)")]
    [Range(0f, 1f)]
    [SerializeField] float dropFractionThreshold = 0.6f;
    [Tooltip("Alternatively, if speed falls below this, consider death")]
    [SerializeField] float stopSpeedThreshold = 1.5f;
    [Tooltip("Delay after impact to evaluate speed drop (seconds)")]
    [SerializeField] float postImpactCheckDelay = 0.25f;

    [Header("Additional Conditions")]
    [Tooltip("If enabled, any heavy impact (as detected by CarCollisionHandler) causes immediate death, without checking speed drop.")]
    [SerializeField] bool dieOnAnyHeavyImpact = false;
    [Tooltip("Optional absolute speed drop threshold (m/s). If > 0 and exceeded after impact, causes death.")]
    [SerializeField] float minAbsoluteDrop = 0f;

    [Header("Score/Distance")]
    [Tooltip("Optional reference to ProgressCounter to read final distance.")]
    [SerializeField] ProgressCounter progressCounter;

    [Header("Debug")]
    [SerializeField] bool debugLogs = true;

    float lastSpeed;
    bool isDead = false;

    void Reset()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carHandler = GetComponent<CarHandler>();
        collisionHandler = GetComponent<CarCollisionHandler>();
    }

    void Awake()
    {
        if (carRigidbody == null) carRigidbody = GetComponent<Rigidbody>();
        if (carHandler == null) carHandler = GetComponent<CarHandler>();
        if (collisionHandler == null) collisionHandler = GetComponent<CarCollisionHandler>();
#if UNITY_2023_1_OR_NEWER
        if (progressCounter == null) progressCounter = UnityEngine.Object.FindFirstObjectByType<ProgressCounter>();
#else
        if (progressCounter == null) progressCounter = FindObjectOfType<ProgressCounter>();
#endif
    }

    void OnEnable()
    {
        if (collisionHandler != null)
        {
            // Subscribe to heavy impact event
            collisionHandler.onHeavyImpact.AddListener(OnHeavyImpact);
            if (debugLogs) Debug.Log("[DeathController] Subscribed to onHeavyImpact.");
        }
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (collisionHandler != null)
        {
            collisionHandler.onHeavyImpact.RemoveListener(OnHeavyImpact);
        }
    }

    void FixedUpdate()
    {
        if (carRigidbody == null) return;
        lastSpeed = carRigidbody.linearVelocity.magnitude;
    }

    void OnHeavyImpact()
    {
        if (isDead) return;
        if (debugLogs) Debug.Log($"[DeathController] Heavy impact event. speedBefore={lastSpeed:F2}");
        if (dieOnAnyHeavyImpact)
        {
            if (debugLogs) Debug.Log("[DeathController] dieOnAnyHeavyImpact = true. Triggering immediate Game Over.");
            TriggerGameOver();
            return;
        }
        // Evaluate speed drop shortly after impact resolution
        StartCoroutine(EvaluatePostImpactCO(lastSpeed));
    }

    IEnumerator EvaluatePostImpactCO(float speedBeforeImpact)
    {
        yield return new WaitForSeconds(postImpactCheckDelay);

        float speedNow = carRigidbody != null ? carRigidbody.linearVelocity.magnitude : 0f;
        float absoluteDrop = Mathf.Max(0f, speedBeforeImpact - speedNow);
        bool strongDrop = speedBeforeImpact > 0f && (speedBeforeImpact - speedNow) / speedBeforeImpact >= dropFractionThreshold;
        bool almostStopped = speedNow <= stopSpeedThreshold;
        bool absDropKill = minAbsoluteDrop > 0f && absoluteDrop >= minAbsoluteDrop;

        if (debugLogs)
        {
            float dropFrac = speedBeforeImpact > 0f ? (speedBeforeImpact - speedNow) / speedBeforeImpact : 0f;
            Debug.Log($"[DeathController] Post-impact check: before={speedBeforeImpact:F2}, now={speedNow:F2}, absDrop={absoluteDrop:F2}, drop={dropFrac:P0}, strongDrop={strongDrop}, absDropKill={absDropKill}, almostStopped={almostStopped}");
        }

        if (strongDrop || almostStopped || absDropKill)
        {
            TriggerGameOver();
        }
    }

    public void TriggerGameOver()
    {
        if (isDead) return;
        isDead = true;

        // Stop time and controls
        Time.timeScale = 0f;
        if (carHandler != null) carHandler.TriggerExploded();
        if (debugLogs) Debug.Log("[DeathController] GAME OVER: Time scaled to 0, controls disabled.");

        // Show UI overlay if provided
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            var goUI = gameOverScreen.GetComponent<GameOverUI>();
            if (goUI != null)
            {
                int meters = 0;
                if (carHandler != null)
                {
                    meters = Mathf.FloorToInt(carHandler.DistanceTraveled);
                }
                else if (progressCounter != null)
                {
                    meters = progressCounter.CurrentMeters;
                }
                goUI.SetDistance(meters);
                if (debugLogs) Debug.Log($"[DeathController] Set final distance on GameOverUI: {meters} m");
            }
            else if (debugLogs) Debug.Log("[DeathController] GameOverScreen has no GameOverUI component.");
        }
        else if (debugLogs) Debug.Log("[DeathController] No GameOverScreen assigned (optional). UI not shown.");
    }
}
