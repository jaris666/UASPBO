using UnityEngine;
using UnityEngine.UI; // Wajib ada biar bisa akses Slider

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBarSlider; // Tarik UI Slider kesini nanti

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    // Fungsi ini akan dipanggil oleh Zombie nanti
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Darah Sisa: " + currentHealth); // Cek di Console

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }
    }

    void Die()
    {
        Debug.Log("GAME OVER - REMY MATI");
        // Disini nanti kita kasih animasi mati atau restart game
        // Untuk sekarang, kita matikan gerakannya dulu
        GetComponent<PlayerMovement>().enabled = false;
        // Opsional: Jatuhkan badan ke lantai (kalau ada animasi death)
    }
}