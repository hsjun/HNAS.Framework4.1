﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.239
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace HNAS.Framework4.Validation.Properties
{
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HNAS.Framework4.Validation.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 Enterprise Library Validation Application Block 的本地化字符串。
        /// </summary>
        public static string BlockName {
            get {
                return ResourceManager.GetString("BlockName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Errors reading configuration for Enterprise Library Validation Application Block 的本地化字符串。
        /// </summary>
        public static string ConfigurationErrorMessage {
            get {
                return ResourceManager.GetString("ConfigurationErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must not contain the characters in &quot;{3}&quot; with mode &quot;{4}&quot;. 的本地化字符串。
        /// </summary>
        public static string ContainsCharactersNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("ContainsCharactersNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must contain the characters in &quot;{3}&quot; with mode &quot;{4}&quot;.  的本地化字符串。
        /// </summary>
        public static string ContainsCharactersNonNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("ContainsCharactersNonNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must not belong to the domain. 的本地化字符串。
        /// </summary>
        public static string DomainNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("DomainNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value does not belong to the domain. 的本地化字符串。
        /// </summary>
        public static string DomainNonNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("DomainNonNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must match Email format. 的本地化字符串。
        /// </summary>
        public static string EmailDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("EmailDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must not be defined in the &quot;{3}&quot; enum type. 的本地化字符串。
        /// </summary>
        public static string EnumConversionNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("EnumConversionNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must be defined in the &quot;{3}&quot; enum type. 的本地化字符串。
        /// </summary>
        public static string EnumConversionNonNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("EnumConversionNonNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The supplied value could not be converted to the target property type. 的本地化字符串。
        /// </summary>
        public static string ErrorCannotPerfomDefaultConversion {
            get {
                return ResourceManager.GetString("ErrorCannotPerfomDefaultConversion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value for &quot;{0}&quot; could not be accessed from an instance of &quot;{1}&quot;. 的本地化字符串。
        /// </summary>
        public static string ErrorValueAccessInvalidType {
            get {
                return ResourceManager.GetString("ErrorValueAccessInvalidType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value for &quot;{0}&quot; could not be accessed from null. 的本地化字符串。
        /// </summary>
        public static string ErrorValueAccessNull {
            get {
                return ResourceManager.GetString("ErrorValueAccessNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Attempted to retrieve value from instance of wrong type. 的本地化字符串。
        /// </summary>
        public static string ExceptionAttemptedValueAccessForInstanceOfInvalidType {
            get {
                return ResourceManager.GetString("ExceptionAttemptedValueAccessForInstanceOfInvalidType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The supplied bound type is not compatible with IComparable. 的本地化字符串。
        /// </summary>
        public static string ExceptionBoundTypeNotIComparable {
            get {
                return ResourceManager.GetString("ExceptionBoundTypeNotIComparable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The string representing the bound value could not be converted to the bound type. 的本地化字符串。
        /// </summary>
        public static string ExceptionCannotConvertBound {
            get {
                return ResourceManager.GetString("ExceptionCannotConvertBound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 At most one range bound can be ignored. 的本地化字符串。
        /// </summary>
        public static string ExceptionCannotIgnoreBothBoundariesInRange {
            get {
                return ResourceManager.GetString("ExceptionCannotIgnoreBothBoundariesInRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 A message template has been set already, resource based message templates are not allowed. 的本地化字符串。
        /// </summary>
        public static string ExceptionCannotSetResourceBasedMessageTemplatesIfTemplateIsSet {
            get {
                return ResourceManager.GetString("ExceptionCannotSetResourceBasedMessageTemplatesIfTemplateIsSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 A message template resource has been set already; template override is not allowed. 的本地化字符串。
        /// </summary>
        public static string ExceptionCannotSetResourceMessageTemplatesIfResourceTemplateIsSet {
            get {
                return ResourceManager.GetString("ExceptionCannotSetResourceMessageTemplatesIfResourceTemplateIsSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Enumerable contains null elements. 的本地化字符串。
        /// </summary>
        public static string ExceptionContainsNullElements {
            get {
                return ResourceManager.GetString("ExceptionContainsNullElements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The property name required to retrieve validation information for integration is not found. 的本地化字符串。
        /// </summary>
        public static string ExceptionIntegrationValidatedPropertyNameNotAvailable {
            get {
                return ResourceManager.GetString("ExceptionIntegrationValidatedPropertyNameNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The property name required to retrieve validation information for integration is invalid or does not belong to a public property. 的本地化字符串。
        /// </summary>
        public static string ExceptionIntegrationValidatedPropertyNotExists {
            get {
                return ResourceManager.GetString("ExceptionIntegrationValidatedPropertyNotExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The property name required to retrieve validation information for integration refers to nonreadable property. 的本地化字符串。
        /// </summary>
        public static string ExceptionIntegrationValidatedPropertyNotReadable {
            get {
                return ResourceManager.GetString("ExceptionIntegrationValidatedPropertyNotReadable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The type required to retrieve validation information for integration is not found. 的本地化字符串。
        /// </summary>
        public static string ExceptionIntegrationValidatedTypeNotAvailable {
            get {
                return ResourceManager.GetString("ExceptionIntegrationValidatedTypeNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The supplied string is not a valid date representation. 的本地化字符串。
        /// </summary>
        public static string ExceptionInvalidDate {
            get {
                return ResourceManager.GetString("ExceptionInvalidDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The field &quot;{0}&quot; on type &quot;{1}&quot; is either missing or non public. 的本地化字符串。
        /// </summary>
        public static string ExceptionInvalidField {
            get {
                return ResourceManager.GetString("ExceptionInvalidField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The method &quot;{0}&quot; on type &quot;{1}&quot; is either missing, non public, void or has parameters. 的本地化字符串。
        /// </summary>
        public static string ExceptionInvalidMethod {
            get {
                return ResourceManager.GetString("ExceptionInvalidMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The property &quot;{0}&quot; on type &quot;{1}&quot; is either missing, non public or read-only. 的本地化字符串。
        /// </summary>
        public static string ExceptionInvalidProperty {
            get {
                return ResourceManager.GetString("ExceptionInvalidProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Value to validate is not of the expected type: expected {0} but got {1} instead. 的本地化字符串。
        /// </summary>
        public static string ExceptionInvalidTargetType {
            get {
                return ResourceManager.GetString("ExceptionInvalidTargetType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The lower bound cannot be null unless it&apos;s type is Ingore. 的本地化字符串。
        /// </summary>
        public static string ExceptionLowerBoundNull {
            get {
                return ResourceManager.GetString("ExceptionLowerBoundNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Method to access value does not have a return value. 的本地化字符串。
        /// </summary>
        public static string ExceptionMethodHasNoReturnValue {
            get {
                return ResourceManager.GetString("ExceptionMethodHasNoReturnValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Method to access value has parameters. 的本地化字符串。
        /// </summary>
        public static string ExceptionMethodHasParameters {
            get {
                return ResourceManager.GetString("ExceptionMethodHasParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The target element type has not been set in the configuration. 的本地化字符串。
        /// </summary>
        public static string ExceptionObjectCollectionValidatorDataTargetTypeNotSet {
            get {
                return ResourceManager.GetString("ExceptionObjectCollectionValidatorDataTargetTypeNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Both resource name and resource type must be defined to retrieve the message template. 的本地化字符串。
        /// </summary>
        public static string ExceptionPartiallyDefinedResourceForMessageTemplate {
            get {
                return ResourceManager.GetString("ExceptionPartiallyDefinedResourceForMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The source property to request validators from does not exist. 的本地化字符串。
        /// </summary>
        public static string ExceptionPropertyNotFound {
            get {
                return ResourceManager.GetString("ExceptionPropertyNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The source property to request validators from is not readable. 的本地化字符串。
        /// </summary>
        public static string ExceptionPropertyNotReadable {
            get {
                return ResourceManager.GetString("ExceptionPropertyNotReadable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 No public readable property with name &quot;{0}&quot; could be found for type &quot;{1}&quot;. 的本地化字符串。
        /// </summary>
        public static string ExceptionPropertyToCompareNotFound {
            get {
                return ResourceManager.GetString("ExceptionPropertyToCompareNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The name for the property to compare is null for PropertyComparisonValidator. 的本地化字符串。
        /// </summary>
        public static string ExceptionPropertyToCompareNull {
            get {
                return ResourceManager.GetString("ExceptionPropertyToCompareNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The self validation method has an invalid signature. It should be &quot;void [method name](ValidationResults)&quot;. 的本地化字符串。
        /// </summary>
        public static string ExceptionSelfValidationMethodWithInvalidSignature {
            get {
                return ResourceManager.GetString("ExceptionSelfValidationMethodWithInvalidSignature", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 This method should not have been called. 的本地化字符串。
        /// </summary>
        public static string ExceptionShouldNotCall {
            get {
                return ResourceManager.GetString("ExceptionShouldNotCall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The source type to request validators from has not been found. 的本地化字符串。
        /// </summary>
        public static string ExceptionTypeNotFound {
            get {
                return ResourceManager.GetString("ExceptionTypeNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Range bounds must have the same type. 的本地化字符串。
        /// </summary>
        public static string ExceptionTypeOfBoundsMustMatch {
            get {
                return ResourceManager.GetString("ExceptionTypeOfBoundsMustMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Upper bound cannot be lower than lower bound. 的本地化字符串。
        /// </summary>
        public static string ExceptionUpperBoundLowerThanLowerBound {
            get {
                return ResourceManager.GetString("ExceptionUpperBoundLowerThanLowerBound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The upper bound cannot be null unless it&apos;s type is Ingore. 的本地化字符串。
        /// </summary>
        public static string ExceptionUpperBoundNull {
            get {
                return ResourceManager.GetString("ExceptionUpperBoundNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Value to validate is null but expected an instance of the non-reference type {0}. 的本地化字符串。
        /// </summary>
        public static string ExceptionValidatingNullOnValueType {
            get {
                return ResourceManager.GetString("ExceptionValidatingNullOnValueType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 A validation attribute of type {0} cannot be used to validate values. 的本地化字符串。
        /// </summary>
        public static string ExceptionValidationAttributeNotSupported {
            get {
                return ResourceManager.GetString("ExceptionValidationAttributeNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value is not null and failed all its validation rules for key &quot;{1}&quot;. 的本地化字符串。
        /// </summary>
        public static string IgnoreNullsDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("IgnoreNullsDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The InjectionValidationSource may only be used to configure generic Enterprise Library validator classes. 的本地化字符串。
        /// </summary>
        public static string IllegalUseOfInjectionValidationSource {
            get {
                return ResourceManager.GetString("IllegalUseOfInjectionValidationSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The ValidationDependencyAttribute was applied to a dependency of type {0}, which is a generic Enterprise Library validator class. 的本地化字符串。
        /// </summary>
        public static string IllegalUseOfValidationDependencyAttribute {
            get {
                return ResourceManager.GetString("IllegalUseOfValidationDependencyAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The Validation Specification Source {0} is invalid. 的本地化字符串。
        /// </summary>
        public static string InvalidValidationSpecificationSource {
            get {
                return ResourceManager.GetString("InvalidValidationSpecificationSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 InvariantCulture cannot be used to deserialize configuration. 的本地化字符串。
        /// </summary>
        public static string InvariantCultureCannotBeUsedToDeserializeConfiguration {
            get {
                return ResourceManager.GetString("InvariantCultureCannotBeUsedToDeserializeConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The operation must be implemented by a subclass. 的本地化字符串。
        /// </summary>
        public static string MustImplementOperation {
            get {
                return ResourceManager.GetString("MustImplementOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must be null. 的本地化字符串。
        /// </summary>
        public static string NonNullNegatedValidatorDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("NonNullNegatedValidatorDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value cannot be null. 的本地化字符串。
        /// </summary>
        public static string NonNullNonNegatedValidatorDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("NonNullNonNegatedValidatorDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The element in the validated collection is not compatible with the expected type. 的本地化字符串。
        /// </summary>
        public static string ObjectCollectionValidatorIncompatibleElementInTargetCollection {
            get {
                return ResourceManager.GetString("ObjectCollectionValidatorIncompatibleElementInTargetCollection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The supplied object is not a collection. 的本地化字符串。
        /// </summary>
        public static string ObjectCollectionValidatorTargetNotCollection {
            get {
                return ResourceManager.GetString("ObjectCollectionValidatorTargetNotCollection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The supplied object is not compatible with the expected type. 的本地化字符串。
        /// </summary>
        public static string ObjectValidatorInvalidTargetType {
            get {
                return ResourceManager.GetString("ObjectValidatorInvalidTargetType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 All validators failed for key &quot;{1}&quot;. 的本地化字符串。
        /// </summary>
        public static string OrCompositeValidatorDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("OrCompositeValidatorDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must not fall within the range &quot;{3}&quot; ({4}) - &quot;{5}&quot; ({6}). 的本地化字符串。
        /// </summary>
        public static string RangeValidatorNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("RangeValidatorNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must fall within the range &quot;{3}&quot; ({4}) - &quot;{5}&quot; ({6}). 的本地化字符串。
        /// </summary>
        public static string RangeValidatorNonNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("RangeValidatorNonNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must not match the regular expression &quot;{3}&quot; with options &quot;{4}&quot;. 的本地化字符串。
        /// </summary>
        public static string RegexValidatorNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("RegexValidatorNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must match the regular expression &quot;{3}&quot; with options &quot;{4}&quot;. 的本地化字符串。
        /// </summary>
        public static string RegexValidatorNonNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("RegexValidatorNonNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must not fall within the range &quot;{3}&quot; ({4}) - &quot;{5}&quot; ({6}) relative to now. 的本地化字符串。
        /// </summary>
        public static string RelativeDateTimeNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("RelativeDateTimeNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must fall within the range &quot;{3}&quot; ({4}) - &quot;{5}&quot; ({6}) relative to now. 的本地化字符串。
        /// </summary>
        public static string RelativeDateTimeNonNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("RelativeDateTimeNonNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 It&apos;s not possible to specify a None DateTime unit if a BoundaryType different from Ignore is used. 的本地化字符串。
        /// </summary>
        public static string RelativeDateTimeValidatorNotValidDateTimeUnit {
            get {
                return ResourceManager.GetString("RelativeDateTimeValidatorNotValidDateTimeUnit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The self validation method thrown an exception while evaluating. 的本地化字符串。
        /// </summary>
        public static string SelfValidationMethodThrownMessage {
            get {
                return ResourceManager.GetString("SelfValidationMethodThrownMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The reference provided to the self validation method is either null or references an instance of a non-compatible type. 的本地化字符串。
        /// </summary>
        public static string SelfValidationValidatorMessage {
            get {
                return ResourceManager.GetString("SelfValidationValidatorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The length of the value must not fall within the range &quot;{3}&quot; ({4}) - &quot;{5}&quot; ({6}). 的本地化字符串。
        /// </summary>
        public static string StringLengthValidatorNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("StringLengthValidatorNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The length of the value must fall within the range &quot;{3}&quot; ({4}) - &quot;{5}&quot; ({6}). 的本地化字符串。
        /// </summary>
        public static string StringLengthValidatorNonNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("StringLengthValidatorNonNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must not be convertible to type &quot;{3}&quot;. 的本地化字符串。
        /// </summary>
        public static string TypeConversionNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("TypeConversionNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value must be convertible to type &quot;{3}&quot;. 的本地化字符串。
        /// </summary>
        public static string TypeConversionNonNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("TypeConversionNonNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Validation using {0} threw an exception: {1} 的本地化字符串。
        /// </summary>
        public static string ValidationAttributeFailed {
            get {
                return ResourceManager.GetString("ValidationAttributeFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Parameter validation failed 的本地化字符串。
        /// </summary>
        public static string ValidationFailedMessage {
            get {
                return ResourceManager.GetString("ValidationFailedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Validation results: 的本地化字符串。
        /// </summary>
        public static string ValidationResultsHeader {
            get {
                return ResourceManager.GetString("ValidationResultsHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似     Result: {0} Message: {1} 的本地化字符串。
        /// </summary>
        public static string ValidationResultTemplate {
            get {
                return ResourceManager.GetString("ValidationResultTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似     Result: {0} Key: {2} Message: {1} 的本地化字符串。
        /// </summary>
        public static string ValidationResultWithKeyTemplate {
            get {
                return ResourceManager.GetString("ValidationResultWithKeyTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Failure to retrieve comparand for key &quot;{0}&quot;: {1}. 的本地化字符串。
        /// </summary>
        public static string ValueAccessComparisonValidatorFailureToRetrieveComparand {
            get {
                return ResourceManager.GetString("ValueAccessComparisonValidatorFailureToRetrieveComparand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value should not have succeeded in the comparison with value for key &quot;{4}&quot; using operator &quot;{5}&quot;. 的本地化字符串。
        /// </summary>
        public static string ValueAccessComparisonValidatorNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("ValueAccessComparisonValidatorNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value failed the comparison with value for key &quot;{4}&quot; using operator &quot;{5}&quot;. 的本地化字符串。
        /// </summary>
        public static string ValueAccessComparisonValidatorNonNegatedDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("ValueAccessComparisonValidatorNonNegatedDefaultMessageTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Value Validator 的本地化字符串。
        /// </summary>
        public static string ValueValidatorDefaultMessageTemplate {
            get {
                return ResourceManager.GetString("ValueValidatorDefaultMessageTemplate", resourceCulture);
            }
        }
    }
}
