// Attack.cs - Attack shark logic
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;

// Attack class

public class Attack:MonoBehaviour
{
	// Attributes

	public AudioSource[] SplashSounds=null;
	public AudioSource[] SubmergeSounds=null;

	// Constants

	private static Vector3 ATTACK_STARTPOS=new Vector3(0.01f,-4.83f,9.51f);
	private static Vector3 ATTACK_STARTROT=new Vector3(-30.0f,180.0f,0.0f);

	private static Vector3 ATTACK_ENDPOS=new Vector3(0.008f,-0.018f,1.183f);
	private static Vector3 ATTACK_ENDROT=new Vector3(-30.0f,180.0f,0.0f);

	private static Vector3 ATTACK_RETURNPOS=new Vector3(0.555f,-3.613f,4.183f);
	private static Vector3 ATTACK_RETURNROT=new Vector3(-8.556399f,105.1058f,28.8634f);

	// States

	private enum StateType
	{
		Idle,
		Waiting,
		Surfacing,
		Opening,
		Attacking,
		Submerging
	}

	// Local data

	private Game m_game;

	private Transform m_parent;
	private GameObject m_shark;

	private Transform m_lowerjaw;
	private Transform m_upperjaw;

	private Bob m_bob;

	private StateType m_state;

	private float m_time;
	private float m_wait;

	private int m_hits;

	private Vector3 m_frompos;
	private Vector3 m_fromrot;

	// Overrides

	// Start
	// Called at startup

	void Start()
	{
		// get game
		GameObject o=GameObject.Find("/Game");
		if(o==null) return;

		m_game=o.GetComponent<Game>();

		// get parent
		m_parent=transform.parent;

		// get shark
		Transform t=transform.Find("GreatWhite");
		if(t==null) return;

		m_shark=t.gameObject;

		// get skeleton parts
		m_lowerjaw=transform.Find("Root/Neck_01/Neck_02/Head/jaw_hinge");
		m_upperjaw=transform.Find("Root/Neck_01/Neck_02/Head/Jaw_Upper");

		// get bob controller
		m_bob=GetComponent<Bob>();

		// initial state
		idle();
	}

	// Update
	// Called once per frame
	
	void Update()
	{
		// needs these
		if(m_game==null ||
			m_parent==null || m_shark==null ||
			m_lowerjaw==null || m_upperjaw==null ||
			m_bob==null)
			return;

		// what state?
		switch(m_state)
		{
			case StateType.Idle:
				// idle state
				idling();
				break;

			case StateType.Waiting:
				// waiting to attack
				waiting();
				break;

			case StateType.Surfacing:
				// surfacing to attack
				surfacing();
				break;

			case StateType.Opening:
				// mouth opening
				opening();
				break;

			case StateType.Attacking:
				// attacking player
				attacking();
				break;

			case StateType.Submerging:
				// submerging again
				submerging();
				break;
		}
	}

	// SharkHit
	// Called when shark hit by player

	public void SharkHit()
	{
		// needs these
		if(m_game==null ||
			m_parent==null || m_shark==null ||
			m_lowerjaw==null || m_upperjaw==null ||
			m_bob==null)
			return;

		// in right state?
		if(m_state!=StateType.Opening &&
			m_state!=StateType.Attacking)
			return;

		// hit shark
		if(m_hits==0) submerge();
		else m_hits--;

		// reset kill time
		m_wait=Time.time+2.0f;
	}

	// ResetShark
	// Call to reset shark to initial state

	public void ResetShark()
	{
		idle();
	}

	// Local functions

	// idle
	// Call to enter idle state

	private void idle()
	{
		// hide shark
		m_shark.SetActive(false);

		// no bob
		m_bob.enabled=false;

		// initial position
		transform.localPosition=ATTACK_STARTPOS;
		transform.localRotation=Quaternion.Euler(ATTACK_STARTROT);

		// reset jaw
		Vector3 lr=m_lowerjaw.localRotation.eulerAngles;
		Vector3 ur=m_upperjaw.localRotation.eulerAngles;

		lr.z=10.33f;
		ur.z=169.485f;

		m_lowerjaw.localRotation=Quaternion.Euler(lr);
		m_upperjaw.localRotation=Quaternion.Euler(ur);

		SplashSounds[0].Stop();
		SplashSounds[1].Stop();

		SubmergeSounds[0].Stop();
		SubmergeSounds[1].Stop();

		m_state=StateType.Idle;
	}

	// idling
	// Called when in idle state

