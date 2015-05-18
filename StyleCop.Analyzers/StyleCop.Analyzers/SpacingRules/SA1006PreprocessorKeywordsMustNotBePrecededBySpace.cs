﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# preprocessor-type keyword is preceded by space.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the preprocessor-type keyword in a preprocessor directive is preceded
    /// by space. For example:</para>
    ///
    /// <code language="cs">
    /// # if DEBUG
    /// </code>
    ///
    /// <para>There should not be any whitespace between the opening hash mark and the preprocessor-type keyword:</para>
    ///
    /// <code language="cs">
    /// #if DEBUG
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1006PreprocessorKeywordsMustNotBePrecededBySpace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1006PreprocessorKeywordsMustNotBePrecededBySpace"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1006";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1006Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1006MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string Category = "StyleCop.CSharp.SpacingRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1006Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1006.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(this.HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens(descendIntoTrivia: true))
            {
                switch (token.Kind())
                {
                case SyntaxKind.HashToken:
                    this.HandleHashToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleHashToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            if (!token.HasTrailingTrivia || token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia))
            {
                return;
            }

            SyntaxToken targetToken = token.GetNextToken(includeDirectives: true);
            if (targetToken.IsMissing)
            {
                return;
            }

            // Preprocessor keyword '{keyword}' must not be preceded by a space.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, targetToken.GetLocation(), targetToken.Text));
        }
    }
}
