﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Helpers.Cineworld.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Helpers.Cineworld.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;xsl:stylesheet version=&quot;1.0&quot;
        ///                xmlns:xsl=&quot;http://www.w3.org/1999/XSL/Transform&quot;&gt;
        ///  &lt;xsl:output method=&quot;xml&quot; indent=&quot;yes&quot;/&gt;
        ///
        ///  &lt;xsl:template match=&quot;/&quot;&gt;
        ///    &lt;xsl:for-each select=&quot;/cinemas/cinema/films/film[not(.=preceding::*)]&quot;&gt;
        ///      &lt;Film&gt;
        ///        &lt;Edi&gt;
        ///          &lt;xsl:value-of select=&quot;@edi&quot;/&gt;
        ///        &lt;/Edi&gt;
        ///        &lt;Title&gt;
        ///          &lt;xsl:value-of select=&quot;@title&quot;/&gt;
        ///        &lt;/Title&gt;
        ///        &lt;Length&gt;
        ///          &lt;xsl:value-of select=&quot;substring- [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string all_performances {
            get {
                return ResourceManager.GetString("all_performances", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;xsl:stylesheet version=&quot;1.0&quot;
        ///                xmlns:xsl=&quot;http://www.w3.org/1999/XSL/Transform&quot;&gt;
        ///  &lt;xsl:output method=&quot;xml&quot; indent=&quot;yes&quot;/&gt;
        ///
        ///  &lt;xsl:template match=&quot;cinemas&quot;&gt;
        ///    &lt;Cinemas&gt;
        ///      &lt;xsl:for-each select=&quot;cinema&quot;&gt;
        ///        &lt;Cinema&gt;
        ///          &lt;Id&gt;
        ///            &lt;xsl:value-of select=&quot;@id&quot;/&gt;
        ///          &lt;/Id&gt;
        ///          &lt;Name&gt;
        ///            &lt;xsl:value-of select=&quot;@name&quot;/&gt;
        ///          &lt;/Name&gt;
        ///          &lt;Films&gt;
        ///            &lt;xsl:for-each select=&quot;listing/film&quot;&gt;        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string listings {
            get {
                return ResourceManager.GetString("listings", resourceCulture);
            }
        }
    }
}
