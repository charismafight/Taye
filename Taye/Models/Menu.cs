using AntDesign.ProLayout;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taye.Models
{
    public class Menu : BaseModel
    {
        public string[] Authority
        {
            get;
            set;
        }

        [NotMapped]
        public virtual MenuDataItem[] Children
        {
            get;
            set;
        }

        public string Icon
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public string ParentKey
        {
            get;
            set;
        }
    }
}
