using UnityEngine;
using TMPro;

public class DebugMenu : MonoBehaviour
{
    Player player;

    float timer;
    float wait = 0.01f;

    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI playerPosText;
    public TextMeshProUGUI directionText;
    public TextMeshProUGUI hitCounterText;

    Vector2Int pos = new Vector2Int();

    EntityMovement playerEM;

    float lowest = float.MaxValue;
    float highest = float.MinValue;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        playerEM = player.GetComponent<EntityMovement>();
    }

    void Update()
    {
        pos.x = Mathf.FloorToInt(player.transform.position.x);
        pos.y = Mathf.FloorToInt(player.transform.position.y);

        if (Input.GetKeyDown(KeyCode.R))
        {
            lowest = float.MaxValue;
        }

        // only show change after wait time
        if (timer > wait)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            if (fps < lowest)
                lowest = fps;
            if (fps > highest)
                highest = fps;

            fpsText.text = fps + " fps \n"
                + "highest = " + highest + "\n"
                + "lowest = " + lowest;

            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }

        playerPosText.text = "Player at: " + pos.x.ToString() + ", " + pos.y.ToString();
        directionText.text = "Direction: " + playerEM.direction;
        hitCounterText.text = "Hit Counter: " + Mathf.Round(player.hitCounter);
    }
}
