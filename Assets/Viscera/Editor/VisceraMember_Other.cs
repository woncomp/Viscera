using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Type = System.Type;
using Array = System.Array;
using Convert = System.Convert;

namespace Viscera
{


    public class ReadonlyLambdaMember : Member
    {
        private System.Func<string> _lambda;
        public ReadonlyLambdaMember(string name, string typeName, System.Func<string> lambda)
            : base(name, typeof(object))
        {
            this.TypeName = typeName;
            this._lambda = lambda;
        }

        public override void Update()
        {
            CachedValue = _lambda == null ? "?? Undefined lambda expression ??" : _lambda();
        }

        protected override void OnGUIValue()
        {
            EditorGUILayout.SelectableLabel((string)CachedValue, GUILayout.Height(18));
        }
    }

    [MemberFactoryHint("MemberFactoryHint_Pred")]
    public class EnumMember : Member
    {
        private bool _isFlags;

        public EnumMember(string name, Type type)
            : base(name, type)
        {
            _isFlags = type.GetCustomAttributes(typeof(System.FlagsAttribute), false).Length > 0;
        }

        protected override void OnGUIValue()
        {
            var _value = (System.Enum)(CachedValue ?? System.Activator.CreateInstance(this.MemberType));
            var val = _isFlags ? EditorGUILayout.EnumMaskField(_value) : EditorGUILayout.EnumPopup(_value);
            if (val != _value)
            {
                SetValue(val);
            }
        }

        static bool MemberFactoryHint_Pred(Type type)
        {
            return type.IsEnum;
        }
    }

    public class InheritanceLevelMember : Member
    {
        public InheritanceLevelMember(Type type)
            : base(string.Empty, type)
        {
            TypeName = type.Name;
        }

        public override void OnGUI()
        {
            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField(TypeName, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }
    }

    [MemberFactoryHint("MemberFactoryHint_Pred", -8000)]
    public class ClassMember : Member
    {
        public ClassMember(string name, Type type)
            : base(name, type)
        {
        }

        protected override void OnGUIValue()
        {
            var _value = CachedValue;
            if (_value == null)
            {
                EditorGUILayout.LabelField("NULL", GUILayout.ExpandWidth(true));
            }
            else if(_value is UnityEngine.Object)
			{
				var _ref = (Object)CachedValue;
				var val = EditorGUILayout.ObjectField(_ref, MemberType, true, GUILayout.ExpandWidth(true));
				if(val != _ref)
				{
					SetValue(val);
				}
            }
            else
            {
                if (GUILayout.Button(S.GoInsideContent, S.GoInsideStyle))
                {
                    Helper.GoInsideEntity = Helper.CreateEntity(AutoEntityName, _value.GetType(), this.Accessor);
                }
            }
        }

        static bool MemberFactoryHint_Pred(Type type)
        {
            return type.IsClass;
        }
    }

    [MemberFactoryHint("MemberFactoryHint_Pred", -8000)]
    public class StructMember : Member
    {
        public StructMember(string name, Type type)
            : base(name, type)
        {
        }

        protected override void OnGUIValue()
        {
            var _value = CachedValue;
            if (GUILayout.Button(S.GoInsideContent, S.GoInsideStyle))
            {
                Helper.GoInsideEntity = Helper.CreateEntity(AutoEntityName, _value.GetType(), this.Accessor);
            }
        }

        static bool MemberFactoryHint_Pred(Type type)
        {
            return type.IsValueType;
        }
    }

    [MemberFactoryHint("MemberFactoryHint_Pred", -9000)]
    public class InterfaceMember : Member
    {
        public InterfaceMember(string name, Type type)
            : base(name, type)
        {
        }

        protected override void OnGUIValue()
        {
            var _value = CachedValue;
            if (GUILayout.Button(S.GoInsideContent, S.GoInsideStyle))
            {
                Helper.GoInsideEntity = Helper.CreateEntity(AutoEntityName, _value.GetType(), this.Accessor);
            }
        }

        static bool MemberFactoryHint_Pred(Type type)
        {
            return type.IsInterface;
        }
    }

