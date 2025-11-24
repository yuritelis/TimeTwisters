using UnityEngine;

public class HealingItem : MonoBehaviour
{
    public string itemID = "default_heal_1";
    public int healAmount = 2;
    public AudioClip pickupSound;

    private AudioSource audioSource;

    private void Awake()
    {
        itemID = "heal_" + System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.ChangeHealth(healAmount);

            if (audioSource != null && pickupSound != null)
                audioSource.PlayOneShot(pickupSound);

            PlayerPrefs.SetInt("HEALING_ITEM_" + itemID, 1);
            PlayerPrefs.Save();

            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<Collider2D>());

            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
    }
}
