﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Migrations;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models.ViewModels {
    public class ChangeRoleViewModel {

        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public List<string> UserRoles { get; set; }

        public ChangeRoleViewModel() {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }

    }
}
