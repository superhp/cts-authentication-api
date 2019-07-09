using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
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
        }

        public async Task AddNewVerificationAsync(string socialEmail, string ctsEmail)
        {
            if (_verifications.Any(x => x.RowKey == socialEmail))
            {
                return; 
            }

            var verification = new Verification(socialEmail, ctsEmail);

            await _table.ExecuteAsync(TableOperation.Insert(verification));
            _verifications.Add(verification);
        }

        public async Task<string> GetCtsEmailAsync(string socialEmail)
        {
            if (_verifications == null)
            {
                await ReadAllVerificationsAsync();
            }
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
    }
}
