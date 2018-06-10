using System;

namespace Hyperion.Pf.Entity
{
    /// <summary>
    /// インスタンスの生成情報を追跡するためのフィールドをエンティティに提供するためのインターフェース
    /// </summary>
    public interface IAuditableEntity
    {
        string CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }

        string UpdatedBy { get; set; }

        DateTime UpdatedDate { get; set; }
    }
}