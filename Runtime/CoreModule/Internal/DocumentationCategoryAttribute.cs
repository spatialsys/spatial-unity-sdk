using System;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This attribute is used for marking documentation categories for the SDK.
    /// </summary>
    public class DocumentationCategoryAttribute : Attribute
    {
        public string Category { get; private set; }

        public DocumentationCategoryAttribute(string category)
        {
            Category = category;
        }
    }
}
