using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace HimonoLib
{
    public class EnumListLabelAttribute : PropertyAttribute
    {
        public System.Type m_enumType  = null;

        public EnumListLabelAttribute( System.Type i_type )
        {
            m_enumType  = i_type;
        }

    } // class EnumListLabelAttribute


#if UNITY_EDITOR

    [CustomPropertyDrawer( typeof( EnumListLabelAttribute ) )]
    public class EnumListLabelDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect i_position, SerializedProperty i_property, GUIContent i_label )
        {
            string  enumText    = null;

            string  countText   = i_label.text.Replace( "Element ", "" );
            int     enumValue   = int.MaxValue;
            if( int.TryParse( countText, out enumValue ) )
            {
                enumText    = GetEnumName( enumValue );
            }

            if( !string.IsNullOrEmpty( enumText ) )
            {
                i_label.text = enumText;
            }

            EditorGUI.PropertyField( i_position, i_property, i_label, true );
        }


        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property );
        }


        private EnumListLabelAttribute MyAttribute
        {
            get
            {
                return (EnumListLabelAttribute)attribute;
            }
        }


        private int EnumSize
        {
            get
            {
                return System.Enum.GetValues( MyAttribute.m_enumType ).Length;
            }
        }


        private string GetEnumName( int i_index )
        {
            if( System.Enum.IsDefined( MyAttribute.m_enumType, i_index ) )
            {
                return System.Enum.GetName( MyAttribute.m_enumType, i_index );
            }
            return null;
        }


    } // class EnumListLabelDrawer


#endif // UNITY_EDITOR


} // namespace HimonoLib

