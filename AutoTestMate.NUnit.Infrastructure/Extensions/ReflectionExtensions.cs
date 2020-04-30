using System.Linq;
using System.Reflection;

namespace AutoTestMate.MsTest.Infrastructure.Extensions
{
	/// <summary>
	/// Contains object extensions relating to reflection.
	/// </summary>
	public static class ReflectionExtensions
	{
		private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		#region Get Property Value

		private static object GetValueProperty(object o, string propertyName, BindingFlags bindingAttr)
		{
			return o?.GetType().GetProperty(propertyName, bindingAttr)?.GetValue(o);
		}

		/// <summary>
		/// Gets the value of a property of the supplied object using reflection.
		/// </summary>
		/// <param name="o">The object whose property value is to be retrieved.</param>
		/// <param name="propertyName">The name of the property (case sensitive).</param>
		/// <returns>The value of the property, or null if it doesn't exist in the object.</returns>
		public static object GetPropertyValue(this object o, string propertyName)
		{
			return propertyName.Split('.').Aggregate(o, (current, name) => GetValueProperty(current, name, DefaultBindingFlags));
		}

		/// <summary>
		/// Gets the value of a property of the supplied object using reflection.
		/// </summary>
		/// <param name="o">The object whose property value is to be retrieved.</param>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="bindingAttr">Searches for the property using the specified binding constraints.</param>
		/// <returns>The value of the property, or null if it doesn't exist in the object.</returns>
		public static object GetPropertyValue(this object o, string propertyName, BindingFlags bindingAttr)
		{
			return propertyName.Split('.').Aggregate(o, (current, name) => GetValueProperty(current, name, bindingAttr));
		}

		/// <summary>
		/// Gets the value of a property of the supplied object using reflection, and returns it as
		/// the specified type.
		/// </summary>
		/// <typeparam name="T">The return type of the property value.</typeparam>
		/// <param name="o">The object whose property value is to be retrieved.</param>
		/// <param name="propertyName">The name of the property (case sensitive).</param>
		/// <returns>The value of the property, or the default value for the type if the
		/// property doesn't exist in the object.</returns>
		public static T GetPropertyValue<T>(this object o, string propertyName)
		{
			return (T)o.GetPropertyValue(propertyName);
		}

		/// <summary>
		/// Gets the value of a property of the supplied object using reflection, and returns it as
		/// the specified type.
		/// </summary>
		/// <typeparam name="T">The return type of the property value.</typeparam>
		/// <param name="o">The object whose property value is to be retrieved.</param>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="bindingAttr">Searches for the property using the specified binding constraints.</param>
		/// <returns>The value of the property, or the default value for the type if the
		/// property doesn't exist in the object.</returns>
		public static T GetPropertyValue<T>(this object o, string propertyName, BindingFlags bindingAttr)
		{
			return (T)o.GetPropertyValue(propertyName, bindingAttr);
		}

		#endregion

		#region Get Field Value

		private static object GetValueField(this object o, string fieldName, BindingFlags bindingAttr)
		{
			return o?.GetType().GetField(fieldName, bindingAttr)?.GetValue(o);
		}

		/// <summary>
		/// Gets the value of a field of the supplied object using reflection.
		/// </summary>
		/// <param name="o">The object whose field value is to be retrieved.</param>
		/// <param name="fieldName">The name of the field (case sensitive).</param>
		/// <returns>The value of the field, or null if it doesn't exist in the object.</returns>
		public static object GetFieldValue(this object o, string fieldName)
		{
			return fieldName.Split('.').Aggregate(o, (current, name) => GetValueField(current, name, DefaultBindingFlags));
		}

		/// <summary>
		/// Gets the value of a field of the supplied object using reflection.
		/// </summary>
		/// <param name="o">The object whose field value is to be retrieved.</param>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="bindingAttr">Searches for the field using the specified binding constraints.</param>
		/// <returns>The value of the field, or null if it doesn't exist in the object.</returns>
		public static object GetFieldValue(this object o, string fieldName, BindingFlags bindingAttr)
		{
			return fieldName.Split('.').Aggregate(o, (current, name) => GetValueField(current, name, bindingAttr));
		}

