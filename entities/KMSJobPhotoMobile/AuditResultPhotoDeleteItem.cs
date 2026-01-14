using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace erpsolution.entities
{
    public class AuditResultPhotoDeleteItem
    {
        /// <summary>
        /// The sequence number of the photo to delete.
        /// </summary>
        [Required]
        [DefaultValue(1)]
        public decimal PhotoSeq { get; set; }

        /// <summary>
        /// Stored file name of the photo.
        /// </summary>
        [Required]
        [DefaultValue("IMG_0001.jpg")]
        public string FileName { get; set; } = string.Empty;
    }
}
