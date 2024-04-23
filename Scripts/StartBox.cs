using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBox : MonoBehaviour
{
    public int id = 0;
    public bool player_in = false;
    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player_in && mat.color.a < .8f)
        {
            Color color = mat.color;
            color.a += 1f * Time.deltaTime;
            mat.color = color;
        }
        else if (!player_in && mat.color.a > .3f)
        {
            Color color = mat.color;
            color.a -= 1f * Time.deltaTime;
            mat.color = color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<Player>().id == id)
        {
            player_in = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<Player>().id == id)
        {
            player_in = false;
        }
    }
}