		/// <summary>
		/// Gets the value of a field of the supplied object using reflection, and returns it as
		/// the specified type.
		/// </summary>
		/// <typeparam name="T">The return type of the field value.</typeparam>
		/// <param name="o">The object whose field value is to be retrieved.</param>
		/// <param name="fieldName">The name of the field (case sensitive).</param>
		/// <returns>The value of the field, or the default value for the type if the
		/// field doesn't exist in the object.</returns>
		public static T GetFieldValue<T>(this object o, string fieldName)
		{
			return (T) o.GetFieldValue(fieldName);
		}

		/// <summary>
		/// Gets the value of a field of the supplied object using reflection, and returns it as
		/// the specified type.
		/// </summary>
		/// <typeparam name="T">The return type of the field value.</typeparam>
		/// <param name="o">The object whose field value is to be retrieved.</param>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="bindingAttr">Searches for the field using the specified binding constraints.</param>
		/// <returns>The value of the field, or the default value for the type if the
		/// field doesn't exist in the object.</returns>
		public static T GetFieldValue<T>(this object o, string fieldName, BindingFlags bindingAttr)
		{
			return (T) o.GetFieldValue(fieldName, bindingAttr);
		}

		#endregion

		#region Member

		private static object GetValueMember(object o, string propertyName, BindingFlags bindingAttr)
		{
			object result;
			MemberInfo[] member = o?.GetType().GetMember(propertyName, MemberTypes.Property | MemberTypes.Field, bindingAttr);
			if (member != null && member.Any(m => m is PropertyInfo))
				result = ((PropertyInfo) member.First(m => m is PropertyInfo)).GetValue(o);
			else if (member != null && member.Any(m => m is FieldInfo))
				result = ((FieldInfo) member.First(m => m is FieldInfo)).GetValue(o);
			else
				result = null;
			return result;
		}

		/// <summary>
		/// Gets the value of a property or field of the supplied object using reflection.
		/// </summary>
		/// <param name="o">The object whose property value is to be retrieved.</param>
		/// <param name="memberName">The name of the property or field (case sensitive).</param>
		/// <returns>The value of the property or field, or null if it doesn't exist in the object.</returns>
		public static object GetMemberValue(this object o, string memberName)
		{
			return memberName.Split('.').Aggregate(o, (current, name) => GetValueMember(current, name, DefaultBindingFlags));
		}

		/// <summary>
		/// Gets the value of a property or field of the supplied object using reflection.
		/// </summary>
		/// <param name="o">The object whose property value is to be retrieved.</param>
		/// <param name="memberName">The name of the property or field.</param>
		/// <param name="bindingAttr">Searches for the property or field using the specified binding constraints.</param>
		/// <returns>The value of the property, or null if it doesn't exist in the object.</returns>
		public static object GetMemberValue(this object o, string memberName, BindingFlags bindingAttr)
		{
			return memberName.Split('.').Aggregate(o, (current, name) => GetValueMember(current, name, bindingAttr));
		}

		/// <summary>
		/// Gets the value of a property or field of the supplied object using reflection, and returns it as
		/// the specified type.
		/// </summary>
		/// <typeparam name="T">The return type of the property or field value.</typeparam>
		/// <param name="o">The object whose property or field value is to be retrieved.</param>
		/// <param name="memberName">The name of the property or field (case sensitive).</param>
		/// <returns>The value of the property, or the default value for the type if the
		/// property doesn't exist in the object.</returns>
		public static T GetMemberValue<T>(this object o, string memberName)
		{
			return (T)o.GetMemberValue(memberName);
		}

		/// <summary>
		/// Gets the value of a property or field of the supplied object using reflection, and returns it as
		/// the specified type.
		/// </summary>
		/// <typeparam name="T">The return type of the property or field value.</typeparam>
		/// <param name="o">The object whose property or field value is to be retrieved.</param>
		/// <param name="memberName">The name of the property or field.</param>
		/// <param name="bindingAttr">Searches for the property or field using the specified binding constraints.</param>
		/// <returns>The value of the property, or the default value for the type if the
		/// property or field doesn't exist in the object.</returns>
		public static T GetMemberValue<T>(this object o, string memberName, BindingFlags bindingAttr)
		{
			return (T)o.GetMemberValue(memberName, bindingAttr);
		}

		#endregion
	}
}
