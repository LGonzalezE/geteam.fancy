using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geteam.fancy.models.security
{
    public class AuthorizationClientsModel:LGzE.Models.BaseModel
    {
        public AuthorizationClientsModel(string connectionstring) : base(connectionstring)
        {
            this.StoredProcedure = "sp_authorization_clients";
            this.StoredProcedureParamsNames = "_perform, _perform, _user, _args, _keywords, _page, _page_size, client_id, name";
        }
    }
}
