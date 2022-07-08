using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace CSObjectWrapEditor
{
    public class LuaNotationGenerator
    {
        class LuaClassSupport
        { 
            public Type ClassType; // 类名
            public Type BaseClassType; //继承的类名
            public List<FieldInfo> FieldInfos = new List<FieldInfo>();
            public List<PropertyInfo> PropertyInfos = new List<PropertyInfo>();
            public List<MethodInfo> MethodInfos = new List<MethodInfo>();

            public void Export(StringBuilder sb)
            {
                sb.AppendLine("do");
                if (ClassType.IsGenericType)
                {
                    sb.AppendLine($"\t---{GetGenericTypeDesc(ClassType)}");
                }

                if (BaseClassType != null)
                {
                    sb.AppendLine($"\t---@class {GetTypeName(ClassType)} : {GetTypeName(BaseClassType)}");
                }
                else
                {
                    sb.AppendLine($"\t---@class {GetTypeName(ClassType)}");
                }

                foreach (var field in FieldInfos)
                {
                    if (field.IsStatic)
                    {
                        sb.AppendLine(
                            $"\t---@field {GetModifier(field)} {field.Name} {GetTypeName(field.FieldType)} @[static]");
                    }
                    else
                    {
                        sb.AppendLine($"\t---@field {GetModifier(field)} {field.Name} {GetTypeName(field.FieldType)}");
                    }
                }

                foreach (var property in PropertyInfos)
                {
                    //if (property.IsStatic())
                    //{
                    //    sb.AppendLine($"\t---@field {GetModifier(property)} {property.Name} {GetTypeName(property.PropertyType)} @[static]");
                    //}
                    //else
                    //{
                        sb.AppendLine($"\t---@field {GetModifier(property)} {property.Name} {GetTypeName(property.PropertyType)}");
                    //}
                }
                sb.AppendLine($"\tlocal LuaClass = {{}}");
                
                foreach (var method in MethodInfos)
                {
                    foreach (var parameter in method.GetParameters())
                    {
                        if (parameter.ParameterType.IsGenericParameter)
                            sb.AppendLine($"\t---@generic {parameter.ParameterType}");
                        sb.AppendLine($"\t---@param {parameter.Name} {GetTypeName(parameter.ParameterType)}");
                    }

                    if (method.ReturnType.IsGenericParameter || method.ReturnType.IsArray &&
                        method.ReturnType.GetElementType().IsGenericParameter)
                        sb.AppendLine($"\t---@generic {method.ReturnType}");
                    sb.AppendLine($"\t---@return {GetTypeName(method.ReturnType)}");
                    if (method.IsStatic)
                    {
                        sb.AppendLine($"\tfunction LuaClass.{method.Name}({string.Join(", ", method.GetParameters().Select(p => GetValidName(p.Name)))}) end");
                    }
                    else
                    {
                        sb.AppendLine($"\tfunction LuaClass:{method.Name}({string.Join(", ", method.GetParameters().Select(p => GetValidName(p.Name)))}) end");
                    }
                }
                sb.AppendLine("end");
            }
        }

        class LuaNamespaceSupport
        {
            public string ClassName;
            public Dictionary<string, string> PropertyDic = new Dictionary<string, string>();

            public void Export(StringBuilder sb)
            {
                sb.AppendLine("do");
                sb.AppendLine($"\t---@class {ClassName}");
                foreach (var key in PropertyDic.Keys)
                {
                    sb.AppendLine($"\t---@field {key} {PropertyDic[key]}");
                }
                sb.AppendLine($"\tlocal LuaNameSpeace = {{}}");
                sb.AppendLine("end");
            }
        }
        
        //记录所有依赖的类型
        private static HashSet<Type> Dependency = new HashSet<Type>();
        //记录所有要生成的Type
        private static HashSet<Type> GeneratedTypeHashSet = new HashSet<Type>();
        //记录需要生成的命名空间
        private static Dictionary<string, LuaNamespaceSupport> luaNamespaceSupportDic = new Dictionary<string, LuaNamespaceSupport>();
        
        // 保存引用到的参数类型，这些类型的详细信息不需要导出到 TS(Lua) 中, 只需要输出类型即可!
        private static List<Type> listRefParamTypes = new List<Type>();
        
        public static string GetModifier(FieldInfo info)
        {
            if (info.IsPublic) return "public";
            if (info.IsPrivate) return "private";
            return "protected";
        }

        public static string GetModifier(PropertyInfo info)
        {
            if (info.GetMethod != null && info.GetMethod.IsPublic) return "public";
            if (info.SetMethod != null && info.SetMethod.IsPublic) return "public";
            if (info.GetMethod != null && info.GetMethod.IsPrivate) return "private";
            if (info.SetMethod != null && info.SetMethod.IsPrivate) return "private";
            return "public";
        }

        /// <summary>
        ///     获取类型的名称描述
        /// </summary>
        /// <param name="t">要获取描述的类型</param>
        /// <param name="dep"></param>
        /// <param name="ts">是否是获取 TS 的描述</param>
        /// <returns></returns>
        public static string GetTypeName(Type t, bool dep = true, bool ts = false)
        {
            if (dep)
            {
                Dependency.Add(t);
                if (t.IsByRef)
                    Dependency.Add(t.GetElementType());
                if (t.IsArray)
                    Dependency.Add(t.GetElementType());
            }

            if (t.IsArray) return GetTypeName(t.GetElementType(), dep, ts) + "[]";

            if (t.IsByRef)
                return GetTypeName(t.GetElementType(), dep, ts);

            if (t.IsGenericParameter)
                return t.Name;

            if (typeof(Delegate).IsAssignableFrom(t))
            {
                if (ts && (t == typeof(UnityAction) || t == typeof(Action)))
                    // TS 直接返回全名即可
                    return t.FullName;

                var method = t.GetMethod("Invoke");
                if (ts)
                    // 输出CS用的函数类型
                    return
                        $"({string.Join(", ", method.GetParameters().Select(p => p.Name + ":" + GetTypeName(p.ParameterType, dep, ts)))}) => {GetTypeName(method.ReturnType, dep)}";
                return
                    $"fun({string.Join(", ", method.GetParameters().Select(p => p.Name + ":" +  GetTypeName(p.ParameterType, dep, ts)))}):{GetTypeName(method.ReturnType, dep)}";
            }

            if (t.IsGenericType)
            {
                var tName = t.Name;
                if (t.Name.Contains('`')) tName = t.Name.Substring(0, t.Name.IndexOf('`'));
                
                return
                    $"{tName}_{string.Join("_", t.GenericTypeArguments.Select(p => GetTypeName(p, dep, ts)))}".Replace(".", "");
            }

            var name = "void";
            if (t == typeof(void))
                return name;
            if (t == typeof(int) || t == typeof(uint) || t == typeof(byte) || t == typeof(short) ||
                t == typeof(ushort) || t == typeof(long) || t == typeof(ulong) || t == typeof(float) ||
                t == typeof(double))
                name = "number";
            else if (t == typeof(bool))
                name = "boolean";
            else if (t == typeof(string))
                name = "string";
            else
                name = t.FullName;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Replace("+", ".");
                name = name.Replace("&", "");
            }

            return name;
        }

        // 获取描述
        private static string GetGenericTypeDesc(Type type)
        {
            var tName = type.Name;
            if (type.Name.Contains('`')) 
                tName = type.Name.Substring(0, type.Name.IndexOf('`'));
            return $"\t---{tName}<{string.Join(", ", type.GenericTypeArguments.Select(p => GetTypeName(p)))}>";
        }

        public static string GetValidName(string name)
        {
            if (name == "and" || name == "elseif" || name == "end" || name == "function" ||
                name == "local" || name == "nil" || name == "repeat" || name == "until")
                return "_" + name;
            return name;
        }
        
        
        [GenCodeMenu]
        public static void GenerateLuaNotation()
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                // 收集所有要生成的Type
                CollectAllGeneratedType();
                // 导出LuaClass
                ExportLuaClass(sb1);
                // 导出LuaEnum
                ExportLuaEnum(sb2);
                // 导出命名空间
                ExportLuaNamespace(sb2);
                //导出依赖的类
                ExportDependency(sb2);
                //导出需要的声明
                ExportHelpDefine(sb2);
                //写文件
                File.WriteAllText(Path.Combine(LuaManager.LuaRootPath, "exported1.lua"), sb1.ToString());
                File.WriteAllText(Path.Combine(LuaManager.LuaRootPath, "exported2.lua"), sb2.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            // 清空数据
            Clean();
        }

        // 清空数据
        private static void Clean()
        {
            Dependency.Clear();
            luaNamespaceSupportDic.Clear();
        }

        // 收集所有要生成的Type
        private static void CollectAllGeneratedType()
        {
            GeneratedTypeHashSet.Clear();
            foreach (var type in Generator.LuaCallCSharp.Distinct())
            {
                if (!GeneratedTypeHashSet.Contains(type))
                {
                    GeneratedTypeHashSet.Add(type);   
                }
            }
        }
        
        // 获取要生成的Class信息
        private static LuaClassSupport GetGenLuaClassSuportInfo(Type type)
        {
             LuaClassSupport luaClassSupport = new LuaClassSupport();
             luaClassSupport.ClassType = type;
                 
             void ExportType(Type tempType)
             {
                 if (tempType == null)
                 {
                     return;
                 }
                 
                 foreach (var field in tempType.GetFields())
                {
                    if (field.FieldType.IsGenericType || field.DeclaringType != tempType || Generator.isMemberInBlackList(field))
                        continue;
                    luaClassSupport.FieldInfos.Add(field);
                }

                foreach (var property in tempType.GetProperties())
                {
                    if (property.PropertyType.IsGenericType || property.DeclaringType != tempType || Generator.isMemberInBlackList(property))
                        continue;
                    luaClassSupport.PropertyInfos.Add(property);
                }

                foreach (var method in tempType.GetMethods())
                {
                    if (method.IsSpecialName || method.IsPrivate || method.DeclaringType != tempType || 
                        Generator.isObsolete(method) || Generator.isMethodInBlackList(method))
                        continue;

                    if (method.ReturnType.IsGenericType || method.GetParameters().Any(p =>
                        p.ParameterType.IsGenericType && !typeof(Delegate).IsAssignableFrom(p.ParameterType)))
                        continue;
                    luaClassSupport.MethodInfos.Add(method);
                }
                
                // 添加扩展方法
                var extension_methods = tempType.IsInterface ? new MethodInfo[0]:Generator.GetExtensionMethods(tempType).ToArray();
                foreach (var method in extension_methods)
                {
                    if (Generator.isObsolete(method) || Generator.isMethodInBlackList(method))
                        continue;
                    luaClassSupport.MethodInfos.Add(method);
                }
                
                if (tempType.BaseType != null)
                {
                    if (GeneratedTypeHashSet.Contains(tempType.BaseType))
                    {
                        luaClassSupport.BaseClassType = tempType.BaseType;
                    }
                    else
                    {
                        ExportType(tempType.BaseType);
                    }
                }
             }
             
             ExportType(type);
             return luaClassSupport;
        }

        // 导出LuaClass
        private static void ExportLuaClass(StringBuilder sb)
        {
            foreach (var type in GeneratedTypeHashSet.Where(type => !type.IsEnum).Distinct())
            {
                LuaClassSupport support = GetGenLuaClassSuportInfo(type);
                support.Export(sb);
                AddLuaNamespceSupport(support.ClassType);
            }
        }

        // 导出LuaEnum
        private static void ExportLuaEnum(StringBuilder sb)
        {
            foreach (var e in GeneratedTypeHashSet.Where(type => type.IsEnum).Distinct())
            {
                sb.AppendLine("do");
                sb.AppendLine($"\t---@class {GetTypeName(e)}");
                foreach (var field in e.GetFields())
                {
                    if (!field.IsStatic)
                        continue;
                    sb.AppendLine($"\t---@field public {GetValidName(field.Name)} number");
                }
                sb.AppendLine($"\tlocal LuaEnum = {{}}");
                sb.AppendLine("end");
                
                AddLuaNamespceSupport(e);
            }
        }

        // 增加命名空间索引，方便读取
        private static void AddLuaNamespceSupport(Type type)
        {
            List<string> namespaces = new List<string>();
            namespaces.Add("CS");
            if (!string.IsNullOrEmpty(type.Namespace))
            {
                foreach (var name in type.Namespace.Split('.'))
                {
                    namespaces.Add(name);
                }
            }
            namespaces.Add(type.Name);

            for (int i = 0; i < namespaces.Count - 1; i++)
            {
                string className = namespaces[i];
                if (!luaNamespaceSupportDic.ContainsKey(className))
                {
                    LuaNamespaceSupport support = new LuaNamespaceSupport();
                    support.ClassName = className;
                    luaNamespaceSupportDic.Add(className, support);
                }
                
                string propertyName = namespaces[i + 1];
                if (!luaNamespaceSupportDic[className].PropertyDic.ContainsKey(propertyName))
                {
                    if (propertyName == type.Name)
                    {
                        luaNamespaceSupportDic[className].PropertyDic.Add(propertyName, GetTypeName(type));
                    }
                    else
                    {
                        luaNamespaceSupportDic[className].PropertyDic.Add(propertyName, propertyName);   
                    }
                }
            }
        }

        //导出命名空间
        private static void ExportLuaNamespace(StringBuilder sb)
        {
            foreach (var support in luaNamespaceSupportDic.Values)
            {
                support.Export(sb);
            }
        }

        private static bool IsBuildinType(Type t)
        {
            if (t == typeof(int) || t == typeof(uint) || t == typeof(byte) || t == typeof(short) ||
                t == typeof(ushort) || t == typeof(long) || t == typeof(ulong) || t == typeof(float) ||
                t == typeof(double))
                return true;
            if (t == typeof(bool))
                return true;
            if (t == typeof(string))
                return true;

            return false;
        }
        
        // 导出依赖的类
        private static void ExportDependency(StringBuilder sb)
        {
            foreach (var type in Dependency)
            {
                if (GeneratedTypeHashSet.Contains(type))
                    continue;
                if (type.FullName == "UnityEngine.RaycastHit") Debug.Log("ok");

                if (type.IsArray)
                    continue;

                if (type.IsByRef)
                    continue;

                if (typeof(Delegate).IsAssignableFrom(type))
                    continue;

                if (string.IsNullOrEmpty(type.FullName))
                    continue;

                if (IsBuildinType(type))
                    continue;
                
                sb.AppendLine("do");
                if (type.IsGenericType)
                {
                    sb.AppendLine($"\t---{GetGenericTypeDesc(type)}");
                }
                sb.AppendLine($"\t---@class {GetTypeName(type, false)}");
                sb.AppendLine($"\tlocal LuaDependency = {{}}");
                sb.AppendLine("end");
            }
        }

        //导出需要的声明
        private static void ExportHelpDefine(StringBuilder sb)
        {
            sb.AppendLine("do");
            sb.AppendLine($"\t---声明CS");
            sb.AppendLine($"\t---@type CS");
            sb.AppendLine($"\tCS = nil");
            sb.AppendLine("end");
        }
    }
}