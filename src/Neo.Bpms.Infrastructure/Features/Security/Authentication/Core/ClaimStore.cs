namespace Neo.Bpms.Infrastructure.Features.Security.Authentication.Core;

class ClaimStore
{
    private static Dictionary<long, string> _claimTypes;
    internal static long GetClaimTypeId(string claimType)
    {
        FetchClaims();
        return _claimTypes.Where(c => c.Value == claimType).Select(c => c.Key).FirstOrDefault();
    }
    internal static string GetClaimType(long claimId)
    {
        FetchClaims();
        if (_claimTypes.TryGetValue(claimId, out var claimType) && claimType != null)
            return claimType;
        switch (claimId)
        {
            //case 51:
            //	return "ProcessRoleId";
            case 101:
                return "CountryRegion";
            case 102:
                return "LicenseSubjectId";
            case 103:
                return "ServiceType";
            case 104:
                return "Skill";
            case 105:
                return "Team";
            case 106:
                return "ProjectGroup";
            case 107:
                return "ContactCategory";
            //case 108:
            //	return "";
            case 109:
                return "Operator";
            case 110:
                return "Switch";
            case 201:
                return "SourceId";
        }
        return claimId.ToString();
    }
    private static void FetchClaims()
    {
        _claimTypes ??= QueryUtility.FetchStringList("UserAndOrganization", "ClaimType", "EnName");
    }
}
