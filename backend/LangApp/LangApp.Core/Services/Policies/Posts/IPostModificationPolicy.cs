using LangApp.Core.Common;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.Posts;

public interface IPostModificationPolicy : IPolicy<Post, ApplicationUser>;