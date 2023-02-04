using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    NOT_STARTED,
    MAZE,
    SCORING,
    DRAFT,
    GAME_END,
}

public class GameController : MonoBehaviour
{

    public static GameController Instance { get; private set; }
    
    const int MAX_PLAYERS = 6;
    public GameState game_state;
    public List<Player> players;
    void Start()
    {
        players = new List<Player>();
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            players.Add(new Player());
        }
        StartGame(6);
    }

    public void StartGame(int human_players) {
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            players[i].Init(i < human_players);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void StartMaze(int maze_id) {

    }

    public void StartDraft(int round) {

    }

    public void CreateRatForPlayer(int i) {

    }
}
