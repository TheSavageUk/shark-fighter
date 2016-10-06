// Game.cs - Game logic
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Game class

public class Game:MonoBehaviour
{
	// Attributes

	public AudioSource TitleMusic=null;
	public AudioSource DeathMusic=null;

	public AudioSource[] HurtSounds=null;

	// Constants

	private const float FADE_IN=0.3f;
	private const float FADE_OUT=1.0f;
	private const float FADE_INSPEED=0.5f;
	private const float FADE_OUTSPEED=0.4f;

	private static Vector3 TITLE_STARTPOS=new Vector3(0.0f,3.59f,14.25f);
	private static Vector3 TITLE_STARTROT=new Vector3(0.0f,0.0f,0.0f);

	private static Vector3 TITLE_ENDPOS=new Vector3(0.0f,-16.0f,14.25f);
	private static Vector3 TITLE_ENDROT=new Vector3(0.0f,0.0f,90.0f);

	private static Vector3 SUBTITLE_STARTPOS=new Vector3(0.0f,0.173f,0.781f);
	private static Vector3 SUBTITLE_STARTROT=new Vector3(0.0f,0.0f,0.0f);

	private static Vector3 SUBTITLE_ENDPOS=new Vector3(0.0f,-0.7f,0.781f);
	private static Vector3 SUBTITLE_ENDROT=new Vector3(0.0f,0.0f,90.0f);

	private const float TITLE_SPEED=0.4f;

	private static Color FOAM_NORMAL=new Color(0.623f,0.623f,0.623f);
	private static Color FOAM_BLOODY=new Color(0.52f,0.0f,0.0f);

	// States

	private enum StateType
	{
		Idle,
		FadingIn,
		Waiting,
		Starting,
		Playing,
		FadingOut,
		Death
	}

	// Local data

	private VignetteAndChromaticAberration m_vignetteLeft;
	private VignetteAndChromaticAberration m_vignetteRight;

	private GameObject m_headline;
	private GameObject m_start;

	private Transform m_title;
	private Transform m_subtitle;

	private Bob m_titlebob;
	private Bob m_subtitlebob;

	private Material m_foamA;
	private Material m_foamB;

	private Punch m_punch;
	private Attack m_attack;

	private Vector3 m_p1;
	private Vector3 m_p2;

	private Vector3 m_r1;
	private Vector3 m_r2;

	private StateType m_state;

	private float m_time;
	private float m_health;

	// Overrides

	// Start
	// Called at startup

	void Start()
	{
		// get camera vignette shader
		GameObject cl=GameObject.Find("/Observer/Head/Camera/Camera Left");
		GameObject cr=GameObject.Find("/Observer/Head/Camera/Camera Right");

		if(cl==null || cr==null) return;

		m_vignetteLeft=cl.GetComponent<VignetteAndChromaticAberration>();
		m_vignetteRight=cr.GetComponent<VignetteAndChromaticAberration>();

		// get title and subtitle?
		m_title=transform.Find("Title");
		m_subtitle=transform.Find("Subtitle");

		if(m_title==null || m_subtitle==null) return;

		m_titlebob=m_title.GetComponent<Bob>();
		m_subtitlebob=m_subtitle.GetComponent<Bob>();

		m_headline=GameObject.Find("/Game/Headline");
		m_start=GameObject.Find("/Game/Start");

		// get foam materials
		GameObject oA=GameObject.Find("/Foam/FoamA");
		GameObject oB=GameObject.Find("/Foam/FoamB");

		if(oA==null || oB==null) return;

		MeshRenderer rA=oA.GetComponent<MeshRenderer>();
		MeshRenderer rB=oB.GetComponent<MeshRenderer>();

		if(rA==null || rB==null) return;

		m_foamA=rA.material;
		m_foamB=rB.material;

		// get player punch
		GameObject o=GameObject.Find("/Observer");
		if(o==null) return;

		m_punch=o.GetComponent<Punch>();

		// get attack shark
		GameObject a=GameObject.Find("/AttackShark/Shark");
		if(a==null) return;

		m_attack=a.GetComponent<Attack>();

		// initial state
		idle();
	}

	// Update
	// Called once per frame

	void Update()
	{
		// must have these
		if(m_vignetteLeft==null || m_vignetteRight==null ||
			m_headline==null || m_start==null ||
			m_title==null || m_titlebob==null ||
			m_subtitle==null || m_subtitlebob==null ||
			m_punch==null)
			return;

		// check for quit
		if(GvrViewer.Instance.VRModeEnabled &&
			GvrViewer.Instance.BackButtonPressed)
			Application.Quit();

		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();

		// what state?
		switch(m_state)
		{
			case StateType.Idle:
				// idle
				idling();
				break;

			case StateType.FadingIn:
				// fading scene in
				fadingIn();
				break;

			case StateType.Waiting:
				// waiting to start
				waiting();
				break;

			case StateType.Starting:
				// starting up
				starting();
				break;

			case StateType.Playing:
				// playing
				playing();
				break;

			case StateType.FadingOut:
				// fading scene out
				fadingOut();
				break;
		}
	}

	// Methods

	// IsPlaying
	// Called to check if game running

	public bool IsPlaying()
	{
		return (m_state==StateType.Playing);
	}

	// PlayerHit
	// Called when player bitten by shark

