using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoTestMate.NUnit.Infrastructure.Helpers
{
	public static class AttributeLifeCycleHelper
    {
        private static readonly Dictionary<Type, Type> InitializedClassAttributes = new Dictionary<Type, Type>();

        private static readonly Dictionary<MethodInfo, Type> InitializedMethodAttributes = new Dictionary<MethodInfo, Type>();

        public static void InitializeClassAttribute(Type classType, Type attributeType)
        {
            if (!ClassAttributeInitialized(classType, attributeType))
            {
                InitializedClassAttributes.Add(classType, attributeType);
            }
            else
            {
                throw new ApplicationException($"Unable to initialize class attribute - {attributeType.Name} for class - {classType.Name} the attribute has already been initialized");
            }
            
        }

        public static bool ClassAttributeInitialized(Type classType, Type attributeType)
        {
            return InitializedClassAttributes.Any(x => x.Key == classType && x.Value == attributeType);
        }

        public static void InitializeMethodAttribute(MethodInfo method, Type attributeType)
        {
            if (!MethodAttributeInitialized(method, attributeType))
            {
                InitializedMethodAttributes.Add(method, attributeType);
            }
            else
            {
                throw new ApplicationException($"Unable to initialize method attribute - {attributeType.Name} for method - {method.Name} the attribute has already been initialized");
            }
        }

        public static bool MethodAttributeInitialized(MethodInfo method, Type attributeType)
        {
            return InitializedMethodAttributes.Any(x => x.Key == method && x.Value == attributeType);
        }
    }
}
