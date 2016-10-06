// Sea.cs - Sea animation
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;

// Sea class

public class Sea:MonoBehaviour
{
	// Attributes

	public float Speed=0.1f;
	public float Intensity=0.01f;

	// Local data

	private MeshRenderer m_renderer=null;

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

		Vector2 v;

		v.x=Mathf.Sin(Time.time*Speed)*Intensity;
		v.y=Mathf.Cos(Time.time*Speed*2.0f)*Intensity;

		m_renderer.sharedMaterial.SetTextureOffset("_MainTex",v);
	}
}
