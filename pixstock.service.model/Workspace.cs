using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Pixstock.Service.Infra.Model;

namespace Pixstock.Service.Model
{
    [Table("svp_Workspace")]
    public class Workspace : IWorkspace
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public string PhysicalPath { get; set; }

        public string VirtualPath { get; set; }

        public DateTime? LastFullBuildDate { get; set; }

        public string TrimWorekspacePath(string path)
        {
            var escaped = Regex.Escape(this.VirtualPath);
            Regex re = new Regex("^" + escaped + @"[/\\]*", RegexOptions.Singleline);
            string key = re.Replace(path, "");

            return key;
        }

    }
}