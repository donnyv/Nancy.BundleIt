namespace BundleItTestSite
{
    using Nancy;

    public class AccountDashboardModule : NancyModule
    {
        public AccountDashboardModule()
        {
            Get["/AccountDashboard"] = parameters =>
            {
                return View["AccountDashboard/index"];
            };
        }
    }
}