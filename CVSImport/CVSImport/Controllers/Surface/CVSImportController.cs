using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
//Added system.IO for input output generating. 
using System.IO;

namespace CVSImport.Controllers.Surface
{
    //Todo: when does this get called?
    public class CVSImportController : SurfaceController
    {
        //Called from http://localhost:1471/umbraco/Surface/CVSImport/ImportCSV
        public ActionResult ImportCSV()
        {
            //Attempt 1: using a stream to get the file from a locked location, then splitting it up into c# objects that can be added via the extended and exposed service. 
            //Todo: does this need to be a hard location?
            using (var reader = new StreamReader(@"D:\Documents\Embrace_Project\MOCK_DATA.csv"))
            {

                List<Member> NewMembers = new List<Member>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var values = line.Split(',');

                    int newMemberId;

                    if (!int.TryParse(values[0], out newMemberId))
                    {
                        continue;
                    }

                    Gender newMemberGender;

                    if (!Enum.TryParse(values[4], out newMemberGender))
                    {
                        continue;
                    }



                    NewMembers.Add(new Member { Id = newMemberId, FirstName = values[1], LastName = values[2], Email = values[3], Gender = newMemberGender });


                }
            }

            Services.MemberService.CreateMember()

            return Content("Done");
        }
    }

    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}