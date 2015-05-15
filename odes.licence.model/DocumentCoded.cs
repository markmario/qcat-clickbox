using System;

namespace Odes.Licence.Model
{
    public class DocumentCoded
    {
        public string Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid DocumentId { get; set; }
        public Guid RequestId { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public string SId { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
}