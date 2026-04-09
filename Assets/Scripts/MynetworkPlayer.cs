using UnityEngine;
using Mirror;
using TMPro;
using Unity.Cinemachine;

public class MynetworkPlayer : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // SyncVar จะส่งค่าจาก Server ไปยัง Client ทุกคนอัตโนมัติ
    [SyncVar(hook = nameof(OnNameChanged))] private string displayName = "";
    [SyncVar(hook = nameof(OnScoreChanged))] private int score = 0;
    [SyncVar(hook = nameof(OnColorChanged))] private Color playerColor = Color.white;

    // ================= START LOGIC =================

    public override void OnStartServer()
    {
        base.OnStartServer();
        // สุ่มสีทันทีที่เกิดบน Server ทำให้ Player 1 และ 2 ได้สีไม่ซ้ำกันตั้งแต่เริ่ม
        playerColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        // บังคับอัปเดตสีทันทีที่ Client เชื่อมต่อ
        OnColorChanged(Color.white, playerColor);
        UpdateDisplay();
    }

    public override void OnStartLocalPlayer()
    {
        // ตั้งค่ากล้อง Cinemachine ให้ติดตามเรา (รองรับ Unity 6)
        var vcam = FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.Follow = transform;
            vcam.LookAt = transform;
        }

        // มั่นใจว่า Singleton ของ UI ถูกตั้งค่า
        FindUI();
    }

    void Update()
    {
        // ตรวจสอบว่าเป็นรถของผู้เล่นเราเองหรือไม่
        if (!isLocalPlayer) return;

        // --- การควบคุมการเคลื่อนที่ ---
        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");
        transform.Rotate(0, 0, -turn * 200f * Time.deltaTime);
        transform.Translate(Vector3.up * move * 5f * Time.deltaTime);

        // --- เปลี่ยนสีด้วยปุ่ม T ---
        if (Input.GetKeyDown(KeyCode.T))
        {
            CmdChangeColor();
        }
    }

    private void FindUI()
    {
        if (GameUIFix.Instance == null)
            GameUIFix.Instance = FindFirstObjectByType<GameUIFix>();
    }

    // ================= SERVER ACTIONS =================

    [Server] public void SetDisplayName(string n) => displayName = n;
    [Server] public void AddScore(int v) => score += v;

   [Command]
    void CmdChangeColor() => playerColor = Random.ColorHSV();

    // ================= RPC (UI & Timer) =================

    [ClientRpc]
public void RpcUpdateTimer(float time)
{
    if (GameUIFix.Instance != null)
    {
        // แก้จาก UpdateTimer เป็น UpdateTimerDisplay ให้ตรงกับใน GameUIFix
        GameUIFix.Instance.UpdateTimerDisplay(time); 
    }
}

    [ClientRpc]
    public void RpcShowWinner(string n, int s)
    {
        if (GameUIFix.Instance == null) FindUI();

        if (GameUIFix.Instance != null)
            GameUIFix.Instance.ShowWinner(n, s);
    }

    // ================= HOOKS =================

    void OnNameChanged(string oldV, string newV) => UpdateDisplay();
    void OnScoreChanged(int oldV, int newV) => UpdateDisplay();

    void OnColorChanged(Color oldV, Color newV)
    {
        // ป้องกัน Error กรณีลืมลากใส่ Inspector
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = newV;
        }
    }

    void UpdateDisplay()
    {
        if (displayNameText == null)
            displayNameText = GetComponentInChildren<TMP_Text>();

        if (displayNameText != null)
            displayNameText.text = $"{displayName} ({score})";
    }

    public int GetScore() => score;
    public string GetName() => displayName;
}