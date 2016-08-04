using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Type = System.Type;
using Array = System.Array;
using Convert = System.Convert;

namespace Viscera {

    [MemberFactoryHint(typeof(System.Single))]
    public class FloatMember : Member
    {
        public FloatMember(string name, Type type)
            :base(name, type)
        {
        }

        protected override void OnGUIValue()
        {
            var _value = (float)CachedValue;
            var val = EditorGUILayout.FloatField(_value, GUILayout.ExpandWidth(true));
            if(val != _value)
            {
                SetValue(val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.Double))]
    public class DoubleMember : Member
    {
        public DoubleMember(string name, Type type)
            :base(name, type)
        {
        }

        protected override void OnGUIValue()
        {
            var _value = (double)CachedValue;
            var valStr = EditorGUILayout.TextField(_value.ToString(), GUILayout.ExpandWidth(true));
            var val = _value;
            if(double.TryParse(valStr, out val))
            { 
                if (val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.Decimal))]
    public class DecimalMember : Member
    {
        public DecimalMember(string name, Type type)
            :base(name, type)
        {
        }

        protected override void OnGUIValue()
        {
            var _value = (decimal)CachedValue;
            var valStr = EditorGUILayout.TextField(_value.ToString(), GUILayout.ExpandWidth(true));
            var val = _value;
            if(decimal.TryParse(valStr, out val))
            { 
                if (val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.Byte))]
    public class ByteMember : Member
    {
        public ByteMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (int)(byte)CachedValue;
            var val = EditorGUILayout.IntField(_value, GUILayout.ExpandWidth(true));
            val = Mathf.Clamp(val, byte.MinValue, byte.MaxValue);
            if(val != _value)
            {
                SetValue((byte)val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.SByte))]
    public class SByteMember : Member
    {
        public SByteMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (int)(sbyte)CachedValue;
            var val = EditorGUILayout.IntField(_value, GUILayout.ExpandWidth(true));
            val = Mathf.Clamp(val, sbyte.MinValue, sbyte.MaxValue);
            if(val != _value)
            {
                SetValue((sbyte)val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.Int16))]
    public class ShortMember : Member
    {

        public ShortMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (int)(short)(CachedValue);
            var val = EditorGUILayout.IntField(_value, GUILayout.ExpandWidth(true));
            val = Mathf.Clamp(val, short.MinValue, short.MaxValue);
            if(val != _value)
            {
                SetValue((short)val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.UInt16))]
    public class UShortMember : Member
    {

        public UShortMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (int)(ushort)(CachedValue);
            var val = EditorGUILayout.IntField(_value, GUILayout.ExpandWidth(true));
            val = Mathf.Clamp(val, ushort.MinValue, ushort.MaxValue);
            if(val != _value)
            {
                SetValue((ushort)val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.Int32))]
    public class IntMember : Member
    {

        public IntMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (int)CachedValue;
            var val = EditorGUILayout.IntField(_value, GUILayout.ExpandWidth(true));
            if(val != _value)
            {
                SetValue(val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.UInt32))]
    public class UIntMember : Member
    {

        public UIntMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (uint)CachedValue;
            var valStr = EditorGUILayout.TextField(_value.ToString(), GUILayout.ExpandWidth(true));
            var val = _value;
            if(uint.TryParse(valStr, out val))
            {
                if (val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.Int64))]
    public class LongMember : Member
    {

        public LongMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (long)CachedValue;
            var valStr = EditorGUILayout.TextField(_value.ToString(), GUILayout.ExpandWidth(true));
            var val = _value;
            if(long.TryParse(valStr, out val))
            {
                if (val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.UInt64))]
    public class ULongMember : Member
    {

        public ULongMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (ulong)CachedValue;
            var valStr = EditorGUILayout.TextField(_value.ToString(), GUILayout.ExpandWidth(true));
            var val = _value;
            if(ulong.TryParse(valStr, out val))
            {
                if (val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.Boolean))]
    public class BoolMember : Member
    {
        public BoolMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            bool _value = (bool)CachedValue;
            var val = EditorGUILayout.Toggle(_value, GUILayout.ExpandWidth(true));
            if(val != _value)
            {
                SetValue(val);
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.Char))]
    public class CharMember : Member
    {
        public CharMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            char _value = (char)CachedValue;
            var str = EditorGUILayout.TextField(_value.ToString(), GUILayout.ExpandWidth(true));
            var intValue = Convert.ToInt32(_value);
            var intValue2 = EditorGUILayout.IntField(intValue, GUILayout.ExpandWidth(true));

            if(intValue2 != intValue)
            {
                SetValue((char)intValue2);
            }
            else if(str.Length > 0)
            {
                char val = str[0];
                if(val != _value)
                {
                    SetValue(val);
                }
            }
        }
    }
    
    [MemberFactoryHint(typeof(System.String), 3000)]
    public class StringMember : Member
    {
        public StringMember(string name, Type type)
            :base(name, type)
        {
        }
        
        protected override void OnGUIValue()
        {
            var _value = (string)CachedValue;
            var val = EditorGUILayout.TextField(_value, GUILayout.ExpandWidth(true));
            if(val != _value)
            {
                SetValue(val);
            }
        }
    }
}