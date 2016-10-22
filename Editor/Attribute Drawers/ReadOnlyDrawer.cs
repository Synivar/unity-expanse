﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Expanse
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        const float DEFAULT_HEIGHT = 16;
        const float VECTOR2_HEIGHT = DEFAULT_HEIGHT + 14;

        RangeAttribute rangeAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsReadOnly(attribute as ReadOnlyAttribute))
                GUI.enabled = false;

            // Check if a Range attribute is also defined on the field
            if (fieldInfo.GetAttribute<RangeAttribute>(out rangeAttribute))
                RangeOnGUI(position, property, label);
            else if (property.propertyType == SerializedPropertyType.Vector2)
                Vector2GUI(position, property, label);
            else
                EditorGUI.PropertyField(position, property, label, property.hasVisibleChildren);

            GUI.enabled = true;
        }

        private void Vector2GUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect extRect = position.SetSize(position.width, VECTOR2_HEIGHT);
            EditorGUI.PropertyField(extRect, property, label, property.hasVisibleChildren);
        }

        private void RangeOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(position, property, rangeAttribute.min, rangeAttribute.max, label);
            }
            else
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                {
                    EditorGUI.IntSlider(position, property, (int)rangeAttribute.min, (int)rangeAttribute.max, label);
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
                return VECTOR2_HEIGHT;
            return base.GetPropertyHeight(property, label);
        }

        // Given a readonly attribute determine if it should currently be readonly
        private static bool IsReadOnly(ReadOnlyAttribute readonlyAtt)
        {
            return (Application.isPlaying && !readonlyAtt.EditableInPlayMode) ||
                (!Application.isPlaying && !readonlyAtt.EditableInEditor);
        }

        // Apply readonly state if it should be
        public static void BeginReadOnlyCheck(FieldInfo fieldMember)
        {
            if (fieldMember.IsDefined(typeof(ReadOnlyAttribute), true))
                GUI.enabled = !IsReadOnly((ReadOnlyAttribute)fieldMember.GetCustomAttributes(typeof(ReadOnlyAttribute), true)[0]);
            else
                GUI.enabled = true;
        }

        // Turn-off read only state
        public static void EndReadOnlyCheck()
        {
            GUI.enabled = true;
        }
    }
}