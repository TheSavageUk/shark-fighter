// ObserverEditor.cs - Observer custom editor script
// Copyright (c) 2016, Savage Software

// Includes

using UnityEngine;
using UnityEditor;

// Namespace

namespace SavageSoftware.Camera.Editors
{
	// ClonerEditor class

	[CustomEditor(typeof(Observer))]
	[SelectionBase]
	[CanEditMultipleObjects]

	public class ObserverEditor:Editor
	{
		// Property names

		private const string PROP_MODE="_Mode";

		private const string PROP_ORBITOBJECT="_OrbitObject";

		private const string PROP_DISTANCECLAMP="_DistanceClamp";
		private const string PROP_DISTANCEMINIMUM="_DistanceMinimum";
		private const string PROP_DISTANCEMAXIMUM="_DistanceMaximum";

		private const string PROP_FIRSTPERSONHEIGHT="_FirstPersonHeight";
		private const string PROP_FIRSTPERSONHEADROOM="_FirstPersonHeadroom";
		private const string PROP_FIRSTPERSONSTEP="_FirstPersonStep";

		private const string PROP_POSITIONCLAMPX="_PositionClampX";
		private const string PROP_POSITIONCLAMPY="_PositionClampY";
		private const string PROP_POSITIONCLAMPZ="_PositionClampZ";
		private const string PROP_POSITIONMINIMUM="_PositionMinimum";
		private const string PROP_POSITIONMAXIMUM="_PositionMaximum";

		private const string PROP_ROTATIONCLAMPX="_RotationClampX";
		private const string PROP_ROTATIONCLAMPY="_RotationClampY";
		private const string PROP_ROTATIONCLAMPZ="_RotationClampZ";
		private const string PROP_ROTATIONMINIMUM="_RotationMinimum";
		private const string PROP_ROTATIONMAXIMUM="_RotationMaximum";

		private const string PROP_SMOOTHPOSITION="_SmoothPosition";
		private const string PROP_SMOOTHROTATION="_SmoothRotation";
		private const string PROP_SMOOTHTIME="_SmoothTime";

		private const string PROP_SHOWCURSOR="_ShowCursor";
		private const string PROP_LOCKCURSOR="_LockCursor";

		private const string PROP_INPUTENABLE="_InputEnable";
		private const string PROP_INPUTPOSITIONSCALE="_InputPositionScale";
		private const string PROP_INPUTROTATIONSCALE="_InputRotationScale";

		// Control labels

		private static GUIContent CONTENT_MODE=new GUIContent("Mode","Observer mode");

		private static GUIContent CONTENT_ORBITOBJECT=new GUIContent("Orbit Object","Object to orbit");

		private static GUIContent CONTENT_FIRSTPERSONHEIGHT=new GUIContent("Height","Eye height");
		private static GUIContent CONTENT_FIRSTPERSONHEADROOM=new GUIContent("Headroom","Headroom height");
		private static GUIContent CONTENT_FIRSTPERSONSTEP=new GUIContent("Step","Maximum step up height");

		private static GUIContent CONTENT_DISTANCECLAMP=new GUIContent("Clamp Distance","Clamp observer distance");
		private static GUIContent CONTENT_POSITIONCLAMP=new GUIContent("Clamp Position","Clamp observer position");
		private static GUIContent CONTENT_ROTATIONCLAMP=new GUIContent("Clamp Rotation","Clamp observer rotation");
		private static GUIContent CONTENT_MINIMUMCLAMP=new GUIContent("Minimum","Minimum clamp value");
		private static GUIContent CONTENT_MAXIMUMCLAMP=new GUIContent("Maximum","Maximum clamp value");

		private static GUIContent CONTENT_SMOOTHPOSITION=new GUIContent("Smooth Position","Smooth changes in observer position");
		private static GUIContent CONTENT_SMOOTHROTATION=new GUIContent("Smooth Rotation","Smooth changes in observer rotation");
		private static GUIContent CONTENT_SMOOTHTIME=new GUIContent("Smooth Time","Smooth time value");

		private static GUIContent CONTENT_SHOWCURSOR=new GUIContent("Show Cursor","Show mouse cursor");
		private static GUIContent CONTENT_LOCKCURSOR=new GUIContent("Lock Cursor","Lock mouse cursor");

