// Observer.cs - Observer component
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;

// Namespace

namespace SavageSoftware.Camera
{
	// Observer class
	
	[AddComponentMenu("Camera/Observer")]
	[DisallowMultipleComponent]
	
	public class Observer:MonoBehaviour
	{
		// Public constants

		public const string NAME_OBSERVER="Observer";
		public const string NAME_CAMERA="Head/Camera";

		// Observer mode

		public enum ObserverType
		{
			Fixed,
			Orbital,
			Spacial,
			FirstPerson,
			ThirdPerson
		}

		// Public properties

		public ObserverType Mode {get {return _Mode;} set {_Mode=value; Validate();}}

		public GameObject OrbitObject {get {return _OrbitObject;} set {_OrbitObject=value; Validate();}}

		public bool DistanceClamp {get {return _DistanceClamp;} set {_DistanceClamp=value; Validate();}}
		public float DistanceMinimum {get {return _DistanceMinimum;} set {_DistanceMinimum=value; Validate();}}
		public float DistanceMaximum {get {return _DistanceMaximum;} set {_DistanceMaximum=value; Validate();}}

		public float FirstPersonHeight {get {return _FirstPersonHeight;} set {_FirstPersonHeight=value; Validate();}}
		public float FirstPersonHeadroom {get {return _FirstPersonHeadroom;} set {_FirstPersonHeadroom=value; Validate();}}
		public float FirstPersonStep {get {return _FirstPersonStep;} set {_FirstPersonStep=value; Validate();}}

		public bool PositionClampX {get {return _PositionClampX;} set {_PositionClampX=value; Validate();}}
		public bool PositionClampY {get {return _PositionClampY;} set {_PositionClampY=value; Validate();}}
		public bool PositionClampZ {get {return _PositionClampZ;} set {_PositionClampZ=value; Validate();}}
		public Vector3 PositionMinimum {get {return _PositionMinimum;} set {_PositionMinimum=value; Validate();}}
		public Vector3 PositionMaximum {get {return _PositionMaximum;} set {_PositionMaximum=value; Validate();}}

		public bool RotationClampX {get {return _RotationClampX;} set {_RotationClampX=value; Validate();}}
		public bool RotationClampY {get {return _RotationClampY;} set {_RotationClampY=value; Validate();}}
		public bool RotationClampZ {get {return _RotationClampZ;} set {_RotationClampZ=value; Validate();}}
		public Vector3 RotationMinimum {get {return _RotationMinimum;} set {_RotationMinimum=value; Validate();}}
		public Vector3 RotationMaximum {get {return _RotationMaximum;} set {_RotationMaximum=value; Validate();}}

		public bool SmoothPosition {get {return _SmoothPosition;} set {_SmoothPosition=value; Validate();}}
		public bool SmoothRotation {get {return _SmoothRotation;} set {_SmoothRotation=value; Validate();}}
		public float SmoothTime {get {return _SmoothTime;} set {_SmoothTime=value; Validate();}}

		public bool ShowCursor {get {return _ShowCursor;} set {_ShowCursor=value; Validate();}}
		public bool LockCursor {get {return _LockCursor;} set {_LockCursor=value; Validate();}}

		public bool InputEnable {get {return _InputEnable;} set {_InputEnable=value; Validate();}}
		public Vector3 InputPositionScale {get {return _InputPositionScale;} set {_InputPositionScale=value; Validate();}}
		public Vector3 InputRotationScale {get {return _InputRotationScale;} set {_InputRotationScale=value; Validate();}}

		// Serialized data

		[SerializeField] private ObserverType _Mode=ObserverType.Spacial;

		[SerializeField] private GameObject _OrbitObject=null;

		[SerializeField] private bool _DistanceClamp=false;
		[SerializeField] private float _DistanceMinimum=1.0f;
		[SerializeField] private float _DistanceMaximum=10.0f;

		[SerializeField] private float _FirstPersonHeight=0.45f;
		[SerializeField] private float _FirstPersonHeadroom=0.05f;
		[SerializeField] private float _FirstPersonStep=0.16f;

		[SerializeField] private bool _PositionClampX=false;
		[SerializeField] private bool _PositionClampY=false;
		[SerializeField] private bool _PositionClampZ=false;
		[SerializeField] private Vector3 _PositionMinimum=new Vector3(-10.0f,-10.0f,-10.0f);
		[SerializeField] private Vector3 _PositionMaximum=new Vector3(10.0f,10.0f,10.0f);

		[SerializeField] private bool _RotationClampX=true;
		[SerializeField] private bool _RotationClampY=false;
		[SerializeField] private bool _RotationClampZ=false;
		[SerializeField] private Vector3 _RotationMinimum=new Vector3(-90.0f,-90.0f,-90.0f);
		[SerializeField] private Vector3 _RotationMaximum=new Vector3(90.0f,90.0f,90.0f);

