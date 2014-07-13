
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;
    using System;
    using ValueObjects;

    /// <summary>
    /// <see cref="IArchive"/>
    /// </summary>
    public class Archive : ArchiveBase, IAggregateRoot
    {
        public Archive() { }

        public static Archive Create(IArchiveCreateInput input)
        {
            return new Archive
            {
                Id = input.Id.Value,
                ArchiveOn = DateTime.Now,
                DataSource = string.Empty,
                Description = input.Description,
                FilePath = string.Empty,
                Password = string.Empty,
                Title = input.Title,
                RdbmsType = input.RdbmsType,
                OntologyID = input.OntologyID,
                UserID = string.Empty
            };
        }

        public void Update(IArchiveUpdateInput input)
        {
            this.Description = input.Description;
            this.Title = input.Title;
        }
    }
}
