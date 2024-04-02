using System;
using System.Linq;
using System.Reflection;

namespace MyProject.Core.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class SignalAttribute : Attribute { } 

    public static class CustomAttributeProcessor
    {
        public static Type[] GetSignalClasses()
        {
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();--> yürütülen uygulamanin tüm assemblyleri icin
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            Type[] observerClasses = types.Where(type => type.IsDefined(typeof(SignalAttribute), false)).ToArray();
            return observerClasses;
        }
    }
}


