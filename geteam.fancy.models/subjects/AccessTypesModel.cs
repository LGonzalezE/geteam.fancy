using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geteam.fancy.models.subjects
{
    public class AccessTypesModel : LGzE.Models.BaseModel
    {
        public AccessTypesModel(string connectionstring) : base(connectionstring)
        {
            this.StoredProcedure = "sp_subjects_access_types";
            this.StoredProcedureParamsNames = "_perform, _user, _args, _keywords, _page, _page_size, access_type_id, name, summary";
        }
    }
}
