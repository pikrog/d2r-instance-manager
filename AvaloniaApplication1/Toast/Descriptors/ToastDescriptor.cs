using AvaloniaApplication1.Semantic;

namespace AvaloniaApplication1.Toast.Descriptors;

public abstract record ToastDescriptor(ToastSeverity Severity)
{
    public SemanticType SemanticType => ToastSemanticTypeMapper.Map(Severity);
}