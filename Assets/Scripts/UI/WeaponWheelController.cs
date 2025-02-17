using UnityEngine;

public class WeaponWheelController : MonoBehaviour
{
    public Animator anim;
    private bool weaponWheelOpened;
    public static int weaponId;
    public GameObject wheelUI;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            weaponWheelOpened = !weaponWheelOpened;

            if(weaponWheelOpened)
            {
                wheelUI.SetActive(true);
                Debug.Log("Playing open anim");
                anim.SetBool("OpenWeaponWheel", true);
                
            }
            else
            {   Debug.Log("Playing close anim");
                anim.SetBool("OpenWeaponWheel", false);
               
            }
        }

        

        switch(weaponId)//to-do, logic for when an item is selected from the weapon wheel. 
        {
            case 0:
                break;

            case 1:
                break;

            case 2:
                break;
        }
    }

}
