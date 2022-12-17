using System.Text.Json.Serialization;

namespace Prices.Core.Application.Models;

public class ErcotPriceFilesList
{
    public ErcotListDocsByRptTypeRes ListDocsByRptTypeRes { get; set; } = null!;
}

public class ErcotListDocsByRptTypeRes
{
    public List<ErcotDocumentList> DocumentList { get; set; } = null!;
}

public class ErcotDocumentList
{
    public ErcotDocument Document { get; set; } = null!;
}

public class ErcotDocument
{
    public DateTime ExpiredDate { get; set; }
    public string ILMStatus { get; set; } = null!;
    public string SecurityStatus { get; set; } = null!;
    public string ContentSize { get; set; } = null!;
    public string Extension { get; set; } = null!;
    public string ReportTypeID { get; set; } = null!;
    public string Prefix { get; set; } = null!;
    public string FriendlyName { get; set; } = null!;
    public string ConstructedName { get; set; } = null!;
    public string DocID { get; set; } = null!;
    public DateTime PublishDate { get; set; }
    public string ReportName { get; set; } = null!;
    public string DUNS { get; set; } = null!;
    public string DocCount { get; set; } = null!;

    [JsonIgnore]
    public string DownloadUrl => $"https://www.ercot.com/misdownload/servlets/mirDownload?doclookupId={DocID}";
}