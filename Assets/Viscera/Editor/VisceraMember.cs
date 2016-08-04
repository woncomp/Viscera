using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Type = System.Type;
using Array = System.Array;
using Convert = System.Convert;

namespace Viscera {
    
    public interface IHasElements
    {
        List<Member> GetElements();
    }

    public class Member {
        public const float indentWidth = 10;
        public static int memberIndent = 0;

        public string MemberName;
        public Type MemberType;

        public string EntityName;
        public string TypeName;
        
        public ValueAccessor Accessor;
        public object CachedValue;

        public string GetValueError;

        private ClassPropertyValueAccessor _propertyValueAccessor;

        private List<Member> _cachedElements;
        private bool _foldToggle;
        private bool _foldToggleNext;
                
        public string AutoEntityName { get { return EntityName ?? MemberName; } }
        
        public Member(string name, Type type)
        {
            this.MemberName = name;
            this.MemberType = type;
            this.TypeName = type.Name;
        }
        
        public virtual void OnGUI()
        {
            var lineHeight = EditorStyles.foldout.lineHeight;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(2);
            if(memberIndent > 0)
                GUILayoutUtility.GetRect(memberIndent * indentWidth, lineHeight, GUILayout.ExpandWidth(false));
            
            Rect r = GUILayoutUtility.GetRect(indentWidth, lineHeight, GUILayout.ExpandWidth(false));
                        
            if(this is IHasElements && CachedValue != null)
            {
                r.x += 2;
                r.y += 2;
                _foldToggleNext = EditorGUI.Foldout(r, _foldToggle, GUIContent.none);
            }
            
            var prefixLabelWidthHint = Helper.MemberNameWidth - memberIndent * indentWidth;
            GUILayout.Label(MemberName, GUILayout.Width(prefixLabelWidthHint), GUILayout.Height(S.LineHeight));
            if(_propertyValueAccessor != null && !_propertyValueAccessor.ShowValue)
            {
                if(GUILayout.Button("Show Value", S.GoInsideStyle))
                {
                    _propertyValueAccessor.ShowValue = true;
                }
            }
            else if(GetValueError != null)
            {
                EditorGUILayout.SelectableLabel(GetValueError, S.ErrorMessageStyle, GUILayout.Height(S.LineHeight));
            }
            else
            { 
                OnGUIValue();
            }

            GUILayout.Label(TypeName, GUILayout.Width(Helper.MemberTypeWidth));

            EditorGUILayout.EndHorizontal();
            
            if(_foldToggle && _cachedElements != null)
            {
                memberIndent++;
                foreach (var elem in _cachedElements)
                {
                    elem.OnGUI();
                }
                memberIndent--;
            }
        }

        public virtual void Update()
        {
            Helper.MemberNameWidth = Mathf.Max(Helper.MemberNameWidth, EditorStyles.label.CalcSize(new GUIContent(MemberName)).x);
            Helper.MemberTypeWidth = Mathf.Max(Helper.MemberTypeWidth, EditorStyles.label.CalcSize(new GUIContent(TypeName)).x);
            
            // A property evaluation could introduce side effect, so shouldn't evaluate until the user gives the order.
            _propertyValueAccessor = Accessor as ClassPropertyValueAccessor;
            if(_propertyValueAccessor != null && !_propertyValueAccessor.ShowValue) return;

            CachedValue = GetValue();
            _foldToggle = _foldToggleNext;
            if (_foldToggle)
            {
                _cachedElements = (this as IHasElements).GetElements();
                if (_cachedElements != null)
                {
                    foreach (var elem in _cachedElements)
                    {
                        elem.Update();
                    }
                }
            }
        }
                
        public object GetValue()
        {
            GetValueError = null;
            if(Accessor == null)
            { 
                GetValueError = "Accessor is null.";
                return null;
            }
            else
            {
                try
                {
                    return Accessor.GetValue();
                }
                catch (System.Exception e)
                {
                    GetValueError = "Exception: " + e.Message;
                }

                return null;
            }
        }
        
        public void SetValue(object value)
        {
            if(Accessor == null)
            { 
                Debug.LogWarning("[SetValue] Accessor is null.");
            }
            else
            { 
                Accessor.SetValue(value);
            }
        }

        protected virtual void OnGUIValue()
        {
            GUILayout.Label("<" + MemberType.Name + ">", GUILayout.ExpandWidth(true));
        }
    }
    
// <-- Priority Table -->
// 9000 	ReferenceMember
// 4000 	DelegateMember
// 3000 	StringMember
// 2000 	ListMember
// 2000 	DictionaryMember
// 1000 	ArrayMember
// 0		EnumMember
// 0		FloatMember
// 0		DoubleMember
// 0		DecimalMember
// 0		ByteMember
// 0		SByteMember
// 0		ShortMember
// 0		UShortMember
// 0		IntMember
// 0		UIntMember
// 0		LongMember
// 0		ULongMember
// 0		BoolMember
// 0		CharMember
// 0		Vector2Member
// 0		Vector3Member
// 0		Vector4Member
// 0		ColorMember
// 0		RectMember
// 0		QuaterionMember
// 0 		LayerMask
// -8000 	ClassMember
// -8000 	StructMember
// -9000 	InterfaceMember
}