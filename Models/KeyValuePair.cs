using System.ComponentModel.DataAnnotations;

namespace ServerAPI.Models
{
    public class KeyValuePair
    {
        public KeyValuePair() { }
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
