using UnityEngine;

[RequireComponent(typeof(BossEdwardController))]
public class EdwardHealth : MonoBehaviour
{
    [Header("Vida")]
    public int currentHealth;
    public int maxHealth = 10;

    private BossEdwardController controller;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<BossEdwardController>();
    }

    public void ChangeHealth(int amount)
    {
        Debug.Log($"<color=yellow>[EdwardHealth]</color> ChangeHealth chamado! amount={amount}, currentHealth={currentHealth}");

        if (isDead)
        {
            Debug.Log("<color=gray>[EdwardHealth]</color> Ignorado: já morto.");
            return;
        }

        currentHealth += amount;

        Debug.Log($"<color=cyan>[EdwardHealth]</color> Novo HP = {currentHealth}");

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentHealth <= 0)
        {
            Debug.Log("<color=red>[EdwardHealth]</color> ENTRANDO NO BLOCO DE MORTE!!!");

            currentHealth = 0;
            isDead = true;

            if (controller != null)
            {
                Debug.Log("<color=orange>[EdwardHealth]</color> Chamando controller.Die()...");
                controller.Die();
            }
            else
            {
                Debug.LogWarning("<color=magenta>[EdwardHealth]</color> Controller é NULL!");
            }

            var cleaner = new GameObject("EdwardCleanupExecutor");
            DontDestroyOnLoad(cleaner);
            cleaner.AddComponent<BossEdwardCleanup>().StartCleanup();

            Debug.Log("<color=lime>[EdwardHealth]</color> Executor criado, boss será destruído em 0.1s...");
            Destroy(gameObject, 0.1f);
        }
    }
}
