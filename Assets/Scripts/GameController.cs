using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {
    NOT_STARTED,
    MAZE,
    SCORING,
    DRAFT,
    GAME_END,
}

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject ScoreScreen;
    [SerializeField]
    GameObject DraftScreen;
    [SerializeField]
    GameObject MazeScreen;


    public static GameController Instance { get; private set; }
    
    public GameObject ratPrefab;
    const int MAX_PLAYERS = 6;
    const int MAX_ROUNDS = 10;
    public int current_round = 0;
    public GameState game_state;
    public List<Player> players;
    void Start()
    {
        current_round = 0;
        players = new List<Player>();
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            players.Add(new Player());
        }

        game_state = GameState.NOT_STARTED;
        StartGame(MAX_PLAYERS);
        GameLoopStep();
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
        FindFirstObjectByType<MazeBehaviourScript>().InitializeMaze(maze_id);
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            CreateRatForPlayer(i);
        }
    }

    public void StartDraft(int round) {
//        FindFirstObjectByType<DraftController>().InitializeDraft(round);
    }

    public void CreateRatForPlayer(int i) {
        MazeBehaviourScript maze = FindFirstObjectByType<MazeBehaviourScript>() as MazeBehaviourScript;
        float x = maze.startX;
        float y = maze.startY;
        GameObject ratObject = Instantiate(ratPrefab, new Vector3(-5.0f + x * 1.0f, 0.0f, 4.5f - y * 1.0f), Quaternion.identity);

        RatBehaviourScript ratScript = ratObject.GetComponent<RatBehaviourScript>();
        if (ratScript != null) {
            ratScript.InitializeRat(x, y, maze, maze.mazeWidth, maze.mazeHeight,i);
        }

    }

    public void RatReachedMazeEnd(int index) {
        if (!players[index].reached_maze_end) {
            players[index].reached_maze_end = true;
            foreach (Player player in players)
            {
                if (!player.reached_maze_end) {
                    return;
                }
            }
            GameLoopStep();
        }
    }

    public void GameLoopStep() {
        switch (game_state)
        {
            case GameState.NOT_STARTED:
                GoToMaze();
                break;
            case GameState.MAZE:
                GoToScoring();
                break;
            case GameState.SCORING:
                if (current_round < MAX_ROUNDS) {
                    GoToDraft();
                } else {
                    GoToGameEnd();
                }
                break;
            case GameState.DRAFT:
                GoToMaze();
                break;
            case GameState.GAME_END:
                GoToMainMenu();
                break;
            default:
                break;
        }
    }
    void GoToInit() {
        game_state = GameState.NOT_STARTED;
    }

    void GoToMaze() {
        DraftScreen.SetActive(false);
        MazeScreen.SetActive(true);
        ScoreScreen.SetActive(false);
        game_state = GameState.MAZE;
        StartMaze(current_round);
    }
    void GoToScoring() {
        DraftScreen.SetActive(false);
        MazeScreen.SetActive(false);
        ScoreScreen.SetActive(true);
        game_state = GameState.SCORING;
        
    }
    void GoToDraft() {
        current_round += 1;
        DraftScreen.SetActive(true);
        MazeScreen.SetActive(false);
        ScoreScreen.SetActive(false);
        game_state = GameState.DRAFT;
        StartDraft(current_round);
        
    }
    void GoToGameEnd() {
        game_state = GameState.GAME_END;
        GameLoopStep();
    }
    void GoToMainMenu() {
        game_state = GameState.NOT_STARTED;
        SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
    }
}
