using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //this is manually added in
using UnityEngine.SceneManagement;

public class SpaceshipControls : MonoBehaviour
{

    // link the rigid body of our ship
    public Rigidbody2D SpaceshipBody; //making this public exposes it to the inspector from Unity Editor
    public float Thrust; //how much we want to be push
    public float TurnThrust;
    private float ThrustInput;
    private float TurnInput;
    public float ScreenTop;
    public float ScreenBottom;
    public float ScreenLeft;
    public float ScreenRight;
    public GameObject Bullet; //we make this as a prefab by dragging sprite into our Assets.
    //Therefore, when bullet dissappears after collision, it can be recreated using prefab from Asset.
    public float BulletForce;
    public float ForceLimit;
    private int Score;
    public int Lives;
    public Text LivesText;
    public Text ScoreText;
    public AudioSource ThrusterSound;
    public AudioSource ExplosionSound;
    public GameObject Explosion;
    public Color InvulernabilityColor;
    public Color NormalColor;
    public GameObject GameOverPanelObject;
    public GameObject Asteroid;

    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        LivesText.text = "Lives: " + Lives;
        ScoreText.text = "Score: " + Score;
        InvokeRepeating("GenerateAsteroids", 1.0f, 8.0f);
    }

    // Update is called once per frame
    // Keyboard input has to be looked for at every frame
    // This is the most constant checking for input/updates
    void Update()
    {
        //Get input from keyboard and apply thrust
        //this checks if key is pressed, then we apply forces to our rigid body
        ThrustInput = Input.GetAxis("Vertical"); //returns euther 1 or null
        TurnInput = Input.GetAxis("Horizontal"); //return either -1 or +1 or null
        if (!ThrusterSound.isPlaying && ThrustInput != 0) {
            ThrusterSound.Play();
        }
        else if (ThrustInput == 0) {
            ThrusterSound.Stop();
        }
        //check for input from the fire key and make bullets
        if (Input.GetButtonDown("Fire1")) { //GetButtonDown returns us a true during the frame the user first pressed down
        //i.e. single fire mode. 1 button press = 1 bullet shot even if you hold it down
        //step 1: make bullet using the prefab. Instantiate makes a clone of an object
        //object, position of object, and rotation
            GameObject NewBullet = Instantiate(Bullet, transform.position, transform.rotation) as GameObject;
            NewBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * BulletForce); //this works
            //NewBullet.transform.localScale = new Vector3(100, 100, 1); //this works since the Bullet has no script
            Destroy(NewBullet, 4f);
        }

        //Rotate the ship, this is less realstic than using torque but more gameplay friendly
        //it rotates around vector3, so we need to tell which axis. Vector3.forward is z axis
        //NOTE: this requires a higher TurnThrust, more like 90
        //Time.deltaTime is how much time has passed since last frame was drawn, so we do this to make it time-dependent instead of frame-dependent
        //This is because Update() runs depending on frame updates, so each frame has different time depending on change
        transform.Rotate(Vector3.forward * -(TurnInput * TurnThrust * Time.deltaTime));

        //Screen Wrapping
        //Transform is the class that stores/manipulates position, rotation, etc.
        Vector2 NewPos = transform.position; //transform.position is actually a Vector3 but it can be casted to a Vector2
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

    void FixedUpdate() //This is a known function for Unity, so just like Update, you can't name it any different
    {
        //this is like the Update function where it constantly runs but this runs at a fixed time interval
        //For the physics system here, it may be more accurate to do a fixedUpdate.
        //Below adds force along the local axis of the ship, so no matter where ship is pointing, it'll apply force in that direction
        SpaceshipBody.AddRelativeForce(Vector2.up * ThrustInput * Thrust); //Vector2.up is a vector (0,1) or a (x,y) vector with y = 1
        //Instead of utilizing our constant thrust, we utilize thrustInput since if not held, we don't move
        //SpaceshipBody.AddTorque(-(TurnInput * TurnThrust)); //means to spin, input is a float number (automatically along the z axis for 2D game)
        //TurnThrust of 5 is good enough for this
        //our frame is -17.5 to 17.5 left to right, -10 to 10 vertical
    }

    //CircleCollider 2D because we want to know if we will hit something else and since it's a circle, we use that
    //Then we want it to act as a trigger
    //check box the trigger so instead of pushing something or bouncing off, it will trigger an action

    //This is another Unity function
    void OnCollisionEnter2D(Collision2D col) {
       //runs automatically whenever two objects with non-trigger colliders hit each other
       //Debug.Log("Spaceship hit a " + col.gameObject.name);
       //Check for hardness of collision
       //relative linear velocity of the two colliding objects means the velocity of this object relative to our spaceship
       //magnitude returns length of vector
       Debug.Log("Spaceship hit an asteroid with force: " + col.relativeVelocity.magnitude);
       if (col.relativeVelocity.magnitude > ForceLimit) {
           Debug.Log("Spaceship exploded: " + col.relativeVelocity.magnitude + " > " + ForceLimit);
           Lives--;
           LivesText.text = "Lives: " + Lives;
           ExplosionSound.Play();
           GameObject ExplosionEffect = Instantiate(Explosion, transform.position, transform.rotation);
           Destroy(ExplosionEffect, 1f);
           GetComponent<SpriteRenderer>().enabled = false;
           GetComponent<Collider2D>().enabled = false; //this find the first matching Collider2D
           //NOTE: don't instantiate AudioSource because it'll create clones of the spaceship
           Invoke("Respawn", 3f);
       }
       if (Lives < 1) {
           GameOver();
       }
    }

    void ScorePoints(int PointsToAdd) {
        Score += PointsToAdd;
        ScoreText.text = "Score: " + Score;
        Debug.Log("player score: " + Score);
    }

    //If we have a prefab and we drag something from the hierachy (the object) it won't work because those are temporary objects.
    //hierachy is the top left panel listing all objects.
    //If we make spaceship a prefab and then drag to asteroid it will work but then it won't represent the spaceship object
    //that we currently have out, just the prefab.

    //UI --> Canvas will be drawn huge but sticks to screen once gameplay starts. It's just big and off to side for ease of editing
    //Anchor means even if we scale our text, it'll keep our text near that anchor point

    //Audio Source is sound being generated, Audio Listener is listening for sounds

    void Respawn() {
        SpaceshipBody.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;
        sr.color = InvulernabilityColor;
        Invoke("Invulnerable", 3f);
    }

    void Invulnerable() {
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = NormalColor;
    }

    void GameOver() {
        CancelInvoke(); //Cancels all invoke statements
        GameOverPanelObject.SetActive(true);
    }

    //Must be public so the button from panel can access this
    public void PlayAgain() {
        //we need to call the Scene Manager
        SceneManager.LoadScene("SampleScene"); //we give it scene name. You must go to build settings and add this scene too
        //When Unity loads scene, it deletes current and loads a new scene.
    }

    public void Quit() {
        if (Application.isEditor) {
        //UnityEditor.EditorApplication.isPlaying = false; //this doesn't work when compiling, can't reference UnityEditor in runtime script.
            Application.Quit();
        }
        else {
            Application.Quit();
        }
    }

    void GenerateAsteroids() {
        //range is either stuck at -20 going from -12 to 12
        //or positive 20 going from -12 to 12
        //or positive
        System.Random Ram = new System.Random();
        //Temp[Ram.Next(0, Temp.Length)]
        int Choice = Random.Range(0,1);
        int[] X = {-20, 20};
        int[] Y = {-12, 12};
        if (Choice == 0) {
            //keep x fixed, randomize y.
            Vector2 Temp = new Vector2(X[Random.Range(0,1)], Random.Range(-12, 12));
            GameObject AsteroidObject = Instantiate(Asteroid, Temp, transform.rotation);
        }
        else {
            //keep y fixed, randomize x.
            Vector2 Temp = new Vector2(Random.Range(-20, 20), Y[Random.Range(0,1)]);
            GameObject AsteroidObject = Instantiate(Asteroid, Temp, transform.rotation);
        }
    }
}
