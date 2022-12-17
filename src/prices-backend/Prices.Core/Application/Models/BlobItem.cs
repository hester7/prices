namespace Prices.Core.Application.Models;

public class BlobItem
{
    public BlobItem(string blobName, IDictionary<string, string> metadata)
    {
        BlobName = blobName;
        Metadata = metadata;
    }

    public string BlobName { get; set; }
    public IDictionary<string, string> Metadata { get; set; }

    public string? VirtualFolder => Path.GetDirectoryName(BlobName);
    public string FileName => Path.GetFileName(BlobName);
    public string MetadataString
    {
        get
        {
            var metadataString = string.Join("; ", Metadata);
            return metadataString.Substring(0, Math.Min(metadataString.Length, 100));
        }
    }
}