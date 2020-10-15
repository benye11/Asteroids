using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidCloneControls : MonoBehaviour
{
    public float MaxThrust;
    public float MaxTorque;
    public Rigidbody2D AsteroidBody;
    public float ScreenTop;
    public float ScreenBottom;
    public float ScreenLeft;
    public float ScreenRight;
    private float DesiredScale;
    public Collider2D AsteroidCollider;
    public GameObject Asteroid;
    public float Mass;
    public int Health; //if this is set to private, we can't do get component
    public GameObject Player;
    private int Points;
    public GameObject Explosion;
    // Start is called before the first frame update
    void Start()
    {
        int[] Temp = {-1,1};
        System.Random Ram = new System.Random();
        float ThrustY = Random.Range(MaxThrust/2, MaxThrust);
        ThrustY = Temp[Ram.Next(0, Temp.Length)] * ThrustY;
        float ThrustX = Random.Range(MaxThrust/2, MaxThrust);
        ThrustX = Temp[Ram.Next(0, Temp.Length)] * ThrustX;
        Vector2 thrust = new Vector2(ThrustX, ThrustY);
        float torque = Random.Range(-MaxTorque, MaxTorque);
        AsteroidBody.AddForce(thrust);
        AsteroidBody.AddTorque(torque);
        setPoints();
        //Debug.Log("health: " + Health + " and points: " + Points); //this won't work bc health might be 0 if we still shooting
        Player = GameObject.FindWithTag("Player");
    }

    void setPoints() {
        int Temp = (int)AsteroidBody.mass;
        if (Temp < 25) {
            Points = 5;
        }
        else if (Temp < 20) {
            Points = 4;
        }
        else if (Temp < 15) {
            Points = 3;
        }
        else if (Temp < 10) {
            Points = 2;
        }
        else {
            Points = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 NewPos = transform.position;
        if (transform.position.y > ScreenTop) {
            NewPos.y = ScreenBottom;
        }
        if (transform.position.y < ScreenBottom) {
            NewPos.y = ScreenTop;
        }
        if (transform.position.x < ScreenLeft) {
            NewPos.x = ScreenRight;
        }
        if (transform.position.x > ScreenRight) {
            NewPos.x = ScreenLeft;
        }
        transform.position = NewPos;
    }

    //The below is also a common Unity function
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("BulletObject")) { //we added a Tag to our prefab Object
            Destroy(other.gameObject); //destroy bullet since it's been used up, we destroy the gameObject (parent)
            //Debug.Log("current health: " + Health);
            Health -= 1;
            if (Health < 1) {
            if (AsteroidBody.mass > 5f) {
                GameObject Asteroid1 = Instantiate(Asteroid, transform.position, transform.rotation);
                GameObject Asteroid2 = Instantiate(Asteroid, transform.position, transform.rotation);
                Asteroid1.transform.localScale  = transform.localScale/2;
                Asteroid1.GetComponent<Rigidbody2D>().mass = AsteroidBody.mass/2f;
                Asteroid2.transform.localScale = transform.localScale/2;
                Asteroid2.GetComponent<Rigidbody2D>().mass = AsteroidBody.mass/2f;
                Debug.Log("destroyed");
            }
            else {
                Debug.Log("destroyed due to mass");
            }
            //Debug.Log("mass: " + AsteroidBody.mass);
            GameObject ExplosionEffect = Instantiate(Explosion, transform.position, transform.rotation);
            ExplosionEffect.transform.localScale = transform.localScale * 3f * AsteroidBody.mass/Mass;
            Destroy(gameObject);
            Player.SendMessage("ScorePoints", Points);
        }
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
       //runs automatically whenever two objects with non-trigger colliders hit each other
       Debug.Log( this.gameObject.name + " hit a " + col.gameObject.name);
    }
}
