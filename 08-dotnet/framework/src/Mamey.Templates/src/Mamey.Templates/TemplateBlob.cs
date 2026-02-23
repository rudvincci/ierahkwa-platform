namespace Mamey.Templates;

/// <summary>Binary payload for a template.</summary>
public sealed record TemplateBlob(TemplateId Id, int Version, string ContentType, byte[] Data, string Sha256);