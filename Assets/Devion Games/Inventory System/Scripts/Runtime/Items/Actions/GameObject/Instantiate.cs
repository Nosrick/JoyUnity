using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Instantiate")]
    public class Instantiate : ItemAction
    {
        [SerializeField]
        private GameObject m_Original = null;
        [SerializeField]
        private string m_BoneName = string.Empty;
        [SerializeField]
        private bool m_IgnorePlayerCollision = true;

        private Transform m_Bone;
        private Transform m_CameraTransform;

        public override void OnSequenceStart()
        {
            this.m_CameraTransform = Camera.main.transform;
            this.m_Bone = FindBone(Player.transform, this.m_BoneName);
            if (this.m_Bone == null)
                this.m_Bone = Player.transform;
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Original == null)
            {
                Debug.LogWarning("The game object you want to instantiate is null.");
                return ActionStatus.Failure;
            }

           GameObject go = GameObject.Instantiate(m_Original, this.m_Bone.position, this.m_Bone.rotation, this.m_Bone);
            if (m_IgnorePlayerCollision) {
                UnityUtility.IgnoreCollision(Player, go);
            }
            return ActionStatus.Success;
        }

        public Transform FindBone(Transform current, string name)
        {
            if (current.name == name)
                return current;
            for (int i = 0; i < current.childCount; ++i)
            {
                Transform found = FindBone(current.GetChild(i), name);
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}
