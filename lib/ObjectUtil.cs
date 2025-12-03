using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace erpsolution.lib
{
    public static class SystemExtension
    {
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static string GetReflectedPropertyValue(this object subject, string field)
        {
            object reflectedValue = subject.GetType().GetProperty(field).GetValue(subject, null);
            return reflectedValue != null ? reflectedValue.ToString() : "";
        }

        public static string ToCamelCase(this string str)
        {
            return string.IsNullOrEmpty(str) || str.Length < 2
            ? str
            : char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }
        public static string FormatProperties(this string src, string splitter = "_")
        {
            List<string> parts = src.Length > 0 && src.IndexOf(splitter) >= 0 ? src.Split(splitter).ToList() : new List<string>() { src };
            List<string> tmp = new List<string>();
            foreach (var p in parts)
            {
                tmp.Add(parts.Count() > 1 ? p.ToCamelCase() : p);
            }
            return string.Join("", tmp);
        }
        public static List<string> FormatProperties(this List<string> srcList, string splitter = "_")
        {
            List<string> desList = new List<string>();
            foreach (var src in srcList)
            {
                desList.Add(src.FormatProperties(splitter));
            }
            return desList;
        }
    }
    public class ObjectUtil
    {
        public static object GetValue(object Ob, string PropertyName)
        {
            if (Ob != null)
            {
                if (Ob is DataRow) return ((DataRow)Ob)[PropertyName];
                if (Ob is DataRowView) return ((DataRowView)Ob)[PropertyName];

                PropertyInfo property = Ob.GetType().GetProperty(PropertyName);
                if (property != null)
                    return property.GetValue(Ob, null);
            }
            return null;
        }

        public static void SetValue(object Ob, string PropertyName, object Value)
        {
            if (Ob is DataRow)
                ((DataRow)Ob)[PropertyName] = Value ?? DBNull.Value;
            else if (Ob is DataRowView)
                ((DataRowView)Ob)[PropertyName] = Value ?? DBNull.Value;
            else
            {
                var p = Ob.GetType().GetProperty(PropertyName);
                if (p != null && p.CanWrite && p.GetSetMethod(true).IsPublic)
                    p.SetValue(Ob, Value, null);
            }
        }

        public static object ExecMethod(object o, string MethodName, params object[] args)
        {
            if (o != null)
            {
                return args != null
                           ? o.GetType().InvokeMember(MethodName, BindingFlags.InvokeMethod, null, o, args)
                           : o.GetType().InvokeMember(MethodName, BindingFlags.InvokeMethod, null, o, null);
            }
            return null;
        }

        public static object ExecFunction(object Fu, params object[] args)
        {
            if (Fu != null)
            {
                return args != null ? ((Delegate)Fu).DynamicInvoke(args) : ((Delegate)Fu).DynamicInvoke();
            }
            return null;
        }

        public static void AssigneValues(object ObSource, object ObDestination)
        {
            if (ObSource != null && ObDestination != null)
            {
                var prs = ObSource.GetType().GetProperties();
                foreach (var pr in prs)
                    if (pr != null && pr.CanWrite && pr.GetSetMethod(true).IsPublic)
                        SetValue(ObDestination, pr.Name, pr.GetValue(ObSource, null));
            }
        }
        /// <summary>
        /// Hepper create lambda expression for a single value in the class
        /// </summary>
        /// <typeparam name="TModel">The class of reference object</typeparam>
        /// <typeparam name="TItem">The type of the item</typeparam>
        /// <param name="expOperationType">Operation to compare: Equal/NotEqual/LessThan/LessThanOrEqual/GreaterThan/GreaterThanOrEqual</param>
        /// <param name="strPropertyName">Name of property</param>
        /// <param name="objValue">Value of property</param>
        /// <returns></returns>
        public static Expression ExpressionBuilder<TModel, TItem>(ExpressionType expOperationType, string strPropertyName, object objValue)
        {
            ParameterExpression parameterExp = Expression.Parameter(typeof(TModel), typeof(TModel).Name);
            Expression<Func<TModel, bool>> expression = Expression.Lambda<Func<TModel, bool>>(Expression.Constant(true), new ParameterExpression[] { parameterExp }); ;

            try
            {
                MemberExpression propExp = Expression.Property(parameterExp, strPropertyName);

                ConstantExpression valueExp = Expression.Constant(objValue, typeof(TItem));
                BinaryExpression body = BinaryExpression.MakeBinary(expOperationType, propExp, valueExp);

                expression = Expression.Lambda<Func<TModel, bool>>(body, new ParameterExpression[] { parameterExp });
            }
            catch { }
            return expression;
        }
       
    }
}
