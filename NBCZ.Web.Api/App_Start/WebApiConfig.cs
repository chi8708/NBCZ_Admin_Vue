using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace NBCZ.Web.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           if (ConfigurationManager.AppSettings["isShowDoc"] == "1") { SwaggerConfig.Register(); }
            // Web API configuration and services
            config.Filters.Add(new AuthorizeAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //json属性小驼峰
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new  CamelCasePropertyNamesContractResolver();
            //返回时间不带T
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter()
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            });

            ////返回是null改成""
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new NullToEmptyStringResolver();
            ////json属性小驼峰
            //// config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //config.Filters.Add(new WebApiExceptionAttribute());

            config.Filters.Add(new WebApiExceptionAttribute());
        }

        /// <summary>
        /// null处理
        /// </summary>
        public class NullToEmptyStringResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                return type.GetProperties()
                        .Select(p =>
                        {
                            var jp = base.CreateProperty(p, memberSerialization);
                            jp.ValueProvider = new NullToEmptyStringValueProvider(p);
                            jp.PropertyName = jp.PropertyName.Substring(0, 1).ToLower() + jp.PropertyName.Substring(1);//cts add首字母小写
                            return jp;
                        }).ToList();

            }



            public class NullToEmptyStringValueProvider : IValueProvider
            {
                PropertyInfo _MemberInfo;

                public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
                {
                    _MemberInfo = memberInfo;
                }
                public object GetValue(object target)
                {
                    object result = _MemberInfo.GetValue(target);


                    if (result == null)
                    {
                        var type = _MemberInfo.PropertyType;
                        if (type == typeof(string))
                        {
                            result = "";
                        }
                        else if (type == typeof(Nullable<Int16>))
                        {
                            result = new Nullable<Int16>(0);
                        }
                        else if (
                            type == typeof(Nullable<Int32>) ||
                            type == typeof(Nullable<Int64>))
                        {
                            result = 0;
                        }
                        else if (type == typeof(Nullable<Decimal>))
                        {
                            result = 0.00M;
                        }
                        else if (type == typeof(Nullable<Double>))
                        {
                            result = 0.00D;
                        }
                        else if (type == typeof(Nullable<DateTime>))
                        {
                            result = DateTime.MinValue;
                        }
                        else if (type == typeof(Nullable<Boolean>))
                        {
                            result = false;
                        }

                        else if (type == typeof(Nullable<Byte>))
                        {
                            result = Byte.MinValue;
                        }
                        else
                        {
                            result = new Object();
                        }
                    }

                    return result;

                }


                public void SetValue(object target, object value)
                {

                    //var props = (_MemberInfo.PropertyType).GetProperties();
                    //if (props != null && props.Length > 0)
                    //{
                    //    //解决对象嵌套对象问题
                    //    _MemberInfo.SetValue(target, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), _MemberInfo.PropertyType));
                    //}
                    //else
                    //{
                    //    _MemberInfo.SetValue(target, value, null);
                    //}
                    try
                    {
                        _MemberInfo.SetValue(target, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), _MemberInfo.PropertyType));
                    }
                    catch (Exception)
                    {
                        _MemberInfo.SetValue(target, value);
                    }

                }
            }
        }
    }
}
