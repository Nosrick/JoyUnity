using UnityEngine;

namespace Tools
{
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        // Interface type.
        public System.Type RequiredType { get; protected set; }
        /// <summary>
        /// Requiring implementation of the <see cref="T:RequireInterfaceAttribute"/> interface.
        /// </summary>
        /// <param name="type">Interface type.</param>
        public RequireInterfaceAttribute(System.Type type)
        {
            this.RequiredType = type;
        }
    }
}