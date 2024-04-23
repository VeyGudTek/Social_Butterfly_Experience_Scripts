using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Data;

public class ButterFly : MonoBehaviour
{
    public float base_speed = 2200;

    public Rigidbody rb;
    public Manager manager;
    public int id;
    public float destination_timer = 0;
    public Vector3 destination;
    public float speed;

    public ParticleSystem death_poof;
    public ParticleSystem milkweed_explosion;
    public Animator flaps;

    public AudioSource death_audio;
    public AudioSource milkweed_audio;

    // Start is called before the first frame update
    void Start()
    {
        speed = base_speed;
        destination = transform.position;
        flaps.SetFloat("offset", Random.Range(0f, .3f));
    }

    // Update is called once per frame
    void Update()
    {
        //Do nothing if manager is not set
        if (manager == null)
        {
            return;
        }

        //TIMER FOR SETTING DESTINATION
        destination_timer -= Time.deltaTime;
        if (destination_timer <= 0)
        {
            set_destination();
            destination_timer = .69f;
        }
        
        //SPEED DECAY
        if ((speed > base_speed / 3) && (manager.playing) && (manager.start_delay_timer > manager.start_delay_time))
        {
            speed -= Time.deltaTime * base_speed / 30f;
        }
        if (speed > base_speed * 2f)
        {
            speed -= Time.deltaTime * base_speed / 5f;
        }

        set_flap_speed();
    }

    private void FixedUpdate()
    {
        move();
        rotate();
    }

    void move()
    {
        Vector3 direction = (destination - transform.position).normalized;
        direction.x /= 1.8f; //USE THIS IF THE MAP IS SCROLLING VERY FAST
        rb.AddForce(direction * speed * Time.fixedDeltaTime);
    }

    void rotate()
    {
        Vector3 direction = destination - transform.position;
        float angle_towards_target = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float current_angle = transform.localEulerAngles.y;

        angle_towards_target = contrain_to_360(angle_towards_target);
        current_angle = contrain_to_360(current_angle);

        float difference_angle = angle_towards_target - current_angle;
        difference_angle = contrain_to_180(difference_angle);

        //Put the rotation angle in the y value of m_EulerAngleVelocity. Don't know what everything else does
        Vector3 m_EulerAngleVelocity = new Vector3(0, difference_angle, 0);
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    //Constrain angle to 360, convert negative angles to positive
    float contrain_to_360(float angle)
    {
        angle = angle % 360;
        if (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }

    //Constrain angle magnitude to 180. Used to get shortest rotation.
    //Assumes angle is constrained to 360.
    float contrain_to_180(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        } 
        else if (angle < -180)
        {
            angle += 360;
        }

        return angle;
    }

    void set_destination()
    {
        List<TrailPoint> all_trails = manager.get_trail_points();
        List<TrailPoint> my_trails;

        //SORT BY DISTANCE, GET ID SPECIFIC TRAIL
        all_trails = all_trails.OrderBy(o => (o.position - transform.position).sqrMagnitude).ToList();
        my_trails = all_trails.Where(o => o.id == id).ToList();

        //SET DESTINATION
        if (my_trails.Count > 0)
        {
            destination = my_trails[0].position;
        }
        else if (all_trails.Count > 0)
        {
            destination = all_trails[0].position;
        }
        //Add variance. If no players are moving, set random destination
        destination += new Vector3(Random.Range(-8.0f, 8.0f), 0f, Random.Range(-8.0f, 8.0f));
    }

    void set_flap_speed()
    {
        flaps.speed = (speed / base_speed) * Random.Range(.8f, 1.0f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "smoke" && speed > base_speed / 3)
        {
            speed -= Time.deltaTime * base_speed / 2;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "milkweed")
        {
            speed = 3f * base_speed;
            milkweed_audio.Play();
            milkweed_explosion.Play();
        }
        if ((other.gameObject.tag == "out" || other.gameObject.tag == "car") && manager.playing)
        {
            ParticleSystem clone = Instantiate(death_poof, gameObject.transform.position, Quaternion.identity);
            clone.Play();
            AudioSource clone_1 = Instantiate(death_audio, gameObject.transform.position, Quaternion.identity);
            clone_1.Play();
            manager.delete_butterfly(gameObject);
        }
    }
}
