using UnityEngine;

public class MiniMapScript : MonoBehaviour
{
    public Transform player;
    private void LateUpdate()
    {
        Vector3 newpos = player.position;
        newpos.y = transform.position.y;
        transform.position = newpos;
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
