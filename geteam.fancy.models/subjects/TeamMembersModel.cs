using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geteam.fancy.models.subjects
{
    public class TeamMembersModel : LGzE.Models.BaseModel
    {
        public TeamMembersModel(string connectionstring) : base(connectionstring)
        {
            this.StoredProcedure = "sp_subjects_team_members";
            this.StoredProcedureParamsNames = "_perform, _user, _args, _keywords, _page, _page_size, subject_id, user_id";
        }
    }
}
