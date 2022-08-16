using System.Collections.Immutable;
using System.Linq;
using System.Text;
using HttpFunctionGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

/* 
 * Adapted from BlazorSourceGeneratorTests
 * https://github.com/b-straub/BlazorSourceGeneratorTests/blob/master/Tests/GeneratorTestFactory.cs
 *
 * MIT License
 * Copyright (c) 2020 b-straub
 */

namespace HttpFunctionGeneratorTests;

public static class GeneratorTestFactory
{
    public static (Compilation Compilation, ImmutableArray<Diagnostic> Diagnostics) RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(source, Encoding.UTF8));

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithOptimizationLevel(OptimizationLevel.Debug)
            .WithGeneralDiagnosticOption(ReportDiagnostic.Default);

        var references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        Compilation compilation = CSharpCompilation.Create("testgenerator", new[] { syntaxTree }, references, compilationOptions);
        var diagnostics = compilation.GetDiagnostics();
        if (!VerifyDiagnostics(diagnostics, "CS0012", "CS0616", "CS0246"))
        {
            // this will make the test fail, check the input source code!
            return (null, diagnostics);
        }

        var generator = new Generator();
        var parseOptions = syntaxTree.Options as CSharpParseOptions;

        var driver = CSharpGeneratorDriver.Create(
            ImmutableArray.Create<ISourceGenerator>(generator),
            ImmutableArray<AdditionalText>.Empty,
            parseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation,
            out var outputCompilation,
            out var generatorDiagnostics);

        return (outputCompilation, generatorDiagnostics);
    }

    private static bool VerifyDiagnostics(ImmutableArray<Diagnostic> actual, params string[] expected)
    {
        var expectedMap = new HashSet<string>(expected);
        return actual.Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.Id.ToString())
            .All(expectedMap.Contains);
    }
}