	public void PlayerHit()
	{
		// must have these
		if(m_vignetteLeft==null || m_vignetteRight==null ||
			m_headline==null || m_start==null ||
			m_title==null || m_titlebob==null ||
			m_subtitle==null || m_subtitlebob==null ||
			m_punch==null)
			return;

		// is playing?
		if(m_state!=StateType.Playing)
			return;

		// health goes down
		m_health-=0.2f;

		// play hurt sound
		if(HurtSounds!=null)
		{
			int n=Random.Range(0,HurtSounds.Length);
			HurtSounds[n].Play();
		}

		// colour foam red
		Color c=Color.Lerp(FOAM_BLOODY,FOAM_NORMAL,m_health);
		
		m_foamA.color=c;
		m_foamB.color=c;

		// player dead yet?
		if(m_health<0.0f)
		{
			// death music
			if(DeathMusic!=null)
				DeathMusic.Play();

			fadeOut();
		}
	}

	// NextShark
	// Call to get next shark time

	public float NextShark()
	{
		// time dependant on player health
		return Random.Range((int) (m_health*10.0f),(int) (m_health*30.0f));
	}

	// Local functions

	// idle
	// Call to enter idle state

	private void idle()
	{
		// black screen
		m_vignetteLeft.intensity=FADE_OUT;
		m_vignetteRight.intensity=FADE_OUT;

		// no music
		if(TitleMusic!=null)
			TitleMusic.pitch=0.0f;

		// set idle
		m_state=StateType.Idle;
	}

	// idling
	// Called when in idle state

	private void idling()
	{
		// gives time to startup
		m_state=StateType.FadingIn;
	}

	// fadeIn
	// Call to enter fade in state

	private void fadeIn()
	{
		// initial title positions
		m_title.localPosition=TITLE_STARTPOS;
		m_title.localRotation=Quaternion.Euler(TITLE_STARTROT);

		m_subtitle.localPosition=SUBTITLE_STARTPOS;
		m_subtitle.localRotation=Quaternion.Euler(SUBTITLE_STARTROT);

		m_titlebob.enabled=true;
		m_subtitlebob.enabled=true;

		m_title.gameObject.SetActive(true);
		m_subtitle.gameObject.SetActive(true);

		m_headline.SetActive(true);
		m_start.SetActive(true);

		// reset foam colour
		m_foamA.color=FOAM_NORMAL;
		m_foamB.color=FOAM_NORMAL;

		// reset attack shark
		m_attack.ResetShark();

		// fade scene in
		m_time=Time.time;
		m_state=StateType.FadingIn;
	}

	// fadingIn
	// Called when in fade in state

	private void fadingIn()
	{
		// vignette fade in over time
		float t=(Time.time-m_time)*FADE_INSPEED;
		float f=Mathf.Lerp(FADE_OUT,FADE_IN,t);

		m_vignetteLeft.intensity=f;
		m_vignetteRight.intensity=f;

		if(TitleMusic!=null)
			TitleMusic.pitch=Mathf.Clamp01(t);

		// goto next state?
		if(t>=1.0f) wait();
	}

	// wait
	// Call to enter waiting state

	private void wait()
	{
		// waiting for player
		m_state=StateType.Waiting;
	}

	// waiting
	// Called when in waiting state

	private void waiting()
	{
		// player hit start?
		if(GvrViewer.Instance.VRModeEnabled &&
			GvrViewer.Instance.Triggered)
			start();

		if(Input.GetKeyDown(KeyCode.Space))
			start();
	}

	// start
	// Call to enter start state

	private void start()
	{
		// get title pos
		m_p1=m_title.localPosition;
		m_r1=m_title.localRotation.eulerAngles;

		m_p2=m_subtitle.localPosition;
		m_r2=m_subtitle.localRotation.eulerAngles;

		m_headline.SetActive(false);
		m_start.SetActive(false);

		// stop title bobing
		m_titlebob.enabled=false;
		m_subtitlebob.enabled=false;

		// starting game
		m_time=Time.time;
		m_state=StateType.Starting;
	}

	// starting
	// Called when in start state

	private void starting()
	{
		// sink titles
		float t=(Time.time-m_time)*TITLE_SPEED;

		m_title.localPosition=Vector3.Lerp(m_p1,TITLE_ENDPOS,t);
		m_title.localRotation=Quaternion.Euler(Vector3.Lerp(m_r1,TITLE_ENDROT,t));

		m_subtitle.localPosition=Vector3.Lerp(m_p2,SUBTITLE_ENDPOS,t);
		m_subtitle.localRotation=Quaternion.Euler(Vector3.Lerp(m_r2,SUBTITLE_ENDROT,t));

		if(TitleMusic!=null)
			TitleMusic.pitch=Mathf.Clamp01(1.0f-t);

		// goto next state?
		if(t>=1.0f) play();
	}

	// play
	// Call to enter play state

	private void play()
	{
		// remove titles
		m_title.gameObject.SetActive(false);
		m_subtitle.gameObject.SetActive(false);

		// initial state
		m_health=1.0f;
		m_state=StateType.Playing;
	}

	// playing
	// Called when in playing state

	private void playing()
	{
		// player pressed punch?
		if(GvrViewer.Instance.VRModeEnabled &&
			GvrViewer.Instance.Triggered)
			m_punch.PunchTrigger();

		if(Input.GetKeyDown(KeyCode.Space))
			m_punch.PunchTrigger();
	}

	// fadeOut
	// Call to enter fade out state

	private void fadeOut()
	{
		// set initial intensity
		m_vignetteLeft.intensity=FADE_IN;
		m_vignetteRight.intensity=FADE_IN;

		// fade scene out
		m_time=Time.time;
		m_state=StateType.FadingOut;
	}

	// fadingOut
	// Called when in fade out state

	private void fadingOut()
	{
		// vignette fade out over time
		float t=(Time.time-m_time)*FADE_OUTSPEED;
		float f=Mathf.Lerp(FADE_IN,FADE_OUT,t);

		m_vignetteLeft.intensity=f;
		m_vignetteRight.intensity=f;

		// goto next state?
		if(t>=1.0f) fadeIn();
	}
}