		private static GUIContent CONTENT_INPUTENABLE=new GUIContent("User Input","Enable user input");
		private static GUIContent CONTENT_INPUTPOSITIONSCALE=new GUIContent("Position Scale","Scaling for user position input");
		private static GUIContent CONTENT_INPUTROTATIONSCALE=new GUIContent("Rotation Scale","Scaling for user rotation input");

		private static string CONTENT_X="X";
		private static string CONTENT_Y="Y";
		private static string CONTENT_Z="Z";

		// Property bindings

		private SerializedProperty m_propMode;

		private SerializedProperty m_propOrbitObject;

		private SerializedProperty m_propDistanceClamp;
		private SerializedProperty m_propDistanceMinimum;
		private SerializedProperty m_propDistanceMaximum;

		private SerializedProperty m_propFirstPersonHeight;
		private SerializedProperty m_propFirstPersonHeadroom;
		private SerializedProperty m_propFirstPersonStep;

		private SerializedProperty m_propPositionClampX;
		private SerializedProperty m_propPositionClampY;
		private SerializedProperty m_propPositionClampZ;
		private SerializedProperty m_propPositionMinimum;
		private SerializedProperty m_propPositionMaximum;

		private SerializedProperty m_propRotationClampX;
		private SerializedProperty m_propRotationClampY;
		private SerializedProperty m_propRotationClampZ;
		private SerializedProperty m_propRotationMinimum;
		private SerializedProperty m_propRotationMaximum;

		private SerializedProperty m_propSmoothPosition;
		private SerializedProperty m_propSmoothRotation;
		private SerializedProperty m_propSmoothTime;

		private SerializedProperty m_propShowCursor;
		private SerializedProperty m_propLockCursor;

		private SerializedProperty m_propInputEnable;
		private SerializedProperty m_propInputPositionScale;
		private SerializedProperty m_propInputRotationScale;

		// Public members

		// CreateGameObject
		// Call to create a new observer object

		[MenuItem("GameObject/3D Object/Observer")]

		public static void CreateGameObject()
		{
			// create new observer
			GameObject o=new GameObject(Observer.NAME_OBSERVER);
			GameObject c=new GameObject(Observer.NAME_CAMERA);

			o.AddComponent<Observer>();
			c.AddComponent<UnityEngine.Camera>();

			c.transform.SetParent(o.transform);
		}

		// OnEnable
		// Called when editor enabled

		void OnEnable()
		{
			// bind to properties
			m_propMode=serializedObject.FindProperty(PROP_MODE);

			m_propOrbitObject=serializedObject.FindProperty(PROP_ORBITOBJECT);

			m_propDistanceClamp=serializedObject.FindProperty(PROP_DISTANCECLAMP);
			m_propDistanceMinimum=serializedObject.FindProperty(PROP_DISTANCEMINIMUM);
			m_propDistanceMaximum=serializedObject.FindProperty(PROP_DISTANCEMAXIMUM);

			m_propFirstPersonHeight=serializedObject.FindProperty(PROP_FIRSTPERSONHEIGHT);
			m_propFirstPersonHeadroom=serializedObject.FindProperty(PROP_FIRSTPERSONHEADROOM);
			m_propFirstPersonStep=serializedObject.FindProperty(PROP_FIRSTPERSONSTEP);

			m_propPositionClampX=serializedObject.FindProperty(PROP_POSITIONCLAMPX);
			m_propPositionClampY=serializedObject.FindProperty(PROP_POSITIONCLAMPY);
			m_propPositionClampZ=serializedObject.FindProperty(PROP_POSITIONCLAMPZ);
			m_propPositionMinimum=serializedObject.FindProperty(PROP_POSITIONMINIMUM);
			m_propPositionMaximum=serializedObject.FindProperty(PROP_POSITIONMAXIMUM);

			m_propRotationClampX=serializedObject.FindProperty(PROP_ROTATIONCLAMPX);
			m_propRotationClampY=serializedObject.FindProperty(PROP_ROTATIONCLAMPY);
			m_propRotationClampZ=serializedObject.FindProperty(PROP_ROTATIONCLAMPZ);
			m_propRotationMinimum=serializedObject.FindProperty(PROP_ROTATIONMINIMUM);
			m_propRotationMaximum=serializedObject.FindProperty(PROP_ROTATIONMAXIMUM);

			m_propSmoothPosition=serializedObject.FindProperty(PROP_SMOOTHPOSITION);
			m_propSmoothRotation=serializedObject.FindProperty(PROP_SMOOTHROTATION);
			m_propSmoothTime=serializedObject.FindProperty(PROP_SMOOTHTIME);

			m_propShowCursor=serializedObject.FindProperty(PROP_SHOWCURSOR);
			m_propLockCursor=serializedObject.FindProperty(PROP_LOCKCURSOR);

			m_propInputEnable=serializedObject.FindProperty(PROP_INPUTENABLE);
			m_propInputPositionScale=serializedObject.FindProperty(PROP_INPUTPOSITIONSCALE);
			m_propInputRotationScale=serializedObject.FindProperty(PROP_INPUTROTATIONSCALE);
		}

