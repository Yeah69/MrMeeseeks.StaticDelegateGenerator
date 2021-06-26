using System.Linq;
using Microsoft.CodeAnalysis;

namespace MrMeeseeks.StaticDelegateGenerator
{
    internal static class RoslynExtensions
    {

        // Picked from https://github.com/YairHalberstadt/stronginject Thank you!
        public static bool IsOrReferencesErrorType(this ITypeSymbol type)
        {
            if (!type.ContainingType?.IsOrReferencesErrorType() ?? false)
                return false;
            return type switch
            {
                IErrorTypeSymbol => true,
                IArrayTypeSymbol array => array.ElementType.IsOrReferencesErrorType(),
                IPointerTypeSymbol pointer => pointer.PointedAtType.IsOrReferencesErrorType(),
                INamedTypeSymbol named => !named.IsUnboundGenericType && named.TypeArguments.Any(IsOrReferencesErrorType),
                _ => false,
            };
        }

        // Picked from https://github.com/YairHalberstadt/stronginject Thank you!
        public static bool IsAccessibleInternally(this ITypeSymbol type)
        {
            if (type is ITypeParameterSymbol)
                return true;
            if (!type.ContainingType?.IsAccessibleInternally() ?? false)
                return false;
            return type switch
            {
                IArrayTypeSymbol array => array.ElementType.IsAccessibleInternally(),
                IPointerTypeSymbol pointer => pointer.PointedAtType.IsAccessibleInternally(),
                INamedTypeSymbol named => named.DeclaredAccessibility is Accessibility.Public or Accessibility.ProtectedOrInternal or Accessibility.Internal
                    && named.TypeArguments.All(IsAccessibleInternally),
                _ => false,
            };
        }

        // Picked from https://github.com/YairHalberstadt/stronginject Thank you!
        public static string FullName(this ITypeSymbol type) =>
            type.ToDisplayString(new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                parameterOptions: SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeParamsRefOut,
                memberOptions: SymbolDisplayMemberOptions.IncludeRef));

        // Picked from https://github.com/YairHalberstadt/stronginject Thank you!
        public static string FullName(this INamespaceSymbol @namespace) => 
            @namespace.ToDisplayString(new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));
    }
}