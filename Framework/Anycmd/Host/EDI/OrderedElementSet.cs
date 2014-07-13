
namespace Anycmd.Host.EDI {
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 排序的本体元素集合。
    /// <remarks>
    /// 该集合中的元素是有先后顺序的。
    /// </remarks>
    /// </summary>
    public sealed class OrderedElementSet : IEnumerable<ElementDescriptor> {
        private readonly HashSet<ElementDescriptor> elements = new HashSet<ElementDescriptor>();
        private readonly List<ElementDescriptor> list = new List<ElementDescriptor>();

        public OrderedElementSet() {

        }

        public int Count {
            get {
                return elements.Count;
            }
        }

        public ElementDescriptor this[int index] {
            get {
                return list[index];
            }
        }

        public void Add(ElementDescriptor element) {
            if (!elements.Contains(element)) {
                elements.Add(element);
                list.Add(element);   
            }
        }

        public IEnumerator<ElementDescriptor> GetEnumerator() {
            for (int i = 0; i < list.Count; i++) {
                yield return list[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            for (int i = 0; i < list.Count; i++) {
                yield return list[i];
            }
        }
    }
}