		[SerializeField] private bool _SmoothPosition=false;
		[SerializeField] private bool _SmoothRotation=false;
		[SerializeField] private float _SmoothTime=5.0f;

		[SerializeField] private bool _ShowCursor=false;
		[SerializeField] private bool _LockCursor=true;

		[SerializeField] private bool _InputEnable=true;
		[SerializeField] private Vector3 _InputPositionScale=new Vector3(0.01f,0.01f,0.01f);
		[SerializeField] private Vector3 _InputRotationScale=new Vector3(2.0f,2.0f,2.0f);

		// Local constants

		const string AXIS_MOUSEX="Mouse X";
		const string AXIS_MOUSEY="Mouse Y";
		const string AXIS_MOUSEWHEEL="Mouse ScrollWheel";

		const float POSITION_NORMAL=1.0f;
		const float POSITION_FINE=0.1f;

		const float WHEEL_FACTOR=10.0f;

		// Local data

		private Transform m_observerTransform;
		private Transform m_cameraTransform;

		private float m_observerDistance;
		private Vector3 m_observerPosition;

		private Quaternion m_observerRotation;
		private Quaternion m_cameraRotation;

		// Public overrides

		// Reset
		// Called when editor reset selected

		void Reset()
		{
			Validate();
		}

		// Start
		// Called when component is created

		void Start()
		{
			// remove and lock cursor?
			Cursor.visible=_ShowCursor;

			Cursor.lockState=_LockCursor?
				CursorLockMode.Locked:
				CursorLockMode.Confined;

			// get camera?
			Transform camera=transform.Find(NAME_CAMERA);
			if(camera==null) return;

			// remove mouse events
			UnityEngine.Camera c=camera.GetComponent<UnityEngine.Camera>();
			c.eventMask=0;

			// get transforms
			m_observerTransform=transform;
			m_cameraTransform=camera;

			// starting transform
			m_observerPosition=m_observerTransform.localPosition;

			m_observerRotation=m_observerTransform.localRotation;
			m_cameraRotation=m_cameraTransform.localRotation;

			// orbital mode?
			if(_Mode==ObserverType.Orbital)
				cameraDistance((transform.position-getOrbitTarget()).magnitude);			
		}

		// Update
		// Called when object updated

		void Update()
		{
			// get controller?
//			Menu menu=Menu.Controller;

			// escape for menu?
//			if(menu!=null && !menu.Visible &&
//				Input.GetKeyUp(KeyCode.Escape))
//				menu.ShowMenu();

			// has user input?
			if(_InputEnable)// &&
//				(menu==null || !menu.Visible) &&
//				Tracking.GetTracking()==null)
			{
				// input position and roation
				inputPosition();

//				if(Input.GetMouseButton(1) ||
//					Input.GetKey(KeyCode.LeftShift))
//				{
					rotationMode(true);
					inputRotation();
//				}
//				else rotationMode(false);
			}

			// update position and roation
			updatePosition();
			updateRotation();
		}

		// Public methods

		// Validate
		// Call to validate properties for this component

		public void Validate()
		{
		}

		// Local functions

		//
		//

		private void rotationMode(bool enable)
		{
			Cursor.visible=enable?false:_ShowCursor;

//			Cursor.lockState=enable?
//				CursorLockMode.Locked:
//				CursorLockMode.Confined;
		}

		// updatePosition
		// Call to update current position

		private void updatePosition()
		{
			// in fixed mode?
			if(_Mode==ObserverType.Fixed) return;

			// must have transform
			if(m_observerTransform==null)
				return;
		
			// smooth out translations?
			if(_SmoothPosition)
			{
				// lerp over time
				float t=_SmoothTime*Time.deltaTime;

				m_observerTransform.localPosition=Vector3.Lerp(
					m_observerTransform.localPosition,m_observerPosition,t);

				return;
			}
		
			// set new position
			m_observerTransform.localPosition=m_observerPosition;
		}

		// updateRotation
		// Call to update current rotation

		private void updateRotation()
		{
			// must have transforms
			if(m_observerTransform==null || m_cameraTransform==null)
				return;

			// smooth out rotations?
			if(_SmoothRotation)
			{
				// slerp over time
				float t=_SmoothTime*Time.deltaTime;

				m_observerTransform.localRotation=Quaternion.Slerp(
					m_observerTransform.localRotation,m_observerRotation,t);

				m_cameraTransform.localRotation=Quaternion.Slerp(
					m_cameraTransform.localRotation,m_cameraRotation,t);

				return;
			}
		
			// set new rotation
			m_observerTransform.localRotation=m_observerRotation;
			m_cameraTransform.localRotation=m_cameraRotation;
		}

