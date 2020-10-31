using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float moveSpeed = 5.9f;
    public float normalMoveSpeed = 5.9f;
    public float frightenedModeMoveSpeed = 3.9f;
    public float consumedMoveSpeed = 15f;

    public bool canMove = true;

    public int pinkyReleaseTimer = 5;
    public int inkyReleaseTimer = 14;
    public int clydeReleaseTimer = 21;
    public float ghostReleaseTimer = 0;

    public float frightenedModeDuration = 10;
    public float startBlinkingAt = 7;

    public bool isInGhostHouse = false;

    public node startingPosition;
    public node homeNode;
    public node ghostHouse;

    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;

    public Sprite eyesUp;
    public Sprite eyesDown;
    public Sprite eyesLeft;
    public Sprite eyesRight;

    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostRight;
    public RuntimeAnimatorController ghostWhite;
    public RuntimeAnimatorController ghostBlue;

    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0;

    private float frightenedModeTimer = 0;
    private float blinkTimer = 0;

    private bool frightenedModeIsWhite = false;

    private float previousMoveSpeed;

    private AudioSource backgroundAudio;

    public enum Mode
    {
        Chase,
        Scatter,
        Frightened,
        Consumed
    }

    Mode currentMode = Mode.Scatter;
    Mode previousMode;

    public enum GhostType
    {
        Red,
        Pink,
        Blue,
        Orange
    }

    public GhostType ghostType = GhostType.Red;

    private GameObject pacman;

    private node current, target, previous;
    private Vector2 direction, nextDirection;

    // Start is called before the first frame update
    void Start()
    {
        SetDifficultyForLevel(gameboard.player1Level);

        backgroundAudio = GameObject.Find("Game").transform.GetComponent<AudioSource>();

        pacman = GameObject.FindGameObjectWithTag("Pacman");
        node n = GetNodeAtPosition(transform.localPosition);

        if(n != null)
        {
            current = n;
        }

        if(isInGhostHouse)
        {
            direction = Vector2.up;
            target = current.neighbours[0];
        }
        else
        {
            direction = Vector2.left;
            target = ChooseNextNode();
        }

        previous = current;

        UpdateAnimatorController();
    }

    public void SetDifficultyForLevel(int level)
    {
        if (level == 1)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 5;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 20;

            frightenedModeDuration = 10;
            startBlinkingAt = 7;

            pinkyReleaseTimer = 5;
            inkyReleaseTimer = 14;
            clydeReleaseTimer = 21;

            moveSpeed = 5.9f;
            normalMoveSpeed = 5.9f;
            frightenedModeMoveSpeed = 2.9f;
            consumedMoveSpeed = 15f;
        }
        else if(level >= 2 && level <= 4)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenedModeDuration = 9;
            startBlinkingAt = 6;

            pinkyReleaseTimer = 4;
            inkyReleaseTimer = 12;
            clydeReleaseTimer = 18;

            moveSpeed = 6.9f;
            normalMoveSpeed = 6.9f;
            frightenedModeMoveSpeed = 3.9f;
            consumedMoveSpeed = 18f;
        }
        else if (level >= 5)
        {
            scatterModeTimer1 = 5;
            scatterModeTimer2 = 5;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1037;

            frightenedModeDuration = 6;
            startBlinkingAt = 3;

            pinkyReleaseTimer = 2;
            inkyReleaseTimer = 6;
            clydeReleaseTimer = 10;

            moveSpeed = 9.9f;
            normalMoveSpeed = 9.9f;
            frightenedModeMoveSpeed = 6.9f;
            consumedMoveSpeed = 24f;
        }
    }

    public void MoveToStartingPosition()
    {
        if (transform.name != "blinky_right")
        {
            isInGhostHouse = true;
        }

        transform.position = startingPosition.transform.position;

        if(isInGhostHouse)
        {
            direction = Vector2.up;
        }
        else
        {
            direction = Vector2.left;
        }

        UpdateAnimatorController();
    }

    public void Restart()
    {
        canMove = true;

        currentMode = Mode.Scatter;

        moveSpeed = normalMoveSpeed;

        previousMoveSpeed = 0;

        ghostReleaseTimer = 0;
        modeChangeIteration = 1;
        modeChangeTimer = 0;

        current = startingPosition;
        
        if(isInGhostHouse)
        {
            direction = Vector2.up;
            target = current.neighbours[0];
        }
        else
        {
            direction = Vector2.left;
            target = ChooseNextNode();
        }

        previous = current;
        UpdateAnimatorController();
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
        {
            ModeUpdate();
            Move();
            ReleaseGhosts();
            CheckCollision();
            CheckIsInGhostHouse();
        }
    }

    void CheckIsInGhostHouse()
    {
        if(currentMode == Mode.Consumed)
        {
            GameObject t = GetTileAtPosition(transform.position);

            if(t != null)
            {
                if(t.transform.GetComponent<tile>() != null)
                {
                    if (t.transform.GetComponent<tile>().isGhostHouse)
                    {
                        moveSpeed = normalMoveSpeed;
                        node n = GetNodeAtPosition(transform.position);

                        if(n != null)
                        {
                            current = n;
                            direction = Vector2.up;
                            target = current.neighbours[0];
                            previous = current;
                            currentMode = Mode.Chase;

                            UpdateAnimatorController();
                        }
                    }
                }
            }
        }
    }

    void CheckCollision()
    {
        // Rectangle around ghost with size of ghost
        // Collision occurs somewhere at the centre of the rectangle
        Rect ghostRect = new Rect(transform.position, transform.GetComponent<SpriteRenderer>().sprite.bounds.size * 0.25f);
        Rect pacmanRect = new Rect(pacman.transform.position, pacman.transform.GetComponent<SpriteRenderer>().sprite.bounds.size * 0.25f);

        if(ghostRect.Overlaps(pacmanRect))
        {
            if(currentMode == Mode.Frightened)
            {
                Consumed();
            }
            else if(currentMode == Mode.Chase || currentMode == Mode.Scatter)
            {
                // Pacman should die
                GameObject.Find("Game").transform.GetComponent<gameboard>().StartDeath();
            }
        }
    }

    void Consumed()
    {
        gameboard.playerOneScore += gameboard.ghostConsumedRunningScore;

        if (gameboard.highScore < gameboard.playerOneScore)
        {
            gameboard.highScore += gameboard.ghostConsumedRunningScore;
        }

        currentMode = Mode.Consumed;
        previousMoveSpeed = moveSpeed;
        moveSpeed = consumedMoveSpeed;

        UpdateAnimatorController();

        GameObject.Find("Game").transform.GetComponent<gameboard>().StartConsumed(this.GetComponent<Ghost>());

        gameboard.ghostConsumedRunningScore = gameboard.ghostConsumedRunningScore * 2;
    }

    void UpdateAnimatorController()
    {
        if(currentMode != Mode.Frightened && currentMode != Mode.Consumed)
        {
            if (direction == Vector2.up)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostUp;
            }
            else if (direction == Vector2.down)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostDown;
            }
            else if (direction == Vector2.left)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            }
            else if (direction == Vector2.right)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
            }
        }
        else if(currentMode == Mode.Frightened)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
        }
        else if(currentMode == Mode.Consumed)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = null;

            if(direction == Vector2.up)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesUp;
            }
            else if (direction == Vector2.down)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesDown;
            }
            else if (direction == Vector2.left)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesLeft;
            }
            else if (direction == Vector2.right)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesRight;
            }
        }
    }

    void Move()
    {
        if(target != current && target != null && !isInGhostHouse)
        {
            if(OvershotTarget())
            {
                // Debug.Log("OVERSHOT");
                current = target;
                transform.localPosition = current.transform.position;
                GameObject otherPortal = GetPortal(current.transform.position);

                if(otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    current = otherPortal.GetComponent<node>();
                }

                target = ChooseNextNode();
                previous = current;
                current = null;

                UpdateAnimatorController();
            }
            else
            {
                // Debug.Log(direction);
                // Debug.Log("Supposed to Move");
                transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    void ModeUpdate()
    {
        if(currentMode != Mode.Frightened)
        {
            modeChangeTimer += Time.deltaTime;

            if(modeChangeIteration == 1)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)
                {
                    modeChangeIteration = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if(modeChangeIteration == 2)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if(modeChangeIteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if(modeChangeIteration == 4)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
            }
        }
        else if(currentMode == Mode.Frightened)
        {
            frightenedModeTimer += Time.deltaTime;

            if(frightenedModeTimer >= frightenedModeDuration)
            {
                backgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<gameboard>().background_audio_normal;
                backgroundAudio.Play();

                frightenedModeTimer = 0;
                ChangeMode(previousMode);
            }
            if(frightenedModeTimer >= startBlinkingAt)
            {
                blinkTimer += Time.deltaTime;

                if(blinkTimer >= 0.1f)
                {
                    blinkTimer = 0;

                    if(frightenedModeIsWhite)
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
                        frightenedModeIsWhite = false;
                    }
                    else
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostWhite;
                        frightenedModeIsWhite = true;
                    }
                }
            }
        }
    }

    void ChangeMode(Mode m)
    {
        if(currentMode == Mode.Frightened)
        {
            moveSpeed = previousMoveSpeed;
        }

        if(m == Mode.Frightened)
        {
            previousMoveSpeed = moveSpeed;
            moveSpeed = frightenedModeMoveSpeed;
        }

        if(currentMode != m)
        {
            previousMode = currentMode;
            currentMode = m;
        }

        UpdateAnimatorController();
    }

    public void StartFrightenedMode()
    {
        if(currentMode != Mode.Consumed)
        {
            gameboard.ghostConsumedRunningScore = 200;

            frightenedModeTimer = 0;
            backgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<gameboard>().background_audio_frightened;
            backgroundAudio.Play();
            ChangeMode(Mode.Frightened);
        }
    }

    Vector2 GetRedGhostTargetTile()
    {
        Vector2 pacmanPos = pacman.transform.localPosition;
        Vector2 targetTile = new Vector2(Mathf.Floor(pacmanPos.x) + 0.5f, Mathf.Floor(pacmanPos.y) + 0.5f);

        return targetTile;
    }

    Vector2 GetPinkGhostTargetTile()
    {
        // Four tiles ahead of Pacman
        // Taking into account position and orientation
        Vector2 pacmanPos = pacman.transform.localPosition;
        Vector2 pacmanOrientation = pacman.GetComponent<pacman>().orientation;

        float pacmanPosX = Mathf.Floor(pacmanPos.x) + 0.5f;
        float pacmanPosY = Mathf.Floor(pacmanPos.y) + 0.5f;

        Vector2 pacmanTile = new Vector2(pacmanPosX, pacmanPosY);
        Vector2 targetTile = pacmanTile + (4 * pacmanOrientation);

        return targetTile;
    }

    Vector2 GetBlueGhostTargetTile()
    {
        // Select the position two tiles in front of Pacman
        // Draw a vector from Blinky to that position
        // Double the length of that vector
        Vector2 pacmanPos = pacman.transform.localPosition;
        Vector2 pacmanOrientation = pacman.GetComponent<pacman>().orientation;

        float pacmanPosX = Mathf.Floor(pacmanPos.x) + 0.5f;
        float pacmanPosY = Mathf.Floor(pacmanPos.y) + 0.5f;

        Vector2 pacmanTile = new Vector2(pacmanPosX, pacmanPosY);

        Vector2 targetTile = pacmanTile + (2 * pacmanOrientation);

        // Temporary vector for Blinky's position
        Vector2 tempBlinkyPosition = GameObject.Find("blinky_right").transform.localPosition;

        float blinkyPosX = Mathf.Floor(tempBlinkyPosition.x) + 0.5f;
        float blinkyPosY = Mathf.Floor(tempBlinkyPosition.y) + 0.5f;

        tempBlinkyPosition = new Vector2(blinkyPosX, blinkyPosY);

        float distance = GetDistance(tempBlinkyPosition, targetTile);
        distance *= 2;

        targetTile = new Vector2(tempBlinkyPosition.x + distance, tempBlinkyPosition.y + distance);

        return targetTile;
    }

    Vector2 GetOrangeGhostTargetTile()
    {
        // Calculate the distance from Pacman
        // If distance > 8 tiles, targeting is the same as Blinky
        // If distance < 8 tiles, target is home node, same as Scatter Mode
        Vector2 pacmanPos = pacman.transform.localPosition;

        float distance = GetDistance(transform.localPosition, pacmanPos);
        Vector2 targetTile = Vector2.zero;

        if(distance > 8)
        {
            targetTile = new Vector2(Mathf.Floor(pacmanPos.x) + 0.5f, Mathf.Floor(pacmanPos.y) + 0.5f);
        }
        else if(distance < 8)
        {
            targetTile = homeNode.transform.position;
        }

        return targetTile;
    }

    Vector2 GetTargetTile()
    {
        Vector2 targetTile = Vector2.zero;
        if(ghostType == GhostType.Red)
        {
            targetTile = GetRedGhostTargetTile();
        }
        else if(ghostType == GhostType.Pink)
        {
            targetTile = GetPinkGhostTargetTile();
        }
        else if(ghostType == GhostType.Blue)
        {
            targetTile = GetBlueGhostTargetTile();
        }
        else if(ghostType == GhostType.Orange)
        {
            targetTile = GetOrangeGhostTargetTile();
        }

        return targetTile;
    }

    Vector2 GetRandomTile()
    {
        float x = Mathf.Floor(Random.Range(0, 28)) + 0.5f;
        float y = Mathf.Floor(Random.Range(0, 31)) + 0.5f;

        return new Vector2(x, y);
    }

    void ReleasePinkGhost()
    {
        if(ghostType == GhostType.Pink && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseBlueGhost()
    {
        if(ghostType == GhostType.Blue && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseOrangeGhost()
    {
        if (ghostType == GhostType.Orange && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseGhosts()
    {
        ghostReleaseTimer += Time.deltaTime;

        if(ghostReleaseTimer > pinkyReleaseTimer)
        {
            ReleasePinkGhost();
        }

        if (ghostReleaseTimer > inkyReleaseTimer)
        {
            ReleaseBlueGhost();
        }

        if (ghostReleaseTimer > clydeReleaseTimer)
        {
            ReleaseOrangeGhost();
        }
    }

    node ChooseNextNode()
    {
        Vector2 targetTile = Vector2.zero;

        if(currentMode == Mode.Chase)
        {
            targetTile = GetTargetTile();
        }
        else if(currentMode == Mode.Scatter)
        {
            targetTile = homeNode.transform.position;
        }
        else if(currentMode == Mode.Frightened)
        {
            targetTile = GetRandomTile();
        }
        else if(currentMode == Mode.Consumed)
        {
            targetTile = ghostHouse.transform.position;
        }

        node moveToNode = null;

        node[] foundNodes = new node[4];
        Vector2[] foundNodesDirection = new Vector2[4];

        int nodeCounter = 0;
        
        for(int i = 0; i < current.neighbours.Length; i++)
        {
            if(current.validDirections[i] != direction * -1)
            {
                if(currentMode != Mode.Consumed)
                {
                    GameObject t = GetTileAtPosition(current.transform.position);

                    if(t.transform.GetComponent<tile>().isGhostHouseEntrance == true)
                    {
                        // Found ghost house, don't want to allow movement
                        if(current.validDirections[i] != Vector2.down)
                        {
                            foundNodes[nodeCounter] = current.neighbours[i];
                            foundNodesDirection[nodeCounter] = current.validDirections[i];
                            nodeCounter++;
                        }
                    }
                    else
                    {
                        foundNodes[nodeCounter] = current.neighbours[i];
                        foundNodesDirection[nodeCounter] = current.validDirections[i];
                        nodeCounter++;
                    }
                }
                else
                {
                    foundNodes[nodeCounter] = current.neighbours[i];
                    foundNodesDirection[nodeCounter] = current.validDirections[i];
                    nodeCounter++;
                }
            }
        }

        if(foundNodes.Length == 1)
        {
            moveToNode = foundNodes[0];
            direction = foundNodesDirection[0];
        }
        if(foundNodes.Length > 1)
        {
            float leastDistance = 100000f;

            for(int i = 0; i < foundNodes.Length; i++)
            {
                if(foundNodesDirection[i] != Vector2.zero)
                {
                    float distance = GetDistance(foundNodes[i].transform.position, targetTile);

                    if(distance < leastDistance)
                    {
                        leastDistance = distance;
                        moveToNode = foundNodes[i];
                        direction = foundNodesDirection[i];
                    }
                }
            }
        }
        return moveToNode;
    }

    node GetNodeAtPosition(Vector2 pos)
    {
        // t = tile
        GameObject t = GameObject.Find("Game").GetComponent<gameboard>().board[(int)(pos.x + 13.5), (int)(pos.y + 14.7)];

        if(t != null)
        {
            // Check if node
            if(t.GetComponent<node>() != null)
            {
                return t.GetComponent<node>();
            }
        }

        return null;
    }

    GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x + 13.5f);
        int tileY = Mathf.RoundToInt(pos.y + 14.7f);

        GameObject tile = GameObject.Find("Game").transform.GetComponent<gameboard>().board[tileX, tileY];

        if (tile != null)
        {
            return tile;
        }

        return null;
    }

    GameObject GetPortal(Vector2 pos)
    {
        // t = tile
        GameObject t = GameObject.Find("Game").GetComponent<gameboard>().board[(int)(pos.x + 13.5), (int)(pos.y + 14.7)];

        if(t != null)
        {
            if(t.GetComponent<tile>().isPortal)
            {
                GameObject otherPortal = t.GetComponent<tile>().portalReceiver;
                return otherPortal;
            }
        }

        return null;
    }

    float LengthFromNode(Vector2 targetPos)
    {
        Vector2 temp = targetPos - (Vector2) previous.transform.position;
        return temp.sqrMagnitude;
    }

    bool OvershotTarget()
    {
        float nodeToTarget = LengthFromNode(target.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float GetDistance(Vector2 pos1, Vector2 pos2)
    {
        float dx = pos1.x - pos2.x;
        float dy = pos1.y - pos2.y;

        float distance = Mathf.Sqrt((dx * dx) + (dy * dy));

        return distance;
    }
}