    [MemberFactoryHint("MemberFactoryHint_Pred", 1000)]
    public class ArrayMember : Member, IHasElements
    {
        private List<Member> _cachedElements = new List<Member>();

        private Type _elementType;
        private int _arrayLength;
        private int _newLength;

        public ArrayMember(string name, Type type)
            : base(name, type)
        {
            _elementType = MemberType.GetElementType();
            _newLength = -1;
        }

        protected override void OnGUIValue()
        {
            if (CachedValue == null)
            {
                EditorGUILayout.LabelField("NULL", GUILayout.ExpandWidth(true));
            }
            else
            {
                _newLength = Helper.DelayedIntField(GUIContent.none, _arrayLength, GUILayout.ExpandWidth(true));
                if (_newLength == _arrayLength) _newLength = -1;
            }
        }

        public override void Update()
        {
            base.Update();
            if (CachedValue == null) return;
            var array = (Array)CachedValue;
            _arrayLength = array.Length;
            if (_newLength >= 0 && _newLength != _arrayLength)
            {
                var newArray = Array.CreateInstance(_elementType, _newLength);
                Array.Copy(array, newArray, Mathf.Min(_arrayLength, _newLength));
                _arrayLength = _newLength;
                CachedValue = array = newArray;
                Accessor.SetValue(array);
            }
        }

        public List<Member> GetElements()
        {
            if (CachedValue == null) return null;
            var array = (Array)CachedValue;
            if (_elementType.IsValueType)
            {
                if (_cachedElements.Count > _arrayLength)
                {
                    _cachedElements.RemoveRange(_arrayLength, _cachedElements.Count - _arrayLength);
                }
                else
                {
                    for (int i = _cachedElements.Count; i < _arrayLength; ++i)
                    {
                        var accessor = new Array1DElementValueAccessor(this.Accessor, i);
                        var m = Helper.CreateMember("Element " + i, _elementType, accessor);
                        m.EntityName = AutoEntityName + "[" + i + "]";
                        _cachedElements.Add(m);
                    }
                }
            }
            else
            {
                var newList = new List<Member>();
                for (int i = 0; i < _arrayLength; ++i)
                {
                    var accessor = new Array1DElementValueAccessor(this.Accessor, i);
                    var value = accessor.GetValue();
                    Member m = null;
                    if (value != null) m = _cachedElements.Find(_m => _m.CachedValue == value);
                    if (m == null)
                    {
                        m = Helper.CreateMember("Element " + i, _elementType, accessor);
                        m.EntityName = AutoEntityName + "[" + i + "]";
                    }
                    else _cachedElements.Remove(m);
                    newList.Add(m);
                }
                _cachedElements = newList;
            }
            return _cachedElements;
        }

        static bool MemberFactoryHint_Pred(Type type)
        {
            return type.IsArray;
        }
    }


    [MemberFactoryHint("MemberFactoryHint_Pred", 2000)]
    public class ListMember : Member, IHasElements
    {
        private List<Member> _cachedElements = new List<Member>();

        private Type _elementType;
        private int _listLength;
        private int _newLength = -1;

        public ListMember(string name, Type type)
            : base(name, type)
        {
            _elementType = MemberType.GetGenericArguments()[0];
            TypeName = "List<" + _elementType.Name + ">";
        }

        protected override void OnGUIValue()
        {
            if (CachedValue == null)
            {
                EditorGUILayout.LabelField("NULL", GUILayout.ExpandWidth(true));
            }
            else
            {
                _newLength = Helper.DelayedIntField(GUIContent.none, _listLength, GUILayout.ExpandWidth(true));
                if (_newLength == _listLength) _newLength = -1;
            }
        }

        public override void Update()
        {
            base.Update();
            if (CachedValue == null) return;
            var list = (System.Collections.IList)CachedValue;
            var listCounter = (System.Collections.ICollection)CachedValue;
            _listLength = listCounter.Count;
            if (_newLength >= 0 && _newLength != _listLength)
            {
                while (_newLength > _listLength)
                {
                    if (_elementType.IsValueType)
                    {
                        list.Add(System.Activator.CreateInstance(_elementType));
                    }
                    else
                    {
                        list.Add(null);
                    }
                    _listLength++;
                }
                while (_newLength < _listLength)
                {
                    list.RemoveAt(_listLength - 1);
                    _listLength--;
                }
            }
        }

