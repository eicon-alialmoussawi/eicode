namespace Spectre.Core.Models.Extenders
{
    public class FailedImportedAwards_View
    {
        public int? Year { set; get; }
        public string Month { set; get; }
        public string Error { set; get; }
        public string? Country { set; get; }

        public decimal? Value { set; get; }
    }
}
