using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using gdRead.Data.Models;
using gdRead.Data.Repositories.Interfaces;

namespace gdRead.Data.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _conStr;
        public DashboardRepository()
        {
            _conStr = ConfigurationManager.ConnectionStrings["gdRead.Data.gdReadContext"].ConnectionString;
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
                    (SELECT (SUM(reserved_page_count) * 8192) / 1024 / 1024 AS DbSizeInMB FROM    sys.dm_db_partition_stats) AS DbSize,
	                (SELECT MAX(LastChecked) FROM Feed) AS LastFetch").FirstOrDefault();
                con.Close();
                return dashboard;
            }
        }
    }
}
