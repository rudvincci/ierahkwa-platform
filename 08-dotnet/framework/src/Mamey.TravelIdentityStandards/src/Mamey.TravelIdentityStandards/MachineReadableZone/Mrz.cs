namespace Mamey.TravelIdentityStandards.MachineReadableZone;

public abstract class Mrz
{
    public Mrz(DocumentType documentType, int lineCount, int lineLength)
    {
        DocumentType = documentType;
        LineCount = lineCount;
        LineLength = lineLength;

    }

    public int LineCount { get; set; }
    public int LineLength { get; set; }
    public DocumentType DocumentType { get; }

}