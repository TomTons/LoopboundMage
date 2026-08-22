using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject npcPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button fireballUpgradeButton;
    [SerializeField] private Button healthUpgradeButton;
    [SerializeField] private GameObject interactPrompt;

    [Header("Costs")]
    [SerializeField] private int fireballCost = 20;
    [SerializeField] private int healthCost = 30;

    private PlayerControls controls;
    private bool playerInRange;
    private bool isPanelOpen;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        controls.Player.Interact.performed -= OnInteract;
        controls.Player.Disable();
    }

    private void Start()
    {
        ClosePanel();

        if (closeButton != null)
            closeButton.onClick.AddListener(OnClosePressed);

        if (fireballUpgradeButton != null)
            fireballUpgradeButton.onClick.AddListener(OnFireballUpgrade);

        if (healthUpgradeButton != null)
            healthUpgradeButton.onClick.AddListener(OnHealthUpgrade);
    }

    // ---------- INPUT ----------

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange || isPanelOpen) return;
        OpenPanel();
    }

    // ---------- BUTTON HANDLERS ----------

    private void OnFireballUpgrade()
    {
        if (UpgradeManager.Instance == null) return;

        bool success = UpgradeManager.Instance.UpgradeFireballDamage();
        if (success)
        {
            Debug.Log("[NPC] Fireball upgraded — respawning player");
            TriggerRespawn();
        }
        else
        {
            Debug.Log("[NPC] Not enough coins for fireball upgrade");
        }
    }

    private void OnHealthUpgrade()
    {
        if (UpgradeManager.Instance == null) return;

        bool success = UpgradeManager.Instance.UpgradeMaxHealth();
        if (success)
        {
            Debug.Log("[NPC] Health upgraded — respawning player");
            TriggerRespawn();
        }
        else
        {
            Debug.Log("[NPC] Not enough coins for health upgrade");
        }
    }

    private void OnClosePressed()
    {
        Debug.Log("[NPC] Close pressed — respawning player");
        TriggerRespawn();
    }

    // ---------- RESPAWN ----------

    private void TriggerRespawn()
    {
        // Reset timescale first
        Time.timeScale = 1f;

        // Close panel silently without firing extra events
        isPanelOpen = false;
        if (npcPanel != null)
            npcPanel.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);

        // Unlock player movement
        PlayerMovement2D player = FindFirstObjectByType<PlayerMovement2D>();
        if (player != null)
            player.SetInteracting(false);

        // Kill player — PlayerSpawner handles the rest
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.ForceKill();
        else
            Debug.LogWarning("[NPC] No PlayerHealth found");
    }

    // ---------- PANEL ----------

    private void OpenPanel()
    {
        isPanelOpen = true;
        npcPanel.SetActive(true);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        PlayerMovement2D player = FindFirstObjectByType<PlayerMovement2D>();
        if (player != null)
            player.SetInteracting(true);

        Time.timeScale = 0f;
        Debug.Log("[NPC] Panel opened");
    }

    private void ClosePanel()
    {
        isPanelOpen = false;

        if (npcPanel != null)
            npcPanel.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);

        PlayerMovement2D player = FindFirstObjectByType<PlayerMovement2D>();
        if (player != null)
            player.SetInteracting(false);

        Time.timeScale = 1f;
    }

    // ---------- TRIGGER ----------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (interactPrompt != null)
            interactPrompt.SetActive(true);
        Debug.Log("[NPC] Player entered range");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (isPanelOpen) ClosePanel();
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
        Debug.Log("[NPC] Player left range");
    }
}