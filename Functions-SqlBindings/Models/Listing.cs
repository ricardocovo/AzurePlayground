using System;

namespace SqlFunctions.Models
{
	public class Listing
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string? ShortDescription { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		public Listing() { }
	}
}
