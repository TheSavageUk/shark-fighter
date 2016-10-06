// Foam.cs - Foam animation
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;

// Foam class

public class Foam:MonoBehaviour
{
	// Attributes

	public float Speed=1.0f;
	public float Offset=0.0f;

	// Local data

	public MeshRenderer m_renderer;

	// Overrides

	// Start
	// Called at startup

	void Start()
	{
		m_renderer=GetComponent<MeshRenderer>();
	}

	// Update
	// Called once per frame

	void Update()
	{
		if(m_renderer==null) return;

		// fade in and out
		float t=(Time.time+Offset)*Speed;
		float s=(Mathf.Sin(t)+1.0f)*0.5f;

		Color c=m_renderer.material.color;
		c.a=Mathf.Lerp(0.0f,0.6f,s);

		m_renderer.material.color=c;

		if(s>0.01f) return;

		// change rotation
		Vector3 r;

		r.x=90.0f;
		r.y=Random.Range(0,360);
		r.z=0.0f;

		transform.localRotation=Quaternion.Euler(r);
	}
}
