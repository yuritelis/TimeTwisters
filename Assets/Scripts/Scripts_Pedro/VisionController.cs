using UnityEngine;

public class VisionController : MonoBehaviour
{
    [Header("Referências")]
    public PlayerHealth playerHealth;
    public Transform visionMask;

    [Header("Configuração da Visão")]
    public float minScale = 0.3f;
    public float maxScale = 3f;
    public float smoothSpeed = 5f;

    private Camera mainCam;

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        mainCam = Camera.main;

        // 🔹 Garante que o círculo esteja no centro da câmera
        if (visionMask != null)
        {
            visionMask.parent = mainCam.transform;
            visionMask.localPosition = new Vector3(0, 0, 1); // 1 unidade na frente da câmera
        }
    }

    private void LateUpdate()
    {
        if (playerHealth == null || visionMask == null)
            return;

        float healthPercent = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        float targetScale = Mathf.Lerp(minScale, maxScale, healthPercent);

        visionMask.localScale = Vector3.Lerp(
            visionMask.localScale,
            new Vector3(targetScale, targetScale, 1f),
            Time.deltaTime * smoothSpeed
        );
    }
}
