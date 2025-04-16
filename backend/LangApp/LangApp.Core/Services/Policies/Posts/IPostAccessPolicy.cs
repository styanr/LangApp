using LangApp.Core.Common;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.Posts;

public interface IPostAccessPolicy : IPolicy<Post, StudyGroup, ApplicationUser>;