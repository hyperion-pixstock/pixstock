using System;

namespace Pixstock.Service.Infra.Model.Eav
{
    public interface IEavDate : IEavBase
    {
        DateTime? Value { get; set; }
    }
}