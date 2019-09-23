using Microsoft.WindowsAzure.Storage.Table;

namespace RbScoreKeeper.Entities
{
    public class PlayerEntity : TableEntity
    {
        public string Name { get; set; }
    }
}
