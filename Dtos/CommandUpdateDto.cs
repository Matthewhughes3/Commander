using System.ComponentModel.DataAnnotations;

namespace Commander.Dtos
{
    public class CommandUpdateDto
    {
        public string HowTo { get; set; }

        public string Line { get; set; }

        public string Platform { get; set; }
    }
}