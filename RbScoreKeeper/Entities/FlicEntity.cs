using Microsoft.WindowsAzure.Storage.Table;

namespace RbScoreKeeper.Entities
{
    public class FlicEntity : TableEntity
    {
        public string Name { get; set; }
    }
}
