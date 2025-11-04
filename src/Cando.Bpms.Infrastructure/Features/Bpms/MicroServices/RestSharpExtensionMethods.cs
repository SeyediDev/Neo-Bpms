using RestSharp;

namespace Neo.Bpms.Infrastructure.Features.Bpms.MicroServices;

public static class RestSharpExtensionMethods
{
    public static bool IsSuccessful(this RestResponse response)
    {
        return StatusIsSuccessful((int)response.StatusCode);
    }
    public static bool WasUnableToConnect(this RestResponse response)
    {
        return response.StatusCode == 0;
    }
    public static bool ReportsServerErrors(this RestResponse response)
    {
        return StatusReportsServerErrors((int)response.StatusCode);
    }
    public static bool ReportsClientErrors(this RestResponse response)
    {
        return StatusReportsClientErrors((int)response.StatusCode);
    }

    private static bool StatusIsSuccessful(int statusCode)
    {
        return statusCode is >= 200 and < 300;
    }

    private static bool StatusReportsServerErrors(int statusCode)
    {
        return statusCode is >= 500 and < 600;
    }

    private static bool StatusReportsClientErrors(int statusCode)
    {
        return statusCode is >= 400 and < 500;
    }
}
