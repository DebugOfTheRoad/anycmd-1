
namespace Anycmd.ViewModel
{

    /// <summary>
    /// 模型视图控制器响应模型
    /// </summary>
    public class ResponseData : IViewModel
    {
        private string _state = "success";

        /// <summary>
        /// 
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 取值：success、info、warning、danger
        /// </summary>
        public string state
        {
            get
            {
                return this.success ? "success" : _state;
            }
            set { _state = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 一般只有在表格内编辑模式的时候才用赋此值
        /// 为处理表格内编辑模式（RowEditor）游离对象时返回Id
        /// </summary>
        public object id { get; set; }

        public ResponseData Suc()
        {
            this.state = "success";
            return this;
        }

        public ResponseData Info()
        {
            this.state = "info";
            return this;
        }

        public ResponseData Warning()
        {
            this.state = "warning";
            return this;
        }

        public ResponseData Danger()
        {
            this.state = "danger";
            return this;
        }

        public ResponseData Error()
        {
            this.state = "error";
            return this;
        }
    }
}
