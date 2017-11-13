using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace geteam.fancy.web.api.Controllers
{
    public class SubjectsController : ControllerBase
    {
        
        // GET api/<controller>/5
        public async System.Threading.Tasks.Task<HttpResponseMessage> Get(int id)
        {

            await this.ValidateToken();

            if (this.Token.PerformStatus != LGzE.Models.PerformsStatus.success)
            {
                return this.JsonResponse(this.ResponseModel);
            }

            geteam.fancy.models.subjects.SubjectsModel subject = new models.subjects.SubjectsModel(this.ConnectionString);
            subject.Attr("subject_id", id);
            subject.Performs("read");


            return this.JsonResponse(subject);
        }

        // POST api/<controller>        
        //public void Post([FromBody]string value)
        [HttpPost]
        public async System.Threading.Tasks.Task<HttpResponseMessage> Post()
        {

            await this.ValidateToken();

            if (this.Token.PerformStatus != LGzE.Models.PerformsStatus.success)
            {
                return this.JsonResponse(this.ResponseModel);
            }

            string perform = string.Empty;
            perform = this.HttpForm["_perform"];
            geteam.fancy.models.subjects.SubjectsModel subject = new models.subjects.SubjectsModel(this.ConnectionString);
            models.subjects.TeamMembersModel teamMember = new models.subjects.TeamMembersModel(this.ConnectionString);
            teamMember.Attr("_user", this.Token["user_id"]);
            subject.Attr("_user", this.Token["user_id"]);

            switch (perform)
            {
                case "new":
                    this.SetAttribs(subject);
                    subject.Performs("new");
                    return this.JsonResponse(subject.ResultSet);
                case "create":
                    this.SetAttribs(subject);
                    subject.Performs("create");
                    return this.JsonResponse(subject.ResultSet);
                case "update":
                    this.SetAttribs(subject);
                    subject.Performs("update");
                    return this.JsonResponse(subject.ResultSet);
                case "search":
                    subject.Attr("_keywords", this.HttpForm["_keywords"]);
                    subject.Attr("_page_size", this.HttpForm["_page_size"]);
                    subject.Query("search");
                    return this.JsonResponse(subject.ResultSet);
                case "my.subjects.search":
                    subject.Attr("_keywords", this.HttpForm["_keywords"]);
                    subject.Attr("_page_size", this.HttpForm["_page_size"]);
                    subject.Query("my.subjects.search");
                    return this.JsonResponse(subject.ResultSet);
                case "my.subject.update":
                    this.SetAttribs(subject);
                    subject.Performs("my.subject.update");
                    return this.JsonResponse(subject.ResultSet);
                case "access_types":
                    subject.Query("access_types");
                    return this.JsonResponse(subject.ResultSet);
                case "categories":
                    subject.Query("categories");
                    return this.JsonResponse(subject.ResultSet);
                case "status":
                    subject.Query("status");
                    return this.JsonResponse(subject.ResultSet);
                case "team.members":
                    this.SetAttribs(subject);
                    subject.Query("team.members");
                    return this.JsonResponse(subject.ResultSet);
                case "team.members.create":
                    this.SetAttribs(teamMember);
                    teamMember.Performs("create");
                    return this.JsonResponse(teamMember.ResultSet);
                case "team.members.delete":
                    this.SetAttribs(teamMember);
                    teamMember.Performs("delete");
                    return this.JsonResponse(teamMember.ResultSet);
                default:
                    subject.Query("");
                    return this.JsonResponse(subject.ResultSet);
            }



        }
        


    }
}