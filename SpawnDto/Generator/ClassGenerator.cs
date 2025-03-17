using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SpawnDto.Core.Attributes;

namespace SpawnDto.Generator;

public class ClassGenerator
{
    public void GenerateClass(Type classType, string className, string outputPath, string dtoNamespace, string convertorOutputPath, string convertorNamespace)
    {
        
        if(!Path.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        if(!Path.Exists(convertorOutputPath))
            Directory.CreateDirectory(convertorOutputPath);
        var attribute = classType.GetCustomAttribute(typeof(SpawnDtoAttribute));

        if (attribute == null || attribute is not SpawnDtoAttribute atr)
            throw new ArgumentNullException($"{classType.FullName} does not have a GenerateDtoAttribute");
        
        List<MethodDeclarationSyntax> methods = new ();
        HashSet<string> convertorNamespaces = new HashSet<string>();
        
        if(classType.Namespace != null)
            convertorNamespaces.Add(classType.Namespace);
        
        foreach (var name in atr.Names)
        {
            HashSet<string> namespaces = new HashSet<string>();
            
            var dto = GenerateDto(classType, name, namespaces);
            methods.Add(GenerateToDtoConvertorMethod(classType, name, convertorNamespaces));
            methods.Add(GenerateFromDtoConvertorMethod(classType, name, convertorNamespaces));
            var cu = GenerateTemplate(classType, dto, namespaces, dtoNamespace);
            SaveClass(cu, Path.Combine(outputPath,  name + ".cs"));
            cu.Members.OfType<NamespaceDeclarationSyntax>()
                .Select(ns => ns.Name.ToString())
                .ToList().ForEach(ns => convertorNamespaces.Add(ns));
        }
        
        var convertor = GenerateConvertorClass(classType, methods);
        var convertorCu = GenerateTemplate(classType, convertor, convertorNamespaces,
            convertorNamespace);
        SaveClass(convertorCu, Path.Combine(convertorOutputPath,  classType.Name + "Convertor.cs"));


    }

    private CompilationUnitSyntax GenerateTemplate(Type classType, ClassDeclarationSyntax classDeclarationSyntax, HashSet<string> namespaces, string namespaceStr)
    {

        var cu = SyntaxFactory.CompilationUnit()
            .AddMembers(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceStr))
                .AddMembers(classDeclarationSyntax)
            );
        
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
            
        var atr = classType.GetCustomAttribute(typeof(DtoBaseAttribute));
        
        if (atr != null && atr is DtoBaseAttribute baseAttribute)
        {
            newClass = newClass.AddBaseListTypes(SyntaxFactory.SimpleBaseType(
                SyntaxFactory.IdentifierName(baseAttribute.BaseType.Name)));
            if(baseAttribute.BaseType.Namespace != null)
                namespaces.Add(baseAttribute.BaseType.Namespace);
        }
        
