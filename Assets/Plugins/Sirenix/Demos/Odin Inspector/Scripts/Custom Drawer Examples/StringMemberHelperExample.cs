namespace Sirenix.OdinInspector.Demos
{
	using UnityEngine;
	using System;
	using Sirenix.OdinInspector;

	#if UNITY_EDITOR
	using Sirenix.OdinInspector.Editor;
	using Sirenix.Utilities.Editor;
	using UnityEditor;
	#endif

	public class StringMemberHelperExample : MonoBehaviour
	{
		[InfoBox("Using StringMemberHelper, it's possible to get a static string, or refer to a member string with very little effort.")]
		[PostLabel("A static label")]
		public int MyIntValue;

		[PostLabel("$DynamicLabel")]
		public string DynamicLabel = "A dynamic label";

		[PostLabel("$Invalid")]
		public float InvalidReference;
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class PostLabelAttribute : Attribute
	{
		public string Name { get; private set; }
	
		public PostLabelAttribute(string name)
		{
			this.Name = name;
		}
	}

	#if UNITY_EDITOR
	[OdinDrawer]
	public sealed class PostLabelAttributeDrawer : OdinAttributeDrawer<PostLabelAttribute>
	{
		protected override void DrawPropertyLayout(InspectorProperty property, PostLabelAttribute attribute, GUIContent label)
		{
			// Get and create string member helper context.
			PropertyContext<StringMemberHelper> context;
			if (property.Context.Get(this, "StringHelper", out context))
			{
				context.Value = new StringMemberHelper(property.ParentType, attribute.Name);
			}

			// Display error
			if (context.Value.ErrorMessage != null)
			{
				SirenixEditorGUI.ErrorMessageBox(context.Value.ErrorMessage);
				this.CallNextDrawer(property, label);
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				this.CallNextDrawer(property, null);

				// Get the string from the string member helper.
				EditorGUILayout.PrefixLabel(context.Value.GetString(property));

				EditorGUILayout.EndHorizontal();
			}

		}
	}
	#endif	
}