		// inputPosition
		// Call to update position with user input

		private void inputPosition()
		{
			// in fixed mode?
			if(_Mode==ObserverType.Fixed) return;

			// what input?
			bool shift=false;//Input.GetKey(KeyCode.LeftShift);

			bool up=Input.GetKey(KeyCode.UpArrow);
			bool down=Input.GetKey(KeyCode.DownArrow);
			bool left=Input.GetKey(KeyCode.LeftArrow);
			bool right=Input.GetKey(KeyCode.RightArrow);

			float wheel=Input.GetAxis(AXIS_MOUSEWHEEL)*WHEEL_FACTOR;

			// get input movememnt
			float scale=shift?POSITION_FINE:POSITION_NORMAL;

			float x=(right?1:(left?-1:0))*scale;
			float y=(up?1:(down?-1:0))*scale;
			float z=wheel*scale;

			// what mode?
			switch(_Mode)
			{
				case ObserverType.Orbital:
					// orbital mode
					cameraDistance(-z*_InputPositionScale.z);

					// set orbit rotation
					cameraOrbit(
						-y*_InputRotationScale.x,
						-x*_InputRotationScale.y);
					return;

				case ObserverType.FirstPerson:
					// first person mode
					z=y;
					y=0;
					break;
			}

			// face front
			if(up || down || left || right)
			{
				m_observerRotation=Quaternion.Euler(0.0f,0.0f,0.0f);
				clampRotation();
			}

			// set new position
			cameraTranslate(
				x*_InputPositionScale.x,
				y*_InputPositionScale.y,
				z*_InputPositionScale.z);
		}

		// inputRotation
		// Call to update rotation with user input
	
		private void inputRotation()
		{
			// what input?
			bool shift=false;//Input.GetKey(KeyCode.LeftShift);

			float mousex=Input.GetAxis(AXIS_MOUSEX);
			float mousey=Input.GetAxis(AXIS_MOUSEY);

			// get input rotation
			float scale=shift?POSITION_FINE:POSITION_NORMAL;

			float x=mousey*scale;
			float y=mousex*scale;

			// what mode?
			switch(_Mode)
			{
				case ObserverType.Orbital:
					// orbital mode
					cameraOrbit(
						x*_InputRotationScale.y,
						y*_InputRotationScale.x);
					return;
			}

			// set new rotation
			cameraRotate(
				x*_InputRotationScale.y,
				y*_InputRotationScale.x);
		}

		// cameraDistance
		// Call to set relative camera distance

		void cameraDistance(float d)
		{
			// set new distance
			m_observerDistance+=d;

			// clamp distance
			clampDistance();

			// re-align to object
			orbitPosition();
			orbitRotation();
		}

		// cameraTranslate
		// Call to set relative camera position

		void cameraTranslate(float x,float y,float z)
		{
			Vector3 r=m_observerTransform.right*x;
			Vector3 u=m_observerTransform.up*y;
			Vector3 f=m_observerTransform.forward*z;

			Vector3 p=m_observerPosition;

			// first person mode?
			if(_Mode==ObserverType.FirstPerson)
			{
				// get new position
				Vector3 forward=f+r;
				p+=forward;

				// raycast into scene
				RaycastHit info1;
				RaycastHit info2;
				RaycastHit info3;
				RaycastHit info4;

				Vector3 p1=p;
				Vector3 p2=p;
				Vector3 p3=p;
				Vector3 p4=p;

				p1.y+=_FirstPersonHeadroom;
				p3.y+=_FirstPersonHeight-_FirstPersonStep;
				p2.y+=(p1.y-p3.y)*0.5f;

				// hit something?
				int layer=1<<8;

				bool hit1=Physics.Raycast(p1,forward,out info1,1.0f,layer);
				bool hit2=Physics.Raycast(p2,forward,out info2,1.0f,layer);
				bool hit3=Physics.Raycast(p3,forward,out info3,1.0f,layer);
				bool hit4=Physics.Raycast(p4,Vector3.down,out info4,1.0f,layer);

				bool collide1=(hit1 && info1.distance<0.1f);
				bool collide2=(hit2 && info2.distance<0.1f);
				bool collide3=(hit3 && info3.distance<0.1f);
				bool collide4=(!hit4 || info4.distance>(_FirstPersonHeight+_FirstPersonStep)*2.0f);

//				if(hit1) Debug.DrawLine(p1,info1.point,collide1?Color.red:Color.blue);
//				if(hit2) Debug.DrawLine(p2,info2.point,collide2?Color.red:Color.blue);
//				if(hit3) Debug.DrawLine(p3,info3.point,collide3?Color.red:Color.blue);
//				if(hit4) Debug.DrawLine(p4,info4.point,collide4?Color.red:Color.green);

				if(collide1 || collide2 || collide3 || collide4)
					return;

				// set player height
				p.y=info4.point.y+_FirstPersonHeight;
			}
			else p+=f+u+r;

			// set new position
			m_observerPosition=p;

			// clamp position
			clampPosition();
		}

