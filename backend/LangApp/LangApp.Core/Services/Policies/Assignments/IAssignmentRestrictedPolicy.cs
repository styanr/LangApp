using LangApp.Core.Common;
using LangApp.Core.Entities.Assignments;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.Assignments;

public interface IAssignmentRestrictedPolicy : IPolicy<Assignment, StudyGroup, ApplicationUser>;