using System;
using UnityEngine;
using Object = System.Object;

[AttributeUsage(AttributeTargets.Field)]
public class TypeFieldAttribute : PropertyAttribute
{
	#region Fields
	public readonly bool hideLabel = default;
	public readonly Type baseType = default;
	#endregion

	#region Constructors
	public TypeFieldAttribute (Type baseType = default, bool hideLabel = default)
	{
		this.hideLabel = hideLabel;
		this.baseType = baseType ?? typeof(Object);
	}
	#endregion
}