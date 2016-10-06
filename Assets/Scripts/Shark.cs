// Shark.cs - Shark animation
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;

// Shark class

public class Shark:MonoBehaviour
{
	// Anim state types

	public enum StateType
	{
		Circle,
		Attack
	}

	// Attributes

	public StateType State=StateType.Circle;

	public float Speed=1.5f;
	public float Distance=2.0f;
	public float Depth=-0.41f;
	public float Direction=1f;
	public float Offset=0.0f;

	// Local data

	private Transform m_tail=null;

	// Overrides

	// Start
	// Called at startup

	void Start()
	{
		m_tail=transform.Find("Root/Spine01/Spine02/Spine03/Spine04/Spine05");
	}

	// Update
	// Called once per frame

	void Update()
	{
		if(m_tail==null) return;

		// what anim state?
		switch(State)
		{
			case StateType.Circle:
				// circle player
				circleAnim(Speed,Distance,Depth,Direction,Offset);
				tailAnim(Speed*40.0f);
				break;

			case StateType.Attack:
				// tail speed
				tailAnim(Speed*20.0f);
				break;
		}
	}

	// Local functions

	// circleAnim
	// Call to animate shark in a circle

	private void circleAnim(float speed,float distance,float depth,float direction,float offset)
	{
		Vector3 p;
		Vector3 r;

		float a=(offset*Mathf.Deg2Rad)+((Time.time*speed)*direction);

		p.x=distance*Mathf.Cos(a);
		p.y=depth;
		p.z=distance*Mathf.Sin(a);

		r.x=-4.4993f;
		r.y=360.0f-(a*Mathf.Rad2Deg)+(direction==1.0f?0.0f:180.0f);
		r.z=0.0f;

		transform.localPosition=p;
		transform.localRotation=Quaternion.Euler(r);
	}

	// tailAnim
	// Call to animate shark tail

	private void tailAnim(float speed)
	{
		Vector3 r=m_tail.transform.localRotation.eulerAngles;
		r.y=-180.0f+(Mathf.Sin(Time.time*speed)*20.0f);

		m_tail.transform.localRotation=Quaternion.Euler(r);	
	}
}
