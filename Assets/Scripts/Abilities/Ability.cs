using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public string abilityName;
    public float cooldownTime;
    public float activeTime;
    public float abilityCost;




    public abstract void Activate(GameObject parent);

}
