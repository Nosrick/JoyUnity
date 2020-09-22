﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [Icon("Time")]
    public class Wait : ItemAction
    {
        [SerializeField]
        private float duration = 1f;

        private float m_Time = 0f;

        public override void OnStart()
        {
            this.m_Time = 0f;
        }

        public override ActionStatus OnUpdate()
        {
            this.m_Time += Time.deltaTime;
            if (this.m_Time > duration)
            {
                return ActionStatus.Success;
            }
            return ActionStatus.Running;
        }
    }
}