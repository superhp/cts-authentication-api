using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Db
{
    public class UserManager : IUserManager
    {
        private List<Verification> _verifications;
        private readonly CloudTable _table;

        public UserManager(IConfiguration configuration)
        {
            var account = CloudStorageAccount.Parse(configuration["StorageConnectionString"]);
            var tableClient = account.CreateCloudTableClient();
            _table = tableClient.GetTableReference("verifications");
            ReadAllVerificationsAsync().Wait();
        }

        private async Task ReadAllVerificationsAsync()
        {
            TableContinuationToken token = null;
            _verifications = new List<Verification>();
            do
            {
                var queryResult = await _table.ExecuteQuerySegmentedAsync(new TableQuery<Verification>(), token);
                _verifications.AddRange(queryResult);
                token = queryResult.ContinuationToken;
            } while (token != null);
        }
        public Guid GetUserGuid(string ctsEmail)
        {
            var verification = _verifications.SingleOrDefault(x => x.CtsEmail == ctsEmail);
            return Guid.Parse(verification.RowKey);
        }
    }
}
