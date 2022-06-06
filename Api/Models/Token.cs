using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public bool IsValid { get; set; }
    }
}
