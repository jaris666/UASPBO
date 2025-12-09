using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackDamage = 10f; // Damage per pukulan
    public LayerMask enemyLayers;

    // Biar gak bisa spam klik pas lagi joget combo
    public float attackCooldown = 2f;
    float nextAttackTime = 0f;

    // --- BAGIAN PENTING (PERBAIKAN BUG) ---
    // Fungsi ini jalan otomatis saat Script/Senjata baru saja aktif
    void OnEnable()
    {
        if (animator != null)
        {
            // Kita paksa matikan sisa-sisa trigger "Attack"
            // Supaya pas senjata muncul, dia gak langsung mukul sendiri
            animator.ResetTrigger("Attack");
        }
    }
    // --------------------------------------

    void Update()
    {
        // Cek Cooldown
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                MulaiCombo();
                // Set cooldown sesuai durasi animasi combo (misal 2 detik)
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void MulaiCombo()
    {
        animator.SetTrigger("Attack"); // Panggil animasi panjang itu
    }

    // --- FUNGSI INI DIPANGGIL ANIMASI (EVENT) ---
    // Fungsi ini akan dijalankan BERKALI-KALI dalam satu animasi sesuai letak Event
    public void BeriDamage()
    {
        // Deteksi musuh
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Kasih Damage
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("KENA PUKUL! " + enemy.name); // Cek console

            var zombieHP = enemy.GetComponent<ZombieHealth>();
            if (zombieHP != null)
            {
                zombieHP.KenaHit(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}