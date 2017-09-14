using UnityEngine;
using System.Collections;

public class audienceContainer : MonoBehaviour
{

    private string[] names = { "idle", "applause", "applause2", "celebration", "celebration2", "celebration3" };
    GameObject player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {

        transform.position = new Vector3(player.transform.position.x+3, player.transform.position.y, player.transform.position.z-5);
    }
}