namespace KLog.DataModel.Entities
{
    public class KLogBase
    {
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public bool IsDeleted { get; set; } 
    }
}