		// cameraRotate
		// Call to set relative camera rotation

		void cameraRotate(float x,float y,float z=0.0f)
		{
			// set new rotation
			m_observerRotation*=Quaternion.Euler(0.0f,y,0.0f);
			m_cameraRotation*=Quaternion.Euler(-x,0.0f,0.0f);

			// clamp rotation
			clampRotation();
		}

		// CameraOrbit
		// Call to set relative camera orbit rotation

		void cameraOrbit(float x,float y,float z=0.0f)
		{
			Vector3 camera=m_observerPosition;
			Vector3 target=getOrbitTarget();

			Quaternion q=Quaternion.LookRotation(camera-target,Vector3.up);

			// set new rotation
			Vector3 v=q.eulerAngles;

			v.x+=x;
			v.y+=y;

			q.eulerAngles=v;

			// set new position
			Vector3 p=target+(q*Vector3.forward*m_observerDistance);
			m_observerPosition=p;

			// align new rotation
			orbitRotation();
		}

		// orbitPosition
		// Call to set orbit mode camera position

		private void orbitPosition()
		{
			Vector3 camera=m_observerPosition;
			Vector3 target=getOrbitTarget();

			Quaternion q=Quaternion.LookRotation(camera-target,Vector3.up);
			Vector3 p=target+(q*Vector3.forward*m_observerDistance);

			m_observerPosition=p;
		}

		// orbitRotation
		// Call to set orbit mode camera rotation

		private void orbitRotation()
		{
			Vector3 camera=m_observerPosition;
			Vector3 target=getOrbitTarget();

			Quaternion q=Quaternion.LookRotation(target-camera,Vector3.up);
			Vector3 e=q.eulerAngles;

			Vector3 ce=new Vector3(e.x,0.0f,0.0f);
			Vector3 oe=new Vector3(0.0f,e.y,0.0f);

			m_cameraRotation.eulerAngles=ce;
			m_observerRotation.eulerAngles=oe;

			clampRotation();
		}

		// clampDistance
		// Call to clamp distance

		private void clampDistance()
		{
			// clamp distance
			if(_DistanceClamp) m_observerDistance=Mathf.Clamp(m_observerDistance,_DistanceMinimum,_DistanceMaximum);
		}

		// clampPosition
		// Call to clamp position

		private void clampPosition()
		{
			// clamp position
			if(_PositionClampX) m_observerPosition.x=Mathf.Clamp(m_observerPosition.x,_PositionMinimum.x,_PositionMaximum.x);
			if(_PositionClampY) m_observerPosition.y=Mathf.Clamp(m_observerPosition.y,_PositionMinimum.y,_PositionMaximum.y);
			if(_PositionClampZ) m_observerPosition.z=Mathf.Clamp(m_observerPosition.z,_PositionMinimum.z,_PositionMaximum.z);
		}

		// clampRotation
		// Call to clamp rotation

		private void clampRotation()
		{
			// clamp rotation
			Vector3 ce=m_cameraRotation.eulerAngles;
			Vector3 oe=m_observerRotation.eulerAngles;

			ce.y=0.0f;
			ce.z=0.0f;

			oe.x=0.0f;
			oe.z=0.0f;

			if(_RotationClampX) ce.x=remap(Mathf.Clamp(remap(ce.x,-180.0f,180.0f),_RotationMinimum.x,_RotationMaximum.x),0.0f,360.0f);
			if(_RotationClampY) oe.y=remap(Mathf.Clamp(remap(oe.y,-180.0f,180.0f),_RotationMinimum.y,_RotationMaximum.y),0.0f,360.0f);

			m_cameraRotation.eulerAngles=ce;
			m_observerRotation.eulerAngles=oe;
		}

		// getOrbitTarget
		// Call to get orbit target world position

		private Vector3 getOrbitTarget()
		{
			if(_OrbitObject==null) return Vector3.zero;
			return _OrbitObject.transform.position;
		}

		// remap
		// Call to re-map a value range

		private float remap(float value,float start,float end)
		{
			float width=end-start;
			float offset=value-start;

			return(offset-(Mathf.Floor(offset/width)*width))+start;
		}
	}
}