		// OnInspectorGUI
		// Called to draw editor controls

		public override void OnInspectorGUI()
		{
			// update serialize
			serializedObject.Update();

			// output cloner controls
			editObserver();

			// apply changes
			serializedObject.ApplyModifiedProperties();
		}

		// Local functions

		// editObserver
		// Call to output observer controls

		private void editObserver()
		{
			// output mode
			EditorGUILayout.PropertyField(m_propMode,CONTENT_MODE);
			EditorGUILayout.Space();

			// what type?
			switch((Observer.ObserverType) m_propMode.enumValueIndex)
			{
				case Observer.ObserverType.Fixed:
					// fixed mode
					editRotationClamp();
					editSmoothing(false);
					editInput(false);
					break;

				case Observer.ObserverType.Orbital:
					// orbital mode
					editOrbital();
					editRotationClamp();
					editSmoothing(true);
					editInput(false);
					break;

				case Observer.ObserverType.Spacial:
					// spacial mode
					editPositionClamp();
					editRotationClamp();
					editSmoothing(true);
					editInput(true);
					break;

				case Observer.ObserverType.FirstPerson:
					// first person mode
					editFirstPerson();
					editPositionClamp();
					editRotationClamp();
					editSmoothing(true);
					editInput(true);
					break;

				case Observer.ObserverType.ThirdPerson:
					// third person mode
					editOrbital();
					editRotationClamp();
					editSmoothing(true);
					editInput(false);
					break;
			}
		}

		// editOrbital
		// Call to output orbital mode controls

		private void editOrbital()
		{
			// output orbital object
			EditorGUILayout.PropertyField(m_propOrbitObject,CONTENT_ORBITOBJECT);
			EditorGUILayout.Space();

			// output distance clamping
			EditorGUILayout.PropertyField(m_propDistanceClamp,CONTENT_DISTANCECLAMP);

			if(m_propDistanceClamp.boolValue)
			{
				EditorGUILayout.PropertyField(m_propDistanceMinimum,CONTENT_MINIMUMCLAMP);
				EditorGUILayout.PropertyField(m_propDistanceMaximum,CONTENT_MAXIMUMCLAMP);
			}
			EditorGUILayout.Space();
		}

		// editFirstPerson
		// Call to output first person controls

		private void editFirstPerson()
		{
			// output first person controls
			EditorGUILayout.PropertyField(m_propFirstPersonHeight,CONTENT_FIRSTPERSONHEIGHT);
			EditorGUILayout.PropertyField(m_propFirstPersonHeadroom,CONTENT_FIRSTPERSONHEADROOM);
			EditorGUILayout.PropertyField(m_propFirstPersonStep,CONTENT_FIRSTPERSONSTEP);

			EditorGUILayout.Space();
		}

		// editPositionClamp
		// Call to output position clamp controls

		private void editPositionClamp()
		{
			// get values
			Vector3 minimum=m_propPositionMinimum.vector3Value;
			Vector3 maximum=m_propPositionMaximum.vector3Value;

			bool enableX=m_propPositionClampX.boolValue;
			bool enableY=m_propPositionClampY.boolValue;
			bool enableZ=m_propPositionClampZ.boolValue;

			// edit values
			editMinMax(
				CONTENT_POSITIONCLAMP,
				ref enableX,
				ref enableY,
				ref enableZ,
				ref minimum,
				ref maximum);

			// set new values
			m_propPositionClampX.boolValue=enableX;
			m_propPositionClampY.boolValue=enableY;
			m_propPositionClampZ.boolValue=enableZ;

			m_propPositionMinimum.vector3Value=minimum;
			m_propPositionMaximum.vector3Value=maximum;

			EditorGUILayout.Space();
		}

		// editRotationClamp
		// Call to output rotation clamp controls

