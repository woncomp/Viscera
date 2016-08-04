using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Type = System.Type;
using Array = System.Array;
using Convert = System.Convert;

namespace Viscera
{ 
    
    [MemberFactoryHint("MemberFactoryHint_Pred", 9000)]
    public class ReferenceMember : Member
    {
        public ReferenceMember(string name, Type type)
            :base(name, type)
        {
        }

        protected override void OnGUIValue()
        {
            var _value = (Object)CachedValue;
            var val = EditorGUILayout.ObjectField(_value, MemberType, true, GUILayout.ExpandWidth(true));
            if(val != _value)
            {
                SetValue(val);
            }
        }

        static bool MemberFactoryHint_Pred(Type type)
        {
            var expectedType = typeof(UnityEngine.Object);
            return type == expectedType || type.IsSubclassOf(expectedType);
        }
    }
    
    [MemberFactoryHint(typeof(UnityEngine.Vector2))]
    public class Vector2Member : Member
    {
		private static float[] s_Vector2Floats = new float[2];

		private static GUIContent[] s_XYLabels = new GUIContent[]
		{
			new GUIContent("X"),
			new GUIContent("Y")
		};

        public Vector2Member(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (Vector2)CachedValue;
            var _r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField);
			s_Vector2Floats[0] = _value.x;
			s_Vector2Floats[1] = _value.y;
			EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(_r, s_XYLabels, s_Vector2Floats);
            if (EditorGUI.EndChangeCheck())
            {
                Vector2 val;
				val.x = s_Vector2Floats[0];
				val.y = s_Vector2Floats[1];
                if (val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(UnityEngine.Vector3))]
    public class Vector3Member : Member
    {
		private static float[] s_Vector3Floats = new float[3];

		private static GUIContent[] s_XYZLabels = new GUIContent[]
		{
			new GUIContent("X"),
			new GUIContent("Y"),
			new GUIContent("Z")
		};

        public Vector3Member(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (Vector3)CachedValue;
            var _r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField);
			s_Vector3Floats[0] = _value.x;
			s_Vector3Floats[1] = _value.y;
            s_Vector3Floats[2] = _value.z;
			EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(_r, s_XYZLabels, s_Vector3Floats);
            if (EditorGUI.EndChangeCheck())
            {
                Vector3 val;
				val.x = s_Vector3Floats[0];
				val.y = s_Vector3Floats[1];
                val.z = s_Vector3Floats[2];
                if (val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(UnityEngine.Vector4))]
    public class Vector4Member : Member
    {
		private static float[] s_Vector4Floats = new float[4];

		private static GUIContent[] s_XYZWLabels = new GUIContent[]
		{
			new GUIContent("X"),
			new GUIContent("Y"),
			new GUIContent("Z"),
			new GUIContent("W")
		};

        public Vector4Member(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (Vector4)CachedValue;
            var _r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField);
			s_Vector4Floats[0] = _value.x;
			s_Vector4Floats[1] = _value.y;
            s_Vector4Floats[2] = _value.z;
            s_Vector4Floats[3] = _value.w;
			EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(_r, s_XYZWLabels, s_Vector4Floats);
            if (EditorGUI.EndChangeCheck())
            {
                Vector4 val;
				val.x = s_Vector4Floats[0];
				val.y = s_Vector4Floats[1];
				val.z = s_Vector4Floats[2];
				val.w = s_Vector4Floats[3];
                if (val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(UnityEngine.Color))]
    public class ColorMember : Member
    {
        public ColorMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (Color)CachedValue;
            EditorGUI.BeginChangeCheck();
            var val = EditorGUILayout.ColorField(string.Empty, _value, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck() && val != _value)
            {
                SetValue(val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(UnityEngine.Rect))]
    public class RectMember : Member
    {
        public RectMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (Rect)CachedValue;
            var val = EditorGUILayout.RectField(_value, GUILayout.ExpandWidth(true));
            if (val != _value)
            {
                SetValue(val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(UnityEngine.Quaternion))]
    public class QuaternionMember : Member
    {
		private static float[] s_Vector4Floats = new float[4];

		private static GUIContent[] s_XYZWLabels = new GUIContent[]
		{
			new GUIContent("X"),
			new GUIContent("Y"),
			new GUIContent("Z"),
			new GUIContent("W")
		};

        public QuaternionMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (Quaternion)CachedValue;
            var _r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField);
			s_Vector4Floats[0] = _value.x;
			s_Vector4Floats[1] = _value.y;
            s_Vector4Floats[2] = _value.z;
            s_Vector4Floats[3] = _value.w;
			EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(_r, s_XYZWLabels, s_Vector4Floats);
            if (EditorGUI.EndChangeCheck())
            {
                Quaternion val;
				val.x = s_Vector4Floats[0];
				val.y = s_Vector4Floats[1];
				val.z = s_Vector4Floats[2];
				val.w = s_Vector4Floats[3];
                if (val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(UnityEngine.Plane))]
    public class PlaneMember : Member
    {
		private static float[] s_Vector4Floats = new float[4];

