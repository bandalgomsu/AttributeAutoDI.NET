using AttributeAutoDI.Attribute;

namespace AttributeAutoDI.Sample;

[Options("Sample:Sample")]
public class SampleOption
{
    public string Sample { get; set; } = "";
}