using System;
using System.Collections.Generic;

namespace Odes.Licence.Model
{
    public class BatchIsolated
    {
        public Guid ProjectId { get; set; }
        public Guid BatchId { get; set; }
        public Guid RequestId { get; set; }
        public int DocumentsCreated { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public string SId { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string Id { get; set; }
        public List<OldDocmentCount> OldBatchValues { get; set; }
    }

    public class OldDocmentCount
    {
        public int DocumentsCreated { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public OldDocmentCount(int documentsCreated, DateTimeOffset dateCreated)
        {
            DocumentsCreated = documentsCreated;
            DateCreated = dateCreated;
        }
    }
}