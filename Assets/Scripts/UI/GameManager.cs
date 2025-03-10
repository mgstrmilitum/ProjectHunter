using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using System;
//using UnityEditor.ProBuilder;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;
    public Player playerScript;
    public PlayerMovement movementScript;
    public PlayerStatsSO statsSO;


    //----Menus-----//
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject WheelMenu;
    public GameObject statsMenu;
    
    public bool isPaused;
    public bool isMainmenu;

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
    public int totalGarlicInLevel;
    private bool readyForNextLvl;
    public GameObject tutorialUI;
    public TMP_Text tutorialText;
    [Header("-----Loading Screen-----")]
    public GameObject loadingScreen;
    public Image loadingBarFill;
    public Image loadingScreenImage;
    public Sprite[] loadingScreenTextures;

    [Header("Progress")]
    public bool beatenLvl1Boss;

    [Header("Input System")]
    public PlayerControls controls;

    [Header("HurtOverlay")]
    public GameObject lavaOverlay;

    [Header("ScoreBoard")]
    public GameStats gameStats;
    public TMP_Text tshotsFired;
    public TMP_Text tshotsHit;
    public TMP_Text tAccuracy;
    public TMP_Text tKills;
    public TMP_Text tGarlic;

    public bool displayStats;

   public struct GameStats
    {
        public int shotsFired;
        public int shotsHit;
        public int numKills;
        public int enemiesTotal;
    }
    void Awake()
    {
        Instance = this;
        movementScript = player.GetComponent<PlayerMovement>();
        Time.timeScale = 1f;
        totalGarlicInLevel = 0;
        //controls = new PlayerControls();
    }

    public void Start()
    {
        readyForNextLvl = false;
        //gameStats.enemiesRemaining = gameStats.enemiesTotal;
    }

    public IEnumerator ShowGarlicStats()
    {
        garlicLabel.SetActive(true);
        yield return new WaitForSeconds(5f);
        garlicLabel.SetActive(false);
    }


    private void Update()
    {
        if (!isMainmenu)
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

            //DisplayLevelStats();
        }
    }

    public void OnLose()
    {
        StatePause();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }
    public void OnWin() {
        StatePause();
        DisplayLevelStats();
        activeMenu = winMenu;
        activeMenu.SetActive(true);

       
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

    public void DisplayLevelStats()
    {
        StatePause();
        tshotsFired.text = gameStats.shotsFired.ToString();
        tshotsHit.text = gameStats.shotsHit.ToString();
        //tAccuracy.text = ((float)(gameStats.shotsHit / gameStats.shotsFired)).ToString();
        tKills.text = gameStats.numKills.ToString() + "/" + gameStats.enemiesTotal.ToString();
        tGarlic.text = playerScript.garlicsInCurrLevel.ToString() + " / " + totalGarlicInLevel.ToString();
        loadingScreenImage.sprite = loadingScreenTextures[statsSO.currentStage-2]; //-2 because this is called after LevelLoader.OnLevelEnd(), so first time this gets called will
                                                                                    //be at end of level 1, after statsSO.currentStage has been incremented to 2
        loadingScreen.SetActive(true);
        activeMenu = statsMenu;
        activeMenu.SetActive(true);
        //tenemiesRemaining.text = gameStats.enemiesRemaining.ToString();
        //tenemiesTotal.text = gameStats.enemiesTotal.ToString();
    }

    public void LoadScene(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);

        while(!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress);

            loadingBarFill.fillAmount = progressValue;
            yield return null;
        }

        yield return new WaitForSeconds(5f);
    }

    internal void SetHealthWithoutLerp()
    {
        float hFraction = (float)playerScript.currentHealth / playerScript.maxHealth;
        float sFraction = (float)playerScript.currentShield / playerScript.maxShield;
        float aFraction = (float)playerScript.currentAp / playerScript.maxAp;

        if (playerHealthBarBack != null)
        {
            playerHealthBar.fillAmount = hFraction;
            playerHealthBarBack.fillAmount = hFraction;
        }

        if (playerShieldBarBack != null)
        {
            playerShieldBar.fillAmount = sFraction;
            playerShieldBarBack.fillAmount = sFraction;
        }

        if (abilityMeterBack != null)
        {
            abilityMeterFront.fillAmount = aFraction;
            abilityMeterBack.fillAmount = aFraction;
        }
    }
}
    #endregion

