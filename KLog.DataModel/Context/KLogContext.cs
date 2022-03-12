using EntityFrameworkCore.Triggers;
using KLog.DataModel.Entities;
using KLog.DataModel.Triggers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace KLog.DataModel.Context
{
    public class KLogContext : DbContextWithTriggers
    {
        public KLogContext(DbContextOptions<KLogContext> options) : base(options) { }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Log> Logs { get; set; }

        public static void SetTriggers()
        {
            SetBaseTriggers();
        }

        private static void SetBaseTriggers()
        {
            List<Type> dbSets = typeof(KLogContext)
                .GetProperties()
                .Where(p => p.PropertyType.Name == "DbSet`1")
                .Select(p => p.PropertyType.GetGenericArguments().FirstOrDefault())
                .ToList();

            List<Type> baseTypes = typeof(KLogContext)
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(KLogBase)) && t.IsClass)
                .ToList();

            List<Type> baseSets = baseTypes
                .Where(t => dbSets.Contains(t))
                .ToList();

            MethodInfo defaultTriggersMethod = typeof(KLogContext)
                .GetMethod(nameof(SetDefaultEntityTriggers));

            foreach (Type t in baseSets)
            {
                MethodInfo genericTriggerMethod = defaultTriggersMethod.MakeGenericMethod(t);
                genericTriggerMethod.Invoke(null, null);
            }
        }

        public static void SetDefaultEntityTriggers<T>() where T : KLogBase
        {
            Triggers<T>.Inserting += BaseTriggers<T>.OnInserting;
            Triggers<T>.Updating += BaseTriggers<T>.OnUpdating;
        }
    }
}
