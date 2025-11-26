using Neo.Bpms.Domain.Features.Security;

namespace Neo.Bpms.Infrastructure.Features.Security;

public class ClubRolesService : IClubRolesService
{
    public List<ClubRoleInfo> GetClubRoles()
    {
        // استفاده از reflection برای واکشی نقش‌های ClubRoles
        try
        {
            var clubRolesType = Type.GetType("Club.Domain.Constants.ClubRoles, Club.Domain");
            if (clubRolesType == null)
            {
                // اگر ClubRoles در دسترس نبود، لیست پیش‌فرض را برگردان
                return GetDefaultRoles();
            }

            var roles = new List<ClubRoleInfo>();
            var properties = clubRolesType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(string) && p.GetMethod != null && !p.Name.Contains("Roles"));

            foreach (var prop in properties)
            {
                var roleName = prop.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(roleName))
                {
                    roles.Add(new ClubRoleInfo
                    {
                        Name = roleName,
                        DisplayName = GetRoleDisplayName(roleName)
                    });
                }
            }

            // اضافه کردن Admin از Roles پایه
            roles.Insert(0, new ClubRoleInfo
            {
                Name = "Admin",
                DisplayName = "مدیر سیستم"
            });

            return roles.OrderBy(r => r.DisplayName).ToList();
        }
        catch
        {
            return GetDefaultRoles();
        }
    }

    private List<ClubRoleInfo> GetDefaultRoles()
    {
        return new List<ClubRoleInfo>
        {
            new ClubRoleInfo { Name = "Admin", DisplayName = "مدیر سیستم" },
            new ClubRoleInfo { Name = "Manager", DisplayName = "مدیر" },
            new ClubRoleInfo { Name = "MarketingManager", DisplayName = "مدیر بازاریابی" },
            new ClubRoleInfo { Name = "FinanceManager", DisplayName = "مدیر مالی" },
            new ClubRoleInfo { Name = "Analyst", DisplayName = "تحلیل‌گر" },
            new ClubRoleInfo { Name = "CallCenterSupport", DisplayName = "پشتیبانی مرکز تماس" },
            new ClubRoleInfo { Name = "CallCenterManager", DisplayName = "مدیر مرکز تماس" }
        };
    }

    private string GetRoleDisplayName(string roleName)
    {
        return roleName switch
        {
            "Admin" => "مدیر سیستم",
            "Manager" => "مدیر",
            "MarketingManager" => "مدیر بازاریابی",
            "FinanceManager" => "مدیر مالی",
            "Analyst" => "تحلیل‌گر",
            "CallCenterSupport" => "پشتیبانی مرکز تماس",
            "CallCenterManager" => "مدیر مرکز تماس",
            _ => roleName
        };
    }
}

