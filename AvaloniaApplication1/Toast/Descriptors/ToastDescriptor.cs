using D2RInstanceManager.Semantic;

namespace D2RInstanceManager.Toast.Descriptors;

public abstract record ToastDescriptor(ToastSeverity Severity)
{
    public SemanticType SemanticType => ToastSemanticTypeMapper.Map(Severity);
}