// Punch.cs - Punch animation
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;

// Punch class

public class Punch:MonoBehaviour
{
	// Attributes

	public AudioSource[] SwingSounds=null;
	public AudioSource[] HitSounds=null;

	// Constants

	private static Vector3 PUNCH_STARTPOS=new Vector3(0.056f,-1.512f,-0.457f);
	private static Vector3 PUNCH_ENDPOS=new Vector3(0.137f,-1.512f,-0.15f);
	private static Vector3 PUNCH_RETRACTPOS=new Vector3(0.056f,-1.963f,-0.557f);

	// States

	private enum StateType
	{
		Idle,
		Punching,
		Retracting
	}

	// Local data

	private Transform m_arm;
	private Attack m_shark;

	private StateType m_state;
	private float m_time;

	private Vector3 m_p1;

	// Overrides

	// Start
	// Called at startup

	void Start()
	{
		// get arm
		m_arm=transform.Find("Head/Arm");

		// get shark
		GameObject o=GameObject.Find("/AttackShark/Shark");
		if(o==null) return;

		m_shark=o.GetComponent<Attack>();

		// initial state
		idle();
	}

	// Update
	// Called once per frame
	
	void Update()
	{
		// must have these
		if(m_arm==null ||
			m_shark==null)
			return;

		// what state?
		switch(m_state)
		{
			case StateType.Idle:
				// idle state
				break;

			case StateType.Punching:
				// punching state
				punching();
				break;

			case StateType.Retracting:
				// arm retracting
				retracting();
				break;
		}
	}

	// PunchTrigger
	// Call to start a punch

	public void PunchTrigger()
	{
		// must have these
		if(m_arm==null ||
			m_shark==null)
			return;

		// start a punch?
		if(m_state==StateType.Idle)
			punch();
	}

	// HitShark
	// Called if arm hits shark

	public void HitShark()
	{
		// must have these
		if(m_arm==null ||
			m_shark==null)
			return;

		// must be punching
		if(m_state!=StateType.Punching)
			return;

		// play effect
		if(HitSounds!=null)
		{
			int n=Random.Range(0,HitSounds.Length);
			if(HitSounds[n]!=null) HitSounds[n].Play();
		}

		// punch hit!
		m_shark.SharkHit();
		retract();
	}

	// Local functions

	// idle
	// Call to enter idle state

	private void idle()
	{
		m_arm.gameObject.SetActive(false);
		m_state=StateType.Idle;
	}

	// punch
	// Call to start a punch

	private void punch()
	{
		// initial position
		m_arm.localPosition=PUNCH_STARTPOS;
		m_arm.gameObject.SetActive(true);

		// play effect
		if(SwingSounds!=null)
		{
			int n=Random.Range(0,SwingSounds.Length);
			if(SwingSounds[n]!=null) SwingSounds[n].Play();
		}

		// start punching
		m_time=Time.time;
		m_state=StateType.Punching;
	}

	// punching
	// Called to animate punch

	private void punching()
	{
		float t=(Time.time-m_time)*2.5f;
		m_arm.localPosition=Vector3.Lerp(PUNCH_STARTPOS,PUNCH_ENDPOS,t);

		if(t>=1.0f) retract();
	}

	// retract
	// Call to retract arm

	private void retract()
	{
		m_p1=m_arm.localPosition;

		m_time=Time.time;
		m_state=StateType.Retracting;
	}

	// retracting
	// Called to animate rectracting

	private void retracting()
	{
		float t=(Time.time-m_time)*2.5f;
		m_arm.localPosition=Vector3.Lerp(m_p1,PUNCH_RETRACTPOS,t);

		if(t>=1.0f) idle();
	}
}
