using UnityEngine;
using System.Collections;

namespace DynamicShadowProjector.Sample {
	public class Swing : MonoBehaviour {
		public float m_minAngle = -30;
		public float m_maxAngle = 30;
		public float m_swingSpeed = 0.1f;

		private Quaternion m_initialRotation;
		private float m_swing;
		void Start ()
		{
			m_initialRotation = transform.rotation;
			m_swing = 0.0f;
		}
		
		void Update ()
		{
			m_swing += m_swingSpeed*Time.deltaTime;
			m_swing -= Mathf.Floor(m_swing);
			float angle = Mathf.Lerp(m_minAngle, m_maxAngle, 0.5f - 0.5f*Mathf.Cos(2.0f*Mathf.PI*m_swing));
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.up) * m_initialRotation;
		}
	}
}
