using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace ClickBoxOdesClickCountReports
{
    using System;

    using ClickBox.Mail;

    using Microsoft.WindowsAzure.Storage.Table;

    public class Functions
    {
        // This function will be triggered based on the schedule you have set for this WebJob
        // This function will enqueue a message on an Azure Queue called queue
        [NoAutomaticTrigger]
        public static void ManualTrigger(TextWriter log, int value, [Queue("queue")] out string message)
        {
            log.WriteLine("Function is invoked with value={0}", value);
            message = value.ToString();
            log.WriteLine("Following message will be written on the Queue={0}", message);
       }

        public static async void TimerJob([TimerTrigger("12:00:00")] TimerInfo timerInfo,
                                    TextWriter log, [Table("UserAccounts")] CloudTable tableBinding,
                                    [Table("MonthlyBatches")] CloudTable monthlyBatchesCloudTable,
                                    [Table("MonthlyDocuments")] CloudTable monthlyDocumentsCloudTable)
        {
            var thisMonth = DateTime.Now.Month + DateTime.Now.Year.ToString();

            log.WriteLine("Running ODES Reports every 12 Hours");

            var filter = TableQuery.GenerateFilterCondition("Product", QueryComparisons.Equal, "QCAT-Odes");

            var query = new TableQuery().Where(filter).Select(new List<string> { "Product", "CompanyName", "UserName" });

            var results = tableBinding.ExecuteQuery(query);
            var monthlyBatchLines = new List<MonthlyBatchLine>();
            var monthlyDocumentLines = new List<MonthlyDocumentLine>();
            var monthlyBatchLinesAsString = string.Empty;
            var monthlyDocumentLinesAstring = string.Empty;
            var allBatchesCount = 0L;
            var allDocsCount = 0L;

            foreach (var result in results)
            {
                //company name user name
                var userName = result["UserName"].StringValue;
                var companyName = result["CompanyName"].StringValue;
                var product = result["Product"].StringValue;

                log.WriteLine($"{companyName} has a {product} license managed by {userName}");

                var numberOfBatchesForTheMonthForCo = GetNumberOfMontlyBatchesForGivenCompany(thisMonth, companyName, monthlyBatchesCloudTable);
                var numberOfDocumentsForTheMonthForCo = GetNumberOfMontlyDocumentsForGivenCompany(
                    thisMonth,
                    companyName,
                    monthlyDocumentsCloudTable);

                allBatchesCount += numberOfBatchesForTheMonthForCo;
                allDocsCount += numberOfDocumentsForTheMonthForCo;

                var printBatchData = $"{companyName} has Isolated {numberOfBatchesForTheMonthForCo} batch(s) this month";
                monthlyBatchLinesAsString += "<div style='margin-bottom:10px;'>" + printBatchData + "</div>";
                monthlyBatchLines.Add(new MonthlyBatchLine() {ReportLine = printBatchData });
                var printDocsData = $"{companyName} has Koded {numberOfDocumentsForTheMonthForCo} document(s) this month";
                monthlyDocumentLinesAstring += "<div style='margin-bottom:10px;'>" + printDocsData + "</div>";
                monthlyDocumentLines.Add(new MonthlyDocumentLine() { ReportLine = printDocsData});

                log.WriteLine(printBatchData);
                log.WriteLine(printDocsData);

                Console.WriteLine(printBatchData);
                Console.WriteLine(printDocsData);
            }

            log.WriteLine(monthlyBatchLinesAsString);
            log.WriteLine(monthlyDocumentLinesAstring);

            var report = new IAmTheWeeklyStatsForTheMonthOdesReport()
                             {
                                 AllBatchesCount = allBatchesCount,
                                 AllDocumentsCount = allDocsCount,
                                 MonthlyDocumentLines =  monthlyDocumentLines,
                                 MonthlyBatchLines = monthlyBatchLines,
                                 MontlyDocumentLinesAsString = monthlyDocumentLinesAstring,
                                 MonthlyBatchLinesAsString = monthlyBatchLinesAsString
                             };

            var sent = await Mailer.SendMonthlyOdesReports(report);
            if (!sent)
            {
                //five fails and into poison queue
                throw new Exception("Failed to send monthly reports");
            }
        }

        private static long GetNumberOfMontlyDocumentsForGivenCompany(
            string thisMonth,
            string companyName,
            CloudTable monthlyDocumentsCloudTable)
        {
            var montlyDocumentsFilter = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, thisMonth),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("CompanyName", QueryComparisons.Equal, companyName));
            var monthlyDoc = new TableQuery().Where(montlyDocumentsFilter).Select(new List<string> { "Company", "ProjectId" });
            return monthlyDocumentsCloudTable.ExecuteQuery(monthlyDoc).LongCount();
        }

        private static long GetNumberOfMontlyBatchesForGivenCompany(string thisMonth, string companyName, CloudTable monthlyBatchesCloudTable)
        {
            var montlyBatchesFilter = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, thisMonth),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("CompanyName", QueryComparisons.Equal, companyName));

            var monthlyBatches = new TableQuery().Where(montlyBatchesFilter).Select(new List<string> { "Company", "ProjectId", "DocumentCount" });
            return monthlyBatchesCloudTable.ExecuteQuery(monthlyBatches).LongCount();
        }
    }
}
