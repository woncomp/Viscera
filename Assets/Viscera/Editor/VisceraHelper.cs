using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Type = System.Type;

namespace Viscera
{
    public class MemberFactoryHintAttribute : System.Attribute
    {
        public string PredicatorMethodName { get; private set; }
        public Type ExpectingType { get; private set; }
        public int Priority { get; private set; }

        public MemberFactoryHintAttribute(string predicatorMethodName, int priority = 0)
        {
            this.PredicatorMethodName = predicatorMethodName;
            this.Priority = priority;
        }

        public MemberFactoryHintAttribute(Type type, int priority = 0)
        {
            this.ExpectingType = type;
            this.Priority = priority;
        }
    }

    class MemberFactoryEntry
    {
        public Type MemberType;
        public MemberFactoryHintAttribute Attribute;
        public ConstructorInfo Constructor;

        public bool CheckType(Type type)
        {
            if(string.IsNullOrEmpty(Attribute.PredicatorMethodName))
            {
                return type == Attribute.ExpectingType;
            }
            else
            {
                var method = MemberType.GetMethod(Attribute.PredicatorMethodName, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static);
                if(method == null || method.ReturnType != typeof(bool)
                    || method.GetParameters().Length != 1
                    || method.GetParameters()[0].ParameterType != typeof(Type))
                {
                    method = null;
                    Debug.LogError("Cannot find method: static bool " + Attribute.PredicatorMethodName + "(Type type) in type " + MemberType.Name);
                    return false;
                }
                return (bool)method.Invoke(null, new object[] { type });
            }
        }
    }
    public class Helper
    {
        
        public static Member CreateMember(string name, Type type, ValueAccessor accessor)
        {
            var m = CreateMemberWithType(name, type);
            m.Accessor = accessor;
            return m;
        }
        static MemberFactoryEntry[] memberSelectorEntries;

        public static Member CreateMemberWithType(string name, Type type)
        {
            if(memberSelectorEntries == null)
            {
                var list = new List<MemberFactoryEntry>();
                foreach(var t in typeof(Member).Assembly.GetTypes())
                {
                    var attributes = t.GetCustomAttributes(typeof(MemberFactoryHintAttribute), false);
                    if(attributes.Length > 0)
                    {
                        var attribute = attributes[0] as MemberFactoryHintAttribute;
                        var constructor = t.GetConstructor(new []{typeof(string), typeof(Type)});
                        if(constructor == null)
                        {
                            Debug.LogError("Member Selector need a constructor of (string name, Type type)");
                            continue;
                        }
                        list.Add(new MemberFactoryEntry(){
                            MemberType = t,
                            Attribute = attribute,
                            Constructor = constructor
                        });
                    }
                }
                list.Sort((a, b)=>
                    {
                        return b.Attribute.Priority - a.Attribute.Priority;
                    });
                memberSelectorEntries = list.ToArray();
            }
            for(int i=0;i<memberSelectorEntries.Length;++i)
            {
                var entry = memberSelectorEntries[i];
                if(entry.CheckType(type))
                {
                    return entry.Constructor.Invoke(null, new object[]{name, type}) as Member;
                }
            }
            return new Member(name, type);
        }

        public static Entity CreateEntity(string name, Type type, ValueAccessor accessor)
        {
            if(type == typeof(GameObject))
            {
                return new GameObjectEntity(name, accessor);
            }
            else if(type == typeof(Component) || type.IsSubclassOf(typeof(Component)))
            {
                return new ComponentEntity(name, accessor);
            }
            else if (type.IsValueType)
            {
                return new StructEntity(name, accessor);
            }
            else
            {
                return new ClassEntity(name, accessor);
            }
        }

        public static string GetPropertyName(PropertyInfo pi)
        {
            string str = pi.Name + " {";
            if(pi.CanRead) str += "get;";
            if(pi.CanWrite) str += "set;";
            str += "}";
            return str;
        }

        public static Entity GoInsideEntity;
        public static float WindowWidth;
        public static float MemberNameWidth;
        public static float MemberTypeWidth;

        public static int DelayedIntField(GUIContent guiContentOfArraySizeField, int value, params GUILayoutOption[] options)
        {
            //int widthOfToggleButton = 22;
            //int widthOfToggleLabel = 36;

            Rect rectOfArraySizeField = GUILayoutUtility.GetRect(guiContentOfArraySizeField, EditorStyles.textField, options);
            //rectOfArraySizeField.width -= (widthOfToggleButton + widthOfToggleLabel);

            object[] parameters = new object[4];
            parameters[0] = rectOfArraySizeField;
            parameters[1] = guiContentOfArraySizeField;
            parameters[2] = value;
            parameters[3] = EditorStyles.numberField;

            MethodInfo methodOfArraySizeField = typeof(EditorGUI).GetMethod("ArraySizeField",
                BindingFlags.NonPublic|BindingFlags.Static,
                null,
                new []{typeof(Rect), typeof(GUIContent), typeof(int), typeof(GUIStyle)},
                null);
            return (int)methodOfArraySizeField.Invoke(null, parameters);
        }
    }

    public static class S
    {
        public static GUIContent GoInsideContent = new GUIContent("Go Inside");
        public static GUIStyle GoInsideStyle = "PreButton";
        public static GUIStyle ErrorMessageStyle;
        public static float LineHeight = 17;
        public static GUIStyle BreadcrumbMid = "GUIEditor.BreadcrumbMid";
        public static GUIStyle BreadcrumbLeft = "GUIEditor.BreadcrumbLeft";
        public static GUIStyle BreadcrumbBar = "LODBlackBox";

        static S()
        {
            ErrorMessageStyle = new GUIStyle(EditorStyles.whiteLabel);
            ErrorMessageStyle.normal.textColor = Color.red;
        }
    }
}