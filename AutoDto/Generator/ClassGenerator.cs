using System.Reflection;
using System.Runtime.CompilerServices;
using AutoDto.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDto.Generator;

public class ClassGenerator
{
    public void GenerateClass(Type classType, string className, string outputPath)
    {
        
        if(!Path.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        var attribute = classType.GetCustomAttribute(typeof(GenerateDtoAttribute));

        if (attribute == null || !(attribute is GenerateDtoAttribute atr))
            throw new ArgumentNullException($"{classType.FullName} does not have a GenerateDtoAttribute");
        
        foreach (var name in atr.Names)
        {
            HashSet<string> namespaces = new HashSet<string>();
            
            var dto = GenerateDto(classType, name, namespaces);
            var cu = GenerateTemplate(classType, dto, namespaces);
            SaveClass(cu, Path.Combine(outputPath,  name + ".cs"));
        }
        

    }

    private CompilationUnitSyntax GenerateTemplate(Type classType, ClassDeclarationSyntax classDeclarationSyntax, HashSet<string> namespaces)
    {

        var cu = SyntaxFactory.CompilationUnit()
            // .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
            .AddMembers(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName((classType.Namespace ??
                classType.Name) + ".Dto"))
                .AddMembers(classDeclarationSyntax)
            );

        // lexicographically sorted
        namespaces
            .OrderBy(w => w.Length)
            .ToList()
            .ForEach(
                w => cu = cu.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(w)))
            );

        return cu;
    }

    private ClassDeclarationSyntax GenerateDto(Type classType, string className, HashSet<string> namespaces)
    {
        var newClass = SyntaxFactory.ClassDeclaration(className)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        
        var fields = classType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty)
            .Where(info =>
            {
                var attributes = info.GetCustomAttributes(typeof(AutoDtoAttribute));
                foreach (var attribute in attributes)
                {
                    if(!(attribute is AutoDtoAttribute atr))
                        continue;
                    if(atr.Dtos.Length > 0 && !atr.Dtos.Contains(className))
                        continue;
                    return true;
                }
                return false;
            }).ToList();
        fields.ForEach(x => newClass = newClass.AddMembers(GenerateProperty(x, className, namespaces)));
        
        
        
        return newClass;
    }

    // private FieldDeclarationSyntax GenerateField(FieldInfo info)
    // {
    //     var atribute = info.GetCustomAttribute(typeof(AutoDtoAttribute));
    //     if(atribute == null || !(atribute is AutoDtoAttribute atr))
    //         throw new ArgumentException("Argument null or missing AutoDtoAttribute");
    //     
    //     return SyntaxFactory.FieldDeclaration(
    //             SyntaxFactory.VariableDeclaration(
    //                 SyntaxFactory.IdentifierName(
    //                     atr.TargetType != null? atr.TargetType!.Name : info.FieldType.Name
    //                     )
    //                 )
    //                 .AddVariables(SyntaxFactory.VariableDeclarator(info.Name))
    //             )
    //             .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
    //
    // }

    private PropertyDeclarationSyntax GenerateProperty(MemberInfo info, string className, HashSet<string> namespaces)
    {
        Attribute? attribute = null;
        
        
        var attributes = info.GetCustomAttributes(typeof(AutoDtoAttribute));
        foreach (var potentionalAttribute in attributes)
        {
            if(!(potentionalAttribute is AutoDtoAttribute atrref))
                continue;
            if(atrref.Dtos.Length > 0 && !atrref.Dtos.Contains(className))
                continue;
            attribute = atrref;
            break;
        }
        if(attribute == null || !(attribute is AutoDtoAttribute atr))
            throw new ArgumentException("Argument null or missing AutoDtoAttribute");
        
        TypeSyntax propertyType;


        var memberType = GetMemberType(info);

        if (memberType.Namespace != null) 
            namespaces.Add(memberType.Namespace);

        if (info.GetCustomAttribute(typeof(NullableAttribute)) != null)
            propertyType = SyntaxFactory.NullableType(SyntaxFactory.ParseTypeName(
                atr.TargetType != null ? atr.TargetType!.Name : memberType.Name
            ));
        else
            propertyType = SyntaxFactory.ParseTypeName(
                atr.TargetType != null? atr.TargetType!.Name : memberType.Name
            );

        var defaultValue = GetMemberValue(info);
        
        var valueExpression = GetLiteralExpression(defaultValue, memberType);

        var property = SyntaxFactory.PropertyDeclaration(
                propertyType,
                SyntaxFactory.Identifier(info.Name))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(
                // Adding the getter
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                // Adding the setter
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            );
            
        if(defaultValue != GetDefaultValue(memberType))
            property = property.WithInitializer(SyntaxFactory.EqualsValueClause(valueExpression))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            // .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression()))
        

        
        return property;
    }

    private ExpressionSyntax GetLiteralExpression(object? value, Type type)
    {
        if (value == null)
            return SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        if (type == typeof(int))
            return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal((int)value));

        if (type == typeof(double))
            return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal((double)value));

        if (type == typeof(bool))
            return (bool)value
                ? SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
                : SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);

        if (type == typeof(string))
            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal((string)value));

        if(type == typeof(float))
            return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal((float)value));
        
        // Fallback: Use "default(Type)"
        return SyntaxFactory.DefaultExpression(SyntaxFactory.ParseTypeName(type.FullName ?? type.Name));
    }
    
    private Type GetMemberType(MemberInfo memberInfo)
    {
        if(memberInfo is PropertyInfo property)
            return property.PropertyType;
        if(memberInfo is FieldInfo field)
            return field.FieldType;
        throw new ArgumentException("Invalid member info");
    }
    
    private object? GetMemberValue(MemberInfo memberInfo)
    {
        if(memberInfo.ReflectedType == null)
            throw new ArgumentException("Member reflected type is null");
        var tmpInstance = Activator.CreateInstance(memberInfo.ReflectedType);
        if(memberInfo is PropertyInfo property)
            return property.GetValue(tmpInstance);
        if(memberInfo is FieldInfo field)
            return field.GetValue(tmpInstance);
        throw new ArgumentException("Invalid member info");
    }
    
    private object? GetDefaultValue(Type type)
    {
        if (type.IsValueType)
            return Activator.CreateInstance(type); // Default for value types
        return null; // Default for reference types
    }
    
    private void SaveClass(CompilationUnitSyntax cu, string filePath)
    {
        string code = cu.NormalizeWhitespace().ToFullString();
        
        Console.WriteLine($"Saving to {filePath}");
        
        File.WriteAllText(filePath, code);
        
        Console.WriteLine("Code generated");
    }


}