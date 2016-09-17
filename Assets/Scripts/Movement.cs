using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Movement : MonoBehaviour {

	public float moveSpeed;
	public float sensitivity;

	private Camera camera;
	public CharacterController controller;
	public Vector3 vel;
	private float targetHeight;
	private RaycastHit hitInfo;
	private float proximityDistance =  2.5f;
	private Ray toFloor;
	public enum state {WALKING, AUTO, GUILOCK, FALLING};
	public state curState;
	private Vector3 temp;
	private Quaternion tempQ;
	private Rigidbody rb;
	private float fallSpeed = 0f;

	// UI Feedback
	//private GameObject NPC;

	// Use this for initialization
	void Start () 
	{
		vel = new Vector3 ();
		rb = GetComponent<Rigidbody> ();
		controller = GetComponent<CharacterController> ();
		camera = GetComponentInChildren<Camera> ();
		toFloor = new Ray (transform.position, -1 * transform.up);
		Physics.Raycast (toFloor, out hitInfo);
		targetHeight = hitInfo.distance;

		curState = state.WALKING;

	}
	
	// Update is called once per frame
	void Update () 
	{
		// Mouse click interaction handling
		// If the object that is clicked has an "Interactable" tag, call the "OnPlayerClicked" function
		// on its attached script.
		if (Input.GetButtonDown("Fire1") && curState == state.WALKING)
		{
			RaycastHit hitInfo = new RaycastHit();
			if (Physics.Raycast(camera.ViewportPointToRay(new Vector3(.5f,.5f,0f)), out hitInfo, proximityDistance))
			{
				if (hitInfo.collider.gameObject.CompareTag("Interactable"))
				{
					hitInfo.collider.gameObject.SendMessage("OnPlayerClicked",SendMessageOptions.DontRequireReceiver);
				}
			}
		}



		camera.transform.localRotation = Quaternion.Euler (camera.transform.localRotation.eulerAngles.x, 0, 0);
		if (curState == state.WALKING) 
		{
			fallSpeed = 0f;
			vel.x = Input.GetAxisRaw ("Horizontal");
			vel.y = 0;
			vel.z = Input.GetAxisRaw ("Vertical");
			vel = transform.TransformDirection (vel);
			vel.Normalize ();
			toFloor.origin = transform.position;
			toFloor.direction = -1 * transform.up;

			if (Physics.Raycast (toFloor, out hitInfo)) 
			{
				if (!hitInfo.collider.isTrigger) 
				{
					if (Mathf.Abs (hitInfo.distance - targetHeight) < .5) 
					{
						transform.position = new Vector3 (transform.position.x, hitInfo.point.y + targetHeight, transform.position.z);
					} 
					else
					{
						curState = state.FALLING;
					}
				}
			} 
			else 
			{
				curState = state.FALLING;
			}
		} 
		else if (curState == state.AUTO) 
		{
			vel.x = vel.y = vel.z = 0;
		} 
		else if (curState == state.FALLING) 
		{
			vel.x = Input.GetAxisRaw ("Horizontal");
			fallSpeed -= 9.8f*Time.deltaTime;
			vel.y = 0;
			vel.z = Input.GetAxisRaw ("Vertical");
			vel = transform.TransformDirection (vel);
			vel.Normalize ();
			vel.y = fallSpeed/moveSpeed;

			toFloor.origin = transform.position;
			toFloor.direction = -1 * transform.up;
			if (Physics.Raycast (toFloor, out hitInfo)) 
			{
				if (!hitInfo.collider.isTrigger) 
				{
					if(Mathf.Abs (hitInfo.distance - targetHeight) < .15) 
					{
						curState = state.WALKING;
					}
				}
			} 
			else 
			{
				
			}
		}
		
		controller.Move (moveSpeed * vel * Time.deltaTime);
		transform.Rotate(Vector3.up, Input.GetAxis ("Mouse X") * Time.deltaTime * sensitivity);

		Vector3 goalCamRot = camera.transform.localEulerAngles;
		float amountToMoveY = -1 * Input.GetAxis ("Mouse Y") * sensitivity * Time.deltaTime;
		goalCamRot.x += amountToMoveY;
		if (goalCamRot.x < 265f && goalCamRot.x > 180f) {
			goalCamRot.x = 265f;
		} else if (goalCamRot.x > 80f && goalCamRot.x < 180f) {
			goalCamRot.x = 80f;
		}

		camera.transform.localEulerAngles = goalCamRot;

	}
		
		
}
