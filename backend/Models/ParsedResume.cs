namespace JobDecisionEngine.Models
{
    public class ParsedResume
    {
        public List<string>? Skills { get; set; }
        public int? Experience { get; set; }
        public List<string>? Roles { get; set; }
        public List<string>? Education { get; set; }
        public List<string>? Certifications { get; set; }
        public List<string>? Warnings { get; set; }
    }
}
