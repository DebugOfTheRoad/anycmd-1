using System;

namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using Exceptions;
    using Util;

    public sealed class ArchiveState : IArchive
    {
        public static readonly ArchiveState Empty = new ArchiveState
        {
            ArchiveOn = SystemTime.MinDate,
            Title = string.Empty,
            CreateBy = string.Empty,
            CreateOn = SystemTime.MinDate,
            CreateUserID = Guid.Empty,
            DataSource = string.Empty,
            FilePath = string.Empty,
            Id = Guid.Empty,
            NumberID = 0,
            _ontologyID = Guid.Empty,
            Password = string.Empty,
            _rdbmsType = string.Empty,
            UserID = string.Empty
        };
        private string _rdbmsType;
        private Guid _ontologyID;
        private OntologyDescriptor _ontology = null;

        private ArchiveState() { }

        public static ArchiveState Create(IAppHost host, ArchiveBase archive)
        {
            if (archive == null)
            {
                throw new ArgumentNullException("archive");
            }
            var data = new ArchiveState
            {
                Host = host,
                ArchiveOn = archive.ArchiveOn,
                CreateBy = archive.CreateBy,
                CreateOn = archive.CreateOn,
                CreateUserID = archive.CreateUserID,
                DataSource = archive.DataSource,
                FilePath = archive.FilePath,
                Id = archive.Id,
                NumberID = archive.NumberID,
                OntologyID = archive.OntologyID,
                Password = archive.Password,
                RdbmsType = archive.RdbmsType,
                Title = archive.Title,
                UserID = archive.UserID
            };

            return data;
        }

        public Guid Id { get; private set; }

        public IAppHost Host { get; private set; }

        public string RdbmsType
        {
            get { return _rdbmsType; }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("数据库类型不能为空");
                }
                Anycmd.Rdb.RdbmsType dbType;
                if (!value.TryParse(out dbType))
                {
                    throw new CoreException("意外的关系数据库类型" + value);
                }
                _rdbmsType = value;
            }
        }

        public string DataSource { get; private set; }

        public string FilePath { get; private set; }

        public int NumberID { get; private set; }

        public string UserID { get; private set; }

        public string Password { get; private set; }

        public string Title { get; private set; }

        public Guid OntologyID
        {
            get { return _ontologyID; }
            private set
            {
                OntologyDescriptor ontology;
                if (!Host.Ontologies.TryGetOntology(value, out ontology))
                {
                    throw new ValidationException("意外的本体标识" + value);
                }
                _ontologyID = value;
            }
        }

        public DateTime ArchiveOn { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public string CreateBy { get; private set; }

        public Guid? CreateUserID { get; private set; }

        /// <summary>
        /// 归档本体
        /// </summary>
        public OntologyDescriptor Ontology
        {
            get
            {
                if (_ontology == null)
                {
                    if (!Host.Ontologies.TryGetOntology(this.OntologyID, out _ontology))
                    {
                        throw new CoreException("意外的本体ID" + this.OntologyID);
                    }
                }
                return _ontology;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ontologyID"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="numberID"></param>
        public void Archive(int numberID)
        {
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(this.OntologyID, out ontology))
            {
                throw new CoreException("非法的本体" + this.OntologyID.ToString());
            }
            if (this.Id == Guid.Empty)
            {
                this.Id = Guid.NewGuid();
            }
            this.ArchiveOn = DateTime.Now;
            this.NumberID = numberID;
            this.FilePath = Host.Config.EntityArchivePath;
            ontology.EntityProvider.Archive(ontology, this);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (!(obj is ArchiveState))
            {
                return false;
            }
            var left = this;
            var right = (ArchiveState)obj;

            return
                left.Id == right.Id &&
                left.RdbmsType == right.RdbmsType &&
                left.DataSource == right.DataSource &&
                left.FilePath == right.FilePath &&
                left.NumberID == right.NumberID &&
                left.UserID == right.UserID &&
                left.Password == right.Password &&
                left.Title == right.Title;
        }

        public static bool operator ==(ArchiveState a, ArchiveState b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(ArchiveState a, ArchiveState b)
        {
            return !(a == b);
        }
    }
}
