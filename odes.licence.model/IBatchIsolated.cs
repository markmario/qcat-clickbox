namespace Odes.Licence.Model
{
    using System;
    using System.Collections.Generic;

    public interface IBatchIsolated
    {
        Guid ProjectId { get; set; }

        Guid BatchId { get; set; }

        Guid RequestId { get; set; }

        int DocumentsCreated { get; set; }

        string MachineName { get; set; }

        string UserName { get; set; }

        string SId { get; set; }

        DateTimeOffset DateCreated { get; set; }

        string Id { get; set; }

        List<OldDocmentCount> OldBatchValues { get; set; }
    }
}