using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
//Added system.IO for input output generating. 
using System.IO;
using System.Net.Mail;

namespace CVSImport.Controllers.Surface
{
    //Todo: when does this get called?
    public class CVSImportController : SurfaceController
    {
        //Called from http://localhost:1471/umbraco/Surface/CVSImport/ImportCSV
        public ActionResult ImportCSV()
        {

            List<Member> newMembers = new List<Member>();

            List<int> userNameIterators = new List<int>();

            List<int> nameIterators = new List<int>();

            List<int> emailIterators = new List<int>();

            //Attempt 1: using a stream to get the file from a locked location, then splitting it up into c# objects that can be added via the extended and exposed service. 
            //Todo: does this need to be a hard location?
            using (var reader = new StreamReader(@"D:\Documents\Embrace_Project\MOCK_DATA.csv"))
            {
                int currentLine = 0;

                

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    List<string> values = line.Split(',').ToList();

                    if (currentLine == 0)
                    {
                        //These strings will be used to find the column names that will form the username. 
                        List<string> userNameComponents = new List<string>() { "first_name", "last_name" };

                        //These strings will be used to find the column names that will form the name. 
                        List<string> nameComponents = new List<string>() { "first_name", "last_name" };

                        //These strings will be used to find the column names that will form the Email. 
                        List<string> emailComponents = new List<string>() { "email" };

                        for (int i = 0; i < values.Count; i++)
                        {
                            if (userNameComponents.Contains(values[i]))
                            {
                                userNameIterators.Add(i);
                            }
                            if (nameComponents.Contains(values[i]))
                            {
                                nameIterators.Add(i);
                            }
                            if (emailComponents.Contains(values[i]))
                            {
                                emailIterators.Add(i);
                            }
                        }

                        currentLine++;

                        continue;
                    }

                    Member newMember = new Member() { Username = "", Name = "", Email = "" };

                    for (int i = 0; i < values.Count; i++)
                    {
                        if (userNameIterators.Contains(i))
                        {
                            newMember.Username += values[i];
                        }
                        if(nameIterators.Contains(i))
                        {
                            newMember.Name += values[i] + " ";
                        }
                        if (emailIterators.Contains(i))
                        {
                            newMember.Email += values[i];
                        }
                    }

                    if (IsValid(newMember.Email))
                    {
                        newMembers.Add(newMember);
                    }

                    if (Services.MemberService.FindByEmail(newMember.Email).Any())
                    {

                    }

                    currentLine++;
                }
            }

            //Services.MemberService.CreateMember()

            return Content("Done");
        }


        //This should be a service. If such a service already exists, replace this one with the existing one. 
        //Done as reccomended by Microsoft.
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    public class Member
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}