using System.Data.SqlClient;
using System.Linq;
using Dapper;
using gdRead.Data.Models;

namespace gdRead.Data.Repositories
{
    public class DashboardRepository
    {
        private readonly string _conStr;
        public DashboardRepository(string conStr)
        {
            _conStr = conStr;
        }

        public Dashboard GetDashboardStats()
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var dashboard = con.Query<Dashboard>(@"SELECT
	(SELECT COUNT(*) FROM AspnetUsers) AS UserCount,
	(SELECT COUNT(*) FROM Feed) AS FeedCount,
	(SELECT COUNT(*) FROM Post) AS PostCount,
	(SELECT COUNT(*) FROM Subscription) AS SubscriptionCount,
	(SELECT MAX(LastChecked) FROM Feed) AS LastFetch").FirstOrDefault();
                con.Close();
                return dashboard;
            }
        }
    }
}
