// Bob.cs - Bob animation
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;

// Bob class

public class Bob:MonoBehaviour
{
	// Attributes

	public float BobHeight=0.1f;
	public float BobSpeed=1.5f;
	public float BobStrength=0.01f;

	public float RockSpeedX=1.2f;
	public float RockStrengthX=0.3f;

	public float RockSpeedZ=1.6f;
	public float RockStrengthZ=0.3f;

	// Overrides

	// Start
	// Called at startup

	void Start()
	{
	}

	// Update
	// Called once per frame

	void Update()
	{
		// has bob enabled?
		if(BobStrength>0.0f)
		{
			// bob in y
			Vector3 p=transform.localPosition;
		
			p.y=BobHeight+Mathf.Sin(Time.time*BobSpeed)*BobStrength;
			transform.localPosition=p;
		}

		// has rock enabled?
		if(RockStrengthX>0.0f || RockStrengthZ>0.0f)
		{
			// rock in x and z
			Vector3 r=transform.localRotation.eulerAngles;

			r.x=Mathf.Sin(Time.time*RockSpeedX)*RockStrengthX;
			r.z=Mathf.Sin(Time.time*RockSpeedZ)*RockStrengthZ;

			transform.localRotation=Quaternion.Euler(r);
		}
	}
}
