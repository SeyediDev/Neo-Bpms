using System;
using System.Collections.Generic;

namespace Neo.Bpms.UI.MVC.Models.Dashboard
{
    public class DashboardPageScriptModel
    {
        public bool CanDesign { get; set; }

        public string ActiveTab { get; set; }

        public string SelectedConfigId { get; set; }

        public bool PersistentIsNull { get; set; }

        public IReadOnlyCollection<string> ClientOnlyConfigIds { get; set; } = Array.Empty<string>();
    }
}

