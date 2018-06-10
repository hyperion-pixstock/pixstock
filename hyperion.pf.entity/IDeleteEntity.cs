namespace Hyperion.Pf.Entity
{
    public interface IDeleteEntity
    {
         void OnDelete(KatalibDbContext context);
    }
}