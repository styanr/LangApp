namespace LangApp.Core.Enums;

public enum UserRole
{
    Teacher,
    Student
}

public static class UserRoleExtensions
{
    public static string GetName(this UserRole role)
    {
        return Enum.GetName(role)!;
    }
}
