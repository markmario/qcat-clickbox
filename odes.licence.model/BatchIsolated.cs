namespace Odes.Licence.Model
{
    using System;
    using System.Collections.Generic;

    public class BatchIsolated : IBatchIsolated
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
}