using LangApp.Core.Common;
using LangApp.Core.Entities.Assignments;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.Assignments;

public interface IAssignmentFullAccessPolicy : IPolicy<Assignment, ApplicationUser>;