		private static GUIContent[] s_XYZWLabels = new GUIContent[]
		{
			new GUIContent("X"),
			new GUIContent("Y"),
			new GUIContent("Z"),
			new GUIContent("W")
		};

        public PlaneMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (Plane)CachedValue;
            var _r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField);
			s_Vector4Floats[0] = _value.normal.x;
			s_Vector4Floats[1] = _value.normal.y;
            s_Vector4Floats[2] = _value.normal.z;
            s_Vector4Floats[3] = _value.distance;
			EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(_r, s_XYZWLabels, s_Vector4Floats);
            if (EditorGUI.EndChangeCheck())
            {
                Vector3 dir;
				dir.x = s_Vector4Floats[0];
				dir.y = s_Vector4Floats[1];
				dir.z = s_Vector4Floats[2];
                Plane val = new Plane();
                val.normal = dir;
				val.distance = s_Vector4Floats[3];
                SetValue(val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(UnityEngine.LayerMask))]
    public class LayerMaskMember : Member
    {

        public LayerMaskMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _layerMask = ((LayerMask)CachedValue);
            var _value = (int)_layerMask;

            var val = LayerMaskField(string.Empty, _value);
            if (val != _value)
            {
                _layerMask.value = val;
                SetValue(_layerMask);
            }
        }

	    /// <summary>
	    /// Layer mask field, originally from:
	    /// http://answers.unity3d.com/questions/60959/mask-field-in-the-editor.html
	    /// </summary>

	    public static int LayerMaskField (string label, int mask, params GUILayoutOption[] options)
	    {
		    List<string> layers = new List<string>();
		    List<int> layerNumbers = new List<int>();

		    string selectedLayers = "";

		    for (int i = 0; i < 32; ++i)
		    {
			    string layerName = LayerMask.LayerToName(i);

			    if (!string.IsNullOrEmpty(layerName))
			    {
				    if (mask == (mask | (1 << i)))
				    {
					    if (string.IsNullOrEmpty(selectedLayers))
					    {
						    selectedLayers = layerName;
					    }
					    else
					    {
						    selectedLayers = "Mixed";
					    }
				    }
			    }
		    }

		    if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
		    {
			    if (mask == 0)
			    {
				    layers.Add("Nothing");
			    }
			    else if (mask == -1)
			    {
				    layers.Add("Everything");
			    }
			    else
			    {
				    layers.Add(selectedLayers);
			    }
			    layerNumbers.Add(-1);
		    }

		    layers.Add((mask == 0 ? "[+] " : "      ") + "Nothing");
		    layerNumbers.Add(-2);

		    layers.Add((mask == -1 ? "[+] " : "      ") + "Everything");
		    layerNumbers.Add(-3);

		    for (int i = 0; i < 32; ++i)
		    {
			    string layerName = LayerMask.LayerToName(i);

			    if (layerName != "")
			    {
				    if (mask == (mask | (1 << i)))
				    {
					    layers.Add("[+] " + layerName);
				    }
				    else
				    {
					    layers.Add("      " + layerName);
				    }
				    layerNumbers.Add(i);
			    }
		    }

		    bool preChange = GUI.changed;

		    GUI.changed = false;

		    int newSelected = 0;

		    if (Event.current.type == EventType.MouseDown)
		    {
			    newSelected = -1;
		    }

		    if (string.IsNullOrEmpty(label))
		    {
			    newSelected = EditorGUILayout.Popup(newSelected, layers.ToArray(), EditorStyles.layerMaskField, options);
		    }
		    else
		    {
			    newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField, options);
		    }

		    if (GUI.changed && newSelected >= 0)
		    {
			    if (newSelected == 0)
			    {
				    mask = 0;
			    }
			    else if (newSelected == 1)
			    {
				    mask = -1;
			    }
			    else
			    {
				    if (mask == (mask | (1 << layerNumbers[newSelected])))
				    {
					    mask &= ~(1 << layerNumbers[newSelected]);
				    }
				    else
				    {
					    mask = mask | (1 << layerNumbers[newSelected]);
				    }
			    }
		    }
		    else
		    {
			    GUI.changed = preChange;
		    }
		    return mask;
	    }
    }

    public class TagMember : Member
    {
        public TagMember(string name, Type type)
            :base(name, type)
        {
            TypeName = "Tag";
        }

        protected override void OnGUIValue()
        {
            var _value = (string)CachedValue;
            var val = EditorGUILayout.TagField(_value, GUILayout.ExpandWidth(true));
            if(val != _value)
            {
                SetValue(val);
            }
        }
    }

    public class ComponentMember : Member
    {
        public Component component;

        public ComponentMember(Component c)
            :base(c.name, c.GetType())
        {
            component = c;
        }

        public override void OnGUI()
        {
            var c = this.component;
            
            EditorGUILayout.InspectorTitlebar(true, c);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Instance ID", c.GetInstanceID().ToString(), "OL Header");
            if(GUILayout.Button(S.GoInsideContent, S.GoInsideStyle, GUILayout.ExpandWidth(true)))
            {
                Helper.GoInsideEntity = new ComponentEntity(c);
            }
            GUILayout.Space(30);
            EditorGUILayout.EndHorizontal();
        }
    }
}