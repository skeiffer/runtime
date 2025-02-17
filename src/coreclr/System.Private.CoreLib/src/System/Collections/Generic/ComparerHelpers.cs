// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using static System.RuntimeTypeHandle;

namespace System.Collections.Generic
{
    /// <summary>
    /// Helper class for creating the default <see cref="Comparer{T}"/> and <see cref="EqualityComparer{T}"/>.
    /// </summary>
    /// <remarks>
    /// This class is intentionally type-unsafe and non-generic to minimize the generic instantiation overhead of creating
    /// the default comparer/equality comparer for a new type parameter. Efficiency of the methods in here does not matter too
    /// much since they will only be run once per type parameter, but generic code involved in creating the comparers needs to be
    /// kept to a minimum.
    /// </remarks>
    internal static class ComparerHelpers
    {
        /// <summary>
        /// Creates the default <see cref="Comparer{T}"/>.
        /// </summary>
        /// <param name="type">The type to create the default comparer for.</param>
        /// <remarks>
        /// The logic in this method is replicated in vm/compile.cpp to ensure that NGen saves the right instantiations,
        /// and in vm/jitinterface.cpp so the jit can model the behavior of this method.
        /// </remarks>
        internal static object CreateDefaultComparer(Type type)
        {
            Debug.Assert(type != null && type is RuntimeType);

            object? result = null;
            var runtimeType = (RuntimeType)type;

            // If T implements IComparable<T> return a GenericComparer<T>
            if (typeof(IComparable<>).MakeGenericType(type).IsAssignableFrom(type))
            {
                result = CreateInstanceForAnotherGenericParameter((RuntimeType)typeof(GenericComparer<int>), runtimeType);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Nullable does not implement IComparable<T?> directly because that would add an extra interface call per comparison.
                var embeddedType = (RuntimeType)type.GetGenericArguments()[0];
                result = CreateInstanceForAnotherGenericParameter((RuntimeType)typeof(NullableComparer<int>), embeddedType);
            }
            // The comparer for enums is specialized to avoid boxing.
            else if (type.IsEnum)
            {
                result = TryCreateEnumComparer(runtimeType);
            }

            return result ?? CreateInstanceForAnotherGenericParameter((RuntimeType)typeof(ObjectComparer<object>), runtimeType);
        }

        /// <summary>
        /// Creates the default <see cref="Comparer{T}"/> for an enum type.
        /// </summary>
        /// <param name="enumType">The enum type to create the default comparer for.</param>
        private static object? TryCreateEnumComparer(RuntimeType enumType)
        {
            Debug.Assert(enumType != null);
            Debug.Assert(enumType.IsEnum);

            // Explicitly call Enum.GetUnderlyingType here. Although GetTypeCode
            // ends up doing this anyway, we end up avoiding an unnecessary P/Invoke
            // and virtual method call.
            TypeCode underlyingTypeCode = Type.GetTypeCode(Enum.GetUnderlyingType(enumType));

            // Depending on the enum type, we need to special case the comparers so that we avoid boxing.
            // Specialize differently for signed/unsigned types so we avoid problems with large numbers.
            switch (underlyingTypeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return CreateInstanceForAnotherGenericParameter((RuntimeType)typeof(EnumComparer<>), enumType);
            }

            return null;
        }

        /// <summary>
        /// Creates the default <see cref="EqualityComparer{T}"/>.
        /// </summary>
        /// <param name="type">The type to create the default equality comparer for.</param>
        /// <remarks>
        /// The logic in this method is replicated in vm/compile.cpp to ensure that NGen saves the right instantiations.
        /// </remarks>
        internal static object CreateDefaultEqualityComparer(Type type)
        {
            Debug.Assert(type != null && type is RuntimeType);

            object? result = null;
            var runtimeType = (RuntimeType)type;

            if (type == typeof(byte))
            {
                // Specialize for byte so Array.IndexOf is faster.
                result = new ByteEqualityComparer();
            }
            else if (type == typeof(string))
            {
                // Specialize for string, as EqualityComparer<string>.Default is on the startup path
                result = new GenericEqualityComparer<string>();
            }
            else if (type.IsAssignableTo(typeof(IEquatable<>).MakeGenericType(type)))
            {
                // If T implements IEquatable<T> return a GenericEqualityComparer<T>
                result = CreateInstanceForAnotherGenericParameter((RuntimeType)typeof(GenericEqualityComparer<string>), runtimeType);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Nullable does not implement IEquatable<T?> directly because that would add an extra interface call per comparison.
                var embeddedType = (RuntimeType)type.GetGenericArguments()[0];
                result = CreateInstanceForAnotherGenericParameter((RuntimeType)typeof(NullableEqualityComparer<int>), embeddedType);
            }
            else if (type.IsEnum)
            {
                // The equality comparer for enums is specialized to avoid boxing.
                result = TryCreateEnumEqualityComparer(runtimeType);
            }

            return result ?? CreateInstanceForAnotherGenericParameter((RuntimeType)typeof(ObjectEqualityComparer<object>), runtimeType);
        }

        /// <summary>
        /// Creates the default <see cref="EqualityComparer{T}"/> for an enum type.
        /// </summary>
        /// <param name="enumType">The enum type to create the default equality comparer for.</param>
        private static object? TryCreateEnumEqualityComparer(RuntimeType enumType)
        {
            Debug.Assert(enumType != null);
            Debug.Assert(enumType.IsEnum);

            // See the METHOD__JIT_HELPERS__UNSAFE_ENUM_CAST and METHOD__JIT_HELPERS__UNSAFE_ENUM_CAST_LONG cases in getILIntrinsicImplementation
            // for how we cast the enum types to integral values in the comparer without boxing.

            TypeCode underlyingTypeCode = Type.GetTypeCode(Enum.GetUnderlyingType(enumType));

            // Depending on the enum type, we need to special case the comparers so that we avoid boxing.
            switch (underlyingTypeCode)
            {
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.UInt16:
                    return CreateInstanceForAnotherGenericParameter((RuntimeType)typeof(EnumEqualityComparer<>), enumType);
            }

            return null;
        }
    }
}
