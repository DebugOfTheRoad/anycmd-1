﻿
namespace Anycmd.Host.AC.Infra
{

    /// <summary>
    /// 在组织结构上的方向类型。
    /// </summary>
    public enum DirectionType
    {
        /// <summary>
        /// 不向下
        /// </summary>
        NotDown = 0,
        /// <summary>
        /// 不向上
        /// </summary>
        NotUp = 1,
        /// <summary>
        /// 既向上又向下
        /// </summary>
        UpAndDown = 2,
        /// <summary>
        /// 既不向上又不向下
        /// </summary>
        NotUpNotDown = 3
    }
}
