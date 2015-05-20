namespace Odes.Licence.Model
{
    using System;

    public class OldDocmentCount
    {
        public OldDocmentCount(int documentsCreated, DateTimeOffset dateCreated)
        {
            this.DocumentsCreated = documentsCreated;
            this.DateCreated = dateCreated;
        }
        
        public int DocumentsCreated { get; set; }

        public DateTimeOffset DateCreated { get; set; }

    }
}