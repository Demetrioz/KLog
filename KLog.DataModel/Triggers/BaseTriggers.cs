using EntityFrameworkCore.Triggers;
using KLog.DataModel.Entities;

namespace KLog.DataModel.Triggers
{
    public static class BaseTriggers<T> where T : KLogBase
    {
        public static void OnInserting(IInsertingEntry entry)
        {
            KLogBase entity = entry.Entity as KLogBase;
            if (entity != null)
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;

                SetCreated(entity, now);
                SetModified(entity, now);
                SetIsDeleted(entity);
            }
        }

        public static void OnUpdating(IUpdatingEntry entry)
        {
            KLogBase entity = entry.Entity as KLogBase;
            if (entity != null)
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;

                SetModified(entity, now);
            }
        }

        public static void SetCreated(KLogBase entity, DateTimeOffset now)
        {
            entity.Created = now;
        }

        public static void SetModified(KLogBase entity, DateTimeOffset now)
        {
            entity.Modified = now;
        }

        public static void SetIsDeleted(KLogBase entity)
        {
            entity.IsDeleted = false;
        }
    }
}
