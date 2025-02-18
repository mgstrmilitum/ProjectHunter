using UnityEngine;

public class MouseScrollWheel : MonoBehaviour
{
    public int selectWeapon_ = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectedWeapon();
    }

    // Update is called once per frame
    void Update()
    {

        int prevoiusSelectedweapon = selectWeapon_;


        if (Input.GetAxis("Mouse ScrollWheel")>0f)
        {
            if (selectWeapon_>= transform.childCount-1)
            {
                selectWeapon_ = 0;
            }
            else
            {
                selectWeapon_++;
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel")<0f)
        {
            if (selectWeapon_ <= 0)
            {
                selectWeapon_ = transform.childCount-1;
            }
            else
            {
                selectWeapon_--;
            }

        }

        if (prevoiusSelectedweapon!=selectWeapon_)
        {
            SelectedWeapon();
        }
    }
    void SelectedWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if ((i==selectWeapon_))
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
