using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DebugViewer
{
    /// <summary>
    /// 表示选择状态
    /// </summary>
    public struct SelectState : IComparable
    {
        /// <summary>
        /// 选择状态
        /// </summary>
        private readonly bool isSelected;

        /// <summary>
        /// 表示选择状态
        /// </summary>
        /// <param name="isSelected">状态</param>
        public SelectState(bool isSelected)
        {
            this.isSelected = isSelected;
        }

        /// <summary>
        /// 转换为好友字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.isSelected ? "√" : "--";
        }

        /// <summary>
        /// 排序比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is SelectState)
            {
                return this.isSelected.CompareTo(((SelectState)obj).isSelected);
            }
            return 1;
        }

        /// <summary>
        /// 隐式转换为bool
        /// </summary>
        /// <param name="selectState"></param>
        /// <returns></returns>
        public static implicit operator bool(SelectState selectState)
        {
            return selectState.isSelected;
        }

        /// <summary>
        /// 从bool隐式转换提到
        /// </summary>
        /// <param name="isSelected"></param>
        /// <returns></returns>
        public static implicit operator SelectState(bool isSelected)
        {
            return new SelectState(isSelected);
        }
    }
}
