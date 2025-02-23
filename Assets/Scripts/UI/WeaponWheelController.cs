using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WeaponWheelController : MonoBehaviour
{
    public Animator anim;
    public static bool weaponWheelOpened;
    public static int weaponId;
    public GameObject wheelUI;
    public static bool buttonSelected = false;
    
    public List<GameObject> WheelButtons;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            weaponWheelOpened = !weaponWheelOpened;

            if(weaponWheelOpened)
            {

                GameManager.Instance.StatePause();
                Time.timeScale = .15f;
                wheelUI.SetActive(true);
                Debug.Log("Playing open anim");
                anim.SetBool("OpenWeaponWheel", true);
                
            }
            else
            {
                GameManager.Instance.isPaused = !GameManager.Instance.isPaused;
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Debug.Log("Playing close anim");
                anim.SetBool("OpenWeaponWheel", false);
               
            }
        }

  
        if(buttonSelected == true)
        {
            SetActiveWeapon(weaponId);
        }
               

    }
    public void SetActiveWeapon(int id)
    {
        foreach (GameObject weapon in WheelButtons)
        {
            WeaponWheel weaponButton = weapon.GetComponent<WeaponWheel>();
            if (weaponButton.Id == id)
            {
                weaponButton.item.SetActive(true);
            }
            else { weaponButton.item.SetActive(false); }
        }
        buttonSelected = false;
    }
}
