using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Mynetwork : NetworkManager
{
    [Header("Game Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float gameTime = 60f;

    private float timer;
    private bool gameEnded = false;
    private List<MynetworkPlayer> players = new List<MynetworkPlayer>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        players.Clear();
        gameEnded = false;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        MynetworkPlayer p = conn.identity.GetComponent<MynetworkPlayer>();
        if (p != null)
        {
            players.Add(p);
            p.SetDisplayName("Player " + players.Count);
        }

        // ✅ เริ่มเกมเมื่อมี 2 คน (ปรับได้)
        if (players.Count >= 2)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        timer = gameTime;
        gameEnded = false;

        CancelInvoke();
        InvokeRepeating(nameof(SpawnCoin), 2f, 2f);
    }

    void Update()
    {
        if (!NetworkServer.active || gameEnded) return;

        timer -= Time.deltaTime;

        foreach (var p in players)
        {
            if (p != null)
                p.RpcUpdateTimer(timer);
        }

        if (timer <= 0)
        {
            timer = 0;
            gameEnded = true;
            EndGame();
        }
    }

    void SpawnCoin()
    {
        if (!NetworkServer.active || gameEnded) return;

        Vector2 pos = new Vector2(Random.Range(-7f, 7f), Random.Range(-4f, 4f));
        GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(coin);
    }

    void EndGame()
    {
        CancelInvoke(nameof(SpawnCoin));

        MynetworkPlayer winner = null;
        int maxScore = -1;

        foreach (var p in players)
        {
            if (p != null && p.GetScore() > maxScore)
            {
                maxScore = p.GetScore();
                winner = p;
            }
        }

        if (winner != null)
        {
            foreach (var p in players)
            {
                if (p != null)
                    p.RpcShowWinner(winner.GetName(), maxScore);
            }
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            MynetworkPlayer p = conn.identity.GetComponent<MynetworkPlayer>();
            if (p != null) players.Remove(p);
        }

        base.OnServerDisconnect(conn);
    }
}