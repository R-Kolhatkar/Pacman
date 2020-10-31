using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pacman : MonoBehaviour
{
    public AudioClip chomp1;
    public AudioClip chomp2;

    public RuntimeAnimatorController chompAnimation;
    public RuntimeAnimatorController deathAnimation;

    public Vector2 orientation;

    public float speed = 6.0f;

    public Sprite idleSprite;

    public bool canMove = true;

    private bool playedChomp1 = false;

    private AudioSource audio;

    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;

    // private int pelletsConsumed = 0;

    private node current, previous, target;

    private node startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        audio = transform.GetComponent<AudioSource>();

        node temp = GetNodeAtPosition(transform.localPosition);

        startingPosition = temp;

        if(temp != null)
        {
            current = temp;
        }

        direction = Vector2.left;
        orientation = Vector2.left;

        ChangePosition(direction);

        SetDifficultyForLevel(gameboard.player1Level);
    }

    public void SetDifficultyForLevel(int level)
    {
        if(level == 1)
        {
            speed = 6;
        }
        if(level >= 2 && level <= 4)
        {
            speed = 7;
        }
        if (level >= 5)
        {
            speed = 10;
        }
    }

    public void MoveToStartingPosition()
    {
        transform.position = startingPosition.transform.position;

        transform.GetComponent<SpriteRenderer>().sprite = idleSprite;

        direction = Vector2.left;
        orientation = Vector2.left;

        UpdateOrientation();
    }

    public void Restart()
    {
        canMove = true;

        current = startingPosition;

        nextDirection = Vector2.left;

        transform.GetComponent<Animator>().runtimeAnimatorController = chompAnimation;
        transform.GetComponent<Animator>().enabled = true;

        ChangePosition(direction);
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
        {
            // Debug.Log("SCORE: " + GameObject.Find("Game").GetComponent<gameboard>().score);
            CheckInput();
            Move();
            UpdateOrientation();
            UpdateAnimationState();
            ConsumePellet();
        }
    }

    void PlayChompSound()
    {
        if (playedChomp1)
        {
            // Play chomp2, set playedChomp1 to false
            audio.PlayOneShot(chomp2);
            playedChomp1 = false;
        }
        else
        {
            // Play chomp1, set playedChomp1 to true
            audio.PlayOneShot(chomp1);
            playedChomp1 = true;
        }
    }

    void CheckInput()
    {
        // Move left
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangePosition(Vector2.left);
        }
        // Move right
        else if (Input.GetKeyDown(KeyCode.L))
        {
            ChangePosition(Vector2.right);
        }
        // Move up
        else if (Input.GetKeyDown(KeyCode.I))
        {
            ChangePosition(Vector2.up);
        }
        // Move down
        else if (Input.GetKeyDown(KeyCode.K))
        {
            ChangePosition(Vector2.down);
        }

    }

    // Change position based on direction
    void ChangePosition(Vector2 d)
    {
        if(d != direction)
        {
            nextDirection = d;
        }
        if(current != null)
        {
            node moveToNode = CanMove(d);

            if (moveToNode != null)
            {
                direction = d;
                target = moveToNode;
                previous = current;
                current = null;
            }
        }
    }

    void Move()
    {
        if(target != current && target != null)
        {
            if(nextDirection == direction * -1)
            {
                direction *= -1;
                node tempNode = target;
                target = previous;
                previous = tempNode;
            }
            
            if(OvershotTarget())
            {
                current = target;

                transform.localPosition = current.transform.position;

                GameObject otherPortal = GetPortal(current.transform.position);

                if (otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    current = otherPortal.GetComponent<node>();
                    // Debug.Log(current);
                }

                node moveToNode = CanMove(nextDirection);

                if(moveToNode != null)
                {
                    direction = nextDirection;
                }

                if(moveToNode == null)
                {
                    moveToNode = CanMove(direction);
                }

                if(moveToNode != null)
                {
                    target = moveToNode;
                    previous = current;
                    current = null;
                }
                else
                {
                    direction = Vector2.zero;
                }
            }
            else
            {
                transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
            }
        }
    }

    void MoveToNode(Vector2 d)
    {
        node moveToNode = CanMove(d);
        if(moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            current = moveToNode;
        }
    }

    void UpdateOrientation()
    {
        if (direction == Vector2.left)
        {
            orientation = Vector2.left;
            transform.localScale = new Vector3(-0.18f, 0.18f, 0.18f);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (direction == Vector2.right)
        {
            orientation = Vector2.right;
            transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (direction == Vector2.up)
        {
            orientation = Vector2.up;
            transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
            transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (direction == Vector2.down)
        {
            orientation = Vector2.down;
            transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
            transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
        }
    }

    void UpdateAnimationState()
    {
        if (direction == Vector2.zero)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idleSprite;
        }
        else
        {
            GetComponent<Animator>().enabled = true;
        }
    }

    void ConsumePellet()
    {
        GameObject o = GetTileAtPosition(transform.position);

        if (o != null)
        {
            tile t = o.GetComponent<tile>();

            if(t != null)
            {
                bool didConsume = false;

                if (!t.didConsume && (t.isPellet || t.isPelletEnergizer))
                {
                    didConsume = true;

                    t.didConsume = true;

                    // Pellets (Dots): 10 pts, Pellet Energizers (Power Pellets): 50 pts
                    if (t.isPellet)
                    {
                        gameboard.playerOneScore += 10;

                        if (gameboard.highScore < gameboard.playerOneScore)
                        {
                            gameboard.highScore += 10;
                        }
                    }
                    else if (t.isPelletEnergizer)
                    {
                        gameboard.playerOneScore += 50;

                        if (gameboard.highScore < gameboard.playerOneScore)
                        {
                            gameboard.highScore += 50;
                        }
                    }
                    gameboard.player1PelletsConsumed++;
                }

                if(t.isResource)
                {
                    ConsumeResource(t);
                }

                if(didConsume)
                {
                    o.GetComponent<SpriteRenderer>().enabled = false;

                    // pelletsConsumed++;
                    PlayChompSound();

                    if(t.isPelletEnergizer)
                    {
                        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

                        foreach (GameObject g in ghosts)
                        {
                            g.GetComponent<Ghost>().StartFrightenedMode();
                        }
                    }
                }
            }
        }
    }

    void ConsumeResource(tile bonusItem)
    {
        // Debug.Log("ConsumeResource\t" + bonusItem.resourceValue);

        gameboard.playerOneScore += bonusItem.resourceValue;

        if (gameboard.highScore < gameboard.playerOneScore)
        {
            gameboard.highScore += bonusItem.resourceValue;
        }

        GameObject.Find("Game").transform.GetComponent<gameboard>().StartConsumeResource(bonusItem.gameObject, bonusItem.resourceValue);
    }

    node CanMove(Vector2 d)
    {
        node moveToNode = null;

        int i = 0;
        bool isSameDirection = false;

        while((i < current.neighbours.Length) && !isSameDirection)
        {
            if (current.validDirections[i] == d)
            {
                moveToNode = current.neighbours[i];
                isSameDirection = true;
            }

            i++;
        }

        return moveToNode;
    }

    GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x + 13.5f);
        int tileY = Mathf.RoundToInt(pos.y + 14.7f);

        GameObject tile = GameObject.Find("Game").GetComponent<gameboard>().board[tileX, tileY];

        if(tile != null)
        {
            return tile;
        }

        return null;
    }

    node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<gameboard>().board[(int)(pos.x + 13.5), (int)(pos.y + 14.7)];

        if(tile != null)
        {
            return tile.GetComponent<node>();
        }

        return null;
    }

    bool OvershotTarget()
    {
        float nodeToTarget = LengthFromNode(target.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float LengthFromNode(Vector2 targetPos)
    {
        Vector2 temp = targetPos - (Vector2)previous.transform.position;
        return temp.sqrMagnitude;
    }

    // Check to see if current node is a portal
    GameObject GetPortal(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<gameboard>().board[(int)(pos.x + 13.5), (int)(pos.y + 14.7)];

        if(tile != null)
        {
            if (tile.GetComponent<tile>() != null)
            {
                if (tile.GetComponent<tile>().isPortal)
                {
                    GameObject otherPortal = tile.GetComponent<tile>().portalReceiver;
                    return otherPortal;
                }
            }
        }

        return null;
    }
}
