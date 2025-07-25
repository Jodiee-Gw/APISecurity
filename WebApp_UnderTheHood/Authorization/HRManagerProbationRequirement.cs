using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp_UnderTheHood.Authorization
{
    public class HRManagerProbationRequirement: IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }

    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if(!context.User.HasClaim(x=>x.Type == "EmployeeDate"))
            {
                return Task.CompletedTask; // If the EmployeeDate claim is not present, do not succeed the requirement.
            }

            //if (context.User.HasClaim(c => c.Type == "Department" && c.Value == "HR") &&
            //    context.User.HasClaim(c => c.Type == "Manager" && c.Value == "true") &&
            //    context.User.HasClaim(c => c.Type == "EmployeeDate"))
            //{
            //    var employeeDate = DateTime.Parse(context.User.FindFirst(c => c.Type == "EmployeeDate").Value);
            //    var probationEndDate = employeeDate.AddMonths(requirement.ProbationMonths);
            //    if (DateTime.UtcNow >= probationEndDate)
            //    {
            //        context.Succeed(requirement);
            //    }
            //}

            if(DateTime.TryParse(context.User.FindFirst(c => c.Type == "EmployeeDate")?.Value, out DateTime employeeDate))
            {
                var period = DateTime.Now - employeeDate;
                if (period.Days >= requirement.ProbationMonths * 30) // Approximate month as 30 days
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
