using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geteam.fancy.models.subjects
{
    public class CategoriesModel : LGzE.Models.BaseModel
    {
        public CategoriesModel(string connectionstring) : base(connectionstring)
        {
            this.StoredProcedure = "sp_subjects_categories";
            this.StoredProcedureParamsNames = "_perform, _user, _args, _keywords, _page, _page_size, category_id, name, summary, isethnic";
        }
    }
}
