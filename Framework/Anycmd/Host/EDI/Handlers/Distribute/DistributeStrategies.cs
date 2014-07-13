
namespace jDTS.EDI.Core.Distribute {
    public class DistributeStrategies {
        public readonly IDistributeStrategy[] Array;

        public DistributeStrategies(params IDistributeStrategy[] members) {
            if (members == null) {
                Array = new IDistributeStrategy[0];
            }
            this.Array = members;
        }
    }
}
