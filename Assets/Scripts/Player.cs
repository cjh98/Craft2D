using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Player : MonoBehaviour
{
    public Transform debugMenu;

    Entity playerEntity;
    EntityMovement entityMovement;

    public byte targetedBlock;

    public Transform selectBox;

    Camera cam;

    public Tool currentTool;

    public float hitCounter;

    public Transform breakAnimation;
    public Animator breakAnimationAnimator;

    public float range;

    float breakSpeedScale = 0.06f;

    public float scrollSpeed;

    public Transform debugScreen;

    public float hitRate;
    float nextTimeToHit = 0f;

    bool noclip = false;

    void Start()
    {
        playerEntity = GetComponent<Entity>();
        entityMovement = GetComponent<EntityMovement>();

        cam = Camera.main;

        transform.position = new Vector3(World.instance.worldSizeInChunks / 2 * World.instance.chunkWidth + World.instance.chunkWidth / 2, 
            World.instance.worldSizeInChunks / 2 * World.instance.chunkWidth + World.instance.chunkWidth / 2, 0);
    }

    void Update()
    {
        entityMovement.isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButton("Fire1"))
        {
            HitBlock();
        }
        else
        {
            hitCounter = 0;
            breakAnimation.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.V))
            noclip = !noclip;


        if (Input.GetKeyDown(KeyCode.B))
            debugMenu.gameObject.SetActive(!debugMenu.gameObject.activeSelf);

        breakAnimation.gameObject.SetActive(hitCounter > 0);

        MoveValidDirection();

        cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;
    }

    void HitBlock()
    {
        Vector2Int blockToHitCoord = GetCoordInDirectionFromPlayer(entityMovement.direction, false);
        targetedBlock = World.instance.GetBlockFromVector2(new Vector2(blockToHitCoord.x, blockToHitCoord.y));

        // ignore non solid blocks
        if (targetedBlock == 255 || !World.instance.blockTypes[targetedBlock].isSolid)
            return;

        LoopHitSound();

        hitCounter += Time.deltaTime * currentTool.damage;

        breakAnimation.position = new Vector3(blockToHitCoord.x, blockToHitCoord.y, 0);
        breakAnimationAnimator.speed = currentTool.damage / World.instance.blockTypes[targetedBlock].strength * breakSpeedScale;

        if (hitCounter > World.instance.blockTypes[targetedBlock].strength)
            DestroyBlock(targetedBlock, blockToHitCoord);
    }

    void LoopHitSound()
    {
        if (Time.time >= nextTimeToHit)
        {
            nextTimeToHit = Time.time + 1f / hitRate;
            AudioManager.instance.Play(World.instance.blockTypes[targetedBlock].hitAudio);
        }
    }

    void DestroyBlock(byte block, Vector2Int blockPos)
    {
        Vector2Int chunkPos = World.instance.GetChunkFromVector2(blockPos);
        Chunk chunk = World.instance.chunks[chunkPos.x, chunkPos.y].GetComponent<Chunk>();

        byte newBlock = World.instance.blockTypes[block].blockBelowValue;

        blockPos = World.instance.GetBlockCoordFromVector2(blockPos);

        chunk.blockMap[blockPos.x, blockPos.y] = newBlock;
        chunk.SetTile(new Vector3Int(blockPos.x, blockPos.y, 0), World.instance.blockTypes[chunk.blockMap[blockPos.x, blockPos.y]].tile);

        AudioManager.instance.Play(World.instance.blockTypes[block].breakAudio);

        hitCounter = 0;
    }

    Vector2Int GetCoordInDirectionFromPlayer(EntityMovement.Direction direction, bool forCollision)
    {
        int playerX = Mathf.RoundToInt(transform.position.x);
        int playerY = Mathf.RoundToInt(transform.position.y);

        float addedRange;

        addedRange = forCollision ? 0 : range;

        switch (direction)
        {
            case EntityMovement.Direction.Up:
                return new Vector2Int(playerX, Mathf.RoundToInt(transform.position.y + playerEntity.height + addedRange));
            case EntityMovement.Direction.Down:
                return new Vector2Int(playerX, Mathf.RoundToInt(transform.position.y - playerEntity.height - addedRange));
            case EntityMovement.Direction.Left:
                return new Vector2Int(Mathf.RoundToInt(transform.position.x - playerEntity.width - addedRange), playerY);
            case EntityMovement.Direction.Right:
                return new Vector2Int(Mathf.RoundToInt(transform.position.x + playerEntity.width + addedRange), playerY);
            default:
                return Vector2Int.zero;
        }
    }

    void MoveValidDirection()
    {
        //------------------------
        // CHECKS FOR COLLISIONS
        //------------------------

        Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        int playerX = Mathf.RoundToInt(transform.position.x);
        int playerY = Mathf.RoundToInt(transform.position.y);

        if (!noclip)
        {
            byte blockLeft = World.instance.GetBlockFromVector2(new Vector3(Mathf.RoundToInt(transform.position.x - playerEntity.width), playerY, 0));
            byte blockRight = World.instance.GetBlockFromVector2(new Vector3(Mathf.RoundToInt(transform.position.x + playerEntity.width), playerY, 0));
            byte blockAbove = World.instance.GetBlockFromVector2(new Vector3(playerX, Mathf.RoundToInt(transform.position.y + playerEntity.height), 0));
            byte blockBelow = World.instance.GetBlockFromVector2(new Vector3(playerX, Mathf.RoundToInt(transform.position.y - playerEntity.height), 0));

            // check up
            if (blockAbove == 255 || World.instance.blockTypes[blockAbove].isSolid)
            {
                if (velocity.y > 0)
                    velocity.y = 0;
            }
            // check down
            if (blockBelow == 255 || World.instance.blockTypes[blockBelow].isSolid)
            {
                if (velocity.y < 0)
                    velocity.y = 0;
            }
            // check right
            if (blockRight == 255 || World.instance.blockTypes[blockRight].isSolid)
            {
                if (velocity.x > 0)
                    velocity.x = 0;
            }
            // check left
            if (blockLeft == 255 || World.instance.blockTypes[blockLeft].isSolid)
            {
                if (velocity.x < 0)
                    velocity.x = 0;
            }
        }

        entityMovement.Move(velocity);
    }
}
