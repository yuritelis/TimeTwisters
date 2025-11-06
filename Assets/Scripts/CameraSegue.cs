using UnityEngine;

public class CameraSegue : MonoBehaviour
{
    public Transform player;
    public float velocidade = 0.05f;

    void Start()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
                player = found.transform;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 posToGo = new Vector3(player.position.x, player.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, posToGo, velocidade);
    }
}
