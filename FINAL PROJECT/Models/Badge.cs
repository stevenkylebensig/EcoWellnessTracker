using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoWellnessTracker.Models
{
	public class Badge
	{
		public string Name { get; set; }
		public string Description { get; set; }

		public Badge(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}
}
