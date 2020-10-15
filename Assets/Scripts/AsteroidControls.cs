using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidControls : MonoBehaviour
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
    private int Points;
    public GameObject Player; //link to the player spaceship, we link this to send message to this ship
    private int Health;
    public GameObject Explosion;

    // Start is called before the first frame update
    void Start()
    {
        //when it comes to existence we just want to give it a little bit of push
        //Initialize with a random amount of torque and thrust to the asteroid
        //range: -MaxThrust to MaxThrust for both x, y.
        //NOTE: Auto-scaler for normal asteroid made it 5.3 for mass
        int[] Temp = {-1,1};
        System.Random Ram = new System.Random();
        float ThrustY = Random.Range(MaxThrust/2, MaxThrust);
        ThrustY = Temp[Ram.Next(0, Temp.Length)] * ThrustY;
        float ThrustX = Random.Range(MaxThrust/2, MaxThrust);
        ThrustX = Temp[Ram.Next(0, Temp.Length)] * ThrustX;
        Vector2 thrust = new Vector2(ThrustX, ThrustY);
        float torque = Random.Range(-MaxTorque, MaxTorque);
        DesiredScale = Random.Range(0.5f,3.5f);
        transform.localScale = new Vector3(DesiredScale, DesiredScale, 1);
        AsteroidBody.AddForce(thrust);
        AsteroidBody.AddTorque(torque);
        AsteroidBody.mass = Mass * DesiredScale;
        Health = (int)AsteroidBody.mass;
        setPoints();
        // Debug.Log("Thrust x: " + thrust.x + "Thrust y:" + thrust.y);
        // Debug.Log("Torque: " + torque);
        // Debug.Log("Scale: " + DesiredScale);
        // Debug.Log("Mass: " + AsteroidBody.mass);
        Debug.Log("health: " + Health + " and points: " + Points);
        //NOTE: We can't assign the spaceship from the inspector because the asteroids will be spawned from prefabs.
        //and we can only link prefabs. Our spaceship isn't a prefab so when we run it won't work.
        //To solve this, we can find the player instead with its tag: (not sure what happens if multiple objects with same tag)
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
        //This will be used for bullets
        //Whenever another object which has a trigger collider touching the collider of our asteroid, it'll fire this function
        //the other is capturing what hit us. This function only captures another object's trigger collider specifically
        //Debug.Log(this.gameObject.name + " hit by " + other.name);
        if (other.CompareTag("BulletObject")) { //we added a Tag to our prefab Object
            Destroy(other.gameObject); //destroy bullet since it's been used up, we destroy the gameObject (parent)
            //Auto-mass is 5.309291 for scale of 1
            //Debug.Log("Original asteroid scale: " + transform.localScale.x);
            //transform.localScale += new Vector3(1,1,0);
            //Debug.Log("New asteroid scale: " + transform.localScale.x);
            Health -= 1;
            if (Health < 1) {
            if (AsteroidBody.mass > 5f) {
                //trigger a new
                //GetComponent
                //You can change localScale only at the start
                GameObject Asteroid1 = Instantiate(Asteroid, transform.position, transform.rotation);
                GameObject Asteroid2 = Instantiate(Asteroid, transform.position, transform.rotation);
                Asteroid1.transform.localScale = transform.localScale/2;
                Asteroid1.GetComponent<Rigidbody2D>().mass = AsteroidBody.mass/2f;
                Asteroid1.GetComponent<AsteroidCloneControls>().Health =(int)(AsteroidBody.mass/2f);
                Asteroid2.transform.localScale = transform.localScale/2;
                Asteroid2.GetComponent<Rigidbody2D>().mass = AsteroidBody.mass/2f;
                Asteroid2.GetComponent<AsteroidCloneControls>().Health =(int)(AsteroidBody.mass/2f);
                Debug.Log("destroyed");
                // Debug.Log("Original asteroid1 scale: " + Asteroid1.GetComponent<Transform>().localScale.x);
                // Asteroid1.transform.localScale = new Vector3(0.2f,0.2f,0);
                // Debug.Log("New asteroid1 scale: " + Asteroid1.GetComponent<Transform>().localScale.x);
                // Asteroid1.GetComponent<Rigidbody2D>().mass = AsteroidBody.mass/2f;
                // float TmpScale = Asteroid1.GetComponent<Transform>().localScale.x/2f;
                // Debug.Log("Original asteroid1 mass: " + Asteroid1.GetComponent<Rigidbody2D>().mass);
                // Debug.Log("Original asteroid1 scale: " + Asteroid1.GetComponent<Transform>().localScale.x);
                // Asteroid1.GetComponent<Rigidbody2D>().mass = AsteroidBody.mass/2f;
                // Asteroid1.GetComponent<Transform>().localScale.Set(TmpScale, TmpScale, 1);
                // GameObject Asteroid2 = Instantiate(Asteroid, transform.position, transform.rotation);
                // Asteroid2.GetComponent<Rigidbody2D>().mass = AsteroidBody.mass/2f;
                // Asteroid2.GetComponent<Transform>().localScale.Set(TmpScale, TmpScale, 1);
                // Debug.Log("New asteroid1 mass: " + Asteroid1.GetComponent<Rigidbody2D>().mass);
                // Debug.Log("New asteroid1 scale: " + Asteroid1.GetComponent<Transform>().localScale.x);
                // Destroy(gameObject);
                // Debug.Log("destroyed");
            }
            else {
                Debug.Log("destroyed due to mass");
            }
            //Debug.Log("mass: " + AsteroidBody.mass);
            GameObject ExplosionEffect = Instantiate(Explosion, transform.position, transform.rotation);
            ExplosionEffect.transform.localScale = transform.localScale * 1.3f * DesiredScale;
            Destroy(ExplosionEffect, 2.1f); //the explosion objects dissappear
            Destroy(gameObject);
            //Below means run this function ScorePoints of player GameObject and send the value
            Player.SendMessage("ScorePoints", Points);
        }
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
       //runs automatically whenever two objects with non-trigger colliders hit each other
       //Debug.Log( this.gameObject.name + " hit a " + col.gameObject.name);

    }
}
