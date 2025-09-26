using UnityEngine;

public class CameraSegue : MonoBehaviour
{
    public float velocidade = 0.05f;
    void Start()
    {

    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Vector3 posToGo = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, posToGo, velocidade);
    }
}