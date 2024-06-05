namespace DmhyAutoDownload.Core.Data.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal class MappedAttribute: Attribute
{
    public bool IsRequired { get; set; } = true;
}