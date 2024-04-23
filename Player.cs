using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //QTM INPUT VARIABLES
    public Transform qtm_input;
    public float input_scale;
    public Vector3 offset_scale;

    //GAMEPLAY VARIABLES
    public bool on_start = false;
    public bool is_moving;
    public int id = 1;
    public float speed = 10;
    public float timer = 0;
    public List<TrailPoint> trail = new List<TrailPoint>();

    public ParticleSystem particles;
    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        //DETERMINE NUMBER OF TRAIL POINTS HERE
        for (int i = 0; i < 5; i++)
        {
            trail.Add(new TrailPoint(id, transform.position + new Vector3(Random.Range(-2.0f, 2.0f), 0, Random.Range(-2.0f, 2.0f))));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //manual_movement();
        realtime_movement();
        update_mat();

        //TIMER FOR UPDATING TRAIL
        timer += Time.deltaTime;
        if (timer > .15)
        {
            update_trail();
            timer = 0;
        }
    }

    void update_trail()
    {
        trail.Add(new TrailPoint(id, transform.position));
        trail.RemoveAt(0);
        check_moving();
        if (is_moving)
        {
            particles.Play();
        }
        else
        {
            particles.Stop();
        }
    }

    //Check distance between all TrailPoints
    void check_moving()
    {
        float distance = 0f;
        for (int i = 0; i < trail.Count - 1; i++)
        {
            distance += Vector3.Distance(trail[i].position, trail[i + 1].position);
        }
        if (distance > 2 || on_start)
        {
            is_moving = true;
        }
        else
        {
            is_moving = false;
        }
    }

    void update_mat()
    {
        if (is_moving && mat.color.a < .8f)
        {
            Color color = mat.color;
            color.a += Time.deltaTime;
            mat.color = color;
        }

        if (!is_moving && mat.color.a > .2f)
        {
            Color color = mat.color;
            color.a -= Time.deltaTime;
            mat.color = color;
        }
    }

    void realtime_movement()
    {
        transform.position = qtm_input.position * input_scale + offset_scale;
    }

    //FOR TESTING WITHOUT QTM INPUT
    void manual_movement()
    {
        Vector3 move_vector = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            move_vector += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            move_vector += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            move_vector += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            move_vector += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            move_vector += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move_vector += new Vector3(0, -1, 0);
        }

        transform.position = transform.position - move_vector.normalized * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "start")
        {
            on_start = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "start")
        {
            on_start = false;
        }
    }
}
