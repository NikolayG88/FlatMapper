using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;


namespace FlatMapper
{
    public class Mapper
    {
        public void MapProperties(object source, object target, IEnumerable<PropertyInfo> ignoreList = null)
        {
            var sProps = source.GetType().GetProperties();

            var tProps = target.GetType().GetProperties();

            if (ignoreList != null)
            {
                sProps.Where(sp => !ignoreList.Contains(sp));

                tProps.Where(tp => !ignoreList.Contains(tp));
            }

            foreach (var prop in sProps)
            {
                var props = GetTargetProperties(tProps, prop);

                SetTargetProperties(prop, props, source, target);
            }
        }

        public To Map<To>(object source) where To : new()
        {
            To result = new To();

            MapProperties(source, result);

            return result;
        }

        private List<PropertyInfo> GetTargetProperties(IEnumerable<PropertyInfo> targetProps, PropertyInfo sourceProp)
        {
            var result = new List<PropertyInfo>();

            foreach (var prop in targetProps)
            {
                var nestedAttr = GetNestedMapAttribute(prop);

                if (nestedAttr != null && !string.IsNullOrWhiteSpace(nestedAttr.TargetProperty))
                {
                    if (sourceProp.Name == nestedAttr.TargetProperty)
                    {
                        if (!result.Contains(prop))
                        {
                            result.Add(prop);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else if (sourceProp.Name == prop.Name)
                {
                    result.Add(prop);
                }
            }

            return result;
        }

        private void SetTargetProperties(PropertyInfo sourceProp, List<PropertyInfo> targetProps, object source, object target)
        {
            if (targetProps.Count != 0)
            {
                foreach (var tProp in targetProps)
                {
                    var nestedAttr = GetNestedMapAttribute(tProp);

                    object value = null;

                    if (nestedAttr != null)
                    {
                        value = GetNestedPropValue(source, nestedAttr.Path);
                    }
                    else
                    {
                        value = sourceProp.GetValue(source);
                    }
                    
                    if (value != null)
                    {
                        //Some auto type conversion if there is a missmatch
                        if (tProp.PropertyType == typeof(string) && value.GetType() != typeof(string))
                        {
                            tProp.SetValue(target, value.ToString());
                        }//Else if types match or target porperty is nullable
                        else if (tProp.PropertyType == value.GetType() || Nullable.GetUnderlyingType(tProp.PropertyType) != null)
                        {
                            tProp.SetValue(target, value);
                        }
                        else
                        {
                            throw new Exception("Flat Mapper Error!",

                                new Exception("Type missmatch, trying to map property with type " + value.GetType().ToString() + " to " + tProp.PropertyType.ToString()));
                        }
                    }
                }
            }
        }
        
        private object GetNestedPropValue(object obj, string propName)
        {
            string[] nameParts = propName.Split('.');

            if (nameParts.Length == 1)
            {
                return obj.GetType().GetProperty(propName).GetValue(obj, null);
            }
            
            foreach (string part in nameParts)
            {
                if (obj == null) { return null; }
                
                Type type = obj.GetType();

                PropertyInfo info = type.GetProperty(part);

                if (info == null) { return null; }
                
                obj = info.GetValue(obj, null);
            }
            
            return obj;
        }
        
        private NestedMap GetNestedMapAttribute(PropertyInfo prop)
        {
            object[] attrs = prop.GetCustomAttributes(true);

            foreach (object attr in attrs)
            {
                NestedMap mapAttr = attr as NestedMap;

                if (mapAttr != null)
                {
                    return mapAttr;
                }
            }
            
            return null;
        }
    }
}

