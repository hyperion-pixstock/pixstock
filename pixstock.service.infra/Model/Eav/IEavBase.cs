namespace Pixstock.Service.Infra.Model.Eav
{
    public interface IEavBase
    {
        string EntityTypeName { get; set; }

        string CategoryName { get; set; }

        long EntityId { get; set; }

        string Key { get; set; }

    }
}