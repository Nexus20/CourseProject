using System.Collections;
using System.Reflection;
using System.Text;

namespace CourseProject.Models.ViewModels
{
    public abstract class SearchViewModel
    {
        public string CreateRequest()
        {
            PropertyInfo[] properties = this.GetType().GetProperties();
            StringBuilder queryStringBuilder = new StringBuilder("?");

            foreach (PropertyInfo property in properties)
            {
                var propertyValue = property.GetValue(this);

                if (propertyValue == null)
                {
                    continue;
                }

                if (propertyValue is IEnumerable value)
                {
                    var sb = new StringBuilder();

                    foreach (var item in value)
                    {
                        sb.Append($"{property.Name}={item}&");
                    }
                    queryStringBuilder.Append(sb);
                }
                else
                {
                    queryStringBuilder.Append($"{property.Name}={propertyValue}&");
                }
            }
            return queryStringBuilder.ToString();
        }
    }
}
