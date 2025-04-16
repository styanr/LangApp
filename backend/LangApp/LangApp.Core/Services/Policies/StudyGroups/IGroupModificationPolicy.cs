using LangApp.Core.Common;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.StudyGroups;

public interface IGroupModificationPolicy : IPolicy<StudyGroup, ApplicationUser>
{
}