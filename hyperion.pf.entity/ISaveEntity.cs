using Microsoft.EntityFrameworkCore;

namespace Hyperion.Pf.Entity
{
    public interface ISaveEntity
    {
         void OnSave(DbContext context);
    }
}