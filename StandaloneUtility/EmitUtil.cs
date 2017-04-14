﻿#if !AOT_ONLY
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Expanse
{
    /// <summary>
    /// A collection of Reflection.Emit related utility functionality.
    /// </summary>
    public static class EmitUtil
    {
        private static bool useMeaningfulNames = true;
        private const string GENERATED_NAME = "GEN";

        public static bool UseMeaningfulNames
        {
            get { return useMeaningfulNames; }
            set { useMeaningfulNames = value; }
        }

        private static Module dynamicModule;
        private static Module DynamicModule
        {
            get
            {
                dynamicModule = dynamicModule ?? GenerateDynamicModule();
                return dynamicModule;
            }
        }

        public static void Prewarm()
        {
            dynamicModule = dynamicModule ?? GenerateDynamicModule();
        }

        private static Module GenerateDynamicModule()
        {
            AssemblyName dynamicAssemblyName = new AssemblyName("DynamicEmitUtilAssembly");
            AssemblyBuilder dynamicAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModuleBuilder = dynamicAssemblyBuilder.DefineDynamicModule("DynamicEmitUtilModule");
            return dynamicModuleBuilder.Assembly.GetModule("DynamicEmitUtilModule");
        }

        /// <summary>
        /// Returns a dynamically compiled method that calls a default constructor for a type.
        /// </summary>
        public static Func<TSource> GenerateDefaultConstructor<TSource>()
        {
            Type type = typeof(TSource);
            DynamicMethod constructorMethod = new DynamicMethod(useMeaningfulNames ? type.FullName + ".ctor" : GENERATED_NAME, type, new Type[0], DynamicModule);
            ILGenerator gen = constructorMethod.GetILGenerator();

            gen.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            gen.Emit(OpCodes.Ret);

            return (Func<TSource>)constructorMethod.CreateDelegate(typeof(Func<TSource>));
        }

        /// <summary>
        /// Returns a dynamically compiled method that gets the value of a field for a type.
        /// </summary>
        public static Func<TSource, TReturn> GenerateGetter<TSource, TReturn>(FieldInfo field)
        {
            DynamicMethod getterMethod = new DynamicMethod(useMeaningfulNames ? field.ReflectedType.FullName + ".get_" + field.Name : GENERATED_NAME, typeof(TReturn), new Type[1] { typeof(TSource) }, DynamicModule);
            ILGenerator gen = getterMethod.GetILGenerator();

            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldsfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
            }

            gen.Emit(OpCodes.Ret);
            return (Func<TSource, TReturn>)getterMethod.CreateDelegate(typeof(Func<TSource, TReturn>));
        }

        /// <summary>
        /// Returns a dynamically compiled method that sets the value of a field for a type.
        /// </summary>
        public static Action<TSource, TValue> GenerateSetter<TSource, TValue>(FieldInfo field)
        {
            DynamicMethod setterMethod = new DynamicMethod(useMeaningfulNames ? field.ReflectedType.FullName + ".set_" + field.Name : GENERATED_NAME, null, new Type[2] { typeof(TSource), typeof(TValue) }, DynamicModule);
            ILGenerator gen = setterMethod.GetILGenerator();

            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, field);
            }

            gen.Emit(OpCodes.Ret);
            return (Action<TSource, TValue>)setterMethod.CreateDelegate(typeof(Action<TSource, TValue>));
        }
    }
}
#endif