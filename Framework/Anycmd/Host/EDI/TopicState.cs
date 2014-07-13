using System;

namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using Exceptions;

    public sealed class TopicState : ITopic
    {
        private Guid _ontologyID;

        private TopicState() { }

        public static TopicState Create(ITopic topic)
        {
            if (topic == null)
            {
                throw new ArgumentNullException("topic");
            }
            return new TopicState
            {
                Code = topic.Code,
                CreateOn = topic.CreateOn,
                Description = topic.Description,
                Id = topic.Id,
                IsAllowed = topic.IsAllowed,
                Name = topic.Name,
                OntologyID = topic.OntologyID
            };
        }

        public Guid Id { get; private set; }

        public Guid OntologyID
        {
            get { return _ontologyID; }
            private set
            {
                OntologyDescriptor ontology;
                if (!NodeHost.Instance.Ontologies.TryGetOntology(value, out ontology))
                {
                    throw new ValidationException("意外的本体标识" + value);
                }
                _ontologyID = value;
            }
        }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public bool IsAllowed { get; private set; }

        public string Description { get; private set; }

        public DateTime? CreateOn { get; private set; }

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
            if (!(obj is TopicState))
            {
                return false;
            }
            var left = this;
            var right = (TopicState)obj;

            return
                left.Id == right.Id &&
                left.OntologyID == right.OntologyID &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.IsAllowed == right.IsAllowed &&
                left.Description == right.Description;
        }

        public static bool operator ==(TopicState a, TopicState b)
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

        public static bool operator !=(TopicState a, TopicState b)
        {
            return !(a == b);
        }
    }
}
