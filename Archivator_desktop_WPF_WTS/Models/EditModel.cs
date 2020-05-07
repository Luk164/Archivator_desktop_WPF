using ArchivatorDb;

namespace Archivator_desktop_WPF_WTS.Models
{
    public class EditModel
    {
        public ArchivatorDbContext context { get; set; }
        public object editedObject { get; set; }
    }
}
