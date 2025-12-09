using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    public float hp = 50f; // Nyawa awal Zombie
    private Animator animator;
    private NavMeshAgent agent;
    private Collider col;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider>();
    }

    public void KenaHit(float damage)
    {
        hp -= damage;
        // Debug.Log("Zombie kena pukul! Sisa HP: " + hp);

        if (hp <= 0)
        {
            Mati();
        }
    }

    void Mati()
    {
        // Matikan semua fungsi zombie biar jadi mayat
        if (animator != null) animator.SetTrigger("Die");

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (col != null) col.enabled = false; // Biar bisa dilewatin

        // Matikan script ngejar
        var ai = GetComponent<ZombieAI>();
        if (ai != null) ai.enabled = false;

        // Hancurkan mayat setelah 5 detik
        Destroy(gameObject, 5f);
    }
}