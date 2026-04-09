using UnityEngine;

public class AdvancedTankController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 150f;
    public float dashForce = 15f; // เพิ่มแรง Dash ให้เห็นผลชัดขึ้น
    
    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootForce = 20f;
    public float recoilForce = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // เก็บค่าสีต้นฉบับไว้ป้องกันสีเพี้ยน
        if (spriteRenderer != null) {
            originalColor = spriteRenderer.color;
        }

        // ตั้งค่า Rigidbody ไม่ให้รถถังหมุนติ้วเวลาชน
        if (rb != null) {
            rb.gravityScale = 0;
            rb.freezeRotation = true; // ล็อคการหมุนจากแรงฟิสิกส์ (ให้หมุนด้วยโค้ดอย่างเดียว)
        }
    }

    void Update() {
        // บังคับให้สีกลับเป็นปกติทุกเฟรม (แก้ปัญหาสีเปลี่ยน)
        if (spriteRenderer != null) {
            spriteRenderer.color = originalColor;
        }

        // 1. การเคลื่อนที่
        float move = Input.GetAxis("Vertical");
        float rotate = Input.GetAxis("Horizontal");

        transform.Translate(Vector2.up * move * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.forward * -rotate * rotateSpeed * Time.deltaTime);

        // 2. ระบบ Dash (กด Spacebar)
        if (Input.GetKeyDown(KeyCode.Space)) {
            Dash();
        }

        // 3. ระบบยิง
        if (Input.GetButtonDown("Fire1")) {
            Shoot();
        }
    }

    void Dash() {
        if (rb != null) {
            // ใส่แรงพุ่งไปข้างหน้าตามทิศทางของรถถัง
            rb.AddForce(transform.up * dashForce, ForceMode2D.Impulse);
            Debug.Log("Dashing!");
        }
    }

    void Shoot() {
        if (bulletPrefab != null && firePoint != null) {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            
            if (bulletRb != null) {
                bulletRb.AddForce(firePoint.up * shootForce, ForceMode2D.Impulse);
            }

            // แรงดีดกลับ (Recoil)
            if (rb != null) {
                rb.AddForce(-transform.up * recoilForce, ForceMode2D.Impulse);
            }
        }
    }
}