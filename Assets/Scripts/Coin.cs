using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Coin : NetworkBehaviour
{
    public GameObject coinPrefab;
    
    [Header("Settings")]
    public float spawnInterval = 0.5f; // ปรับให้เล็กลง (เช่น 1 หรือ 1.5) เพื่อให้เหรียญเกิดไวและเยอะขึ้น
    public int maxCoins = 20;        // กำหนดจำนวนเหรียญสูงสุดในฉากพร้อมกัน (กันเครื่องค้าง)

    [Header("Spawn Locations")]
    // ลากจุด Empty Object ที่วางไว้ตามถนนไกลๆ มาใส่ใน List นี้
    public List<Transform> roadSpawnPoints; 

    private int currentCoinCount = 0;

    void Start()
    {
        // เริ่มการ Spawn เฉพาะบน Server
        if (isServer) 
        {
            InvokeRepeating("SpawnCoin", 2f, spawnInterval);
        }
    }

    void SpawnCoin()
    {
        // เช็กจำนวนเหรียญในฉาก ถ้าเยอะเกินที่ตั้งไว้จะไม่สร้างเพิ่ม
        GameObject[] existingCoins = GameObject.FindGameObjectsWithTag("Coin");
        if (existingCoins.Length >= maxCoins) return;

        if (roadSpawnPoints.Count == 0) 
        {
            Debug.LogWarning("กรุณาลากจุดเกิดบนถนนใส่ใน List roadSpawnPoints ด้วยครับ!");
            return;
        }

        // สุ่มเลือกจุดจากทั่วมุมตึกหรือถนนที่วางไว้ไกลๆ
        int randomIndex = Random.Range(0, roadSpawnPoints.Count);
        Transform spawnPoint = roadSpawnPoints[randomIndex];

        // สร้างเหรียญ (Quaternion.identity คือการสั่งไม่ให้หมุนตอนเกิด)
        GameObject coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);
        
        // สั่งให้โผล่ในหน้าจอของผู้เล่นทุกคน (Mirror)
        NetworkServer.Spawn(coin); 
    }




        private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;

        MynetworkPlayer player = other.GetComponent<MynetworkPlayer>();

        if (player != null)
        {
            player.AddScore(1);
            NetworkServer.Destroy(gameObject);
        }
    }
}
