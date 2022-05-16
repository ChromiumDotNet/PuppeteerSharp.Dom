using System.ComponentModel;
using System.Runtime.Serialization;
using PuppeteerSharp.Dom.Converters;

namespace PuppeteerSharp.Dom
{
    /// <summary>
    /// HtmlButtonElement Type
    /// </summary>
    [TypeConverter(typeof(StringToEnumTypeConverter))]
    public enum HtmlElementSelectionDirectionType
    {
        /// <summary>
        /// Forward
        /// </summary>
        [EnumMember(Value = "forward")]
        Forward,
        /// <summary>
        /// Backward
        /// </summary>
        [EnumMember(Value = "backward")]
        Backward,
        /// <summary>
        /// if the direction is unknown or irrelevant. Default value.
        /// </summary>
        [EnumMember(Value = "none")]
        Bone,
    }
}
