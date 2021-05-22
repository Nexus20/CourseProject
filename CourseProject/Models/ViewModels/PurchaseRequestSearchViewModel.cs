using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CourseProject.Models.ViewModels {
    public class PurchaseRequestSearchViewModel : SearchViewModel {

        public int? Id { get; set; }
        public int[] RequestStates { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

    }
}
