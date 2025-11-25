using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraSegue : MonoBehaviour
{
    public Transform player;
    public Collider2D mapaCollider;
    public float velocidade = 0.05f;
    public float cameraZ = -10f;
    public Vector2 offset;

    private Camera cam;
    private float halfHeight;
    private float halfWidth;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private bool overridingTarget = false;
    private Transform overrideTarget;
    public float velocidadeCutscene = 0.1f;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnEnable()
    {
        AtualizarReferenciaPlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScreen") return;
        AtualizarReferenciaPlayer();
    }

    private void AtualizarReferenciaPlayer()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
            {
                player = found.transform;
                Vector3 pos = player.position;
                pos.z = cameraZ;
                transform.position = pos;
            }
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "TitleScreen") return;

        cam = GetComponent<Camera>();

        if (player == null)
            AtualizarReferenciaPlayer();

        if (player != null)
        {
            Vector3 startPos = new Vector3(player.position.x, player.position.y, cameraZ);
            transform.position = startPos;
        }

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;

        if (mapaCollider != null)
        {
            Bounds b = mapaCollider.bounds;
            minBounds = b.min;
            maxBounds = b.max;
        }

        Vector3 pos2 = transform.position;
        pos2.z = cameraZ;
        transform.position = pos2;
    }

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "TitleScreen") return;
        if (player == null)
        {
            AtualizarReferenciaPlayer();
            if (player == null) return;
        }

        Transform target = overridingTarget && overrideTarget != null ? overrideTarget : player;

        Vector3 posToGo = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            cameraZ
        );

        if (mapaCollider != null)
        {
            float clampX = Mathf.Clamp(posToGo.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
            float clampY = Mathf.Clamp(posToGo.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
            posToGo = new Vector3(clampX, clampY, posToGo.z);
        }

        float vel = overridingTarget ? velocidadeCutscene : velocidade;

        transform.position = Vector3.Lerp(transform.position, posToGo, vel);
    }

    public void BeginTemporaryFocus(Transform target)
    {
        overrideTarget = target;
        overridingTarget = true;
    }

    public void EndTemporaryFocus()
    {
        overridingTarget = false;
        overrideTarget = null;
    }

    public IEnumerator FocusSequence(Transform target, float settleTime, float holdTime)
    {
        BeginTemporaryFocus(target);
        yield return new WaitForSeconds(settleTime);
        yield return new WaitForSeconds(holdTime);
        EndTemporaryFocus();
    }
}