        classType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty)
            .Where(info =>
            {
                var attributes = info.GetCustomAttributes(typeof(DtoPropertyAttribute));
                foreach (var attribute in attributes)
                {
                    if(!(attribute is DtoPropertyAttribute atr))
                        continue;
                    if(atr.Dtos.Length > 0 && !atr.Dtos.Contains(className))
                        continue;
                    return true;
                }
                return false;
            }).ToList()
            .ForEach(x => newClass = newClass.AddMembers(GenerateProperty(x, className, namespaces)));
        
        return newClass;
    }

    private ClassDeclarationSyntax GenerateConvertorClass(Type classType, List<MethodDeclarationSyntax> methods)
    {
        var staticConvertor = SyntaxFactory.ClassDeclaration(classType.Name + "Convertor")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword));
        
        methods.ForEach(x => staticConvertor = staticConvertor.AddMembers(x));
        return staticConvertor;
    }
    
    private MethodDeclarationSyntax GenerateToDtoConvertorMethod(Type classType, string className, HashSet<string> namespaces)
    {
        var method = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.ParseTypeName(className), // Return dto type
                "To" + className // Method name
            )
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("model"))
                    .WithType(SyntaxFactory.ParseTypeName(classType.Name))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.ThisKeyword)));
        
        List<StatementSyntax> statements = new();
        
        statements.Add(SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.ParseTypeName(className))
                .AddVariables(SyntaxFactory.VariableDeclarator("dto")
                    .WithInitializer(SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.ParseTypeName(className)
                        ).WithArgumentList(SyntaxFactory.ArgumentList()) // new Dto()
                    )))));
        
        classType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty)
            .Where(info =>
            {
                var attributes = info.GetCustomAttributes(typeof(DtoPropertyAttribute));
                foreach (var attribute in attributes)
                {
                    if(!(attribute is DtoPropertyAttribute atr))
                        continue;
                    if(atr.Dtos.Length > 0 && !atr.Dtos.Contains(className))
                        continue;
                    return true;
                }
                return false;
            }).ToList()
            .ForEach(x => statements.Add(GenerateValueConversion(x, className, namespaces)));
        
        statements.Add(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("dto")));
        
        return method.WithBody(SyntaxFactory.Block(statements));
    }

    private MethodDeclarationSyntax GenerateFromDtoConvertorMethod(Type classType, string className, HashSet<string> namespaces)
    {
                var method = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.ParseTypeName(classType.Name), // Return dto type
                "ToModel" // Method name
            )
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("dto"))
                    .WithType(SyntaxFactory.ParseTypeName(className))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.ThisKeyword)));
        
        List<StatementSyntax> statements = new();
        
        statements.Add(SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.ParseTypeName(classType.Name))
                .AddVariables(SyntaxFactory.VariableDeclarator("model")
                    .WithInitializer(SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.ParseTypeName(classType.Name)
                        ).WithArgumentList(SyntaxFactory.ArgumentList()) // new Dto()
                    )))));
        
        classType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty)
            .Where(info =>
            {
                var attributes = info.GetCustomAttributes(typeof(DtoPropertyAttribute));
                foreach (var attribute in attributes)
                {
                    if(!(attribute is DtoPropertyAttribute atr))
                        continue;
                    if(atr.Dtos.Length > 0 && !atr.Dtos.Contains(className))
                        continue;
                    return true;
                }
                return false;
            }).ToList()
            .ForEach(x => statements.Add(GenerateValueConversion(x, className, namespaces, false)));
        
        statements.Add(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("model")));
        
        return method.WithBody(SyntaxFactory.Block(statements));
    }
    
    private ExpressionStatementSyntax GenerateValueConversion(MemberInfo info, string className, HashSet<string> namespaces, bool toDto = true)
    {
        DtoPropertyAttribute? attribute = null;
        
        var attributes = info.GetCustomAttributes(typeof(DtoPropertyAttribute));
        foreach (var potentionalAttribute in attributes)
        {
            if(!(potentionalAttribute is DtoPropertyAttribute atrref))
                continue;
            if(atrref.Dtos.Length > 0 && !atrref.Dtos.Contains(className))
                continue;
            attribute = atrref;
            break;
        }
        if(attribute == null)
            throw new ArgumentException("Argument null or missing SpawnDtoAttribute");
        
        ExpressionSyntax dtoProperty = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName("dto"),
            SyntaxFactory.IdentifierName(attribute.CustomName ?? info.Name));

        
        
        ExpressionSyntax model = SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("model"),
                                            SyntaxFactory.IdentifierName(info.Name)
                                            );
        if (attribute.Convertor != null && attribute.Convertor.Namespace != null)
            namespaces.Add(attribute.Convertor.Namespace);
        if (attribute.Convertor != null && toDto)
        {
            model = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName( attribute.Convertor.Name + '.' + attribute.ToDtoMethod!) // we know that it exists;
                ).WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                model
                            )
                        )
                    )
                );
        }
        else if (attribute.Convertor == null && toDto && attribute.WillBeGenerated)
        {
            model = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("model." + info.Name),
                    SyntaxFactory.IdentifierName("To" + attribute.TargetType!.Name)
                ));
        }
        if (attribute.Convertor != null && !toDto)
        {
            dtoProperty = SyntaxFactory.InvocationExpression(
                SyntaxFactory.IdentifierName( attribute.Convertor.Name + '.' + attribute.FromDtoMethod!) // we know that it exists;
            ).WithArgumentList(
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(
                            dtoProperty
                        )
                    )
                )
            );
        }
        else if (attribute.Convertor == null && !toDto && attribute.WillBeGenerated)
        {
            dtoProperty = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("dto." + info.Name),
                    SyntaxFactory.IdentifierName("ToModel")
                ));
        }

        if(toDto)
            return SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression, dtoProperty, model));
        return SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression, model, dtoProperty));
    }
    
    private PropertyDeclarationSyntax GenerateProperty(MemberInfo info, string className, HashSet<string> namespaces)
    {
        DtoPropertyAttribute? attribute = null;
        
        var attributes = info.GetCustomAttributes(typeof(DtoPropertyAttribute));
        foreach (var potentionalAttribute in attributes)
        {
            if(!(potentionalAttribute is DtoPropertyAttribute atrref))
                continue;
            if(atrref.Dtos.Length > 0 && !atrref.Dtos.Contains(className))
                continue;
            attribute = atrref;
            break;
        }
        if(attribute == null)
            throw new ArgumentException("Argument null or missing SpawnDtoAttribute");
        
        TypeSyntax propertyType;

        // custom target type has bigger priority
        var memberType = attribute.TargetType ?? GetMemberType(info);

        if (memberType.Namespace != null) 
            namespaces.Add(memberType.Namespace);

        if (info.GetCustomAttribute(typeof(NullableAttribute)) != null)
            propertyType = SyntaxFactory.NullableType(SyntaxFactory.ParseTypeName(
                memberType.Name
            ));
        else
            propertyType = SyntaxFactory.ParseTypeName(
                memberType.Name
            );

        var defaultValue = GetMemberValue(info, attribute);
        
        var valueExpression = GetLiteralExpression(defaultValue, memberType);

        var property = SyntaxFactory.PropertyDeclaration(
                propertyType,
                SyntaxFactory.Identifier(attribute.CustomName ?? info.Name))
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
        return SyntaxFactory.DefaultExpression(SyntaxFactory.ParseTypeName(type.Name));
    }
    
    private Type GetMemberType(MemberInfo memberInfo)
    {
        if(memberInfo is PropertyInfo property)
            return property.PropertyType;
        if(memberInfo is FieldInfo field)
            return field.FieldType;
        throw new ArgumentException("Invalid member info");
    }
    
    private object? GetMemberValue(MemberInfo memberInfo, DtoPropertyAttribute atr)
    {
        object? value = null;
        if(memberInfo.ReflectedType == null)
            throw new ArgumentException("Member reflected type is null");
        var tmpInstance = Activator.CreateInstance(memberInfo.ReflectedType);
        if(memberInfo is PropertyInfo property)
            value = property.GetValue(tmpInstance);
        else if(memberInfo is FieldInfo field)
            value = field.GetValue(tmpInstance);
        else
            throw new ArgumentException("Invalid member info");
        
        if (atr.Convertor != null)
        {
            var toDtoMethod = atr.Convertor.GetMethod(atr.ToDtoMethod!);

            // we know that the method exists thanks to SpawnDtoAttribute constructor
            return toDtoMethod!.Invoke(null, [value]);
        }
        

        return value;
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