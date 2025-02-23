using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;
public class WeaponWheel : MonoBehaviour
{
    public int Id;
    public string itemName;
    public GameObject item;

    private Animator anim;
    public Sprite icon;
    public TextMeshProUGUI itemText;
    public Image selectedItem;

    private bool selected = false;


    void Start()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (selected)
        {
            //selectedItem.sprite = icon;
            itemText.text = itemName;

        }
    }

    public void HoverEnter()
    {
        anim.SetBool("Hover", true);
        itemText.text = itemName;
    }
    public void Selected() { selected = true; WeaponWheelController.buttonSelected = true;  WeaponWheelController.weaponId = Id; }
    public void Deselected() { selected = false; WeaponWheelController.weaponId = 0; }

    public void HoverExit()
    {
        anim.SetBool("Hover", false);
        itemText.text = "";
    }
}
