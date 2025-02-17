﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeAnalysis;
namespace Microsoft.Interop.Analyzers
{
    public static class AnalyzerDiagnostics
    {
        public static class Ids
        {
            public const string Prefix = "SYSLIB";
            public const string ConvertToGeneratedComInterface = Prefix + "1096";
            public const string AddGeneratedComClassAttribute = Prefix + "1097";
            public const string ComHostingDoesNotSupportGeneratedComInterface = Prefix + "1098";
            public const string RuntimeComApisDoNotSupportSourceGeneratedCom = Prefix + "1099";
        }

        public static class Metadata
        {
            public const string MayRequireAdditionalWork = nameof(MayRequireAdditionalWork);
            public const string AddStringMarshalling = nameof(AddStringMarshalling);
        }

        private const string Category = "Interoperability";

        private static LocalizableResourceString GetResourceString(string resourceName)
        {
            return new LocalizableResourceString(resourceName, SR.ResourceManager, typeof(FxResources.Microsoft.Interop.ComInterfaceGenerator.SR));
        }

        public static readonly DiagnosticDescriptor ConvertToGeneratedComInterface =
            new DiagnosticDescriptor(
                Ids.ConvertToGeneratedComInterface,
                GetResourceString(nameof(SR.ConvertToGeneratedComInterfaceTitle)),
                GetResourceString(nameof(SR.ConvertToGeneratedComInterfaceMessage)),
                Category,
                DiagnosticSeverity.Info,
                isEnabledByDefault: true,
                description: GetResourceString(nameof(SR.ConvertToGeneratedComInterfaceDescription)));

        public static readonly DiagnosticDescriptor AddGeneratedComClassAttribute =
            new DiagnosticDescriptor(
                Ids.AddGeneratedComClassAttribute,
                GetResourceString(nameof(SR.AddGeneratedComClassAttributeTitle)),
                GetResourceString(nameof(SR.AddGeneratedComClassAttributeMessage)),
                Category,
                DiagnosticSeverity.Info,
                isEnabledByDefault: true,
                description: GetResourceString(nameof(SR.AddGeneratedComClassAttributeDescription)));

        public static readonly DiagnosticDescriptor ComHostingDoesNotSupportGeneratedComInterface =
            new DiagnosticDescriptor(
                Ids.ComHostingDoesNotSupportGeneratedComInterface,
                GetResourceString(nameof(SR.ComHostingDoesNotSupportGeneratedComInterfaceTitle)),
                GetResourceString(nameof(SR.ComHostingDoesNotSupportGeneratedComInterfaceMessage)),
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: GetResourceString(nameof(SR.ComHostingDoesNotSupportGeneratedComInterfaceDescription)));

        public static readonly DiagnosticDescriptor RuntimeComApisDoNotSupportSourceGeneratedCom =
            new DiagnosticDescriptor(
                Ids.RuntimeComApisDoNotSupportSourceGeneratedCom,
                GetResourceString(nameof(SR.RuntimeComApisDoNotSupportSourceGeneratedComTitle)),
                GetResourceString(nameof(SR.RuntimeComApisDoNotSupportSourceGeneratedComMessage)),
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: GetResourceString(nameof(SR.RuntimeComApisDoNotSupportSourceGeneratedComDescription)));
    }
}
