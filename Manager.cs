using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using OscJack;

public class Manager : MonoBehaviour
{
    public List<Scroll> scroll_scripts = new List<Scroll>();
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> butterflies = new List<GameObject>();
    public GameObject butterfly_prefab;
    public GameObject butterfly_parent;

    public bool playing = false;
    public bool game_ended = false;
    public float total_gameplay_time = 300f;
    public float gameplay_timer = 300f;
    public int start_butterflies;
    public Slider progress_bar;

    public GameObject winterstorm;

    public List<GameObject> music = new List<GameObject>();
    public GameObject intro_assets;
    public List<GameObject> end_assets = new List<GameObject>();

    public float text_timer = 0f;
    public float start_delay_time = 30f;
    public float start_delay_timer = 0f;

    private void Start()
    {
        send_osc(5000, "/restart", 1);
        spawn_butterflies(9, 5);

        winterstorm.SetActive(false);

        start_butterflies = butterflies.Count;

        if (progress_bar != null)
        {
            progress_bar.gameObject.SetActive(false);
        }

        music[0].SetActive(true);
        music[1].SetActive(false);
        music[2].SetActive(false);
        music[3].SetActive(false);
        music[4].SetActive(false);
    }

    private void Update()
    {
        start_game();
        StartGameAssets();
        update_progress_bar();
        check_game_over();

        if (playing && winterstorm.activeInHierarchy)
        {
            gameplay_timer -= Time.deltaTime * 1.75f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void spawn_butterflies(int num_shared, int num_specific)
    {
        //SPAWN COMMUNAL BUTTERFLIES
        spawn_butterflies_helper(num_shared, -1);
        //SPAWN PLAYER-SPECIFIC BUTTERFLIES
        foreach (GameObject player in players)
        {
            spawn_butterflies_helper(num_specific, player.GetComponent<Player>().id);
        }
    }

    void spawn_butterflies_helper(int num_butterflies, int player_id)
    {
        Vector3 spawn_position = new Vector3(0, 0, 0);

        //SPAWN BUTTERFLIES
        for (int i = 0; i < num_butterflies; i++)
        {
            //SET POSITION, CHECK OVERLAPPING
            bool overlapping = false;
            do
            {
                spawn_position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                overlapping = false;
                foreach (GameObject butterfly in butterflies)
                {
                    if ((spawn_position - butterfly.transform.position).magnitude < 2)
                    {
                        overlapping = true;
                    }
                }
            }
            while (overlapping);

            //INSTANTIATE BUTTERFLY AND SET VARIABLES
            GameObject new_butterfly = Instantiate(butterfly_prefab, spawn_position, Quaternion.identity, butterfly_parent.transform);
            new_butterfly.GetComponent<ButterFly>().manager = this;
            new_butterfly.GetComponent<ButterFly>().id = player_id;
            butterflies.Add(new_butterfly);
        }
    }

    void start_game()
    {
        if (playing)
        {
            return;
        }

        bool players_in_start = true;
        bool players_active = false;
        foreach(GameObject player in players)
        {
            if (player.GetComponent<Player>().is_moving)
            {
                players_active = true;
                if (!player.GetComponent<Player>().on_start)
                {
                    players_in_start = false;
                }
            }
        }

        if (players_in_start && players_active)
        {
            playing = true;
        }
    }

    void update_progress_bar()
    {
        if (progress_bar != null)
        {
            progress_bar.value = (total_gameplay_time - gameplay_timer) / total_gameplay_time;
        }
    }

    void check_game_over()
    {
        //END GAME
        if (butterflies.Count < 1 || gameplay_timer < 0)
        {
            playing = false;
            text_timer += Time.deltaTime;

            winterstorm.SetActive(false);

            music[1].SetActive(false);

            end_assets[0].SetActive(true);
            end_assets[7].SetActive(true);
            end_assets[9].SetActive(true);

            //GAME OVER
            if (butterflies.Count < 1 && text_timer > 9f)
            {
                end_assets[1].SetActive(true);

                end_assets[7].SetActive(false);
                end_assets[8].SetActive(true);

                DecideEndMap();
            }

            //LEVEL COMPLETE
            if (gameplay_timer < 0 && text_timer > 9f)
            {
                end_assets[2].SetActive(true);
                end_assets[3].SetActive(true);

                end_assets[7].SetActive(false);
                end_assets[8].SetActive(true);
            }

            //RESTART LEVEL
            if (text_timer > 30f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            foreach (Scroll scroll in scroll_scripts)
            {
                scroll.scrolling = false;
            }

            //TRIGGER ONCE
            if (!game_ended)
            {
                music[4].SetActive(true); // play letter open sfx

                if (butterflies.Count < 1)
                {
                    music[2].SetActive(true);
                    send_osc(5000, "/lose", 1);
                }
                if (gameplay_timer < 0)
                {
                    music[3].SetActive(true);
                    send_osc(5000, "/win", 1);
                }
                game_ended = true;
            }
        }
    }

    //PLAY WHEN GAME STARTS
    void StartGameAssets()
    {
        if (!playing || winterstorm.activeInHierarchy)
        {
            return;
        }
        
        intro_assets.SetActive(true);

        //delayed assets
        start_delay_timer += Time.deltaTime;
        if (start_delay_timer > start_delay_time)
        {
            foreach (Scroll scroll in scroll_scripts)
            {
                scroll.scrolling = true;
            }
            if (progress_bar != null)
            {
                progress_bar.gameObject.SetActive(true);
            }
            send_osc(5000, "/play", 1);

            winterstorm.SetActive(true);
            intro_assets.SetActive(false);

            music[0].SetActive(false);
            music[1].SetActive(true);
        }
    }

    //END MAPS
    void DecideEndMap()
    {
        //Michigan
        if (gameplay_timer >= total_gameplay_time * (2.0f/3.0f))
        {
            end_assets[4].SetActive(true);
            Debug.Log("Michigan");
        }

        //TX-OK
        else if (gameplay_timer >= total_gameplay_time * (1.0f/3.0f))
        {
            end_assets[5].SetActive(true);
            Debug.Log("TX");
        }

        //Mexico
        else
        {
            end_assets[6].SetActive(true);
            Debug.Log("Africa");
        }
    }

    //CONSOLIDATE TRAIL POINTS OF ALL PLAYERS
    public List<TrailPoint> get_trail_points()
    {
        List<TrailPoint> trail_points = new List<TrailPoint>();
        foreach (GameObject player in players)
        {
            if (player.GetComponent<Player>().is_moving)
            {
                trail_points.Add(player.GetComponent<Player>().trail[0]);
            }
        }

        return trail_points;
    }

    public void delete_butterfly(GameObject butterfly)
    {
        butterflies.Remove(butterfly);
        Destroy(butterfly);
        redistribute_ids();
    }

    void redistribute_ids()
    {
        List<Butterfly_ID_List> butterfly_lists = new List<Butterfly_ID_List>();
        List<GameObject> communal_butterflies = butterflies.Where(o => o.GetComponent<ButterFly>().id == -1).ToList();

        //Populate List and Sort, ASSUMES PLAYER IDS RANGE FROM 0 TO 3
        for (int id = 0; id < 4; id++)
        {
            butterfly_lists.Add(new Butterfly_ID_List(butterflies.Where(o => o.GetComponent<ButterFly>().id == id).ToList(), id));
        }
        butterfly_lists = butterfly_lists.OrderBy(o => o.butterflies.Count).ToList();

        //If Less butterflies than player, turn all butterflies to communal
        //Else If a player lost a butterfly and there are communal butterflies, redistribute 1 from communal to player
        //Else If a player has 2 less then another player, redistribute 1 from player to player
        if (butterflies.Count < players.Count)
        {
            foreach(GameObject butterfly in butterflies)
            {
                butterfly.GetComponent<ButterFly>().id = -1;
            }
        }
        if (butterfly_lists[0].butterflies.Count < butterfly_lists[3].butterflies.Count && communal_butterflies.Count > 0)
        {
            communal_butterflies[0].GetComponent<ButterFly>().id = butterfly_lists[0].id;
        }
        else if (butterfly_lists[0].butterflies.Count < butterfly_lists[3].butterflies.Count - 1)
        {
            butterfly_lists[3].butterflies[0].GetComponent<ButterFly>().id = butterfly_lists[0].id;
        }
    }

    //Contains List of Butterflies and ID associate with them
    //Needed to access the ID where the list contains no butterflies in redistribute_ids()
    private class Butterfly_ID_List
    {
        public int id;
        public List<GameObject> butterflies;

        public Butterfly_ID_List(List<GameObject> input_butterflies, int input_id)
        {
            id = input_id;
            butterflies = input_butterflies;
        }
    }

    //Function and Overloads for sending OSC
    //Parameters are Send arguments
    //Client Arguments are (IP Address, Port)
    //Send Arguments are (/OSC Address, value)
    public string ip_address = "127.0.0.1";
    void send_osc(int port, string address, string message)
    {
        using (var client = new OscClient(ip_address, port))
        {
            client.Send(address, message);
        }
    }

    void send_osc(int port, string address, float value)
    {
        using (var client = new OscClient(ip_address, port))
        {
            client.Send(address, value);
        }
    }

    void send_osc(int port, string address, int value)
    {
        using (var client = new OscClient(ip_address, port))
        {
            client.Send(address, value);
        }
    }
}
