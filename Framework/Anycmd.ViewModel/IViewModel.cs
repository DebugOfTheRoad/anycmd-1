
namespace Anycmd.ViewModel
{
    /// <summary>
    /// 标记接口，所有的视图模型必须标记该接口。
    /// <para>
    /// 负责向用户显示（View的职责）信息，并且解析用户命令（Controller的职责）。
    /// 外部的执行者有时可能会是其他的计算机系统，不一定非是人（WebService）。
    /// </para>
    /// </summary>
    public interface IViewModel
    {
    }
}
