using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geteam.fancy.models.users
{
    public class RolesModel : LGzE.Models.BaseModel
    {
        public RolesModel(string connectionstring) : base(connectionstring)
        {
            this.StoredProcedure = "sp_users_roles";
            this.StoredProcedureParamsNames = "_perform, _user, _args, _keywords, _page, _page_size, role_id, name, summary, enable";
        }
    }
}
