// Arm.cs - Arm collision
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;

// Arm class

public class Arm:MonoBehaviour
{
	// Local data

	private Punch m_punch;

	// Overrides

	// Start
	// Called at startup

	void Start()
	{
		// get punch controller
		GameObject o=GameObject.Find("/Observer");
		if(o==null) return;

		m_punch=o.GetComponent<Punch>();
	}

	// OnTriggerEnter
	// Called when collision detected

	void OnTriggerEnter(Collider other)
	{
		if(m_punch==null) return;

		// arm hit shark?
		if(other.gameObject.name=="GreatWhite")
			m_punch.HitShark();
	}
}
