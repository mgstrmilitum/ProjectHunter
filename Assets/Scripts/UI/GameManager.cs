using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    bool isPaused;


    //----Player Health-----//
    [Header("-----Player Info-----")]
    public Image playerHealthBar;
    public Image playerHealthBarBack;
    public Image playerShieldBar;
    public Image playerShieldBarBack;
    public float barLerpSpeed;
    void Awake()
    {
        Instance = this;

        playerScript = player.GetComponent<Player>();
        movementScript = player.GetComponent<PlayerMovement>();
        Time.timeScale = 1f;

    }




    private void Update()
    {
        UpdatePlayerUI();

        
        if (Input.GetButtonDown("Cancel"))
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
    #region Menus
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
    #endregion

    #region UI
    void UpdatePlayerUI()
    {

        float hFraction = (float)playerScript.currentHealth / playerScript.maxHealth;
        float sFraction = (float)playerScript.currentShield / playerScript.maxShield;


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
    }
}
    #endregion