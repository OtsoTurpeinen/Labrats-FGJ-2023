using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    GameObject GameEndScreen;
    
    public ScoreController scoreController;
    public DraftController draftController;


    public GeneScriptableObject[] genePool;

    public static GameController Instance { get; private set; }
    
    public GameObject ratPrefab;
    const int MAX_PLAYERS = 6;
    const int MAX_ROUNDS = 3;
    public int current_round = 0;

    public int selected_player_count = 1;
    public GameState game_state;
    public List<Player> players;
    public List<RatUIController> RatUiControllers; //Set in editor, references to the ratUIs in the bottom of the screen
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
            //is player human + reference to RatUiController for the player/rat
            players[i].Init(i < human_players, RatUiControllers[i]);
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


    public RatGene GetRandomFromPool() {
        GeneScriptableObject def = genePool[Random.Range(0,genePool.Length)];
        RatGene gene = new RatGene();
        gene.fancy_name = def.fancy_name;
        gene.perk = def.perk;
        gene.likelyhood = def.likelyhood;
        gene.value = def.value;
        gene.type = def.type;
        return gene;
    }

    public void StartMaze(int maze_id) {
        FindFirstObjectByType<MazeBehaviourScript>().InitializeMaze(maze_id);
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            CreateRatForPlayer(i);
        }
    }

    public void StartDraft(int round) {
        draftController.StartDraft();
//        FindFirstObjectByType<DraftController>().InitializeDraft(round);
    }

    public void CreateRatForPlayer(int i) {
        MazeBehaviourScript maze = FindFirstObjectByType<MazeBehaviourScript>() as MazeBehaviourScript;
        float x = maze.startX;
        float y = maze.startY;
        GameObject ratObject = Instantiate(ratPrefab, new Vector3(-5.0f + x * 1.0f, 0.0f, 4.5f - y * 1.0f), Quaternion.identity);

        RatBehaviourScript ratScript = ratObject.GetComponent<RatBehaviourScript>();
        if (ratScript != null) {
            ratScript.InitializeRat(x, y, maze, maze.mazeWidth, maze.mazeHeight,i,players[i].my_rat_genetics, players[i].color);
        }

    }

    public void RatReachedMazeEnd(int index, GameObject rat) {
        //Debug.Log("Rat reached maze end! " + index);
        if (!players[index].reached_maze_end) {
            players[index].reached_maze_end = true;
            int score = MAX_PLAYERS+1;
            foreach (Player player in players)
            {
                if (player.reached_maze_end)
                {
                    score--;
                }
            }
            Destroy(rat);
            players[index].AddScore(score);
            //Debug.Log("Scored points! " + score);
            if (score <= 1)
            {
                //Debug.Log("About to step gameloop " + game_state);
                foreach (var p in players)
                {
                    p.reached_maze_end = false;
                }
                GameLoopStep();
            }
        }
    }

    public void GameLoopStep() {
        switch (game_state)
        {
            case GameState.NOT_STARTED:
                //Debug.Log("GAMESTATE Start->maze");
                GoToMaze();
                break;
            case GameState.MAZE:
                //Debug.Log("GAMESTATE Scoring");
                GoToScoring();
                break;
            case GameState.SCORING:
                //Debug.Log("GAMESTATE Drafting");
                if (current_round < MAX_ROUNDS) {
                    GoToDraft();
                } else {
                    GoToGameEnd();
                }
                break;
            case GameState.DRAFT:
                //Debug.Log("GAMESTATE Maze");
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
        //MazeScreen.SetActive(true);
        ScoreScreen.SetActive(false);
        game_state = GameState.MAZE;
        GameEndScreen.SetActive(false);
        StartMaze((current_round + 1));
    }
    void GoToScoring() {
        
        MazeBehaviourScript maze = FindFirstObjectByType<MazeBehaviourScript>() as MazeBehaviourScript;
        maze.CleanUpMaze();
        DraftScreen.SetActive(false);
        //MazeScreen.SetActive(false);
        ScoreScreen.SetActive(true);
        GameEndScreen.SetActive(false);
        game_state = GameState.SCORING;

        ScoreScreen.GetComponent<ScoreController>().UpdateOverallScores(players);
        GameLoopStep();
    }
    void GoToDraft() {
        current_round += 1;
        DraftScreen.SetActive(true);
        //MazeScreen.SetActive(false);
        ScoreScreen.SetActive(true);
        GameEndScreen.SetActive(false);
        game_state = GameState.DRAFT;
        StartDraft(current_round);
        
    }
    void GoToGameEnd() {
        game_state = GameState.GAME_END;
        List<Player> orderedList = players.OrderByDescending(x => x.OverallScore).ToList();
        GameEndScreen.SetActive(true);
        EndScreen endScreen = GameEndScreen.GetComponent<EndScreen>();
        

    }
    void GoToMainMenu() {
        game_state = GameState.NOT_STARTED;
        SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
    }
}
