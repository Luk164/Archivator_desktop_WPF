using System;
using System.Collections.Generic;
using System.Text;
using ArchivatorDb;
using ArchivatorDb.Entities;

namespace Archivator_desktop_WPF_WTS.Models
{
    public class EditModel
    {
        public ArchivatorDbContext context { get; set; }
        public object editedObject { get; set; }
    }
}
