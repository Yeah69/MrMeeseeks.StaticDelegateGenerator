using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace MrMeeseeks.StaticDelegateGenerator
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Console.WriteLine("Hello");
            const string staticDelegateSuffix = "StaticDelegate";

            if(context
                .Compilation
                .GetTypeByMetadataName(typeof(StaticDelegateAttribute).FullName ?? "") is not { } attributeType)
                return;


            foreach (var attributeData in context
                .Compilation
                .Assembly
                .GetAttributes()
                .Where(ad => ad.AttributeClass?.Equals(attributeType, SymbolEqualityComparer.Default) ?? false))
            {
                var countConstructorArguments = attributeData.ConstructorArguments.Length;
                if (countConstructorArguments is not 1)
                {
                    // Invalid code, ignore
                    continue;
                }

                var typeConstant = attributeData.ConstructorArguments[0];
                if (typeConstant.Kind != TypedConstantKind.Type)
                {
                    // Invalid code, ignore
                    continue;
                }
                if (!CheckValidType(typeConstant, out var type))
                {
                    continue;
                }

                var symbols = type
                    .GetMembers()
                    .Where(s => s is { IsStatic: true, DeclaredAccessibility: Accessibility.Public } and 
                        (IPropertySymbol or IMethodSymbol { IsExtensionMethod: false, IsInitOnly: false, MethodKind: MethodKind.Ordinary }))
                    .ToArray();

                if (symbols.Any())
                {
                    var typeName = type.Name;
                    var typeFullName = type.FullName();
                    var interfaceBuilder = new StringBuilder();
                    var implementationBuilder = new StringBuilder();

                    interfaceBuilder
                        .Append("namespace ").Append(type.ContainingNamespace.FullName()).AppendLine()
                        .Append("{").AppendLine()
                        .Append("    internal interface I").Append(typeName).Append(staticDelegateSuffix).AppendLine()
                        .Append("    {").AppendLine();

                    implementationBuilder
                        .Append("namespace ").Append(type.ContainingNamespace.FullName()).AppendLine()
                        .Append("{").AppendLine()
                        .Append("    internal class ").Append(typeName).Append(staticDelegateSuffix).Append(" : I").Append(typeName).Append(staticDelegateSuffix).AppendLine()
                        .Append("    {").AppendLine();

                    foreach (var symbol in symbols)
                    {
                        switch (symbol.Kind)
                        {
                            case SymbolKind.Property:
                            {
                                var propertySymbol = (IPropertySymbol)symbol;
                                if (propertySymbol.GetMethod is null && propertySymbol.SetMethod is null)
                                    break;
                                var propertyName = propertySymbol.Name;
                                var propertyTypeFullName = propertySymbol.Type.FullName();
                                interfaceBuilder.AppendLine(
                                    $"            {propertyTypeFullName} {propertyName} {{{(propertySymbol.GetMethod is { } ? " get;" : "")}{(propertySymbol.SetMethod is { } ? " set;" : "")} }}");
                                implementationBuilder.AppendLine(
                                    $"            public {propertyTypeFullName} {propertyName} {{{(propertySymbol.GetMethod is { } ? $" get => {typeFullName}.{propertyName};" : "")}{(propertySymbol.SetMethod is { } ? $" set => {typeFullName}.{propertyName} = value;" : "")} }}");
                                break;
                            }
                            case SymbolKind.Method:
                            {
                                //
                                var methodSymbol = (IMethodSymbol)symbol;
                                if (!methodSymbol.CanBeReferencedByName)
                                    break;
                                var returnTypeFullName = methodSymbol.ReturnsVoid 
                                    ? "void "
                                    : methodSymbol.ReturnType.FullName();
                                var returnInCall = methodSymbol.ReturnsVoid
                                    ? ""
                                    : "return ";

                                var methodName = methodSymbol.Name;
                                var genericTypes = methodSymbol.IsGenericMethod 
                                    ? $"<{string.Join(", ", methodSymbol.TypeParameters.Select(tps => tps.FullName()))}>"
                                    : "";
                                var parametersDeclarationBuilder = new List<string>();
                                var parametersCallBuilder = new List<string>();
                                foreach (var methodSymbolParameter in methodSymbol.Parameters)
                                {
                                    var parameterTypeFullName = methodSymbolParameter.Type.FullName();
                                    var parameterName = methodSymbolParameter.Name;
                                    var refPart = methodSymbolParameter.RefKind switch
                                    {
                                        RefKind.In => "in ",
                                        RefKind.Out => "out ",
                                        RefKind.Ref => "ref ",
                                        RefKind.None => "",
                                        _ => ""
                                    };
                                    var paramsPart = methodSymbolParameter.IsParams
                                        ? "params "
                                        : "";
                                    parametersDeclarationBuilder.Add($"{refPart}{paramsPart}{parameterTypeFullName} {parameterName}");
                                    parametersCallBuilder.Add($"{refPart}{parameterName}");
                                }

                                var parameterDeclaration = string.Join(", ", parametersDeclarationBuilder);
                                var parameterCall = string.Join(", ", parametersCallBuilder);
                                interfaceBuilder
                                    .Append($"            {returnTypeFullName} {methodName}{genericTypes}({parameterDeclaration});").AppendLine();
                                implementationBuilder
                                    .Append($"            public {returnTypeFullName} {methodName}{genericTypes}({parameterDeclaration})").AppendLine()
                                    .Append($"            {{").AppendLine()
                                    .Append($"                {returnInCall}{typeFullName}.{methodName}({parameterCall});").AppendLine()
                                    .Append($"            }}").AppendLine();
                                    break;
                            }

                        }
                    }

                    interfaceBuilder
                        .Append("    }").AppendLine()
                        .Append("}").AppendLine();

                    implementationBuilder
                        .Append("    }").AppendLine()
                        .Append("}").AppendLine();

                    var interfaceSource = CSharpSyntaxTree
                        .ParseText(SourceText.From(interfaceBuilder.ToString(), Encoding.UTF8))
                        .GetRoot()
                        .NormalizeWhitespace()
                        .SyntaxTree
                        .GetText();
                    var implementationSource = CSharpSyntaxTree
                        .ParseText(SourceText.From(implementationBuilder.ToString(), Encoding.UTF8))
                        .GetRoot()
                        .NormalizeWhitespace()
                        .SyntaxTree
                        .GetText();
                    context.AddSource(
                        $"I{typeName}{staticDelegateSuffix}.g.cs",
                        interfaceSource);
                    context.AddSource(
                        $"{typeName}{staticDelegateSuffix}.g.cs",
                        implementationSource);

                }
            }
        }

        private bool CheckValidType(TypedConstant typedConstant, out INamedTypeSymbol type)
        {
            type = (typedConstant.Value as INamedTypeSymbol)!;
            if (typedConstant.Value is null)
                return false;
            if (type.IsOrReferencesErrorType())
                // we will report an error for this case anyway.
                return false;
            if (type.IsUnboundGenericType)
                return false;
            if (!type.IsAccessibleInternally())
                return false;

            return true;
        }
    }
}
