using System;

namespace RbScoreKeeper.Models
{
    public class FlicButtonBinding
    {
        public FlicButtonBinding(Guid flicId, Guid playerId)
        {
            BindingId = Guid.NewGuid();
            PlayerId = playerId;
            FlicId = flicId;
        }

        public Guid BindingId { get; set; }

        public Guid FlicId { get; set; }

        public Guid PlayerId { get; set; }
    }
}
