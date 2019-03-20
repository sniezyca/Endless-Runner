using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour {

    private CharacterController controller;
    private Animator animator;
    private Transform playerTransform;
    private GameObject[] highHurdles, lowHurdles;

    private bool isDead = false;
    private bool invincible = false;

    private float initialSpeed = 5.0f;
    private float speed;

    private float verticalVelocity=-0.5f;
    private float jumpHeight=3.5f;
    private float gravity=10.0f;

    private float horizontalVelocity;
    private float turningTime = 0.4f;
    private float turningSpeed = 2.5f;
    private int exactExpectedPositionX=0;

    public Image slot1;
    public Image slot2;
    private bool turtle = false;
    private bool ghost = false;
    private int ghostLength = 5;
    
    public Image active2;
    public Text counter2;
    private bool count = false;
    private int time_left;
    private bool doIt = true;

    private Vector3 moveVector;

    private Vector3 controllerStartCenter;
    private float controllerStartRadius;
    private float controllerStartHeight;
    private Vector3 controllerSlideCenter;
    private float controllerSlideRadius = 0.3f;
    private float controllerSlideHeight =0.0f;
    private Vector3 controllerCenterDiff;
    private float controllerRadiusDiff;
    private float controllerHeightDiff;
    private int controllerChangingDurationInFrames = 35;

    private int jumpHash = Animator.StringToHash("Jump");
    private int slideHash = Animator.StringToHash("Slide");
    private int stopHash = Animator.StringToHash("Stop");
    private int fallHash = Animator.StringToHash("Fall");
    
    private int remainingFramesToEndControllerChanging;
    private bool blockActions = false;


	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
        setColliderSettingsWhileSliding();       
        playerTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        speed = initialSpeed;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isDead == false)
        {
            //%%%%%%%%%%%%%%%%%%%%  Player Control %%%%%%%%%%%%%%%%%%%//
            if (controller.isGrounded && blockActions == false)
            {           
                //Jumping
                if(Input.GetKeyDown(KeyCode.UpArrow))
                {
                    StartCoroutine(Jump());         
                }
                //Sliding
                else if(Input.GetKeyDown(KeyCode.DownArrow))
                {
                    StartCoroutine(Slide());
                    remainingFramesToEndControllerChanging = controllerChangingDurationInFrames;
                }
                //Right
                else if (Input.GetKeyDown(KeyCode.RightArrow) && exactExpectedPositionX!=1)
                {                    
                    StartCoroutine(Turn(true,turningTime,speed,initialSpeed));                
                }
                //Left
                else if (Input.GetKeyDown(KeyCode.LeftArrow) && exactExpectedPositionX != -1)
                {
                    StartCoroutine(Turn(false,turningTime,speed,initialSpeed));
                }
            }
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //If it isn't grounded then simulate gravity...
            else
            {
            verticalVelocity -= gravity * Time.deltaTime;
            }   
            //Changing Player's Collider while Sliding
                if(remainingFramesToEndControllerChanging>1)
                {
                    controller.radius -= controllerRadiusDiff;
                    controller.height -= controllerHeightDiff;
                    controller.center -= controllerCenterDiff;
                    remainingFramesToEndControllerChanging -= 1;
            }
                else if(remainingFramesToEndControllerChanging==1)
                {
                    controller.center = controllerSlideCenter;
                    remainingFramesToEndControllerChanging -= 1;
            }            

            //Creating moveVector which will be used by controller.Move(); to change Player position.
            moveVector = new Vector3(x: horizontalVelocity, y: verticalVelocity, z: speed);
        
            //Changing Player's position
            controller.Move(moveVector * Time.deltaTime);

            LaunchPU();

        }

        if (count==true)
        {

            if (doIt == true)
            {
                time_left = (int)Time.time;
                doIt = false;
            }
            counter2.text = (time_left + ghostLength - (int)Time.time).ToString();
        }

	}

    public void SetSpeed(float modifier)
    {
        speed = initialSpeed + modifier;
    }


    //Event handling (Triggering PowerUps and Hurdles
     void OnTriggerEnter(Collider other) // maybe use cases?
    {

        if (other.tag == "HighHurdles")
        {
            if (invincible == true)
                return;

            isDead = true;
            Debug.Log(message: "I'm so high right now");
            animator.SetTrigger(stopHash);
            GetComponent<Score>().OnDeath();

        }
        else if (other.tag == "LowHurdles")
        {
            if (invincible == true)
                return;
            Collider[] colliders=other.GetComponentsInParent<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
            StartCoroutine(Fall());
            Debug.Log(message: "Why so low?");
            animator.SetTrigger(fallHash);
            GetComponent<Score>().OnDeath();
        }

        else if (other.tag == "Coin")
        {
            Debug.Log(message: "Money, money, money");
            //add points
            GetComponent<Score>().Money();
            other.gameObject.SetActive(false);
            //make disappear
            // GetComponent<PowerUpManager>().Disappear(gameobject????);

        }

        else if (other.tag == "Easy")
        {   
            Debug.Log(message: "Calm down, baby");
            
            //show turtle in slot 1
            turtle = true;
            slot1.color = new Color(0, 0, 0, 255);
            other.gameObject.SetActive(false);

            //GetComponent<Score>().SlowDown();

            //make disappear
        }

        else if (other.tag == "Invincible")
        {
            Debug.Log(message: "Can't touch this");

            //show ghost in slot 2
            ghost = true;
            slot2.color = new Color(0, 0, 0, 255);
            other.gameObject.SetActive(false);

            //make disappear
        }
        
    }

    IEnumerator Turn(bool directionXpositive, float time, float actualSpeed, float initialSpeed)
    {
        blockActions = true;
        if (directionXpositive == true)
        {
            exactExpectedPositionX++;
            horizontalVelocity = turningSpeed * speed / initialSpeed;
        }
        else
        {
            exactExpectedPositionX--;
            horizontalVelocity = -turningSpeed * speed / initialSpeed;
        }
        yield return new WaitForSeconds(time * initialSpeed / actualSpeed);
        horizontalVelocity = 0;
        if (isDead == false)
        {
        playerTransform.position = new Vector3(x: exactExpectedPositionX, y: playerTransform.position.y, z: playerTransform.position.z);
        }
        blockActions = false;
    }

    IEnumerator Fall()
    {
        blockActions = true;
        speed = 4.0f;       
        yield return new WaitForSeconds(0.9f);
        isDead = true;

    }
    
    IEnumerator Jump()
    {
        blockActions = true;
        animator.SetTrigger(jumpHash);
        verticalVelocity = jumpHeight;
        yield return new WaitForSeconds(0.8f);
        blockActions = false;
    }

    IEnumerator Slide()
    {
        blockActions = true;
        animator.SetTrigger(slideHash);       
        yield return new WaitForSeconds(1.25f);
        blockActions = false;

        controller.radius = controllerStartRadius;
        controller.height = controllerStartHeight;
        controller.center = controllerStartCenter;
    }

    IEnumerator DontDie()
    {

        SetCollisionsOff();
        
        active2.color = new Color(212, 0, 0, 255);
        counter2.color= new Color(212, 0, 0, 255);
        invincible = true;
        count = true;
        doIt = true;

        yield return new WaitForSeconds(ghostLength);
        
        SetCollisionsOn();
        invincible = false;
        count = false;
        doIt = false;
        active2.color = new Color(212, 0, 0, 0);
        counter2.color = new Color(212, 0, 0, 0);
    }

    private void LaunchPU()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && turtle==true)
        {
            Debug.Log("one one one");
            GetComponent<Score>().SlowDown();
            slot1.color = new Color(0, 0, 0, 0);
            
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && ghost == true)
        {
            Debug.Log("two two two");
            StartCoroutine(DontDie());
            slot2.color = new Color(0, 0, 0, 0);
            
        }
    }

    private void SetCollisionsOn()
    {
        highHurdles = GameObject.FindGameObjectsWithTag("HighHurdles");
        foreach (GameObject hurdles in highHurdles)
        {

            if (hurdles.GetComponent<Collider>() != null)
            {
                Collider col = hurdles.GetComponent<Collider>();
                col.enabled = true;
            }


        }

        lowHurdles = GameObject.FindGameObjectsWithTag("LowHurdles");
        foreach (GameObject hurdles in lowHurdles)
        {
            if (hurdles.GetComponent<Collider>() != null)
            {
                Collider col = hurdles.GetComponent<Collider>();
                col.enabled = true;
            }
        }
    }

    private void SetCollisionsOff()
    {
        highHurdles = GameObject.FindGameObjectsWithTag("HighHurdles");
        foreach (GameObject hurdles in highHurdles)
        {

            if (hurdles.GetComponent<Collider>() != null)
            {
                Collider col = hurdles.GetComponent<Collider>();
                col.enabled = false;
            }


        }

        lowHurdles = GameObject.FindGameObjectsWithTag("LowHurdles");
        foreach (GameObject hurdles in lowHurdles)
        {
            if (hurdles.GetComponent<Collider>() != null)
            {
                Collider col = hurdles.GetComponent<Collider>();
                col.enabled = false;
            }
        }
    }

    private void setColliderSettingsWhileSliding()
    {
        controllerStartCenter = controller.center;
        controllerSlideCenter.Set(0.0f, 0.3f, 0.2f);
        controllerCenterDiff = (controllerStartCenter - controllerSlideCenter) / controllerChangingDurationInFrames;
        controllerCenterDiff.z *= 8;
        controllerStartHeight = controller.height;
        controllerHeightDiff = (controllerStartHeight - controllerSlideHeight) / controllerChangingDurationInFrames;
        controllerStartRadius = controller.radius;
        controllerRadiusDiff = (controllerStartRadius - controllerSlideRadius) / controllerChangingDurationInFrames;
    }
}
