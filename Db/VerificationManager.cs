using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Db
{
    public class VerificationManager : IVerificationManager
    {
        private List<Verification> _verifications;
        private readonly CloudTable _table;

        public VerificationManager(IConfiguration configuration)
        {
            var account = CloudStorageAccount.Parse(configuration["StorageConnectionString"]);
            var tableClient = account.CreateCloudTableClient();
            _table = tableClient.GetTableReference("verifications");
            ReadAllVerificationsAsync().Wait();
        }

        public async Task<Guid> AddNewVerificationAsync(string socialEmail, string ctsEmail)
        {
            var newUserGuid = new Guid();
            var verification = new Verification(newUserGuid, socialEmail, ctsEmail);

            await _table.ExecuteAsync(TableOperation.Insert(verification));
            _verifications.Add(verification);

            return newUserGuid;
        }

        public string GetCtsEmail(string socialEmail)
        {
            return _verifications.SingleOrDefault(x => x.RowKey == socialEmail)?.CtsEmail;
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

        public bool IsVerified(string socialEmail)
        {
            return _verifications.Any(x => x.RowKey == socialEmail);
        }
    }
}
