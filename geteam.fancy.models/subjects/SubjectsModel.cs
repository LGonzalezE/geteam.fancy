using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geteam.fancy.models.subjects
{
    public class SubjectsModel : LGzE.Models.BaseModel
    {
        public CategoriesModel Category { get; set; }
        public AccessTypesModel AccessType { get; set; }
        public StatusModel Status { get; set; }

        public SubjectsModel(string connectionstring) : base(connectionstring)
        {
            this.StoredProcedure = "sp_subjects";
            this.StoredProcedureParamsNames = "_perform, _user, _args, _keywords, _page, _page_size, subject_id, category_id, status_id, access_type_id, name, summary, notes";
          
        }
    }
}
