using System;

namespace KrisMecn.Attributes
{
    /// <summary>
    /// Defines display name for this command module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ModuleDisplayNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the display name defined for this module.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Defines display name for this command module.
        /// </summary>
        /// <param name="displayName"></param>
        public ModuleDisplayNameAttribute(string displayName)
        {
            this.DisplayName = displayName;
        }
    }
}