	private void idling()
	{
		// game playing?
		if(m_game.IsPlaying())
			wait();
	}

	// wait
	// Call to enter wait state

	private void wait()
	{
		// get next shark time
		m_time=Time.time;
		m_wait=m_time+m_game.NextShark();

		m_state=StateType.Waiting;
	}

	// waiting
	// Called when in waiting state

	private void waiting()
	{
		// still waiting?
		if(Time.time>=m_wait ||
			Input.GetKeyDown(KeyCode.Alpha1))
			surface();
	}

	// surface
	// Call to enter surface state

	private void surface()
	{
		// rotate randomly around player
		Vector3 sr;

		sr.x=0.0f;
		sr.y=Random.Range(0,360);
		sr.z=0.0f;

		m_parent.localRotation=Quaternion.Euler(sr);

		// show shark
		m_shark.SetActive(true);

		// put on loop
		SplashSounds[0].loop=true;
		SplashSounds[1].loop=true;

		// set hit points
		m_hits=Random.Range(1,4);

		m_time=Time.time;
		m_state=StateType.Surfacing;
	}

	// surfacing
	// Called when in surface state

	private void surfacing()
	{
		// bring shark to surface
		float t=(Time.time-m_time)*0.2f;

		Vector3 p=Vector3.Lerp(ATTACK_STARTPOS,ATTACK_ENDPOS,t);
		Vector3 r=Vector3.Lerp(ATTACK_STARTROT,ATTACK_ENDROT,t);

		transform.localPosition=p;
		transform.localRotation=Quaternion.Euler(r);

		// splay splashing noise
		if(t>0.9f && !SplashSounds[0].isPlaying)
			SplashSounds[0].Play();

		if(t>=1.0f) open();
	}

	// open
	// Call to enter open state

	private void open()
	{
		// enable bob
		m_bob.enabled=true;

		m_time=Time.time;
		m_state=StateType.Opening;
	}

	// opening
	// Called when in open state

	private void opening()
	{
		// open sharks mouth
		float t=(Time.time-m_time)*4.5f;

		Vector3 lr=m_lowerjaw.localRotation.eulerAngles;
		lr.z=Mathf.Lerp(10.33f,-8.0f,t);
		m_lowerjaw.localRotation=Quaternion.Euler(lr);

		if(t>=1.0f) attack();
	}

	// attack
	// Call to enter attack state

	private void attack()
	{
		// play more splashing
		SplashSounds[1].Play();			

		// reset kill time
		m_time=Time.time;
		m_wait=m_time+2.0f;

		m_state=StateType.Attacking;
	}

	// attacking
	// Called when in attack state

	private void attacking(bool animonly=false)
	{
		// animate jaw
		float t=(Mathf.Sin((Time.time-m_time)*4.5f)+1.0f)*0.5f;

		Vector3 lr=m_lowerjaw.localRotation.eulerAngles;
		Vector3 ur=m_upperjaw.localRotation.eulerAngles;

		lr.z=Mathf.Lerp(-4.0f,-8.0f,t);
		ur.z=Mathf.Lerp(169.485f,184.0f,1.0f-t);

		m_lowerjaw.localRotation=Quaternion.Euler(lr);
		m_upperjaw.localRotation=Quaternion.Euler(ur);

		// anim for submerge?
		if(animonly) return;

		// kill time up?
		if(Time.time>m_wait)
		{
			// player hit
			m_game.PlayerHit();
			m_wait=Time.time+2.0f;
		}
	}

	// submerge
	// Call to enter submerge state

	private void submerge()
	{
		//
		SplashSounds[0].loop=false;
		SplashSounds[1].loop=false;

		int n=Random.Range(0,SubmergeSounds.Length);
		SubmergeSounds[n].Play();

		// disable bob
		m_bob.enabled=false;

		m_frompos=transform.localPosition;
		m_fromrot=ATTACK_ENDROT;//transform.localRotation.eulerAngles;

		m_time=Time.time;
		m_state=StateType.Submerging;
	}

	// submerging
	// Called when in submerge state

	private void submerging()
	{
		// animate jaw still
		attacking(true);

		// sink shark
		float t=(Time.time-m_time)*0.3f;

		Vector3 p=Vector3.Lerp(m_frompos,ATTACK_RETURNPOS,t);
		Vector3 r=Vector3.Lerp(m_fromrot,ATTACK_RETURNROT,t);

		transform.localPosition=p;
		transform.localRotation=Quaternion.Euler(r);

		// back to idle?
		if(t>=1.0f) idle();
	}
}
