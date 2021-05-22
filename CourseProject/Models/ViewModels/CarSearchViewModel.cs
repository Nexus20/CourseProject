using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CourseProject.Models.ViewModels {
    public class CarSearchViewModel {

        public int? brandId { get; set; }
        public int? modelId { get; set; }
        public int[] fuelTypes { get; set; }
        public int[] bodyTypes { get; set; }
        public int[] transmissionTypes { get; set; }
        public int[] carStates { get; set; }
        public int? priceFrom { get; set; }
        public int? priceTo { get; set; }
        public int? newSearch { get; set; }


        public string CreateRequest() {
            PropertyInfo[] properties = this.GetType().GetProperties();
            StringBuilder queryStringBuilder = new StringBuilder("?");

            foreach (PropertyInfo property in properties) {
                var propertyValue = property.GetValue(this);

                if (propertyValue == null) {
                    continue;
                }

                if (propertyValue is IEnumerable value) {
                    var sb = new StringBuilder();

                    foreach (var item in value) {
                        sb.Append($"{property.Name}={item}&");
                    }
                    queryStringBuilder.Append(sb);
                }
                else {
                    queryStringBuilder.Append($"{property.Name}={propertyValue}&");
                }
            }
            
            return queryStringBuilder.ToString();
        }

    }
}
