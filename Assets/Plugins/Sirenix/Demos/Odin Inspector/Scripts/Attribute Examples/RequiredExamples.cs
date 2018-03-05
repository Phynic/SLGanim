namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class RequiredExamples : MonoBehaviour
    {
        [InfoBox("Required displays an error when objects are missing.")]
        [Required]
        public GameObject MyGameObject;

        [Required("Custom error message.")]
        public Rigidbody MyRigidbody;

		[InfoBox("Use $ to indicate a member string as message.")]
		[Required("$DynamicMessage")]
		public GameObject GameObject;

		public string DynamicMessage = "Dynamic error message";
    }
}