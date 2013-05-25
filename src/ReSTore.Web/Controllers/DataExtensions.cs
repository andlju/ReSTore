using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers
{
    public static class DataExtensions
    {
        public static Item ToItem(this object obj)
        {
            var item = new Item();

            var type = obj.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string prompt = null;
                var displayAttribute = prop.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                    prompt = displayAttribute.Prompt;

                var value = prop.GetValue(obj);
                if (value != null)
                {
                    item.Data.Add(new Data() { Name = prop.Name.ToCase(Case.CamelCase), Value = value.ToString(), Prompt = prompt });
                }
            }

            return item;
        }

        public static void PopulateTemplate(this Template template, Type type)
        {
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string prompt = null;
                var displayAttribute = prop.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                    prompt = displayAttribute.Prompt;

                template.Data.Add(new Data() { Name = prop.Name.ToCase(Case.CamelCase), Prompt = prompt });
            }
        }

        public static TObj FromTemplate<TObj>(this Template template) where TObj : new()
        {
            var obj = new TObj();
            var type = obj.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var data =
                    template.Data.SingleOrDefault(
                        d => d.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase));
                if (data != null && data.Value != null)
                {
                    var val = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(data.Value);
                    prop.SetValue(obj, val);
                }
            }
            return obj;
        }
    }
}