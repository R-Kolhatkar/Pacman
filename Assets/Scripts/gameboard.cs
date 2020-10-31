using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameboard : MonoBehaviour
{
    private static int boardWidth = 28;
    private static int boardHeight = 31;

    private bool didStartDeath = false;
    private bool didStartConsumed = false;

    public static int player1Level = 1;

    public static int player1PelletsConsumed = 0;

    public int totalPellets = 0;
    public static int highScore = 0;
    public static int playerOneScore = 0;
    public static int pacmanLives = 3;

    public static int ghostConsumedRunningScore;

    public bool shouldBlink = false;

    public float blinkIntervalTime = 0.1f;
    private float blinkIntervalTimer = 0;

    public AudioClip background_audio_normal;
    public AudioClip background_audio_frightened;
    public AudioClip backgroundAudioPacmanDeath;
    public AudioClip consumedGhostAudioClip;
    public AudioClip consumedResourceAudioClip;

    public Sprite mazeBlue;
    public Sprite mazeWhite;

    public Text playerText;
    public Text readyText;

    public Text highScoreText;
    public Text player1UP;
    public Text player1ScoreText;
    public Image playerLives2;
    public Image playerLives3;

    public Text consumedGhostScoreText;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    public Image[] levelImage;

    private bool didIncrementLevel = false;

    bool didSpawnBonusItem1;
    bool didSpawnBonusItem2;

    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));


        foreach(GameObject o in objects) {
            Vector2 pos = o.transform.position;

            if (o.name != "pacman_1" && o.name != "Nodes" && o.name != "NonNodes" && o.name != "Maze" &&
                o.name != "Pellets" && o.tag != "Ghost" && o.tag != "GhostHome" && o.name != "Canvas" &&
                o.tag != "UIElements")
            {
                // Check to make sure tile component exists
                if(o.GetComponent<tile>() != null)
                {
                    // Check if we have a pellet or a pellet energizer
                    if(o.GetComponent<tile>().isPellet || o.GetComponent<tile>().isPelletEnergizer)
                    {
                        totalPellets++;
                    }
                }

                board[(int)(pos.x + 13.5), (int)(pos.y + 14.7)] = o;
            }
            else
            {
                // Debug.Log("Found Pacman at: " + pos);
            }
        }

        if(player1Level == 1)
        {
            GetComponent<AudioSource>().loop = false;
            GetComponent<AudioSource>().Play();
        }

        StartGame();
    }

    void Update()
    {
        UpdateUI();

        CheckPelletsConsumed();

        CheckShouldBlink();

        SpawnBonusItems();
    }

    void SpawnBonusItems()
    {
        if (player1PelletsConsumed >= 70 && player1PelletsConsumed < 170)
        {
            if (!didSpawnBonusItem1)
            {
                didSpawnBonusItem1 = true;
                SpawnBonusItemForLevel(player1Level, 1);
            }
        }
        else if (player1PelletsConsumed >= 170)
        {
            if(!didSpawnBonusItem2)
            {
                didSpawnBonusItem2 = true;
                SpawnBonusItemForLevel(player1Level, 2);
            }
        }
    }

    void SpawnBonusItemForLevel(int level, int resourceCount)
    {
        GameObject bonusItem = null;
        if(level == 1)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_cherries", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {

            }
        }
        else if(level == 2)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_strawberry", typeof(GameObject)) as GameObject;
            }
            else if (resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_cherries", typeof(GameObject)) as GameObject;
            }
        }
        else if(level == 3)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {

            }
        }
        else if(level == 4)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_strawberry", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 5)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 6)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {

            }
        }
        else if (level == 7)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 8)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 9)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 10)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 11)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 12)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 13)
        {
            if(resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
            else if(resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 14)
        {
            if (resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
            else if (resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 15)
        {
            if (resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
            else if (resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 16)
        {
            if (resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
            else if (resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 17)
        {
            if (resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
            else if (resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 18)
        {
            if (resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
            else if (resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
        }
        else if (level == 19)
        {
            if (resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
            else if (resourceCount == 2)
            {
                
            }
        }
        else if (level == 20)
        {
            if (resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
            else if (resourceCount == 2)
            {
                
            }
        }
        else if (level >= 21)
        {
            if (resourceCount == 1)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
            else if (resourceCount == 2)
            {
                bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
            }
        }
        Instantiate(bonusItem);
    }

    void UpdateUI()
    {
        player1ScoreText.text = playerOneScore.ToString();
        highScoreText.text = highScore.ToString();

        if(pacmanLives == 3)
        {
            playerLives3.enabled = true;
            playerLives2.enabled = true;
        }
        else if(pacmanLives == 2)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = true;
        }
        else if(pacmanLives == 1)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = false;
        }

        for(int i = 0; i < levelImage.Length; i++)
        {
            Image image = levelImage[i];
            image.enabled = false;
        }

        for (int i = 1; i < levelImage.Length + 1; i++)
        {
            if(player1Level >= i)
            {
                Image image = levelImage[i - 1];
                image.enabled = true;
            }
        }
    }

    void CheckPelletsConsumed()
    {
        // change back to totalpellets after thorough testing
        if(totalPellets == player1PelletsConsumed)
        {
            PlayerWin();
        }
    }

    void PlayerWin()
    {
        // Debug.Log("Player can move to the next level");

        if(!didIncrementLevel)
        {
            didIncrementLevel = true;
            player1Level++;
            StartCoroutine(ProcessWin(2));
        }
    }

    IEnumerator ProcessWin(float delay)
    {
        player1PelletsConsumed = 0;

        GameObject pacman = GameObject.Find("pacman_1");
        pacman.transform.GetComponent<pacman>().canMove = false;
        pacman.transform.GetComponent<Animator>().enabled = false;

        transform.GetComponent<AudioSource>().Stop();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = false;
            ghost.transform.GetComponent<Animator>().enabled = false;
        }

        yield return new WaitForSeconds(delay);

        StartCoroutine(BlinkBoard(2));
    }

    IEnumerator BlinkBoard(float delay)
    {
        GameObject pacman = GameObject.Find("pacman_1");
        pacman.GetComponent<SpriteRenderer>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        // Blink the board
        shouldBlink = true;

        yield return new WaitForSeconds(delay);

        // Restart the game at the next level
        shouldBlink = false;
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        SceneManager.LoadScene("Main");
    }

    private void CheckShouldBlink()
    {
        if(shouldBlink)
        {
            if(blinkIntervalTimer < blinkIntervalTime)
            {
                blinkIntervalTimer += Time.deltaTime;
            }
            else
            {
                blinkIntervalTimer = 0;

                if (GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite == mazeBlue)
                {
                    GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = mazeWhite;
                }
                else
                {
                    GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = mazeBlue;
                }
            }
        }
    }

    public void StartGame()
    {
        // Hide all Ghosts
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
            ghost.transform.GetComponent<Ghost>().canMove = false;
        }

        GameObject pacman = GameObject.Find("pacman_1");
        pacman.transform.GetComponent<SpriteRenderer>().enabled = false;
        pacman.transform.GetComponent<pacman>().canMove = false;

        StartCoroutine(ShowObjectsAfter(2.25f));

        StartCoroutine(StartBlinking(player1UP));
    }

    public void StartConsumed(Ghost consumedGhost)
    {
        if(!didStartConsumed)
        {
            didStartConsumed = true;

            // Pause all the ghosts

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            // Pause pacman
            GameObject pacman = GameObject.Find("pacman_1");
            pacman.transform.GetComponent<pacman>().canMove = false;

            // Hide pacman
            pacman.transform.GetComponent<SpriteRenderer>().enabled = false;

            // Hide consumed ghost
            consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = false;

            // Stop background music
            transform.GetComponent<AudioSource>().Stop();

            Vector2 pos = consumedGhost.transform.position;
            Vector2 viewportPoint = Camera.main.WorldToViewportPoint(pos);

            consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewportPoint;
            consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewportPoint;

            consumedGhostScoreText.text = ghostConsumedRunningScore.ToString();

            consumedGhostScoreText.GetComponent<Text>().enabled = true;

            // Play the consumed sound
            transform.GetComponent<AudioSource>().PlayOneShot(consumedGhostAudioClip);

            // Wait for audio clip to finish
            StartCoroutine(ProcessConsumedAfter(0.75f, consumedGhost));
        }
    }

    public void StartConsumeResource(GameObject bonusItem, int resourceValue)
    {
        // Debug.Log("StartConsumeResource\t" + bonusItem.name);

        Vector2 pos = bonusItem.transform.position;
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(pos);

        consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewportPoint;
        consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewportPoint;

        consumedGhostScoreText.text = resourceValue.ToString();

        consumedGhostScoreText.GetComponent<Text>().enabled = true;

        Destroy(bonusItem.gameObject);

        // Play the consumed sound
        transform.GetComponent<AudioSource>().PlayOneShot(consumedResourceAudioClip);

        StartCoroutine(ProcessConsumedResource(0.75f));
    }

    IEnumerator ProcessConsumedResource(float delay)
    {
        yield return new WaitForSeconds(delay);

        consumedGhostScoreText.GetComponent<Text>().enabled = false;
    }

    IEnumerator StartBlinking(Text blinkText)
    {
        yield return new WaitForSeconds(0.25f);

        blinkText.GetComponent<Text>().enabled = !blinkText.GetComponent<Text>().enabled;
        StartCoroutine(StartBlinking(blinkText));
    }

    IEnumerator ProcessConsumedAfter(float delay, Ghost consumedGhost)
    {
        yield return new WaitForSeconds(delay);

        // Hide the score
        consumedGhostScoreText.GetComponent<Text>().enabled = false;

        // Show pacman
        GameObject pacman = GameObject.Find("pacman_1");
        pacman.transform.GetComponent<SpriteRenderer>().enabled = true;

        // Show consumed ghost
        consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = true;

        // Resume all ghosts
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }

        // Resume pacman
        pacman.transform.GetComponent<pacman>().canMove = true;

        // Start background music
        transform.GetComponent<AudioSource>().Play();

        didStartConsumed = false;
    }

    IEnumerator ShowObjectsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
        }

        GameObject pacman = GameObject.Find("pacman_1");
        pacman.transform.GetComponent<SpriteRenderer>().enabled = true;

        playerText.transform.GetComponent<Text>().enabled = false;

        StartCoroutine(StartGameAfter(2));
    }

    IEnumerator StartGameAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }

        GameObject pacman = GameObject.Find("pacman_1");
        pacman.transform.GetComponent<pacman>().canMove = true;

        readyText.transform.GetComponent<Text>().enabled = false;

        GetComponent<AudioSource>().loop = true;

        transform.GetComponent<AudioSource>().clip = background_audio_normal;
        transform.GetComponent<AudioSource>().Play();
    }

    public void StartDeath()
    {
        if(!didStartDeath)
        {
            StopAllCoroutines();

            GameObject bonusItem = GameObject.Find("bonusItem");

            if(bonusItem)
            {
                Destroy(bonusItem.gameObject);
            }

            didStartDeath = true;

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            GameObject pacman = GameObject.Find("pacman_1");
            pacman.transform.GetComponent<pacman>().canMove = false;

            pacman.transform.GetComponent<Animator>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessDeathAfter(2));
        }
    }

    IEnumerator ProcessDeathAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(ProcessDeathAnimation(1.9f)); // Background AudioClip is 1.9f long
    }

    IEnumerator ProcessDeathAnimation(float delay)
    {
        GameObject pacman = GameObject.Find("pacman_1");

        pacman.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
        pacman.transform.localRotation = Quaternion.Euler(0, 0, 0);

        pacman.transform.GetComponent<Animator>().runtimeAnimatorController = pacman.transform.GetComponent<pacman>().deathAnimation;
        pacman.transform.GetComponent<Animator>().enabled = true;

        transform.GetComponent<AudioSource>().clip = backgroundAudioPacmanDeath;
        transform.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestart(1));
    }

    IEnumerator ProcessRestart(float delay)
    {
        pacmanLives -= 1;

        if(pacmanLives == 0)
        {
            playerText.transform.GetComponent<Text>().enabled = true;

            readyText.transform.GetComponent<Text>().text = "GAME OVER";
            readyText.transform.GetComponent<Text>().color = Color.red;

            readyText.transform.GetComponent<Text>().enabled = true;

            GameObject pacman = GameObject.Find("pacman_1");
            pacman.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessGameOver(2));
        }
        else
        {
            playerText.transform.GetComponent<Text>().enabled = true;
            readyText.transform.GetComponent<Text>().enabled = true;

            GameObject pacman = GameObject.Find("pacman_1");
            pacman.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            yield return new WaitForSeconds(delay);

            StartCoroutine(ProcessRestartShowObjects(1));
        }
    }

    IEnumerator ProcessGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        playerOneScore = 0;
        player1PelletsConsumed = 0;

        player1Level = 1;

        pacmanLives = 3;

        SceneManager.LoadScene("GameMenu");
    }

    IEnumerator ProcessRestartShowObjects(float delay)
    {
        playerText.transform.GetComponent<Text>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
            ghost.transform.GetComponent<Ghost>().MoveToStartingPosition();
        }

        GameObject pacman = GameObject.Find("pacman_1");

        pacman.transform.GetComponent<Animator>().enabled = false;
        
        pacman.transform.GetComponent<SpriteRenderer>().enabled = true;
        pacman.transform.GetComponent<pacman>().MoveToStartingPosition();

        yield return new WaitForSeconds(delay);

        Restart();
    }

    public void Restart()
    {
        readyText.transform.GetComponent<Text>().enabled = false;

        GameObject pacman = GameObject.Find("pacman_1");
        pacman.transform.GetComponent<pacman>().Restart();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().Restart();
        }

        transform.GetComponent<AudioSource>().clip = background_audio_normal;
        transform.GetComponent<AudioSource>().Play();

        didStartDeath = false;
    }
}
