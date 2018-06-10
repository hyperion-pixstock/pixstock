using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Pixstock.Service.Infra;
using Pixstock.Service.Infra.Model;
using Pixstock.Service.Infra.Repository;
using Pixstock.Service.Model;

namespace Pixstock.Service.Gateway.Repository
{
    public class EventLogRepository : PixstockAppRepositoryBase<EventLog, IEventLog>, IEventLogRepository
    {
        public EventLogRepository(IAppDbContext context)
            : base((DbContext)context, "EventLog")
        {

        }

        public IEventLog Load(long id)
        {
            return _dbset.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEventLog New()
        {
            var entity = new EventLog();
            return this.Add(entity);
        }
    }
}