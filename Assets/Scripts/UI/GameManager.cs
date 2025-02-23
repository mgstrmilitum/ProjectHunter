using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
//using UnityEditor.ProBuilder;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;
    public Player playerScript;
    public PlayerMovement movementScript;

    //----Menus-----//
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject WheelMenu; 
    bool isPaused;


    //----Player Health-----//
    [Header("-----Player Info-----")]
    public cameraController mainCamera;
    public Image playerHealthBar;
    public Image playerHealthBarBack;
    public Image playerShieldBar;
    public Image playerShieldBarBack;
    public float barLerpSpeed;

    [Header("-----Player Info-----")]
    public Image abilityMeterFront;
    public Image abilityMeterBack;

    [Header("-----Context-Specific Buttons-----")]
    [SerializeField] public GameObject buttonInteract;
    [SerializeField] public TMP_Text buttonInfo;
    public GameObject buttonLocked;
    public GameObject garlicLabel;
    public TMP_Text garlicCount;

    [Header("Progress")]
    public bool beatenLvl1Boss;

    [Header("Input System")]
    public PlayerControls controls;

    void Awake()
    {
        Instance = this;
        movementScript = player.GetComponent<PlayerMovement>();
        Time.timeScale = 1f;
        //controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    public IEnumerator ShowGarlicStats()
    {
        garlicLabel.SetActive(true);
        yield return new WaitForSeconds(5f);
        garlicLabel.SetActive(false);
    }


    private void Update()
    {
        UpdatePlayerUI();

        
        if (Input.GetButtonDown("Cancel") && WeaponWheelController.weaponWheelOpened == false)
        {
            if (activeMenu == null)
            {
                StatePause();
                activeMenu = pauseMenu;
                activeMenu.SetActive(isPaused);
            }
            else if (activeMenu == pauseMenu)
            {
                StateUnpause();
            }
        }
    }

    public void OnLose()
    {
        StatePause();
        activeMenu = loseMenu;
        activeMenu.SetActive(isPaused);
    }
    public void OnWin() {
        StatePause();
        activeMenu = winMenu;
        activeMenu.SetActive(isPaused);

       
    }
    #region Menus

    public void HandlePauseMenu()
    {

    }
    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;
    }
    public void LoadLevel(int level)
    {
        SceneManager.LoadSceneAsync(level);
    }
    #endregion

    #region UI
    void UpdatePlayerUI()
    {

        float hFraction = (float)playerScript.currentHealth / playerScript.maxHealth;
        float sFraction = (float)playerScript.currentShield / playerScript.maxShield;
        float aFraction = (float)playerScript.currentAp / playerScript.maxAp;

        if (playerHealthBarBack != null)
        {
            if (playerHealthBarBack.fillAmount > hFraction)
            {
                playerHealthBarBack.color = Color.red;
                playerHealthBar.fillAmount = hFraction;
                playerHealthBarBack.fillAmount = Mathf.Lerp(playerHealthBarBack.fillAmount, hFraction, Time.deltaTime * barLerpSpeed);
            }
            else if (playerHealthBarBack != null && playerHealthBar.fillAmount < hFraction)
            {
                playerHealthBarBack.color = Color.green;
                playerHealthBarBack.fillAmount = hFraction;

                playerHealthBar.fillAmount = Mathf.Lerp(playerHealthBar.fillAmount, hFraction, Time.deltaTime * barLerpSpeed);
            }
        }

        if (playerShieldBarBack != null)
        {
            if (playerShieldBarBack.fillAmount > sFraction)
            {
                playerShieldBarBack.color = Color.red;
                playerShieldBar.fillAmount = sFraction;
                playerShieldBarBack.fillAmount = Mathf.Lerp(playerShieldBarBack.fillAmount, sFraction, Time.deltaTime * barLerpSpeed);
            }
            if (playerShieldBar.fillAmount < sFraction)
            {
                playerShieldBarBack.color = Color.green;
                playerShieldBarBack.fillAmount = sFraction;

                playerShieldBar.fillAmount = Mathf.Lerp(playerShieldBar.fillAmount, sFraction, Time.deltaTime * barLerpSpeed);
            }
        }
        if (abilityMeterBack != null)
        {
            if (abilityMeterBack.fillAmount > aFraction)
            {
                abilityMeterBack.color = Color.red;
                abilityMeterFront.fillAmount = aFraction;
                abilityMeterBack.fillAmount = Mathf.Lerp(abilityMeterBack.fillAmount, aFraction, Time.deltaTime * barLerpSpeed);
            }
            if (abilityMeterFront.fillAmount < aFraction)
            {
                abilityMeterBack.color = Color.green;
                abilityMeterBack.fillAmount = aFraction;

               abilityMeterFront.fillAmount = Mathf.Lerp(abilityMeterFront.fillAmount, aFraction, Time.deltaTime * barLerpSpeed);
            }
        }
    }
}
    #endregion

