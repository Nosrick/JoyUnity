using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace DevionGames
{
	public static class AttributeUtility
	{
		private readonly static Dictionary<Type, object[]> typeAttributeCache;
		private readonly static Dictionary<FieldInfo, object[]> fieldAttributeCache;
		private readonly static Dictionary<Type, Texture2D> iconCache;

		static AttributeUtility()
		{
			AttributeUtility.typeAttributeCache = new Dictionary<Type, object[]>();
			AttributeUtility.fieldAttributeCache = new Dictionary<FieldInfo, object[]>();
			AttributeUtility.iconCache = new Dictionary<Type, Texture2D>();
		}

		public static object[] GetCustomAttributes(Type type)
		{
			object[] customAttributes;
			if (!AttributeUtility.typeAttributeCache.TryGetValue(type, out customAttributes))
			{
				customAttributes = type.GetCustomAttributes(true);
				AttributeUtility.typeAttributeCache.Add(type, customAttributes);
			}
			return customAttributes;
		}

		public static object[] GetCustomAttributes(FieldInfo field)
		{
			object[] customAttributes;
			if (!AttributeUtility.fieldAttributeCache.TryGetValue(field, out customAttributes))
			{
				customAttributes = field.GetCustomAttributes(true);
				AttributeUtility.fieldAttributeCache.Add(field, customAttributes);
			}
			return customAttributes;
		}

		public static bool IsSerialized(this FieldInfo field)
		{
			object[] objArray = AttributeUtility.GetCustomAttributes(field);
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				if (objArray[i] is SerializeField)
				{
					return true;
				}
			}
			return field.IsPublic && !field.IsNotSerialized;
		}


		public static T GetAttribute<T>(this Type type)
		{
			object[] objArray = AttributeUtility.GetCustomAttributes(type);
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				if (objArray[i].GetType() == typeof(T) || objArray[i].GetType().IsSubclassOf(typeof(T)))
				{
					return (T)objArray[i];
				}
			}
			return default(T);
		}

		public static T GetAttribute<T>(this FieldInfo field)
		{
			object[] objArray = AttributeUtility.GetCustomAttributes(field);
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				if (objArray[i].GetType() == typeof(T) || objArray[i].GetType().IsSubclassOf(typeof(T)))
				{
					return (T)objArray[i];
				}
			}
			return default(T);
		}

		public static bool HasAttribute<T>(this FieldInfo field)
		{
			return field.HasAttribute(typeof(T));
		}

		public static bool HasAttribute(this FieldInfo field, Type attributeType)
		{
			object[] objArray = AttributeUtility.GetCustomAttributes(field);
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				if (objArray[i].GetType() == attributeType || objArray[i].GetType().IsSubclassOf(attributeType))
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasAttribute(this MemberInfo field, Type attributeType)
		{
			object[] objArray = field.GetCustomAttributes(true);
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				if (objArray[i].GetType() == attributeType || objArray[i].GetType().IsSubclassOf(attributeType))
				{
					return true;
				}
			}
			return false;
		}

		public static string GetCategory(this object obj)
		{
			return GetCategory(obj.GetType());
		}

		public static string GetCategory(this Type type)
		{
			object[] objArray = AttributeUtility.GetCustomAttributes(type);
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				CategoryAttribute categoryAttribute = objArray[i] as CategoryAttribute;
				if (categoryAttribute != null)
				{
					return categoryAttribute.Category;
				}
			}
			return string.Empty;
		}

		public static string GetTooltip(this object obj)
		{
			return AttributeUtility.GetTooltip(obj.GetType());
		}

		public static string GetTooltip(this Type type)
		{
			return AttributeUtility.GetTooltip(AttributeUtility.GetCustomAttributes(type));
		}

		public static string GetTooltip(this FieldInfo field)
		{
			return AttributeUtility.GetTooltip(AttributeUtility.GetCustomAttributes(field));
		}

		public static string GetTooltip(object[] attributes)
		{
			object[] objArray = attributes;
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				TooltipAttribute tooltipAttribute = objArray[i] as TooltipAttribute;
				if (tooltipAttribute != null)
				{
					return tooltipAttribute.Text;
				}
			}
			return string.Empty;
		}

		public static string GetHelpUrl(this object obj)
		{
			return GetHelpUrl(obj.GetType());
		}

		public static string GetHelpUrl(this Type type)
		{
			object[] objArray = AttributeUtility.GetCustomAttributes(type);
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				HelpURLAttribute infoAttribute = objArray[i] as HelpURLAttribute;
				if (infoAttribute != null)
				{
					return infoAttribute.URL;
				}
			}
			return string.Empty;
		}

		public static string GetObsoleteMessage(this object obj)
		{
			return GetObsoleteMessage(obj.GetType());
		}

		public static string GetObsoleteMessage(this Type type)
		{
			object[] objArray = AttributeUtility.GetCustomAttributes(type);
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				ObsoleteAttribute infoAttribute = objArray[i] as ObsoleteAttribute;
				if (infoAttribute != null)
				{
					return infoAttribute.Message;
				}
			}
			return string.Empty;
		}

#if UNITY_EDITOR
		public static Texture2D GetIcon(this Type type)
		{
			Texture2D icon = null;
			if (!iconCache.TryGetValue(type, out icon))
			{
				object[] objArray = AttributeUtility.GetCustomAttributes(type);
				for (int i = 0; i < (int)objArray.Length; i++)
				{
					IconAttribute infoAttribute = objArray[i] as IconAttribute;
					if (infoAttribute != null)
					{

						icon = Resources.Load<Texture2D>(infoAttribute.path);

						if (icon == null)
						{
							icon = UnityEditor.EditorGUIUtility.FindTexture(infoAttribute.path);
						}

						if (icon == null && infoAttribute.type != null)
						{
							icon = (Texture2D)UnityEditor.EditorGUIUtility.ObjectContent(null, infoAttribute.type).image;
						}
						if (icon == null)
						{
							string category = type.GetCategory().Replace("/", ".");
							icon = (Texture2D)UnityEditor.EditorGUIUtility.ObjectContent(null, TypeUtility.GetType(category)).image;
						}

						iconCache.Add(type, icon);
						break;
					}
				}


			}
			return icon;
		}
#endif
	}
}