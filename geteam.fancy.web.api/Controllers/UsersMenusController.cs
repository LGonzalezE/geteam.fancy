using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace geteam.fancy.web.api.Controllers
{
    public class UsersMenusController : ControllerBase
    {
        // GET api/<controller>
        public async System.Threading.Tasks.Task<HttpResponseMessage> Get()
        {
            await this.ValidateToken();

            if (this.Token.PerformStatus != LGzE.Models.PerformsStatus.success)
            {
                return this.JsonResponse(this.ResponseModel);
            }

            models.users.UsersModel users = new models.users.UsersModel(this.ConnectionString);
            users.Attr("_user", this.Token.Attr("user_id"));
            users.Query("menu");

            return this.JsonResponse(users.ResultSet);
        }


    }
}