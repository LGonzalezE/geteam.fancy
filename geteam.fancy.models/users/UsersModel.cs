using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geteam.fancy.models.users
{
    public class UsersModel : LGzE.Models.BaseModel
    {

        public UsersModel() : base()
        {
            
        }

        public UsersModel(string connectionstring) : base(connectionstring)
        {
            
        }

        protected override void Init()
        {
            this.StoredProcedure = "sp_users";
            this.StoredProcedureParamsNames = "_perform, _user, _args, _keywords, _page, _page_size, user_id, role_id, name, password, lastaccessdate";
        }
    }
}
