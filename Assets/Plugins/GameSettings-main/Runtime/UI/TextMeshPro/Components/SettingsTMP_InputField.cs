﻿using TMPro;
using UnityEngine;

namespace GameSettings.UI.TMPro
{
    [RequireComponent(typeof(TMP_InputField))]
    [AddComponentMenu("UI/Game Settings/Settings Input Field - TextMeshPro")]
    public class SettingsTMP_InputField : SettingsSelectable
    {
        public TMP_InputField tmp_InputField => (TMP_InputField)selectable;
        public ISettingTMP_InputFieldInterpreter tmp_InputFieldInterpreter => (ISettingTMP_InputFieldInterpreter)selectableInterpreter;

        protected virtual void OnEnable()
        {
            if(tmp_InputFieldInterpreter != null)
            {
                tmp_InputField.onValidateInput += tmp_InputFieldInterpreter.ValidateInput;
                tmp_InputField.onValueChanged.AddListener(tmp_InputFieldInterpreter.ValueChanged);
                tmp_InputField.onEndEdit.AddListener(tmp_InputFieldInterpreter.EndedEdit);
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            tmp_InputFieldInterpreter?.UpdateView(tmp_InputField);
        }

        protected virtual void OnDisable()
        {
            if(tmp_InputFieldInterpreter != null)
            {
                tmp_InputField.onValidateInput -= tmp_InputFieldInterpreter.ValidateInput;
                tmp_InputField.onValueChanged.RemoveListener(tmp_InputFieldInterpreter.ValueChanged);
                tmp_InputField.onEndEdit.RemoveListener(tmp_InputFieldInterpreter.EndedEdit);
            }
        }

        public override void ResetUI()
        {
            base.ResetUI();
            tmp_InputFieldInterpreter?.ResetUI(tmp_InputField);
        }
    }
}