		private void editRotationClamp()
		{
			// get values
			Vector3 minimum=m_propRotationMinimum.vector3Value;
			Vector3 maximum=m_propRotationMaximum.vector3Value;

			bool enableX=m_propRotationClampX.boolValue;
			bool enableY=m_propRotationClampY.boolValue;
			bool enableZ=m_propRotationClampZ.boolValue;

			// edit values
			editMinMax(
				CONTENT_ROTATIONCLAMP,
				ref enableX,
				ref enableY,
				ref enableZ,
				ref minimum,
				ref maximum);

			// set new values
			m_propRotationClampX.boolValue=enableX;
			m_propRotationClampY.boolValue=enableY;
			m_propRotationClampZ.boolValue=enableZ;

			m_propRotationMinimum.vector3Value=minimum;
			m_propRotationMaximum.vector3Value=maximum;

			EditorGUILayout.Space();
		}

		// editSmoothing
		// Call to output smoothing controls

		private void editSmoothing(bool position)
		{
			// output smoothing
			if(position) EditorGUILayout.PropertyField(m_propSmoothPosition,CONTENT_SMOOTHPOSITION);
			EditorGUILayout.PropertyField(m_propSmoothRotation,CONTENT_SMOOTHROTATION);

			// output time too?
			if((position && m_propSmoothPosition.boolValue) ||
				m_propSmoothRotation.boolValue)
				EditorGUILayout.PropertyField(m_propSmoothTime,CONTENT_SMOOTHTIME);

			EditorGUILayout.Space();
		}

		// editInput
		// Call to output user input controls

		private void editInput(bool position)
		{
			// output cursor options
			EditorGUILayout.PropertyField(m_propShowCursor,CONTENT_SHOWCURSOR);
			EditorGUILayout.PropertyField(m_propLockCursor,CONTENT_LOCKCURSOR);
			EditorGUILayout.Space();

			// output user input
			EditorGUILayout.PropertyField(m_propInputEnable,CONTENT_INPUTENABLE);

			// output scale controls?
			if(m_propInputEnable.boolValue)
			{
				if(position) EditorGUILayout.PropertyField(m_propInputPositionScale,CONTENT_INPUTPOSITIONSCALE);
				EditorGUILayout.PropertyField(m_propInputRotationScale,CONTENT_INPUTROTATIONSCALE);
			}
		}

		// editMinMax
		// Call to output min and max controls

		private void editMinMax(GUIContent label,ref bool enableX,ref bool enableY,ref bool enableZ,ref Vector3 minimum,ref Vector3 maximum)
		{
			// get field widths
			float w=Screen.width;

			float w0=w/3.0f;
			float w1=(w-w0)/4.5f;

			GUILayoutOption[] c0={GUILayout.Width(w0)};
			GUILayoutOption[] c1={GUILayout.Width(w1)};

			// start table
			GUILayout.BeginHorizontal();

			// output prefix labels
			GUILayout.BeginVertical(c0);
//			EditorGUILayout.PrefixLabel(label);
			GUILayout.Label(label);
			if(enableX || enableY || enableZ)
			{
				GUILayout.Label(CONTENT_MINIMUMCLAMP);
				GUILayout.Label(CONTENT_MAXIMUMCLAMP);
			}
			GUILayout.EndVertical();

			// output controls
			editMinMaxControl(CONTENT_X,ref enableX,ref minimum.x,ref maximum.x,c1);
			editMinMaxControl(CONTENT_Y,ref enableY,ref minimum.y,ref maximum.y,c1);
			editMinMaxControl(CONTENT_Z,ref enableZ,ref minimum.z,ref maximum.z,c1);

			// close table
			GUILayout.EndHorizontal();
		}

		// editMinMaxControl
		// Call to output min and max control group

		private void editMinMaxControl(string label,ref bool enable,ref float minimum,ref float maximum,GUILayoutOption[] options)
		{
			// output labels
			GUILayout.BeginVertical();
			GUILayout.Label(label);
			if(enable)
			{
				GUILayout.Label(label);
				GUILayout.Label(label);
			}
			GUILayout.EndVertical();
		
			// output controls
			GUILayout.BeginVertical();
			enable=EditorGUILayout.Toggle(enable,options);
			if(enable)
			{
				minimum=EditorGUILayout.FloatField(minimum);
				maximum=EditorGUILayout.FloatField(maximum);
			}
			GUILayout.EndVertical();
		}
	}
}
