namespace ClickBox.Mail
{
    using System.Collections.Generic;

    using Mandrill.Models;

    public class IAmTheWeeklyStatsForTheMonthOdesReport
    {
        public string From => "licensing@qcat.com.au";
        public string FromName => "QCAT Back Office Automation Manager";
        public List<EmailAddress> To =>  new List<EmailAddress>
        {
            new EmailAddress("simon@qcat.com.au"),
            new EmailAddress("mark@qcat.com.au"),
            //new EmailAddress("jlakshman@lawimage.com.au"),
            //new EmailAddress("mik@lawimage.com.au")
        };
        public long AllDocumentsCount { get; set; }
        public long AllBatchesCount { get; set; }
        public List<MonthlyBatchLine> MonthlyBatchLines { get; set; }
        public List<MonthlyDocumentLine> MonthlyDocumentLines { get; set; }

        public string MontlyDocumentLinesAsString { get; set; }

        public string MonthlyBatchLinesAsString { get; set; }
    }

    public class MonthlyBatchLine
    {
        public string ReportLine { get; set; }
    }

    public class MonthlyDocumentLine
    {
        public string ReportLine { get; set; }
    }
}