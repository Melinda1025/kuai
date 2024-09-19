using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Utils
{    
    public delegate void PropertySetter<T, TProperty>(T obj, TProperty value);
    public delegate TProperty PropertyGetter<T, TProperty>(T obj);
    /// <summary>
    /// 使用字符串为key跨页面使用属性的Setter, 比如在A页面有个Value属性，
    /// 通过此类可以在B页面直接调用PropertyAccessor.Instance.SetValue方法修改A页面的Value属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class PropertyAccessor<T, TProperty>
    {
        private static readonly Lazy<PropertyAccessor<T, TProperty>> _instance = new(() => new PropertyAccessor<T, TProperty>());
        public static PropertyAccessor<T, TProperty> Instance => _instance.Value;

        private readonly Dictionary<string, (T Target, PropertyGetter<T, TProperty> Getter)> Getters = new();
        private readonly Dictionary<string, (T Target, PropertySetter<T, TProperty> Setter)> Setters = new();        

        public void Register(string key, T target, Expression<Func<T, TProperty>> expression)
        {
            if (expression.Body is not MemberExpression member)
            {
                throw new ArgumentException("表达式不是成员表达式");
            }
            var propertyInfo = member.Member as PropertyInfo;
            var getter = Delegate.CreateDelegate(
                typeof(PropertySetter<T, TProperty>),
                propertyInfo.GetSetMethod()) as PropertySetter<T, TProperty>;
            Setters.TryAdd(key, (target, getter));
            var setter = Delegate.CreateDelegate(
                typeof(PropertyGetter<T, TProperty>),
                propertyInfo.GetGetMethod()) as PropertyGetter<T, TProperty>;
            Getters.TryAdd(key, (target, setter));
        }

        public TProperty GetValue(string key)
        {
            if (Getters.TryGetValue(key, out var pair))
            {
                return pair.Getter.Invoke(pair.Target);
            }
            else
            {
                return default;
            }
        }
        public void SetValue(string key, TProperty value)
        {
            if (Setters.TryGetValue(key, out var pair))
            {
                pair.Setter.Invoke(pair.Target, value);
            }            
        }

        public bool Unregister(string key)
        {
            return Getters.Remove(key, out _) && Setters.Remove(key, out _);
        }
    }
}
