using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoWellnessTracker.Models
{
	public class User
	{
		public string Username { get; set; }
		public HealthMetrics HealthMetrics { get; set; }
		public EnvironmentalMetrics EnvironmentalMetrics { get; set; }
		public List<Badge> Badges { get; set; }

		public User(string username)
		{
			Username = username;
			HealthMetrics = new HealthMetrics();
			EnvironmentalMetrics = new EnvironmentalMetrics();
			Badges = new List<Badge>();
		}

		// Other methods and properties...
	}
}
