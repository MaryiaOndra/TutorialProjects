using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offcet;

    void Update()
    {
        transform.position = player.position + offcet;
    }
}
