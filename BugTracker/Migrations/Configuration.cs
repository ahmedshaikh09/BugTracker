namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using BugTracker.Models.Domain;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "BugTracker.Models.ApplicationDbContext";
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            // Seeding Priorities

            var priority1 = new TicketPriority();
            priority1.Name = "Low";

            context.TicketPriority.AddOrUpdate(p => p.Name, priority1);

            var priority2 = new TicketPriority();
            priority2.Name = "Medium";

            context.TicketPriority.AddOrUpdate(p => p.Name, priority2);

            var priority3 = new TicketPriority();
            priority3.Name = "High";

            context.TicketPriority.AddOrUpdate(p => p.Name, priority3);

            // Seeding Statuses 

            var status1 = new TicketStatuses();
            status1.Name = "Open";

            context.TicketStatuses.AddOrUpdate(p => p.Name, status1);

            var status2 = new TicketStatuses();
            status2.Name = "Resolved";

            context.TicketStatuses.AddOrUpdate(p => p.Name, status2);

            var status3 = new TicketStatuses();
            status3.Name = "Rejected";

            context.TicketStatuses.AddOrUpdate(p => p.Name, status3);

            // Seeding Types

            var type1 = new TicketTypes();
            type1.Name = "Bug";

            context.TicketTypes.AddOrUpdate(p => p.Name, type1);

            var type2 = new TicketTypes();
            type2.Name = "Feature";

            context.TicketTypes.AddOrUpdate(p => p.Name, type2);

            var type3 = new TicketTypes();
            type3.Name = "Database";

            context.TicketTypes.AddOrUpdate(p => p.Name, type3);

            var type4 = new TicketTypes();
            type4.Name = "Support";

            context.TicketTypes.AddOrUpdate(p => p.Name, type4);

            //Seeding Users And Roles

            var roleManager =
               new RoleManager<IdentityRole>(
                   new RoleStore<IdentityRole>(context));

            //UserManager, used to manage users
            var userManager =
                new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(context));

            //Adding Admin role 
            if (!context.Roles.Any(p => p.Name == "Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }
            //Adding projectmanager role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "ProjectManager"))
            {
                var moderatorRole = new IdentityRole("ProjectManager");
                roleManager.Create(moderatorRole);
            }
            //Adding admin role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Developer"))
            {
                var developerRole = new IdentityRole("Developer");
                roleManager.Create(developerRole);
            }
            //Adding submitter role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Submitter"))
            {
                var submitterRole = new IdentityRole("Submitter");
                roleManager.Create(submitterRole);
            }


            ApplicationUser adminUser;

            if (!context.Users.Any(
                p => p.UserName == "admin@mybugtracker.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@mybugtracker.com";
                adminUser.Email = "admin@mybugtracker.com";
                adminUser.DisplayName = "Admin";

                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context
                    .Users
                    .First(p => p.UserName == "admin@mybugtracker.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }


        }
    }
}
