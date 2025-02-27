using UnityEngine;

public class MiniMapScript : MonoBehaviour
{
    public Transform player;
    private void LateUpdate()
    {
        Vector3 newpos = player.position;
        newpos.y = player.position.y + 45f;
        transform.position = newpos;
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
