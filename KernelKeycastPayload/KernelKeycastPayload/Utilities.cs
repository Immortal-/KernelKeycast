using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace KernelKeycastPayload
{
    class Utilities
    { 
        public static TReturn CallApi<TReturn>(string name,string method, Type[] typeArray,params object[] arguments)
        {

                var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("temp"), AssemblyBuilderAccess.Run);
                var mb = asm.DefineDynamicModule("module");
                var meth = mb.DefinePInvokeMethod(method, name, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PinvokeImpl,
                    CallingConventions.Standard, typeof(TReturn), typeArray, CallingConvention.Winapi, CharSet.Ansi);
                meth.SetImplementationFlags(MethodImplAttributes.PreserveSig);
                mb.CreateGlobalFunctions();
                return (TReturn)mb.GetMethod(method).Invoke(null, arguments);
           
         
        }
    }
}
