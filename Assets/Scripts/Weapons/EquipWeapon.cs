using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EquipWeapon : MonoBehaviour
{
    public Transform equipPos;
    public Weapon currentWeapon;

    [Header("Right Hand Target")]
    [SerializeField] private TwoBoneIKConstraint rightHandIK;
    [SerializeField] private Transform rightHandTarget;
    
    [Header("Left Hand Target")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;
    [Header("-----------")]

    [SerializeField] private Transform IKRightHandPos;
    [SerializeField] private Transform IKLeftHandPos;

    void Start()
    {
        
    }

    
    void Update()
    {
        Equip();
    }

    public void Equip()
    {
        currentWeapon.transform.parent = equipPos.transform; 
        currentWeapon.transform.position = equipPos.position;
        currentWeapon.transform.rotation = equipPos.rotation;

        rightHandTarget.SetPositionAndRotation(IKRightHandPos.position, IKRightHandPos.rotation);
        
        leftHandTarget.SetPositionAndRotation(IKLeftHandPos.position, IKLeftHandPos.rotation);
    }
}
