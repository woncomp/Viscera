using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Type = System.Type;

namespace Viscera {

    public abstract class Entity
    {
        public string Name;
        public List<Member> Members = new List<Member>();
            
        private Vector2 _scrollPos;

        public Entity(string name)
        {
            this.Name = name;
        }

        public void DoGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            OnGUI();
            EditorGUILayout.Space();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
        }

        public abstract void OnGUI();
        public abstract bool CheckValue();
        public virtual void Update()
        {
            for(int i=0;i<Members.Count;++i)
            {
                Members[i].Update();
            }
        }
    }

    public class GameObjectEntity : Entity
    {

        public GameObject gameObject;

        private ValueAccessor _accessor;

        public GameObjectEntity(GameObject go)
            :base(go.name)
        {
            this.gameObject = go;
            ScanMembers();
        }

        public GameObjectEntity(string name, ValueAccessor accessor)
            :base(name)
        {
            this._accessor = accessor;
        }

        public override bool CheckValue()
        {
            if (_accessor != null)
            {
                var newTarget = _accessor.GetValue() as GameObject;
                if (gameObject == null)
                {
                    gameObject = newTarget;
                    
                    ScanMembers();
                    return true;
                }
                else return newTarget == gameObject;
            }
            return true;
        }

        public override void OnGUI()
        {
            if(GUILayout.Button("Select this GameObject"))
            {
                Selection.activeGameObject = gameObject;
            }
            
            foreach(var m in Members)
            {
                m.OnGUI();
            }
        }

        void ScanMembers()
        {
            var components = gameObject.GetComponents<Component>();
            var newList = new List<Member>();
            foreach (var component in components)
            {
                var m = Members.Find(_m => (_m as ComponentMember).component == component);
                if (m == null) m = new ComponentMember(component);
                newList.Add(m);
            }
            Members.Clear();
            Members.AddRange(newList);
        }
    }

    public class StructEntity : Entity
    {
        private ValueAccessor _accessor;

        public StructEntity(string name, ValueAccessor accessor)
            :base(name)
        {
            this._accessor = accessor;
        }

        public override void OnGUI()
        {            
            foreach(var m in Members)
            {
                m.OnGUI();
            }
        }

        public override bool CheckValue()
        {
            if(Members.Count == 0)
            {
                var obj = _accessor.GetValue();
                var type = obj.GetType();
                var flags = BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
                foreach (var field in type.GetFields(flags))
                {
                    if(field.Name.EndsWith("k__BackingField")) continue;
                    var m = Helper.CreateMember(field.Name, field.FieldType, new StructFieldValueAccessor(_accessor, field));
                    Members.Add(m);
                }
                foreach (var property in type.GetProperties(flags))
                {
                    if (!property.CanRead) continue;
                    if (property.GetIndexParameters().Length > 0) continue;
                    var m = Helper.CreateMember(Helper.GetPropertyName(property), property.PropertyType, new StructPropertyValueAccessor(_accessor, property));
                    Members.Add(m);
                }
            }
            return true;
        }

    }

    public class ClassEntity : Entity
    {
        protected ValueAccessor _accessor;
        protected object _target;

        public ClassEntity(string name, ValueAccessor accessor)
            :base(name)
        {
            this._accessor = accessor;
        }

        public override void OnGUI()
        {            
            foreach(var m in Members)
            {
                m.OnGUI();
            }
        }

        public override bool CheckValue()
        {
 	        var newTarget = _accessor.GetValue();
            if(_target == null)
            {
                _target = newTarget;
                ScanMembers(_target, _target.GetType(), Members);
                return true;
            }
            else return newTarget == _target;
        }

        const BindingFlags DefaultBindingFlags = BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;

        public static void ScanMembers(object obj, Type type, List<Member> output)
        {
            if(obj == null) return;

            bool shouldBreak = false;
            while (type != typeof(object))
            {
                output.Add(new InheritanceLevelMember(type));
                if(type == typeof(MonoBehaviour) || type == typeof(Behaviour) || type == typeof(Component))
                {
                    shouldBreak = true;
                    CreateMembersForUnityObject(obj as Object, type, output);
                }
                else
                {
                    var flags = DefaultBindingFlags|BindingFlags.DeclaredOnly;
                    foreach (var field in type.GetFields(flags))
                    {
                        if(field.Name.EndsWith("k__BackingField")) continue;
                        var m = Helper.CreateMember(field.Name, field.FieldType, new ClassFieldValueAccessor(obj, field));
                        output.Add(m);
                    }
                    foreach (var property in type.GetProperties(flags))
                    {
                        if (!property.CanRead) continue;
                        if (property.GetIndexParameters().Length > 0) continue;
                        var m = Helper.CreateMember(Helper.GetPropertyName(property), property.PropertyType, new ClassPropertyValueAccessor(obj, property));
                        output.Add(m);
                    }
                }
                if(shouldBreak) break;
                type = type.BaseType;
            }
        }

        static void CreateMembersForUnityObject(Object obj, Type type, List<Member> output)
        {
            if(obj == null) return;
            var flags = DefaultBindingFlags;
            foreach (var property in type.GetProperties(flags))
            {
                if(property.PropertyType.IsSubclassOf(typeof(Component))) continue;
                if(property.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).Length > 0) continue;
                if(property.GetIndexParameters().Length > 0) continue;

                var accessor = new ClassPropertyValueAccessor(obj, property);
                Member m = null;
                if(property.Name == "tag")
                {
                    m = new TagMember(Helper.GetPropertyName(property), property.PropertyType);
                    m.Accessor = accessor;
                }
                else
                { 
                    m = Helper.CreateMember(Helper.GetPropertyName(property), property.PropertyType, accessor);
                }
                output.Add(m);
            }
            foreach (var field in type.GetFields(flags))
            {
                var m = Helper.CreateMember(field.Name, field.FieldType, new ClassFieldValueAccessor(obj, field));
                output.Add(m);
            }
            output.Add(new ReadonlyLambdaMember("Instance ID", "", () => obj.GetInstanceID().ToString()));
        }
    }

    public class ComponentEntity : ClassEntity
    {
        private int _instanceId;
        private Type _type;
        private GameObject _gameObject;
        public ComponentEntity(Component c)
            :base(c.GetType().Name, null)
        {
            this._target = c;
            this._instanceId = c.GetInstanceID();
            this._type = c.GetType();
            this._gameObject = c.gameObject;
        }

        public ComponentEntity(string name, ValueAccessor accessor)
            :base(name, accessor)
        {
        }
        
        public override bool CheckValue()
        {
            if(_accessor != null)
                return base.CheckValue();

            if(Members.Count == 0)
            {
                ScanMembers(_target, _target.GetType(), Members);
            }
            foreach(var c in _gameObject.GetComponents(_type))
            {
                if(c.GetInstanceID() == _instanceId)
                {
                    if(this._target != c)
                    { 
                        Members.Clear();
                        ScanMembers(c, c.GetType(), Members);
                    }
                    this._target = c;
                    break;
                }
            }
            return _target != null;
        }
    }

    public class UnityObjectEntity : ClassEntity
    {
        private UnityEngine.Object _object;

        public UnityObjectEntity(string name, ValueAccessor accessor)
            :base(name, accessor)
        {

        }

        public UnityObjectEntity(Object obj)
            :base(obj.name, null)
        {
            _object = obj;
            ScanMembers(_object, obj.GetType(), Members);
        }

        public override bool CheckValue()
        {
            if(_accessor != null)
                return base.CheckValue();
            return _object != null;
        }
    }
}