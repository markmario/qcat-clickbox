namespace Odes.Licence.Model
{
    using System;

    public interface IDocumentCoded
    {
        string Id { get; set; }
        Guid ProjectId { get; set; }
        Guid DocumentId { get; set; }
        Guid RequestId { get; set; }
        string MachineName { get; set; }
        string UserName { get; set; }
        string SId { get; set; }
        DateTimeOffset DateCreated { get; set; }
    }
}