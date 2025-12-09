using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Wajib ada buat fitur Delay

public class ZombieAI : MonoBehaviour
{
    [Header("Target & Sensor")]
    public Transform targetPlayer;
    public float jarakPandang = 20f;
    public float jarakSerang = 2.5f;

    [Header("Setting Serangan")]
    public float jedaSerangan = 2.0f;
    public float damagePukulan = 10f;

    // BERAPA DETIK NUNGGU SAMPAI TANGANNYA KENA? (Sesuaikan ini nanti)
    public float delayKenaHit = 0.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float timerSerangan = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        animator.applyRootMotion = false;
        agent.stoppingDistance = jarakSerang - 0.5f;
        agent.speed = 6f;
    }

    void Update()
    {
        if (targetPlayer == null) return;

        float jarak = Vector3.Distance(transform.position, targetPlayer.position);

        // --- 1. JAUH (Diam) ---
        if (jarak > jarakPandang)
        {
            agent.isStopped = true;
            animator.SetBool("isMoving", false);
            animator.ResetTrigger("Attack");
        }
        // --- 2. KEJAR ---
        else if (jarak > jarakSerang)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
            animator.SetBool("isMoving", true);
            animator.ResetTrigger("Attack");
        }
        // --- 3. SERANG ---
        else
        {
            agent.isStopped = true;
            animator.SetBool("isMoving", false);

            Vector3 arah = (targetPlayer.position - transform.position).normalized;
            arah.y = 0;
            if (arah != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(arah), Time.deltaTime * 10f);
            }

            // Logika Serang
            if (Time.time >= timerSerangan)
            {
                // 1. Mainkan Animasi DULUAN
                animator.SetTrigger("Attack");

                // 2. Jalankan Perhitungan Damage TAPI TUNGGU SEBENTAR
                StartCoroutine(ProsesDamage());

                // 3. Reset Timer
                timerSerangan = Time.time + jedaSerangan;
            }
        }
    }

    // Fungsi Khusus buat Nunggu (Coroutine)
    IEnumerator ProsesDamage()
    {
        // Tunggu sekian detik sesuai settingan di Inspector
        yield return new WaitForSeconds(delayKenaHit);

        // Cek lagi jaraknya (biar kalau player udah kabur jauh, gak kena hit ghaib)
        float jarakSekarang = Vector3.Distance(transform.position, targetPlayer.position);

        // Kalau masih dekat, baru kurangi darah
        if (jarakSekarang <= jarakSerang + 1.0f)
        {
            PlayerHealth playerHp = targetPlayer.GetComponent<PlayerHealth>();
            if (playerHp != null)
            {
                playerHp.TakeDamage(damagePukulan);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jarakPandang);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jarakSerang);
    }
}