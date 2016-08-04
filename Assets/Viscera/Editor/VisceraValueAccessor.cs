using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Type = System.Type;
using Array = System.Array;

namespace Viscera {

    public abstract class ValueAccessor
    {
        public abstract void SetValue(object value);
        public abstract object GetValue();
    }

    public class ConstantValueAccessor : ValueAccessor
    {
        private object value;
        public ConstantValueAccessor(object value)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object value)
        {
            this.value = value;
        }
    }
    
    public class StructFieldValueAccessor : ValueAccessor
    {
        private ValueAccessor _structValueAccessor;
        private FieldInfo _fieldInfo;

        public StructFieldValueAccessor(ValueAccessor outer, FieldInfo info)
        {
            this._structValueAccessor = outer;
            this._fieldInfo = info;
        }

        public override void SetValue(object value)
        {
            var structValue = _structValueAccessor.GetValue();
            _fieldInfo.SetValue(structValue, value);
            _structValueAccessor.SetValue(structValue);
        }

        public override object GetValue()
        {
            var structValue = _structValueAccessor.GetValue();
            return _fieldInfo.GetValue(structValue);
        }
    }

    public class StructPropertyValueAccessor : ValueAccessor
    {
        static readonly object[] DEFAULT_INDEX_ARRAY = new object[0];

        private ValueAccessor _structValueAccessor;
        private PropertyInfo _propertyInfo;
        public StructPropertyValueAccessor(ValueAccessor outer, PropertyInfo info)
        {
            this._structValueAccessor = outer;
            this._propertyInfo = info;
        }

        public override void SetValue(object value)
        {
            if(!_propertyInfo.CanWrite) return;
            var structValue = _structValueAccessor.GetValue();
            _propertyInfo.SetValue(structValue, value, DEFAULT_INDEX_ARRAY);
            _structValueAccessor.SetValue(structValue);
        }

        public override object GetValue()
        {
            if(!_propertyInfo.CanRead) return null;
            var structValue = _structValueAccessor.GetValue();
            return _propertyInfo.GetValue(structValue, DEFAULT_INDEX_ARRAY);
        }
    }

    public class ClassFieldValueAccessor : ValueAccessor
    {
        protected FieldInfo _fieldInfo;
        protected object _parent;

        public ClassFieldValueAccessor(object parent, FieldInfo info)
        {
            _fieldInfo = info;
            _parent = parent;
        }

        public override void SetValue(object value)
        {
            if (_parent == null) return;
            _fieldInfo.SetValue(_parent, value);
        }

        public override object GetValue()
        {
            if (_parent == null) return null;
            return _fieldInfo.GetValue(_parent);
        }
    }

    public class ClassPropertyValueAccessor : ValueAccessor
    {
        static readonly object[] DEFAULT_INDEX_ARRAY = new object[0];

        public bool ShowValue = false;

        protected PropertyInfo _propertyInfo;
        protected object _parent;

        public ClassPropertyValueAccessor(object parent, PropertyInfo info)
        {
            _propertyInfo = info;
            _parent = parent;
        }

        public override void SetValue(object value)
        {
            if (_parent == null || !_propertyInfo.CanWrite) return;
            _propertyInfo.SetValue(_parent, value, DEFAULT_INDEX_ARRAY);
        }

        public override object GetValue()
        {
            if (_parent == null || !_propertyInfo.CanRead) return null;
            return _propertyInfo.GetValue(_parent, DEFAULT_INDEX_ARRAY);
        }
    }

    public class Array1DElementValueAccessor : ValueAccessor
    {
        private ValueAccessor _arrayValueAccessor;
        private int _elementIndex;

        public Array1DElementValueAccessor(ValueAccessor outer, int index)
        {
            this._arrayValueAccessor = outer;
            this._elementIndex = index;
        }
        
        public override void SetValue(object value)
        {
            var array = (Array)_arrayValueAccessor.GetValue();
            if (array == null || array.GetLength(0) <= _elementIndex) return;
            array.SetValue(value, _elementIndex);
        }

        public override object GetValue()
        {
            var array = (Array)_arrayValueAccessor.GetValue();
            if (array == null || array.GetLength(0) <= _elementIndex) return null;
            return array.GetValue(_elementIndex);
        }
    }

    public class GenericListElementValueAccessor : ValueAccessor
    {
        private ValueAccessor _listValueAccessor;
        private int _elementIndex;

        public GenericListElementValueAccessor(ValueAccessor outer, int index)
        {
            this._listValueAccessor = outer;
            this._elementIndex = index;
        }
        
        public override void SetValue(object value)
        {
            var list = (IList)_listValueAccessor.GetValue();
            var listCounter = (ICollection)list;
            if (list == null || listCounter.Count <= _elementIndex) return;
            list[_elementIndex] = value;
        }

        public override object GetValue()
        {
            var list = (IList)_listValueAccessor.GetValue();
            var listCounter = (ICollection)list;
            if (list == null || listCounter.Count <= _elementIndex) return null;
            return list[_elementIndex];
        }
    }

    public class GenericDictionaryValueAccessor : ValueAccessor
    {
        public object key;

        private ValueAccessor _dictValueAccessor;

        public GenericDictionaryValueAccessor(ValueAccessor outer, object key)
        {
            this._dictValueAccessor = outer;
            this.key = key;
        }
        
        public override void SetValue(object value)
        {
            var dict = (IDictionary)_dictValueAccessor.GetValue();
            if(dict == null || !dict.Contains(key)) return;
            dict[key] = value;
        }

        public override object GetValue()
        {
            var dict = (IDictionary)_dictValueAccessor.GetValue();
            if(dict == null || !dict.Contains(key)) return null;
            return dict[key];
        }
    }
}