using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public static class SceneObjectExtensions
{
	public static IntPtr? ExposedGetPointer( this SceneObject sceneObject )
	{
		if ( sceneObject is null ) return null;

		var nativeField = sceneObject.GetType().GetField( "native", BindingFlags.Instance | BindingFlags.NonPublic );
		var selfField = nativeField.FieldType.GetField( "self", BindingFlags.Instance | BindingFlags.NonPublic );

		var native = nativeField.GetValue( sceneObject );
		var self = selfField.GetValue( native );

		return (IntPtr)self;
	}
}