        public List<Member> GetElements()
        {
            if (CachedValue == null) return null;
            var list = (System.Collections.IList)CachedValue;
            if (_elementType.IsValueType)
            {
                if (_cachedElements.Count > _listLength)
                {
                    _cachedElements.RemoveRange(_listLength, _cachedElements.Count - _listLength);
                }
                else
                {
                    for (int i = _cachedElements.Count; i < _listLength; ++i)
                    {
                        var accessor = new GenericListElementValueAccessor(this.Accessor, i);
                        var m = Helper.CreateMember("Element " + i, _elementType, accessor);
                        m.EntityName = AutoEntityName + "[" + i + "]";
                        _cachedElements.Add(m);
                    }
                }
            }
            else
            {
                var newList = new List<Member>();
                for (int i = 0; i < _listLength; ++i)
                {
                    var accessor = new GenericListElementValueAccessor(this.Accessor, i);
                    var value = accessor.GetValue();
                    Member m = null;
                    if (value != null) m = _cachedElements.Find(_m => _m.CachedValue == value);
                    if (m == null)
                    {
                        m = Helper.CreateMember("Element " + i, _elementType, accessor);
                        m.EntityName = AutoEntityName + "[" + i + "]";
                    }
                    else _cachedElements.Remove(m);
                    newList.Add(m);
                }
                _cachedElements = newList;
            }
            return _cachedElements;
        }

        static bool MemberFactoryHint_Pred(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
    }

    [MemberFactoryHint("MemberFactoryHint_Pred", 2000)]
    public class DictionaryMember : Member, IHasElements
    {
        private List<Member> _cachedElements = new List<Member>();

        private Type _keyType;
        private Type _valueType;

        private int _elementCount;

        public DictionaryMember(string name, Type type)
            : base(name, type)
        {
            var types = MemberType.GetGenericArguments();
            _keyType = types[0];
            _valueType = types[1];
            TypeName = "Dict<" + _keyType.Name + ',' + _valueType.Name + ">";
        }

        protected override void OnGUIValue()
        {
            EditorGUILayout.LabelField("Count:" + _elementCount, GUILayout.ExpandWidth(true));
        }

        public override void Update()
        {
            base.Update();
            if (CachedValue == null) return;
            var dictCounter = (System.Collections.ICollection)CachedValue;
            _elementCount = dictCounter.Count;
        }

        public List<Member> GetElements()
        {
            if (CachedValue == null) return null;
            var dict = (System.Collections.IDictionary)CachedValue;
            var enumerator = dict.GetEnumerator();

            var newList = new List<Member>();
            while (enumerator.MoveNext())
            {
                var key = enumerator.Key;

                Member m = null;
                if (key == null) continue;
                m = _cachedElements.Find(_m => (_m.Accessor as GenericDictionaryValueAccessor).key == key);
                if (m == null)
                {
                    var accessor = new GenericDictionaryValueAccessor(this.Accessor, key);
                    var keyString = key.ToString();
                    m = Helper.CreateMember(keyString + ":", _valueType, accessor);
                    m.EntityName = AutoEntityName + "[" + keyString + "]";
                }
                else _cachedElements.Remove(m);
                newList.Add(m);
            }
            _cachedElements = newList;
            return _cachedElements;
        }

        static bool MemberFactoryHint_Pred(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }
    }

    [MemberFactoryHint("MemberFactoryHint_Pred", 4000)]
    public class DelegateMember : Member
    {
        public DelegateMember(string name, Type type)
            : base(name, type)
        {
        }

        protected override void OnGUIValue()
        {
            EditorGUILayout.LabelField("Delegate", GUILayout.ExpandWidth(true));
        }

        static bool MemberFactoryHint_Pred(Type type)
        {
            var expectedType = typeof(System.Delegate);
            return type == expectedType || type.IsSubclassOf(expectedType);
        }
    }
}