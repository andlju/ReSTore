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
                var value = prop.GetValue(obj);
                if (value != null)
                {
                    item.Data.Add(new Data() { Name = prop.Name.ToCase(Case.CamelCase), Value = value.ToString() });
                }
            }

            return item;
        }
    }
}