using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour {

    private Transform playerTransform;
    private Vector3 initialTransformDiff;
    private Vector3 moveVector;

    private float transition = 0.0f;
    public float initialAnimationDuration = 3.0f;
    private Vector3 animationOffset = new Vector3(0, 5, -10);

	// Use this for initialization
	void Start ()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        initialTransformDiff = transform.position-playerTransform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        moveVector = playerTransform.position + initialTransformDiff;
        //X
        moveVector.x = 0.0f;
        //Y
        
        //Z

        if(transition>1.0f)
        {
            transform.position = moveVector;
        }
        else
        {
            //Animation at start of the game
            transform.position = Vector3.Lerp(moveVector + animationOffset,moveVector, transition);
            transition += Time.deltaTime * 1 / initialAnimationDuration ;
        }
	}
}
