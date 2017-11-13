using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geteam.fancy.models.subjects
{
    public class StatusModel : LGzE.Models.BaseModel
    {
        public StatusModel(string connectionstring) : base(connectionstring)
        {
            this.StoredProcedure = "sp_subjects_status";
            this.StoredProcedureParamsNames = "_perform, _user, _args, _keywords, _page, _page_size, status_id, name, summary";
        }
    }
}
