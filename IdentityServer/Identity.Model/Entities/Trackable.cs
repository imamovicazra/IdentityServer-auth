using EntityFrameworkCore.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.Entities
{
    public interface ITrackable
    {
        public DateTime UpdatedAt { get; set; }
    }

    public abstract class Trackable : ITrackable
    {
        public DateTime UpdatedAt { get; set; }

        public Trackable()
        {
            Triggers<Trackable>.Updating += entry => entry.Entity.UpdatedAt = DateTime.Now;
        }
    }
}
