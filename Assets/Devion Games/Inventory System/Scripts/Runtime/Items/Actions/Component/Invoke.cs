using DevionGames.UIWidgets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [Icon("Component")]
    [ComponentMenu("Component/Invoke")]
    [System.Serializable]
    public class Invoke : ItemAction
    {
        [Tooltip("The component to invoke the method on")]
        [SerializeField]
        private string m_ComponentName = string.Empty;
        [Tooltip("The name of the field")]
        [SerializeField]
        private string m_MethodName = string.Empty;
        [Tooltip("Arguments for method.")]
        [SerializeField]
        private List<ArgumentVariable> m_Arguments=new List<ArgumentVariable>();
        [SerializeField]
        private NotificationOptions m_FailureNotification=null;

        public override ActionStatus OnUpdate()
        {
            var type = TypeUtility.GetType(m_ComponentName);
            if (type == null)
            {
                Debug.LogWarning("Unable to invoke - type is null");
                return ActionStatus.Failure;
            }

            var component = Player.GetComponent(type);
            if (component == null)
            {
                Debug.LogWarning("Unable to invoke with component " + m_ComponentName);
                return ActionStatus.Failure;
            }

            List<object> parameterList = new List<object>();
            List<Type> typeList = new List<Type>();
            for (int i = 0; i < m_Arguments.Count; i++) {
                ArgumentVariable argument = m_Arguments[i];
                if (!argument.IsNone)
                {
                    object value = argument.GetValue();
                    parameterList.Add(value);
                    typeList.Add(value.GetType());
                }
            }
           
            var methodInfo = component.GetType().GetMethod(this.m_MethodName, typeList.ToArray());

            if (methodInfo == null)
            {
                Debug.LogWarning("Unable to invoke method " + this.m_MethodName + " on component " + this.m_ComponentName);
                return ActionStatus.Failure;
            }
            bool? result = methodInfo.Invoke(component, parameterList.ToArray()) as bool?;
            if (result != null && !(bool)result) {
                if(!string.IsNullOrEmpty(this.m_FailureNotification.text))
                    this.m_FailureNotification.Show();
                return ActionStatus.Failure;
            }
            return ActionStatus.Success;
        }
    }
}