using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject senjataDiTangan; // Referensi ke senjata yang ngumpet di tangan
    public GameObject senjataDiTanah;  // Referensi ke dirinya sendiri (di tanah)

    void OnTriggerStay(Collider other)
    {
        // Cek apakah yang nabrak adalah Player?
        if (other.gameObject.tag == "Player")
        {
            // Pesan Debug buat ngecek (muncul di Console)
            // Debug.Log("Siap Ambil! Tekan E");

            if (Input.GetKey(KeyCode.E))
            {
                Ambil();
            }
        }
    }

    void Ambil()
    {
        // 1. Munculkan senjata di tangan
        senjataDiTangan.SetActive(true);

        // 2. Aktifkan Script Attack Player (Biar bisa mukul)
        // Pastikan Remy punya script 'PlayerCombat' ya!
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Nyalakan script combat (kalau tadinya dimatikan)
            var combatScript = player.GetComponent<PlayerCombat>();
            if (combatScript != null) combatScript.enabled = true;
        }

        // 3. Hapus senjata di tanah
        Destroy(senjataDiTanah);
    }
}